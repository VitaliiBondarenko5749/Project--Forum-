using Forum_DAL.Models;

namespace Forum_DAL.Repositories.Contracts
{
    public interface IReplyRepository : IGenericRepository<Reply> 
    {
        // Отримання всіх відповідей до поста
        Task<IEnumerable<Reply>> GetRepliesForPostAsync(int postId);
    }
}
