using Forum_DAL.Models;
using Forum_DAL.Repositories.Contracts;
using System.Data;
using System.Data.SqlClient;

namespace Forum_DAL.Repositories
{
    public class ReplyToReplyRepository : GenericRepository<ReplyToReply>, IReplyToReplyRepository
    {
        public ReplyToReplyRepository(SqlConnection sqlConnection, IDbTransaction dbTransaction)
            : base(sqlConnection, dbTransaction, "forum.ReplyToReplies") { }
    }
}
