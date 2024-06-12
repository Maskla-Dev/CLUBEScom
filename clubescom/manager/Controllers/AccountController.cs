using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using clubescom.manager.models;
using System.IO;
using System.Threading.Tasks;

namespace clubescom.manager.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AccountController : ControllerBase
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly SignInManager<AppUser> _signInManager;

        public AccountController(UserManager<AppUser> userManager, SignInManager<AppUser> signInManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
        }

        [HttpPost("register")]
        public async Task<ActionResult> Register(string username, string password, string email, string phoneNumber,
            string name, IFormFile profileImage)
        {
            var user = new AppUser { UserName = username, Email = email, PhoneNumber = phoneNumber, Name = name };

            if (profileImage != null)
            {
                var uploads = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "users", "Images");
                if (!Directory.Exists(uploads))
                {
                    Directory.CreateDirectory(uploads);
                }

                // Check if the file is an image
                var contentType = profileImage.ContentType;
                var isImage = contentType.StartsWith("image/");
                if (!isImage)
                {
                    return BadRequest("Invalid file type. Only image files are allowed.");
                }

                var filePath = Path.Combine(uploads, profileImage.FileName);
                using (var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    await profileImage.CopyToAsync(fileStream);
                }

                user.ProfileImagePath = filePath;
            }

            var result = await _userManager.CreateAsync(user, password);

            if (result.Succeeded)
            {
                await _signInManager.SignInAsync(user, isPersistent: false);
                return Ok(new { ProfileImagePath = user.ProfileImagePath });
            }

            return BadRequest(result.Errors);
        }

        [HttpPost("login")]
        public async Task<ActionResult> Login(string username, string password)
        {
            var user = await _userManager.FindByNameAsync(username);
            if (user == null)
            {
                return Unauthorized();
            }

            var result =
                await _signInManager.PasswordSignInAsync(user, password, isPersistent: false, lockoutOnFailure: false);

            if (result.Succeeded)
            {
                return Ok(new { ProfileImagePath = user.ProfileImagePath });
            }

            return Unauthorized();
        }
    }
}