using AutoMapper;
using Dapper;
using Forum_DAL.Repositories.Contracts;
using System.Data;
using System.Data.SqlClient;
using System.Reflection;
using System.Text;
using AutoMapper.Configuration.Annotations;

namespace Forum_DAL.Repositories
{
    public abstract class GenericRepository<T> : IGenericRepository<T> where T : class
    {
        protected SqlConnection sqlConnection;

        /*
        Транзакція — група послідовних операцій з базою даних, яка є логічною одиницею роботи з даними.

        Інтерфейс IDbTransaction надає транзакцію, виконувану з джерелом даних. 
        Реалізується за допомогою постачальників даних .NET, у яких є доступ до реляційної бази даних.
        */
        protected IDbTransaction dbTransaction;
        private readonly string tableName;

        protected GenericRepository(SqlConnection sqlConnection, IDbTransaction dbTransaction, string tableName)
        {
            this.sqlConnection = sqlConnection;
            this.dbTransaction = dbTransaction;
            this.tableName = tableName;
        }

        public async Task<IEnumerable<T>> GetAllAsync()
        {
            return await sqlConnection.QueryAsync<T>($"SELECT * FROM {tableName};",
                transaction: dbTransaction);
        }

        public async Task<T> GetAsync(int id)
        {
            T result = await sqlConnection.QuerySingleOrDefaultAsync<T>($"SELECT * FROM {tableName} WHERE Id=@Id;",
                param: new { Id = id }, transaction: dbTransaction);

            if (result == null)
            {
                throw new KeyNotFoundException($"{tableName} with id [{id}] could not be found.");
            }

            return result;
        }

        public async Task DeleteAsync(int id)
        {
            await sqlConnection.ExecuteAsync($"DELETE FROM {tableName} WHERE Id=@Id;", param: new { Id = id },
                transaction: dbTransaction);
        }

        public async Task<int> AddAsync(T model)
        {
            string insertQuery = GenerateInsertQuery();

            return await sqlConnection.ExecuteScalarAsync<int>(insertQuery, param: model, transaction: dbTransaction);
        }

        public async Task<int> AddRangeAsync(IEnumerable<T> list)
        {
            string query = GenerateInsertQuery();

            return await sqlConnection.ExecuteAsync(query, param: list);
        }

        public async Task ReplaceAsync(T model)
        {
            string updateQuery = GenerateUpdateQuery();

            await sqlConnection.ExecuteAsync(updateQuery, param: model, transaction: dbTransaction);
        }

        // Генерація INSERT запиту
        private string GenerateInsertQuery()
        {
            StringBuilder insertQuery = new StringBuilder($"INSERT INTO {tableName} (");
            List<string> properties = GenerateListOfProperties(GetProperties);

            properties.ForEach(property => { insertQuery.Append($"[{property}],"); });

            insertQuery.Remove(insertQuery.Length - 1, 1).Append(") VALUES (");

            properties.ForEach(property => { insertQuery.Append($"@{property},"); });

            insertQuery.Remove(insertQuery.Length - 1, 1).Append("); SELECT SCOPE_IDENTITY();");

            return insertQuery.ToString();
        }

        // Отримання списку властивостей
        private IEnumerable<PropertyInfo> GetProperties
        {
            get
            {
                return typeof(T).GetProperties()
                   .Where(prop => prop.GetCustomAttributes(false)
                   .All(attr => attr.GetType().Namespace != "AutoMapper.Configuration.Annotations" 
                   || attr.GetType().Name != "IgnoreAttribute"));
            }
        }

        // Генерація списку властивостей
        private List<string> GenerateListOfProperties(IEnumerable<PropertyInfo> propertyList)
        {
            return propertyList.Select(p => p.Name).Where(p => !p.Equals("Id")).ToList();
        }

        // Генерація UPDATE запиту
        private string GenerateUpdateQuery()
        {
            StringBuilder updateQuery = new StringBuilder($"UPDATE {tableName} SET ");
            List<string> properties = GenerateListOfProperties(GetProperties);

            properties.ForEach(property =>
            {
                updateQuery.Append($"{property}=@{property},");
            });

            updateQuery.Remove(updateQuery.Length - 1, 1).Append(" WHERE Id = @Id");

            return updateQuery.ToString();
        }
    }
}
