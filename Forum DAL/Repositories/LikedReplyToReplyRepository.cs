using Dapper;
using Forum_DAL.Models;
using Forum_DAL.Repositories.Contracts;
using System.Data;
using System.Data.SqlClient;

namespace Forum_DAL.Repositories
{
    public class LikedReplyToReplyRepository : GenericRepository<LikedReplyToReply>, ILikedReplyToReplyRepository
    {
        public LikedReplyToReplyRepository(SqlConnection sqlConnection, IDbTransaction dbTransaction)
            : base(sqlConnection, dbTransaction, "forum.LikedRepliesToReply") { }

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

        // Отримання ReplyToReplyId з таблиці LikedRepliesToReply
        public async Task<int> GetReplyToReplyIdFromLikesAsync(int replyToReplyId, int userId)
        {
            string sqlQuery = "SELECT TOP 1 ReplyToReplyId FROM forum.LikedRepliesToReply WHERE" +
                " ReplyToReplyId = @ReplyToReplyId AND UserId = @UserId;";

            return await sqlConnection.QueryFirstAsync<int>(sqlQuery, param: new { ReplyToReplyId = replyToReplyId, UserId = userId },
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
