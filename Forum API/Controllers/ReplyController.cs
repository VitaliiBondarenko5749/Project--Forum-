using Catalog_of_Games_DAL.Entities;
using Forum_DAL.Models;
using Forum_DAL.Repositories.Contracts;
using Microsoft.AspNetCore.Mvc;

namespace Forum_API.Controllers
{
    [Route("api/posts")]
    [ApiController]
    public class ReplyController : ControllerBase
    {
        private readonly ILogger<ReplyController> logger;
        private IUnitOfWork unitOfWork;

        public ReplyController(ILogger<ReplyController> logger, IUnitOfWork unitOfWork)
        {
            this.logger = logger;
            this.unitOfWork = unitOfWork;
        }

        // Додавання коментаря до поста
        [HttpPost("{postId}/comments")]
        public async Task<ActionResult> PostCommentAsync(int postId, [FromBody] Reply reply)
        {
            try
            {
                if (reply == null)
                {
                    logger.LogInformation("Empty client-side json!");

                    return BadRequest("Object \"Reply\" type is null.");
                }

                if (!ModelState.IsValid)
                {
                    logger.LogInformation("Incorrect client-side json!");

                    return BadRequest("Incorrect object \"Reply\" type!");
                }

                Post post = await unitOfWork.PostRepository.GetAsync(postId);

                if (post == null)
                {
                    logger.LogInformation("Empty client-side json!");

                    return BadRequest("Object \"Post\" type is null.");
                }

                PostReply postReply = new PostReply()
                {
                    PostId = post.Id,
                    ReplyId = await unitOfWork.ReplyRepository.AddAsync(reply)
                };
            
                await unitOfWork.PostReplyRepository.AddAsync(postReply);
                
                unitOfWork.Commit();

                return Ok(reply);
            }
            catch (Exception ex)
            {
                logger.LogError($"Transaction fail! Something went wrong in \"PostCommentAsync(...)\" method. Error type:" +
                    $" {ex.Message}");

                return StatusCode(StatusCodes.Status500InternalServerError, "error");
            }
        }

        // Видалення коментаря
        [HttpDelete("{postId}/comments/{commentId}")]
        public async Task<ActionResult> DeleteCommentAsync(int postId, int commentId)
        {
            try
            {
                // Отримуємо сутність поста, щоб перевірити, чи існує взагалі відповідь з таким Id
                Post post = await unitOfWork.PostRepository.GetAsync(postId);

                if (post == null)
                {
                    logger.LogInformation($"Post with id: {postId}, was not found in the database!");

                    return NotFound();
                }

                // Отримуємо сутність відповіді, щоб перевірити, чи існує взагалі відповідь з таким Id
                Reply reply = await unitOfWork.ReplyRepository.GetAsync(commentId);

                if (reply == null)
                {
                    logger.LogInformation($"Reply with id: {commentId}, was not found in the database!");

                    return NotFound();
                }

                // Отримання значення ReplyId з таблиці PostsReplies, для того щоб перевірити коментар та пост на зв'язаність
                commentId = await unitOfWork.PostReplyRepository.GetReplyIdAsync(post.Id, reply.Id);

                // Видалення записів з таблиці PostsReplies, ReplyId яких еквівалентно з параметром
                await unitOfWork.PostReplyRepository.DeleteReplyFromPostAsync(reply.Id);

                // Видалення записів(лайків) з таблиці LikedReplies, ReplyId який еквівалентно зі значеннями колекції repliesId
                await unitOfWork.LikedReplyRepository.DeleteAllLikesFromReplyAsync(reply.Id);

                // Отриманння RepliesToReplyId з таблиці RepliesToReply_Reply, які зв'язані з основною відповіддю
                ICollection<int>? repliesToReplyId = (List<int>)await unitOfWork.ReplyToReply_ReplyRepository.GetRepliesToReplyIdAsync(reply.Id);

                // Видалення записів з таблиці RepliesToReply_Reply, ReplyId яких пов'язані з параметром
                await unitOfWork.ReplyToReply_ReplyRepository.DeleteRepliesToReplyAsync(reply.Id);

                foreach (int replyToReplyId in repliesToReplyId)
                {
                    // Видалення записів з таблиці LikedRepliesToReply, ReplyToReplyId який еквівалентно зі значеннями з колекції
                    await unitOfWork.LikedReplyToReplyRepository.DeleteLikedRepliesToReply(replyToReplyId);

                    // Видаляємо відповіді на відповіді
                    await unitOfWork.ReplyToReplyRepository.DeleteAsync(replyToReplyId);
                }

                // Видалення відповіді на пост
                await unitOfWork.ReplyRepository.DeleteAsync(reply.Id);

                unitOfWork.Commit();

                return Ok();
            }
            catch (Exception ex)
            {
                logger.LogError($"Transaction fail! Something went wrong in \"DeleteCommentAsync(...)\" method. Error type:" +
                   $" {ex.Message}");

                return StatusCode(StatusCodes.Status500InternalServerError, "error");
            }
        }

        // Додання лайку до коментаря
        [HttpPost("{postId}/comments/{commentId}/like")]
        public async Task<ActionResult> AddLikeToCommentAsync(int postId, int commentId, int userId)
        {
            try
            {
                // Перевіряємо чи існує пост з Id
                Post post = await unitOfWork.PostRepository.GetAsync(postId);

                if(post == null)
                {
                    return BadRequest("Object \"Post\" type is null.");
                }
                
                // Перевіряємо, чи існує такий коментар
                Reply reply = await unitOfWork.ReplyRepository.GetAsync(commentId);

                if (post == null)
                {
                    return BadRequest("Object \"Reply\" type is null.");
                }

                LikedReply likedReply = new LikedReply()
                {
                    // Отримання значення ReplyId з таблиці PostsReplies, для того щоб перевірити коментар та пост на зв'язаність
                    ReplyId = await unitOfWork.PostReplyRepository.GetReplyIdAsync(post.Id, reply.Id)
                };

                // Перевіримо, чи поставив вже користувач лайк
                try
                {
                    likedReply.UserId = await unitOfWork.LikedReplyRepository.GetUserIdFromLikedRepliesAsync(userId, likedReply.ReplyId);
                }
                catch
                {
                    likedReply.UserId = 0;
                }

                if (likedReply.UserId != 0)
                {
                    logger.LogInformation($"User with id:{userId} has already liked it.");

                    return BadRequest($"User with id:{userId} has already liked it.");
                }

                await unitOfWork.LikedReplyRepository.AddAsync(likedReply);

                unitOfWork.Commit();

                return Ok();
            }
            catch (Exception ex)
            {
                logger.LogError($"Transaction fail! Something went wrong in \"AddLikeToCommentAsync(...)\" method. Error type:" +
                    $" {ex.Message}");

                return StatusCode(StatusCodes.Status500InternalServerError, "error");
            }
        }

        // Видалення лайку з коментаря
        [HttpDelete("{postId}/comments/{commentId}/like")]
        public async Task<ActionResult> DeleteLikeFromCommentAsync(int postId, int commentId, int userId)
        {
            try
            {
                // Отримуємо сутність поста, щоб перевірити, чи існує взагалі відповідь з таким Id
                Post post = await unitOfWork.PostRepository.GetAsync(postId);

                if (post == null)
                {
                    logger.LogInformation($"Post with id: {postId}, was not found in the database!");

                    return NotFound();
                }

                // Отримуємо сутність відповіді, щоб перевірити, чи існує взагалі відповідь з таким Id
                Reply reply = await unitOfWork.ReplyRepository.GetAsync(commentId);

                if (reply == null)
                {
                    logger.LogInformation($"Reply with id: {commentId}, was not found in the database!");

                    return NotFound();
                }

                // Отримання значення ReplyId з таблиці PostsReplies, для того щоб перевірити коментар та пост на зв'язаність
                commentId = await unitOfWork.PostReplyRepository.GetReplyIdAsync(post.Id, reply.Id);

                // Видалення запису з таблиці LikedReplies, щоб забрати лайк
                await unitOfWork.LikedReplyRepository.DeleteLikeFromReplyAsync(commentId, userId);

                unitOfWork.Commit();

                return Ok();
            }
            catch(Exception ex)
            {
                logger.LogError($"Transaction fail! Something went wrong in \"DeleteLikeFromCommentAsync(...)\" method. Error type:" +
                    $" {ex.Message}");

                return StatusCode(StatusCodes.Status500InternalServerError, "error");
            }
        }
    }
}