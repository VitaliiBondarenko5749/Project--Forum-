using Dapper;
using Forum_DAL.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Forum_DAL.Repositories.Contracts
{
    public class PostGameRepository : GenericRepository<PostGame>, IPostGameRepository
    {
        public PostGameRepository(SqlConnection sqlConnection, IDbTransaction dbTransaction)
            : base(sqlConnection, dbTransaction, "forum.PostGames") { }

        // Отримання айдішок всіх ігор з таблиці PostsGames, PostId яких еквівалентні з параметром
        public async Task<IEnumerable<int>> GetGamesIdAsync(int postId)
        {
            string sqlQuery = "SELECT GameId FROM forum.PostsGames WHERE PostId = @PostId;";

            return await sqlConnection.QueryAsync<int>(sqlQuery, param: new { PostId = postId },
                transaction: dbTransaction);
        }

        // Видалення ігор з таблиці PostsGames
        public async Task DeleteGamesFromPostAsync(int postId)
        {
            string sqlQuery = "DELETE FROM forum.PostsGames WHERE PostId = @PostId;";

            await sqlConnection.QueryAsync(sqlQuery, param: new { PostId = postId }, transaction: dbTransaction);
        }
    }
}
