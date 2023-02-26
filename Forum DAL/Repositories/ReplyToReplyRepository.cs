using Dapper;
using Forum_DAL.Models;
using Forum_DAL.Repositories.Contracts;
using Microsoft.VisualBasic;
using System.Data;
using System.Data.SqlClient;

namespace Forum_DAL.Repositories
{
    public class ReplyToReplyRepository : GenericRepository<ReplyToReply>, IReplyToReplyRepository
    {
        public ReplyToReplyRepository(SqlConnection sqlConnection, IDbTransaction dbTransaction)
            : base(sqlConnection, dbTransaction, "forum.RepliesToReply") { }

        // Отримуємо всі відповіді на основну
        public async Task<IEnumerable<ReplyToReply>> GetRepliesToReplyAsync(int replyId)
        {
            string sqlQuery = "SELECT * FROM forum.RepliesToReply AS RTR INNER JOIN forum.RepliesToReply_Reply AS RTRR" +
                " ON RTRR.ReplyToReplyId = RTR.Id WHERE RTR.Id = @ReplyId;";

            return await sqlConnection.QueryAsync<ReplyToReply>(sqlQuery, param: new { ReplyId = replyId },
                transaction: dbTransaction);
        }

        // Отримуємо лайки відповідей на відповідь
        public async Task<int> GetLikesForReplyToReplyAsync(int replyToReplyId)
        {
            string sqlQuery = "SELECT COUNT(ReplyToReplyId) FROM forum.LikedRepliesToReply WHERE ReplyToReplyId =" +
                " @ReplyToReplyId;";

            return await sqlConnection.ExecuteScalarAsync<int>(sqlQuery, param: new { ReplyToReplyId = replyToReplyId },
                transaction: dbTransaction);
        }

        // Видалення записів з таблиці LikedRepliesToReply, ReplyToReplyId який еквівалентно зі значенням параметра
        public async Task DeleteLikedRepliesToReply(int replyToReplyId)
        {
            string sqlQuery = "DELETE FROM forum.LikedRepliesToReply WHERE ReplyToReplyId = @ReplyToReplyId;";

            await sqlConnection.QueryAsync(sqlQuery, param: new { ReplyToReplyId = replyToReplyId },
                transaction: dbTransaction);
        }

        // Вставка нового запису в таблицю RepliesToReply_Reply
        public async Task PutNewReplyToComment(int replyId, int replyToCommentId)
        {
            string sqlQuery = "INSERT INTO forum.RepliesToReply_Reply(ReplyId, ReplyToReplyId)" +
                " VALUES (@ReplyId, @ReplyToReplyId);";

            await sqlConnection.QueryAsync(sqlQuery, param: new { ReplyId = replyId, ReplyToReplyId = replyToCommentId },
                transaction: dbTransaction);
        }

        // Видалення значень з таблиці RepliesToReply_Reply
        public async Task DeleteReplyFromCommentAsync(int replyId, int replyToReplyId)
        {
            string sqlQuery = "DELETE FROM forum.RepliesToReply_Reply WHERE ReplyId = @ReplyId AND" +
                " ReplyToReplyId = @ReplyToReplyId;";

            await sqlConnection.QueryAsync(sqlQuery, param: new { ReplyId = replyId, ReplyToReplyId = replyToReplyId },
                transaction: dbTransaction);
        }

        // Отримання значення ReplyToReplyId з таблиці RepliesToReply_Reply, щоб перевірити зв'язаність
        public async Task<int> GetReplyToReplyIdAsync(int replyId, int replyToReplyId)
        {
            string sqlQuery = "SELECT TOP 1 ReplyToReplyId FROM forum.RepliesToReply_Reply WHERE" +
                " ReplyId = @ReplyId AND ReplyToReplyId = @ReplyToReplyId;";

            return sqlConnection.QueryFirst<int>(sqlQuery, param: new { ReplyId = replyId, ReplyToReplyId = replyToReplyId },
            transaction: dbTransaction);
        }

        // Отримання ReplyToReplyId з таблиці LikedRepliesToReply
        public async Task<int> GetReplyToReplyIdFromLikesAsync(int replyToReplyId, int userId)
        {
            string sqlQuery = "SELECT TOP 1 ReplyToReplyId FROM forum.LikedRepliesToReply WHERE" +
                " ReplyToReplyId = @ReplyToReplyId AND UserId = @UserId;";

            return sqlConnection.QueryFirst<int>(sqlQuery, param: new { ReplyToReplyId = replyToReplyId, UserId = userId },
                transaction: dbTransaction);
        }

        // Вставка нового запису в таблицю LikedRepliesToReply
        public async Task PostNewLikeForReplyAsync(int replyToReplyId, int userId)
        {
            string sqlQuery = "INSERT INTO forum.LikedRepliesToReply(ReplyToReplyId, UserId) VALUES (@ReplyToReplyId, @UserId);";

            await sqlConnection.QueryAsync(sqlQuery, param: new { ReplyToReplyId = replyToReplyId, UserId = userId },
                transaction: dbTransaction);
        }

        // Видалення запису з таблиці LikedRepliesToReply, щоб забрати лайк
        public async Task DeleteLikeFromReplyAsync(int replyToReplyId, int userId)
        {
            string sqlQuery = "DELETE FROM forum.LikedRepliesToReply WHERE ReplyToReplyId = @ReplyToReplyId AND" +
                " UserId = @UserId;";

            await sqlConnection.QueryAsync(sqlQuery, param: new { ReplyToReplyId = replyToReplyId, UserId = userId },
                transaction: dbTransaction);
        }
    }
}
