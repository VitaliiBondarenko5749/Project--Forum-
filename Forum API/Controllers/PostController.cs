using Catalog_of_Games_DAL.Entities;
using Forum_DAL.Models;
using Forum_DAL.Repositories.Contracts;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using System.Linq;

namespace Forum_API.Controllers
{
    [Route("api/[controller]")]
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

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Post>>> GetAllPostsAsync()
        {
            try
            {
                IEnumerable<Post> postLists = await unitOfWork.PostRepository.GetAllAsync();

                unitOfWork.Commit();

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

        [HttpGet("{id}")]
        public async Task<ActionResult<Post>> GetByIdAsync(int id)
        {
            try
            {
                Post post = await unitOfWork.PostRepository.GetAsync(id);

                if (post == null)
                {
                    logger.LogInformation($"Post with id: {id}, was not found in the database.");

                    return NotFound();
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
        public async Task<ActionResult> UpdatePostAsync(int id, [FromBody] Post post)
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

                Post postEntity = await unitOfWork.PostRepository.GetAsync(id);

                if(postEntity == null)
                {
                    logger.LogInformation($"Post with id: {id}, was not found in the database!");

                    return NotFound();
                }

                await unitOfWork.PostRepository.ReplaceAsync(post);

                unitOfWork.Commit();

                return StatusCode(StatusCodes.Status204NoContent);
            }
            catch(Exception ex)
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
                Post postEntity = await unitOfWork.PostRepository.GetAsync(id);

                if(postEntity == null)
                {
                    logger.LogInformation($"Post with id: {id}, was not found in the database!");

                    return NotFound();
                }

                await unitOfWork.PostRepository.DeleteAsync(id);
                
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