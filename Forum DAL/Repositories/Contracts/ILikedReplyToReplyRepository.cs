using Forum_DAL.Models;

namespace Forum_DAL.Repositories.Contracts
{
    public interface ILikedReplyToReplyRepository : IGenericRepository<LikedReplyToReply>
    {
        // Отримуємо лайки відповідей на відповідь
        Task<int> GetLikesForReplyToReplyAsync(int replyToReplyId);

        // Видалення записів з таблиці LikedRepliesToReply, ReplyToReplyId який еквівалентно зі значеннями з колекції
        Task DeleteLikedRepliesToReply(int replyToReplyId);

        // Отримання ReplyToReplyId з таблиці LikedRepliesToReply
        Task<int> GetReplyToReplyIdFromLikesAsync(int replyToReplyId, int userId);

        // Видалення запису з таблиці LikedRepliesToReply, щоб забрати лайк
        Task DeleteLikeFromReplyAsync(int replyToReplyId, int userId);
    }
}
