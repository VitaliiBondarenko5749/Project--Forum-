using Forum_DAL.Models;
using Forum_DAL.Repositories.Contracts;
using Microsoft.AspNetCore.Mvc;

namespace Forum_API.Controllers
{
    [Route("api/posts")]
    [ApiController]
    public class ReplyToReplyController : ControllerBase
    {
        private readonly ILogger<ReplyToReplyController> logger;
        private IUnitOfWork unitOfWork;

        public ReplyToReplyController(ILogger<ReplyToReplyController> logger, IUnitOfWork unitOfWork)
        {
            this.logger = logger;
            this.unitOfWork = unitOfWork;
        }

        // Додавання відповіді коментаря до поста
        [HttpPost("{postId}/comments/{commentId}/replies")]
        public async Task<ActionResult> PostReplyToCommentAsync(int postId, int commentId,
            [FromBody] ReplyToReply replyToComment)
        {
            try
            {
                if (replyToComment == null)
                {
                    logger.LogInformation("Empty client-side json!");

                    return BadRequest("Object \"ReplyToReply\" type is null.");
                }

                if (!ModelState.IsValid)
                {
                    logger.LogInformation("Incorrect client-side json!");

                    return BadRequest("Incorrect object \"ReplyToReply\" type!");
                }

                // Отримуємо сутність поста
                Post post = await unitOfWork.PostRepository.GetAsync(postId);

                if (post == null)
                {
                    logger.LogInformation("Empty client-side json!");

                    return BadRequest("Object \"Post\" type is null.");
                }

                // Отримуємо сутність коментаря
                Reply reply = await unitOfWork.ReplyRepository.GetAsync(commentId);

                if (reply == null)
                {
                    logger.LogInformation("Empty client-side json!");

                    return BadRequest("Object \"Post\" type is null.");
                }

                ReplyToReply_Reply replyToReply_Reply = new ReplyToReply_Reply()
                {
                    // Перевіряємо чи зв'язані потс і коментар
                    ReplyId = await unitOfWork.PostReplyRepository.GetReplyIdAsync(post.Id, reply.Id),

                    // Додаємо нову відповідь на коментар
                    ReplyToReplyId = await unitOfWork.ReplyToReplyRepository.AddAsync(replyToComment)
                };

                await unitOfWork.ReplyToReply_ReplyRepository.AddAsync(replyToReply_Reply);

                unitOfWork.Commit();

                return Ok(replyToComment);
            }
            catch (Exception ex)
            {
                logger.LogError($"Transaction fail! Something went wrong in \"PostReplyToCommentAsync(...)\" method. Error type:" +
                    $" {ex.Message}");

                return StatusCode(StatusCodes.Status500InternalServerError, "error");
            }
        }

        // Видалення відповіді
        [HttpDelete("{postId}/comments/{commentId}/replies/{replyId}")]
        public async Task<ActionResult> DeleteReplyFromCommentAsync(int postId, int commentId, int replyId)
        {
            try
            {
                // Отримуємо сутність поста
                Post post = await unitOfWork.PostRepository.GetAsync(postId);

                if (post == null)
                {
                    return BadRequest("Object \"Post\" type is null.");
                }

                // Отримуємо сутність коментаря
                Reply reply = await unitOfWork.ReplyRepository.GetAsync(commentId);

                if (reply == null)
                {
                    return BadRequest("Object \"Post\" type is null.");
                }

                // Перевіряємо чи зв'язані потс і коментар
                commentId = await unitOfWork.PostReplyRepository.GetReplyIdAsync(post.Id, reply.Id);

                ReplyToReply replyToReply = await unitOfWork.ReplyToReplyRepository.GetAsync(replyId);

                if (replyToReply == null)
                {
                    return BadRequest("Object \"ReplyToReply\" is null.");
                }

                // Видалення запису з таблиці RepliesToReply_Reply
                await unitOfWork.ReplyToReply_ReplyRepository.DeleteReplyFromCommentAsync(reply.Id, replyToReply.Id);

                // Видалення записів з таблиці LikedRepliesToReply
                await unitOfWork.LikedReplyToReplyRepository.DeleteLikedRepliesToReply(replyToReply.Id);

                // Видалення самої відповіді
                await unitOfWork.ReplyToReplyRepository.DeleteAsync(replyToReply.Id);

                unitOfWork.Commit();

                return Ok();
            }
            catch (Exception ex)
            {
                logger.LogError($"Transaction fail! Something went wrong in \"DeleteReplyFromCommentAsync(...)\" method. Error type:" +
                    $" {ex.Message}");

                return StatusCode(StatusCodes.Status500InternalServerError, "error");
            }
        }

        // Додання лайку до відповіді на коментар
        [HttpPost("/posts/{postId}/comments/{commentId}/replies/{replyId}/like")]
        public async Task<ActionResult> AddLikeToReplyAsync(int postId, int commentId, int replyId, int userId)
        {
            try
            {
                // Перевіряємо чи існує пост з Id
                Post post = await unitOfWork.PostRepository.GetAsync(postId);

                if (post == null)
                {
                    return BadRequest("Object \"Post\" type is null.");
                }

                // Перевіряємо, чи існує такий коментар
                Reply reply = await unitOfWork.ReplyRepository.GetAsync(commentId);

                if (post == null)
                {
                    return BadRequest("Object \"Reply\" type is null.");
                }

                // Отримання значення ReplyId з таблиці PostsReplies, для того щоб перевірити коментар та пост на зв'язаність
                commentId = await unitOfWork.PostReplyRepository.GetReplyIdAsync(post.Id, reply.Id);

                // Перевіряємо чи існує така відповідь на коментар
                ReplyToReply replyToReply = await unitOfWork.ReplyToReplyRepository.GetAsync(replyId);

                if (replyToReply == null)
                {
                    return BadRequest("Object \"ReplyToReply\" type is null.");
                }

                // Перевіряємо відповідь і сам коментар на зв'язаність
                replyId = await unitOfWork.ReplyToReply_ReplyRepository.GetReplyToReplyIdAsync(reply.Id, replyToReply.Id);

                int newUserId = 0;

                // Перевіримо, чи поставив вже користувач лайк
                try
                {
                    newUserId = await unitOfWork.LikedReplyToReplyRepository.GetReplyToReplyIdFromLikesAsync(commentId, userId);
                }
                catch
                {
                    newUserId = 0;
                }

                if (newUserId != 0)
                {
                    logger.LogInformation($"User with id:{userId} has already liked it.");

                    return BadRequest($"User with id:{userId} has already liked it.");
                }

                LikedReplyToReply likedReplyToReply = new LikedReplyToReply()
                {
                    UserId = userId,
                    ReplyToReplyId = commentId
                };
               
                // Вставка нового лайку
                await unitOfWork.LikedReplyToReplyRepository.AddAsync(likedReplyToReply);

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

        // Видалення лайку з відповіді на коментар
        [HttpDelete("/posts/{postId}/comments/{commentId}/replies/{replyId}/like")]
        public async Task<ActionResult> DeleteLikeToReplyAsync(int postId, int commentId, int replyId, int userId)
        {
            try
            {
                // Перевіряємо чи існує пост з Id
                Post post = await unitOfWork.PostRepository.GetAsync(postId);

                if (post == null)
                {
                    return BadRequest("Object \"Post\" type is null.");
                }

                // Перевіряємо, чи існує такий коментар
                Reply reply = await unitOfWork.ReplyRepository.GetAsync(commentId);

                if (post == null)
                {
                    return BadRequest("Object \"Reply\" type is null.");
                }

                // Отримання значення ReplyId з таблиці PostsReplies, для того щоб перевірити коментар та пост на зв'язаність
                commentId = await unitOfWork.PostReplyRepository.GetReplyIdAsync(post.Id, reply.Id);

                // Перевіряємо чи існує така відповідь на коментар
                ReplyToReply replyToReply = await unitOfWork.ReplyToReplyRepository.GetAsync(replyId);

                if (replyToReply == null)
                {
                    return BadRequest("Object \"ReplyToReply\" type is null.");
                }

                await unitOfWork.LikedReplyToReplyRepository.DeleteLikeFromReplyAsync(replyToReply.Id, userId);

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
    }
}
