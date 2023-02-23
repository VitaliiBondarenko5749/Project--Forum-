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
    }
}
