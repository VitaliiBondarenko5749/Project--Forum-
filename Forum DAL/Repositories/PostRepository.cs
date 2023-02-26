using Dapper;
using Forum_DAL.Models;
using Forum_DAL.Repositories.Contracts;
using System.Data;
using System.Data.SqlClient;

namespace Forum_DAL.Repositories
{
    public class PostRepository : GenericRepository<Post>, IPostRepository
    {
        public PostRepository(SqlConnection sqlConnection, IDbTransaction dbTransaction) 
            : base(sqlConnection, dbTransaction, "forum.Posts") { }

        // Вставити значення в PostsGames, яка зв'язує пости та ігри
        public async Task PutPostAndGameAsync(int postId, int gameId)
        {
            string sqlQuery = "INSERT INTO forum.PostsGames(PostId, GameId) VALUES (@PostId, @GameId);";

            await sqlConnection.QueryAsync(sqlQuery, param: new { PostId = postId, GameId = gameId },
                transaction: dbTransaction);
        }

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
