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
    }
}
