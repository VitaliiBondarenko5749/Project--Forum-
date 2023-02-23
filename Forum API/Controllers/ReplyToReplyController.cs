using Forum_DAL.Models;
using Forum_DAL.Repositories.Contracts;
using Microsoft.AspNetCore.Mvc;

namespace Forum_API.Controllers
{
    [Route("api/[controller]")]
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

        [HttpPost]
        public async Task<ActionResult> PostReplyAsync([FromBody] ReplyToReply replyToReply)
        {
            try
            {
                if (replyToReply == null)
                {
                    logger.LogInformation("Empty client-side json!");

                    return BadRequest("Object \"ReplyToReply\" type is null.");
                }

                if (!ModelState.IsValid)
                {
                    logger.LogInformation("Incorrect client-side json!");

                    return BadRequest("Incorrect object \"ReplyToReply\" type!");
                }

                int createdId = await unitOfWork.ReplyToReplyRepository.AddAsync(replyToReply);

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

        [HttpPut("{id}")]
        public async Task<ActionResult> UpdateReplyAsync(int id, [FromBody] ReplyToReply replyToReply)
        {
            try
            {
                if (replyToReply == null)
                {
                    logger.LogInformation("Empty client-side json!");

                    return BadRequest("Object \"ReplyToReply\" type is null.");
                }

                if (!ModelState.IsValid)
                {
                    logger.LogInformation("Incorrect client-side json!");

                    return BadRequest("Incorrect object \"ReplyToReply\" type!");
                }

                ReplyToReply replyToReplyEntity = await unitOfWork.ReplyToReplyRepository.GetAsync(id);

                if (replyToReplyEntity == null)
                {
                    logger.LogInformation($"Reply to reply with id: {id}, was not found in the database!");

                    return NotFound();
                }

                await unitOfWork.ReplyToReplyRepository.ReplaceAsync(replyToReply);

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

        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteByIdAsync(int id)
        {
            try
            {
                ReplyToReply replyToReplyEntity = await unitOfWork.ReplyToReplyRepository.GetAsync(id);

                if (replyToReplyEntity == null)
                {
                    logger.LogInformation($"Reply to reply with id: {id}, was not found in the database!");

                    return NotFound();
                }

                await unitOfWork.ReplyToReplyRepository.DeleteAsync(id);

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
