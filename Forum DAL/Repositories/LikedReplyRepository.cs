using Dapper;
using Forum_DAL.Models;
using Forum_DAL.Repositories.Contracts;
using System.Data;
using System.Data.SqlClient;

namespace Forum_DAL.Repositories
{
    public class LikedReplyRepository : GenericRepository<LikedReply>, ILikedReplyRepository
    {
        public LikedReplyRepository(SqlConnection sqlConnection, IDbTransaction dbTransaction)
            : base(sqlConnection, dbTransaction, "forum.LikedReplies") { }

        // Отримання лайків для відповідей
        public async Task<int> GetLikesForReplyAsync(int replyId)
        {
            string sqlQuery = "SELECT COUNT(ReplyId) FROM forum.LikedReplies WHERE ReplyId = @ReplyId;";

            return await sqlConnection.ExecuteScalarAsync<int>(sqlQuery, param: new { ReplyId = replyId },
                transaction: dbTransaction);
        }

        // Видалення записів(лайків) з таблиці LikedReplies, ReplyId який еквівалентно зі значеннями колекції repliesId
        public async Task DeleteAllLikesFromReplyAsync(int replyId)
        {
            string sqlQuery = "DELETE FROM forum.LikedReplies WHERE ReplyId = @ReplyId;";

            await sqlConnection.QueryAsync(sqlQuery, param: new { ReplyId = replyId }, transaction: dbTransaction);
        }

        // Отримання UserId з таблиці LikedReplies, для первірки, чи поставив він вже лайк
        public async Task<int> GetUserIdFromLikedRepliesAsync(int userId, int replyId)
        {
            string sqlQuery = "SELECT TOP 1 UserId FROM forum.LikedReplies WHERE UserId = @UserId AND ReplyId = @ReplyId;";

            return sqlConnection.QueryFirst<int>(sqlQuery, param: new { UserId = userId, ReplyId = replyId },
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
