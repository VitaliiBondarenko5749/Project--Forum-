using Dapper;
using Forum_DAL.Models;
using Forum_DAL.Repositories.Contracts;
using System.Data;
using System.Data.SqlClient;

namespace Forum_DAL.Repositories
{
    public class ReplyToReply_ReplyRepository : GenericRepository<ReplyToReply_Reply>, IReplyToReply_ReplyRepository
    {
        public ReplyToReply_ReplyRepository(SqlConnection sqlConnection, IDbTransaction dbTransaction)
            : base(sqlConnection, dbTransaction, "forum.RepliesToReply_Reply") { }

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

            return await sqlConnection.QueryFirstAsync<int>(sqlQuery, param: new { ReplyId = replyId, ReplyToReplyId = replyToReplyId },
            transaction: dbTransaction);
        }
    }
}
