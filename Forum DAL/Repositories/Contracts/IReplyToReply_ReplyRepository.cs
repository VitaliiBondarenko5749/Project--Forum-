using Forum_DAL.Models;

namespace Forum_DAL.Repositories.Contracts
{
    public interface IReplyToReply_ReplyRepository : IGenericRepository<ReplyToReply_Reply>
    {
        // Отриманння RepliesToReplyId з таблиці RepliesToReply_Reply, які зв'язані з основною відповіддю
        Task<IEnumerable<int>> GetRepliesToReplyIdAsync(int replyId);

        // Видалення записів з таблиці RepliesToReply_Reply, ReplyId яких пов'язані з параметром
        Task DeleteRepliesToReplyAsync(int replyId);

        // Видалення значень з таблиці RepliesToReply_Reply
        Task DeleteReplyFromCommentAsync(int replyId, int replyToReplyId);

        // Отримання значення ReplyToReplyId з таблиці RepliesToReply_Reply, щоб перевірити зв'язаність
        Task<int> GetReplyToReplyIdAsync(int replyId, int replyToReplyId);
    }
}
