using Forum_DAL.Models;

namespace Forum_DAL.Repositories.Contracts
{
    public interface IReplyRepository : IGenericRepository<Reply> 
    {
        // Отримання всіх відповідей до поста
        Task<IEnumerable<Reply>> GetRepliesForPostAsync(int postId);

        // Отримання лайків для відповідей
        Task<int> GetLikesForReplyAsync(int replyId);

        // Отримання Id-значень з таблиці PostsReplies, які зв'язані з постом, який ми хочемо видалити
        Task<IEnumerable<int>> GetRepliesIdAsync(int postId);

        // Видалення значень з таблиці PostsReplies, PostId яких еквівалентний з Id постом, який ми хочемо видалити 
        Task DeleteAllRepliesFromPostsAsync(int postId);

        // Видалення всіх записів(лайків) з таблиці LikedReplies, ReplyId яких еквівалентні зі значеннями колекції repliesId
        Task DeleteAllLikesFromReplyAsync(int replyId);

        // Отриманння RepliesToReplyId з таблиці RepliesToReply_Reply, які зв'язані з основною відповіддю
        Task<IEnumerable<int>> GetRepliesToReplyIdAsync(int replyId);

        // Видалення записів з таблиці RepliesToReply_Reply, ReplyId яких пов'язані з параметром
        Task DeleteRepliesToReplyAsync(int replyId);

        // Вставка нового запису в таблицю PostsReplies
        Task PutPostAndGameAsync(int postId, int replyId);

        // Видалення записів з таблиці PostsReplies, ReplyId яких еквівалентно з параметром
        Task DeleteReplyFromPostAsync(int replyId);

        // Отримання UserId з таблиці LikedReplies, для первірки, чи поставив він вже лайк
        Task<int> GetUserIdFromLikedRepliesAsync(int userId, int replyId);

        // Вставити нові значення в таблицю LikedReplies
        Task PostNewLikeForReplyAsync(int replyId, int userId);

        // Отримання значення ReplyId з таблиці PostsReplies, для того щоб перевірити коментар та пост на зв'язаність
        Task<int> GetReplyIdAsync(int postId, int replyId);

        // Видалення запису з таблиці LikedReplies, щоб забрати лайк
        Task DeleteLikeFromReplyAsync(int replyId, int userId);
    }
}
