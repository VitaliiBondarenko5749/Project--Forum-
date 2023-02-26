using Dapper;
using Forum_DAL.Models;
using Forum_DAL.Repositories.Contracts;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace Forum_DAL.Repositories
{
    public class ReplyRepository : GenericRepository<Reply>, IReplyRepository
    {
        public ReplyRepository(SqlConnection sqlConnection, IDbTransaction dbTransaction)
            : base(sqlConnection, dbTransaction, "forum.Replies") { }

        // Отримання всіх відповідей до поста
        public async Task<IEnumerable<Reply>> GetRepliesForPostAsync(int postId)
        {
            string sqlQuery = "SELECT * FROM forum.Replies" +
                " INNER JOIN forum.PostsReplies ON forum.Replies.Id = forum.PostsReplies.ReplyId" +
                " WHERE forum.PostsReplies.PostId = @PostId;";

            IEnumerable<Reply> replies = await sqlConnection.QueryAsync<Reply>(sqlQuery, param: new { PostId = postId },
                transaction: dbTransaction);

            return replies;
        }

        // Отримання лайків для відповідей
        public async Task<int> GetLikesForReplyAsync(int replyId)
        {
            string sqlQuery = "SELECT COUNT(ReplyId) FROM forum.LikedReplies WHERE ReplyId = @ReplyId;";

            return await sqlConnection.ExecuteScalarAsync<int>(sqlQuery, param: new { ReplyId = replyId },
                transaction: dbTransaction);
        }

        // Отримання Id-значень з таблиці PostsReplies, які зв'язані з постом, який ми хочемо видалити
        public async Task<IEnumerable<int>> GetRepliesIdAsync(int postId)
        {
            string sqlQuery = "SELECT ReplyId FROM forum.PostsReplies WHERE PostId = @PostId;";

            return await sqlConnection.QueryAsync<int>(sqlQuery, param: new { PostId = postId },
                transaction: dbTransaction);
        }

        // Видалення значень з таблиці PostsReplies, PostId яких еквівалентний з Id постом, який ми хочемо видалити 
        public async Task DeleteAllRepliesFromPostsAsync(int postId)
        {
            string sqlQuery = "DELETE FROM forum.PostsReplies WHERE PostId = @PostId;";

            await sqlConnection.QueryAsync(sqlQuery, param: new { PostId = postId }, transaction: dbTransaction);
        }

        // Видалення записів(лайків) з таблиці LikedReplies, ReplyId який еквівалентно зі значеннями колекції repliesId
        public async Task DeleteAllLikesFromReplyAsync(int replyId)
        {
            string sqlQuery = "DELETE FROM forum.LikedReplies WHERE ReplyId = @ReplyId;";

            await sqlConnection.QueryAsync(sqlQuery, param: new { ReplyId = replyId }, transaction: dbTransaction);
        }

        // Отриманння RepliesToReplyId з таблиці RepliesToReply_Reply, які зв'язані з основною відповіддю
        public async Task<IEnumerable<int>> GetRepliesToReplyIdAsync(int replyId)
        {
            string sqlQuery = "SELECT ReplyToReplyId FROM forum.RepliesToReply_Reply WHERE ReplyId = @ReplyId;";

            return await sqlConnection.QueryAsync<int>(sqlQuery, param: new { ReplyId = replyId },
                transaction: dbTransaction);
        }

        // Видалення записів з таблиці RepliesToReply_Reply, ReplyId яких пов'язані з параметром
        public async Task DeleteRepliesToReplyAsync(int replyId)
        {
            string sqlQuery = "DELETE FROM forum.RepliesToReply_Reply WHERE ReplyId = @ReplyId";

            await sqlConnection.QueryAsync(sqlQuery, param: new { ReplyId = replyId }, transaction: dbTransaction);
        }

        // Вставка нового запису в таблицю PostsReplies
        public async Task PutPostAndGameAsync(int postId, int replyId)
        {
            string sqlQuery = "INSERT INTO forum.PostsReplies(PostId, ReplyId) VALUES (@PostId, @ReplyId);";

            await sqlConnection.QueryAsync(sqlQuery, param: new { PostId = postId, ReplyId = replyId }, transaction:
                dbTransaction);
        }

        // Видалення записів з таблиці PostsReplies, ReplyId яких еквівалентно з параметром
        public async Task DeleteReplyFromPostAsync(int replyId)
        {
            string sqlQuery = "DELETE FROM forum.PostsReplies WHERE ReplyId = @ReplyId;";

            await sqlConnection.QueryAsync(sqlQuery, param: new { ReplyId = replyId }, transaction: dbTransaction);
        }

        // Отримання UserId з таблиці LikedReplies, для первірки, чи поставив він вже лайк
        public async Task<int> GetUserIdFromLikedRepliesAsync(int userId, int replyId)
        {
            string sqlQuery = "SELECT TOP 1 UserId FROM forum.LikedReplies WHERE UserId = @UserId AND ReplyId = @ReplyId;";

            return sqlConnection.QueryFirst<int>(sqlQuery, param: new { UserId = userId, ReplyId = replyId },
                transaction: dbTransaction);
        }

        // Вставити нові значення в таблицю LikedReplies
        public async Task PostNewLikeForReplyAsync(int replyId, int userId)
        {
            string sqlQuery = "INSERT INTO forum.LikedReplies(ReplyId, UserId) VALUES (@ReplyId, @UserId);";

            await sqlConnection.QueryAsync(sqlQuery, param: new { ReplyId = replyId, UserId = userId },
                transaction: dbTransaction);
        }

        // Отримання значення ReplyId з таблиці PostsReplies, для того щоб перевірити коментар та пост на зв'язаність
        public async Task<int> GetReplyIdAsync(int postId, int replyId)
        {
            string sqlQuery = "SELECT TOP 1 ReplyId FROM forum.PostsReplies WHERE PostId = @PostId AND ReplyId = @ReplyId;";

            return sqlConnection.QueryFirst<int>(sqlQuery, param: new { PostId = postId, ReplyId = replyId },
                transaction: dbTransaction);
        }

        // Видалення запису з таблиці LikedReplies, щоб забрати лайк
        public async Task DeleteLikeFromReplyAsync(int replyId, int userId)
        {
            string sqlQuery = "DELETE FROM forum.LikedReplies WHERE ReplyId = @ReplyId AND UserId = @UserId;";

            await sqlConnection.QueryAsync(sqlQuery, param: new { ReplyId = replyId, UserId = userId }, 
                transaction: dbTransaction);
        }
    }
}
