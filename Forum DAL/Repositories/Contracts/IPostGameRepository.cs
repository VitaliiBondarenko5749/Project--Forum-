using Forum_DAL.Models;

namespace Forum_DAL.Repositories.Contracts
{
    public interface IPostGameRepository : IGenericRepository<PostGame>
    {
        // Видалення ігор з таблиці PostsGames
        Task DeleteGamesFromPostAsync(int postId);

        // Отримання айдішок всіх ігор з таблиці PostsGames, PostId яких еквівалентні з параметром
        Task<IEnumerable<int>> GetGamesIdAsync(int postId);
    }
}
