using clubescom.manager.Controllers.Utils;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using clubescom.manager.models;
using Microsoft.AspNetCore.Authorization;

namespace clubescom.manager.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PostsController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly IConfiguration _configuration;

        public PostsController(AppDbContext context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
        }

        // GET: api/Posts
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Post>>> GetPosts()
        {
            var posts = await _context.Posts.Where(p => p.enabled).ToListAsync();

            var postDtos = posts.Select(post => new PostDto(post, _configuration)).ToList();

            return Ok(postDtos);
        }

        // GET: api/Posts/{id}
        [HttpGet("{id}")]
        public async Task<ActionResult<PostDto>> GetPost(Guid id)
        {
            var post = await _context.Posts.FindAsync(id);

            if (post == null || !post.enabled)
            {
                return NotFound();
            }

            return Ok(new PostDto(post, _configuration));
        }

        // GET: api/Posts/ByClub?clubId={clubId}
        [HttpGet("ByClub/{clubId}")]
        public async Task<ActionResult<IEnumerable<PostDto>>> GetPosts([FromQuery] Guid clubId)
        {
            var posts = await _context.Posts.Where(p => (p.Club.ID == clubId) && (p.enabled)).ToListAsync();

            if (!posts.Any())
            {
                return NotFound();
            }

            var postDtos = posts.Select(post => new PostDto(post, _configuration)).ToList();

            return Ok(postDtos);
        }

        // POST: api/Posts
        [HttpPost]
        [Authorize]
        public async Task<ActionResult<Post>> CreatePost(Post post, IFormFile preview)
        {
            var authenticatedUser = await _context.Users.FindAsync(User.Identity.Name);

            if (authenticatedUser != null && authenticatedUser.Id != post.Club.President.Id)
            {
                return Forbid();
            }

            if (ImageManager.IsValidFile(preview))
            {
                post.Preview = await ImageManager.New(preview,
                    Path.Combine(Directory.GetCurrentDirectory(), "wwwroot",
                        _configuration.GetValue<string>("PreviewsPath")));
            }
            else
            {
                post.Preview = null;
            }

            _context.Posts.Add(post);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetPost), new { id = post.ID }, post);
        }

        // PUT: api/Post/{id}
        [HttpPut("{id}")]
        [Authorize]
        public async Task<IActionResult> UpdatePost(Guid id, Post post, IFormFile preview)
        {
            var authenticatedUser = await _context.Users.FindAsync(User.Identity.Name);

            if (authenticatedUser != null && authenticatedUser.Id != post.Club.President.Id)
            {
                return Forbid();
            }

            if (id != post.ID)
            {
                return BadRequest();
            }

            if (preview != null)
            {
                if (!ImageManager.IsValidFile(preview))
                {
                    return BadRequest("Invalid file type for logo. Only image files are allowed.");
                }

                post.Preview = await ImageManager.NewOrReplace(preview,
                    Path.Combine(Directory.GetCurrentDirectory(), "wwwroot",
                        _configuration.GetValue<string>("PreviewsPath")));
            }

            _context.Entry(post).State = EntityState.Modified;
            await _context.SaveChangesAsync();

            return NoContent();
        }

        // DELETE: api/Post/{id}
        [HttpDelete("{id}")]
        [Authorize]
        public async Task<IActionResult> DeletePost(Guid id)
        {
            var post = await _context.Posts.FindAsync(id);

            var authenticatedUser = await _context.Users.FindAsync(User.Identity.Name);

            if (authenticatedUser != null && authenticatedUser.Id != post.Club.President.Id)
            {
                return Forbid();
            }


            if (post == null)
            {
                return NotFound();
            }

            ImageManager.Delete(post.Preview);

            _context.Posts.Remove(post);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}