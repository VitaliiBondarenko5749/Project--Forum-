using Forum_DAL.Models;
using Forum_DAL.Repositories.Contracts;
using Microsoft.AspNetCore.Mvc;

namespace Forum_API.Controllers
{
    [Route("api/[controller]")]
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

        [HttpPost]
        public async Task<ActionResult> PostReplyAsync([FromBody] Reply reply)
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

                int createdId = await unitOfWork.ReplyRepository.AddAsync(reply);

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
        public async Task<ActionResult> UpdateReplyAsync(int id, [FromBody] Reply reply)
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

                Reply replyEntity = await unitOfWork.ReplyRepository.GetAsync(id);

                if (replyEntity == null)
                {
                    logger.LogInformation($"Reply with id: {id}, was not found in the database!");

                    return NotFound();
                }

                await unitOfWork.ReplyRepository.ReplaceAsync(reply);

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
                Reply postEntity = await unitOfWork.ReplyRepository.GetAsync(id);

                if (postEntity == null)
                {
                    logger.LogInformation($"Reply with id: {id}, was not found in the database!");

                    return NotFound();
                }

                await unitOfWork.ReplyRepository.DeleteAsync(id);

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
