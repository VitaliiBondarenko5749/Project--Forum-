using Forum_DAL.Models;

namespace Forum_DAL.Repositories.Contracts
{
    public interface ILikedReplyRepository : IGenericRepository<LikedReply>
    {
        // Отримання лайків для відповідей
        Task<int> GetLikesForReplyAsync(int replyId);

        // Видалення всіх записів(лайків) з таблиці LikedReplies, ReplyId яких еквівалентні зі значеннями колекції repliesId
        Task DeleteAllLikesFromReplyAsync(int replyId);

        // Отримання UserId з таблиці LikedReplies, для первірки, чи поставив він вже лайк
        Task<int> GetUserIdFromLikedRepliesAsync(int userId, int replyId);

        // Видалення запису з таблиці LikedReplies, щоб забрати лайк
        Task DeleteLikeFromReplyAsync(int replyId, int userId);
    }
}
