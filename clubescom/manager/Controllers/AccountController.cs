using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using clubescom.manager.models;
using System.IO;
using System.Threading.Tasks;
using clubescom.manager.Controllers.Utils;
using Microsoft.AspNetCore.Authorization;

namespace clubescom.manager.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AccountController : ControllerBase
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly SignInManager<AppUser> _signInManager;
        private readonly IConfiguration _configuration;

        public AccountController(UserManager<AppUser> userManager, SignInManager<AppUser> signInManager,
            IConfiguration configuration)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _configuration = configuration;
        }

        [HttpPost("register")]
        [Authorize]
        public async Task<ActionResult> Register(string username, string password, string email, string phoneNumber,
            string name, IFormFile profileImage)
        {
            var user = new AppUser { UserName = username, Email = email, PhoneNumber = phoneNumber, Name = name };

            if (ImageManager.IsValidFile(profileImage))
            {
                user.ProfileImagePath = await ImageManager.New(profileImage,
                    Path.Combine(Directory.GetCurrentDirectory(), "wwwroot",
                        _configuration.GetValue<string>("UsersPath")));
            }

            var result = await _userManager.CreateAsync(user, password);

            if (result.Succeeded)
            {
                return Ok(new { message = "User created" });
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
                var userDto = new AppUserDto(user, _configuration);
                return Ok(userDto);
            }

            return Unauthorized();
        }
    }
}