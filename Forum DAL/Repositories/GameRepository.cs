using Catalog_of_Games_DAL.Entities;
using Dapper;
using Forum_DAL.Repositories.Contracts;
using System.Data;
using System.Data.SqlClient;

namespace Forum_DAL.Repositories
{
    public class GameRepository : GenericRepository<Game>, IGameRepository
    {
        public GameRepository(SqlConnection sqlConnection, IDbTransaction dbTransaction)
            : base(sqlConnection, dbTransaction, "gamecatalog.Games") { }

        // Отримання ігор для постів
        public async Task<IEnumerable<Game>> GetGamesForPostAsync(int postId)
        {
            string sqlQuery = "SELECT * FROM gamecatalog.Games" +
                " INNER JOIN forum.PostsGames ON gamecatalog.Games.Id = forum.PostsGames.GameId" +
                " WHERE forum.PostsGames.PostId = @PostId;";

            IEnumerable<Game> games = await sqlConnection.QueryAsync<Game>(sqlQuery, param: new { PostId = postId },
                transaction: dbTransaction);

            return games;
        }

        // Знайти гру за іменем
        public async Task<int> GetGameByNameAsync(string gameName)
        {
            string sqlQuery = "SELECT TOP 1 Id FROM gamecatalog.Games WHERE GmName = @GmName;";

            return await sqlConnection.QueryFirstAsync<int>(sqlQuery, param: new {GmName = gameName},
                transaction: dbTransaction);
        }
    }
}
