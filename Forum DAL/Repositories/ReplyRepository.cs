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
    }
}
