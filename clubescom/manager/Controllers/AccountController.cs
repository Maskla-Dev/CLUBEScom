using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using clubescom.manager.models;
using System.IO;
using System.Threading.Tasks;
using clubescom.manager.Controllers.Utils;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;

namespace clubescom.manager.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AccountController : ControllerBase
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly SignInManager<AppUser> _signInManager;
        private readonly AppDbContext _context;
        private readonly IConfiguration _configuration;

        public AccountController(AppDbContext context, UserManager<AppUser> userManager, SignInManager<AppUser> signInManager,
            IConfiguration configuration)
        {
            _context = context;
            _userManager = userManager;
            _signInManager = signInManager;
            _configuration = configuration;
        }

        [HttpPost("register")]
        [Authorize]
        public async Task<ActionResult> Register([FromBody] RegisterUser register)
        {
            var userRole = await _context.Roles.FirstOrDefaultAsync(r => r.Name == "User");
            var user = new AppUser { UserName = register.username, Email = register.email, PhoneNumber = register.phoneNumber, Name = register.name, Roles = userRole};

            if (ImageManager.IsValidFile(register.profileImage))
            {
                user.ProfileImagePath = await ImageManager.New(register.profileImage,
                    Path.Combine(Directory.GetCurrentDirectory(), "wwwroot",
                        _configuration.GetValue<string>("UsersPath")));
            }

            var result = await _userManager.CreateAsync(user, register.password);

            if (result.Succeeded)
            {
                return Ok(new { message = "User created" });
            }

            return BadRequest(result.Errors);
        }

        [HttpPost("login")]
        public async Task<ActionResult> Login([FromBody] UserCredentials credentials)
        {
            var user = await _userManager.FindByNameAsync(credentials.Username);
            if (user == null)
            {
                return Unauthorized();
            }

            Console.WriteLine("User: " + user.Name);
            var result =
                await _signInManager.PasswordSignInAsync(user, credentials.Password, isPersistent: false, lockoutOnFailure: false);

            if (result.Succeeded)
            {
                var userDto = new AppUserDto(user, _configuration);
                return Ok(userDto);
            }
            else
            {
                foreach (var error in result.ToString())
                {
                    Console.WriteLine($"Cannot login: {error}");
                }
            }
            return Unauthorized();
        }
    }
}