using Forum_DAL.Models;

namespace Forum_DAL.Repositories.Contracts
{
    public interface IReplyToReplyRepository : IGenericRepository<ReplyToReply> 
    {
        // Отримуємо всі відповіді на основну
        Task<IEnumerable<ReplyToReply>> GetRepliesToReplyAsync(int replyId);

        // Отримуємо лайки відповідей на відповідь
        Task<int> GetLikesForReplyToReplyAsync(int replyToReplyId);

        // Видалення записів з таблиці LikedRepliesToReply, ReplyToReplyId який еквівалентно зі значеннями з колекції
        Task DeleteLikedRepliesToReply(int replyToReplyId);

        // Вставка нового запису в таблицю RepliesToReply_Reply
        Task PutNewReplyToComment(int replyId, int replyToCommentId);

        // Видалення значень з таблиці RepliesToReply_Reply
        Task DeleteReplyFromCommentAsync(int replyId, int replyToReplyId);

        // Отримання значення ReplyToReplyId з таблиці RepliesToReply_Reply, щоб перевірити зв'язаність
        Task<int> GetReplyToReplyIdAsync(int replyId, int replyToReplyId);

        // Отримання ReplyToReplyId з таблиці LikedRepliesToReply
        Task<int> GetReplyToReplyIdFromLikesAsync(int replyToReplyId, int userId);

        // Вставка нового запису в таблицю LikedRepliesToReply
        Task PostNewLikeForReplyAsync(int replyToReplyId, int userId);

        // Видалення запису з таблиці LikedRepliesToReply, щоб забрати лайк
        Task DeleteLikeFromReplyAsync(int replyToReplyId, int userId);
    }
}
