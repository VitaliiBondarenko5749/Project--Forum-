using Catalog_of_Games_DAL.Entities;
using Forum_DAL.Models;
using Forum_DAL.Repositories.Contracts;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using System.Linq;

namespace Forum_API.Controllers
{
    [Route("api/posts")]
    [ApiController]
    public class PostController : ControllerBase
    {
        private readonly ILogger<PostController> logger;
        private IUnitOfWork unitOfWork;

        public PostController(ILogger<PostController> logger, IUnitOfWork unitOfWork)
        {
            this.logger = logger;
            this.unitOfWork = unitOfWork;
        }

        // Отримання всіх постів 
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Post>>> GetAllPostsAsync()
        {
            try
            {
                IEnumerable<Post> postLists = await unitOfWork.PostRepository.GetAllAsync();

                foreach (Post post in postLists)
                {
                    post.Games = (List<Game>)await unitOfWork.GameRepository.GetGamesForPostAsync(post.Id);
                }

                logger.LogInformation("Get all values from the database!");

                return Ok(postLists);
            }
            catch (Exception ex)
            {
                logger.LogError($"Transaction fail! Something went wrong in \"GetAllPostsAsync()\" method. Error type:" +
                    $" {ex.Message}");

                return StatusCode(StatusCodes.Status500InternalServerError, "error");
            }
        }

        // Отримання поста за Id
        [HttpGet("{postId}")]
        public async Task<ActionResult<Post>> GetByIdAsync(int postId)
        {
            try
            {
                Post post = await unitOfWork.PostRepository.GetAsync(postId);

                if (post == null)
                {
                    logger.LogInformation($"Post with id: {postId}, was not found in the database.");

                    return NotFound();
                }

                // Отримання ігор, які зв'язані з постом
                post.Games = (List<Game>)await unitOfWork.GameRepository.GetGamesForPostAsync(post.Id);

                // Отримання коментарів, які зв'язані з постом
                post.Replies = (List<Reply>)await unitOfWork.ReplyRepository.GetRepliesForPostAsync(post.Id);

                foreach (Reply reply in post.Replies)
                {
                    // Отримання кількоті лайків, які зв'язані з коментарем
                    reply.NumberOfLikes = await unitOfWork.ReplyRepository.GetLikesForReplyAsync(reply.Id);

                    // Отрмання всіх відповідей на коментар
                    reply.RepliesToReply = (List<ReplyToReply>)await
                        unitOfWork.ReplyToReplyRepository.GetRepliesToReplyAsync(reply.Id);

                    foreach (ReplyToReply replyToReply in reply.RepliesToReply)
                    {
                        // Отримання кількості лайків на всі відповіді на коментвр
                        replyToReply.NumberOfLikes = await unitOfWork.ReplyToReplyRepository
                            .GetLikesForReplyToReplyAsync(replyToReply.Id);
                    }
                }

                logger.LogInformation("Got Post from the database!");

                return Ok(post);
            }
            catch (Exception ex)
            {
                logger.LogError($"Transaction fail! Something went wrong in \"GetByIdAsync(...)\" method. Error type:" +
                    $" {ex.Message}");

                return StatusCode(StatusCodes.Status500InternalServerError, "error");
            }
        }

        // Вставка нового поста
        [HttpPost]
        public async Task<ActionResult> PostPostAsync([FromBody] Post post)
        {
            try
            {
                if (post == null)
                {
                    logger.LogInformation("Empty client-side json!");

                    return BadRequest("Object \"Post\" type is null.");
                }

                if (!ModelState.IsValid)
                {
                    logger.LogInformation("Incorrect client-side json!");

                    return BadRequest("Incorrect object \"Post\" type!");
                }

                int createdId = await unitOfWork.PostRepository.AddAsync(post);

                foreach (Game game in post.Games)
                {
                    try
                    {
                        int gameId = await unitOfWork.GameRepository.GetGameByNameAsync(game.GmName);
                        await unitOfWork.PostRepository.PutPostAndGameAsync(createdId, gameId);
                    }
                    catch
                    {
                        continue;
                    }
                }

                unitOfWork.Commit();

                return StatusCode(StatusCodes.Status201Created);
            }
            catch (Exception ex)
            {
                logger.LogError($"Transaction fail! Something went wrong in \"PostPostAsync(...)\" method. Error type:" +
                    $" {ex.Message}");

                return StatusCode(StatusCodes.Status500InternalServerError, "error");
            }
        }

        // Оновлення поста за Id
        [HttpPut("{postId}")]
        public async Task<ActionResult> UpdatePostAsync(int postId, [FromBody] Post post)
        {
            try
            {
                if (post == null)
                {
                    logger.LogInformation("Empty client-side json!");

                    return BadRequest("Object \"Post\" type is null.");
                }

                if (!ModelState.IsValid)
                {
                    logger.LogInformation("Incorrect client-side json!");

                    return BadRequest("Incorrect object \"Post\" type!");
                }

                Post postEntity = await unitOfWork.PostRepository.GetAsync(postId);

                if (postEntity == null)
                {
                    logger.LogInformation($"Post with id: {postId}, was not found in the database!");

                    return NotFound();
                }

                post.Id = postId;

                // Отримання айдішок всіх ігор з таблиці PostsGames, PostId яких еквівалентний з параметром
                ICollection<int>? gamesId = (List<int>)await unitOfWork.PostRepository.GetGamesIdAsync(post.Id);

                // Видалення з таблиці PostsGames
                await unitOfWork.PostRepository.DeleteGamesFromPostAsync(post.Id);

                foreach (Game game in post.Games)
                {
                    try
                    {
                        // Пробуємо отримати Id заданої нами гри
                        int gameId = await unitOfWork.GameRepository.GetGameByNameAsync(game.GmName);

                        if (!gamesId.Contains(gameId)) // Якщо значення айді гри немає в колекції 
                        {
                            // Вставка нових значень в таблицю PostsGames
                            await unitOfWork.PostRepository.PutPostAndGameAsync(post.Id, gameId);
                        }
                    }
                    catch
                    {
                        // При невдалому отриманні айді гри, нам викине вийняток і почне нову ітерацію
                        continue;
                    }
                }

                // Оновлюємо пост
                await unitOfWork.PostRepository.ReplaceAsync(post);

                unitOfWork.Commit();

                return StatusCode(StatusCodes.Status204NoContent);
            }
            catch (Exception ex)
            {
                logger.LogError($"Transaction fail! Something went wrong in \"UpdatePostAsync(...)\" method. Error type:" +
                    $" {ex.Message}");

                return StatusCode(StatusCodes.Status500InternalServerError, "error");
            }
        }

        // Видалення поста за Id
        [HttpDelete("{postId}")]
        public async Task<ActionResult> DeleteByIdAsync(int postId)
        {
            try
            {
                // Отримуємо сутність поста
                Post postEntity = await unitOfWork.PostRepository.GetAsync(postId);

                if (postEntity == null)
                {
                    logger.LogInformation($"Post with id: {postId}, was not found in the database!");

                    return NotFound();
                }

                //Видаляємо всі записи з таблиці PostsGames PostId який еквівалентний параметру id у методі
                await unitOfWork.PostRepository.DeleteGamesFromPostAsync(postId);

                #region PostsReplies

                // Отримання Id-значень з таблиці PostsReplies, які зв'язані з постом, який ми хочемо видалити
                ICollection<int>? repliesId = (List<int>)await unitOfWork.ReplyRepository.GetRepliesIdAsync(postId);

                // Видалення значень з таблиці PostsReplies, PostId яких еквівалентний з Id постом, який ми хочемо видалити
                await unitOfWork.ReplyRepository.DeleteAllRepliesFromPostsAsync(postId);

                #endregion

                ICollection<int> repliesToReplyId = new List<int>();

                foreach (int replyId in repliesId)
                {
                    // Видалення записів(лайків) з таблиці LikedReplies, ReplyId який еквівалентно зі значеннями колекції repliesId
                    await unitOfWork.ReplyRepository.DeleteAllLikesFromReplyAsync(replyId);

                    // Отриманння RepliesToReplyId з таблиці RepliesToReply_Reply, які зв'язані з основною відповіддю
                    ICollection<int>? secRepliesToReplyId = (List<int>)await unitOfWork.ReplyRepository.GetRepliesToReplyIdAsync(replyId);

                    repliesToReplyId.Union(secRepliesToReplyId); // Записуємо значення в основну колекцію з тимчасової

                    // Видалення записів з таблиці RepliesToReply_Reply, ReplyId яких пов'язані з параметром
                    await unitOfWork.ReplyRepository.DeleteRepliesToReplyAsync(replyId);
                }

                foreach (int replyToReplyId in repliesToReplyId.Distinct())
                {
                    // Видалення записів з таблиці LikedRepliesToReply, ReplyToReplyId який еквівалентно зі значеннями з колекції
                    await unitOfWork.ReplyToReplyRepository.DeleteLikedRepliesToReply(replyToReplyId);

                    // Видаляємо відповіді на відповіді
                    await unitOfWork.ReplyToReplyRepository.DeleteAsync(replyToReplyId);
                }

                foreach (int replyId in repliesId.Distinct())
                {
                    // Видалення всіх відповідей на пост
                    await unitOfWork.ReplyRepository.DeleteAsync(replyId);
                }

                // Видалення самого поста
                await unitOfWork.PostRepository.DeleteAsync(postId);

                unitOfWork.Commit();

                return NoContent();
            }
            catch (Exception ex)
            {
                logger.LogError($"Transaction fail! Something went wrong in \"DeleteByIdAsync(...)\" method. Error type:" +
                   $" {ex.Message}");

                return StatusCode(StatusCodes.Status500InternalServerError, "error");
            }
        }
    }
}