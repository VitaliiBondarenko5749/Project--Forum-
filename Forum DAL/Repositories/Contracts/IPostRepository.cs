using Forum_DAL.Models;

namespace Forum_DAL.Repositories.Contracts
{
    public interface IPostRepository : IGenericRepository<Post> 
    {
        // Вставити значення в PostsGames, яка зв'язує пости та ігри
        Task PutPostAndGameAsync(int postId, int gameId);

        // Видалення ігор з таблиці PostsGames
        Task DeleteGamesFromPostAsync(int postId);

        // Отримання айдішок всіх ігор з таблиці PostsGames, PostId яких еквівалентні з параметром
        Task<IEnumerable<int>> GetGamesIdAsync(int postId);
    }
}
