using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using clubescom.manager.models;
using Microsoft.AspNetCore.Authorization;

namespace clubescom.manager.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PostController : ControllerBase
    {
        private readonly AppDbContext _context; // Replace 'YourDbContext' with your actual DbContext class

        public PostController(AppDbContext context)
        {
            _context = context;
        }

        // GET: api/Post
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Post>>> GetPosts()
        {
            var posts = await _context.Posts.Where(p => p.enabled).ToListAsync();

            var postDtos = posts.Select(post => new PostDto(post)).ToList();

            return Ok(postDtos);
        }

        // GET: api/Post/{id}
        [HttpGet("{id}")]
        public async Task<ActionResult<PostDto>> GetPost(Guid id)
        {
            var post = await _context.Posts.FindAsync(id);

            if (post == null || !post.enabled)
            {
                return NotFound();
            }

            return Ok(new PostDto(post));
        }

        // GET: api/Post?clubId={clubId}
        [HttpGet]
        public async Task<ActionResult<IEnumerable<PostDto>>> GetPosts([FromQuery] Guid clubId)
        {
            var posts = await _context.Posts.Where(p => (p.Club.ID == clubId) && (p.enabled)).ToListAsync();

            if (!posts.Any())
            {
                return NotFound();
            }

            var postDtos = posts.Select(post => new PostDto(post)).ToList();

            return Ok(postDtos);
        }

        // POST: api/Post
        [HttpPost]
        [Authorize]
        public async Task<ActionResult<Post>> CreatePost(Post post, IFormFile preview)
        {
            var authenticatedUser = await _context.Users.FindAsync(User.Identity.Name);

            if (authenticatedUser != null && authenticatedUser.ID != post.Club.ID)
            {
                return Forbid();
            }

            if (preview != null)
            {
                var uploads = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "posts", "Images");
                if (!Directory.Exists(uploads))
                {
                    Directory.CreateDirectory(uploads);
                }

                // Check if the file is an image
                var contentType = preview.ContentType;
                var isImage = contentType.StartsWith("image/");
                if (!isImage)
                {
                    return BadRequest("Invalid file type. Only image files are allowed.");
                }

                var filePath = Path.Combine(uploads, preview.FileName);
                using (var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    await preview.CopyToAsync(fileStream);
                }

                post.Preview = filePath;
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

            if (authenticatedUser != null && authenticatedUser.ID != post.Club.ID)
            {
                return Forbid();
            }

            if (id != post.ID)
            {
                return BadRequest();
            }

            if (preview != null)
            {
                var previewContentType = preview.ContentType;
                var isPreviewImage = previewContentType.StartsWith("image/");
                if (!isPreviewImage)
                {
                    return BadRequest("Invalid file type for logo. Only image files are allowed.");
                }

                // Delete the existing logo file
                if (System.IO.File.Exists(post.Preview))
                {
                    System.IO.File.Delete(post.Preview);
                }

                // Save the new logo file
                var previewUploads = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "posts", "logos");
                if (!Directory.Exists(previewUploads))
                {
                    Directory.CreateDirectory(previewUploads);
                }

                var previewFilePath = Path.Combine(previewUploads, preview.FileName);
                using (var fileStream = new FileStream(previewFilePath, FileMode.Create))
                {
                    await preview.CopyToAsync(fileStream);
                }

                post.Preview = previewFilePath;
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
            var authenticatedUser = await _context.Users.FindAsync(User.Identity.Name);

            if (authenticatedUser != null)
            {
                return Forbid();
            }

            var post = await _context.Posts.FindAsync(id);

            if (post == null)
            {
                return NotFound();
            }

            _context.Posts.Remove(post);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}