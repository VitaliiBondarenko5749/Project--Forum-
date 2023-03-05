using Forum_DAL.Models;

namespace Forum_DAL.Repositories.Contracts
{
    public interface IPostReplyRepository : IGenericRepository<PostReply> 
    {
        // Отримання Id-значень з таблиці PostsReplies, які зв'язані з постом, який ми хочемо видалити
        Task<IEnumerable<int>> GetRepliesIdAsync(int postId);

        // Видалення значень з таблиці PostsReplies, PostId яких еквівалентний з Id постом, який ми хочемо видалити 
        Task DeleteAllRepliesFromPostsAsync(int postId);

        // Видалення записів з таблиці PostsReplies, ReplyId яких еквівалентно з параметром
        Task DeleteReplyFromPostAsync(int replyId);

        // Отримання значення ReplyId з таблиці PostsReplies, для того щоб перевірити коментар та пост на зв'язаність
        Task<int> GetReplyIdAsync(int postId, int replyId);
    }
}
