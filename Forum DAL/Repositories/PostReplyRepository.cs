using Forum_DAL.Models;
using Forum_DAL.Repositories.Contracts;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dapper;

namespace Forum_DAL.Repositories
{
    public class PostReplyRepository : GenericRepository<PostReply>, IPostReplyRepository
    {
        public PostReplyRepository(SqlConnection sqlConnection, IDbTransaction dbTransaction)
            : base(sqlConnection, dbTransaction, "forum.PostsReplies") { }

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

        // Видалення записів з таблиці PostsReplies, ReplyId яких еквівалентно з параметром
        public async Task DeleteReplyFromPostAsync(int replyId)
        {
            string sqlQuery = "DELETE FROM forum.PostsReplies WHERE ReplyId = @ReplyId;";

            await sqlConnection.QueryAsync(sqlQuery, param: new { ReplyId = replyId }, transaction: dbTransaction);
        }

        // Отримання значення ReplyId з таблиці PostsReplies, для того щоб перевірити коментар та пост на зв'язаність
        public async Task<int> GetReplyIdAsync(int postId, int replyId)
        {
            string sqlQuery = "SELECT TOP 1 ReplyId FROM forum.PostsReplies WHERE PostId = @PostId AND ReplyId = @ReplyId;";

            return sqlConnection.QueryFirst<int>(sqlQuery, param: new { PostId = postId, ReplyId = replyId },
                transaction: dbTransaction);
        } 
    }
}
