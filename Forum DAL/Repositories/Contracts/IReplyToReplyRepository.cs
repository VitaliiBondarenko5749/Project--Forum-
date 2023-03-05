using Forum_DAL.Models;

namespace Forum_DAL.Repositories.Contracts
{
    public interface IReplyToReplyRepository : IGenericRepository<ReplyToReply> 
    {
        // Отримуємо всі відповіді на основну
        Task<IEnumerable<ReplyToReply>> GetRepliesToReplyAsync(int replyId);
    }
}
