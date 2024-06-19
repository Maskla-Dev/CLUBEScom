using clubescom.manager.models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using clubescom.manager.Controllers.Utils;

namespace clubescom.manager.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ClubsController : ControllerBase
{
    private readonly AppDbContext _context;
    private readonly IConfiguration _configuration;

    public ClubsController(AppDbContext context, IConfiguration configuration)
    {
        _context = context;
        _configuration = configuration;
    }

    // Get: api/Clubs
    [HttpGet]
    public async Task<ActionResult<IEnumerable<ClubDto>>> GetClubs()
    {
        // Retrieve all clubs from database
        var clubs = await _context.Clubs.ToListAsync();
        var clubDtos = mapClubs(clubs);
        return Ok(clubDtos);
    }

    // Get: api/Clubs/{name}
    [HttpGet("{name}")]
    public async Task<ActionResult<ClubDto>> GetClub(string name)
    {
        // Retrieve club from database
        var club = await _context.Clubs.FirstOrDefaultAsync(c => c.Name == name);
        if (club == null)
        {
            return NotFound();
        }

        var clubDto = mapClubs(new List<Club> { club }).First();
        return Ok(clubDto);
    }

    // POST: api/Clubs
    [HttpPost]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<Club>> PostClub(string name, string optionalPhoneNumber, string optionalEmail,
        IFormFile logo, IFormFile banner)
    {
        var club = new Club
        {
            Name = name,
            OptionalPhoneNumber = optionalPhoneNumber,
            OptionalEmail = optionalEmail
        };

        if (!ImageManager.IsValidFile(logo))
        {
            return BadRequest("Invalid file type for logo. Only image files are allowed.");
        }

        club.Logo = await ImageManager.New(logo,
            Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", _configuration.GetValue<string>("LogosPath")));

        if (!ImageManager.IsValidFile(banner))
        {
            return BadRequest("Invalid file type for banner. Only image files are allowed.");
        }

        club.Banner = await ImageManager.New(banner,
            Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", _configuration.GetValue<string>("BannersPath")));

        _context.Clubs.Add(club);
        await _context.SaveChangesAsync();

        return CreatedAtAction(nameof(GetClub), new { name = club.Name }, club);
    }

    // PUT: api/Clubs/{name}
    [HttpPut("{name}")]
    [Authorize(Roles = "Admin, President")]
    public async Task<IActionResult> PutClub(string name, string optionalPhoneNumber, string optionalEmail,
        IFormFile? logo, IFormFile? banner)
    {
        var club = await _context.Clubs.FirstOrDefaultAsync(c => c.Name == name);
        if (club == null)
        {
            return NotFound();
        }

        club.OptionalPhoneNumber = optionalPhoneNumber;
        club.OptionalEmail = optionalEmail;

        if (logo != null)
        {
            if (!ImageManager.IsValidFile(logo))
            {
                return BadRequest("Invalid file type for logo. Only image files are allowed.");
            }

            club.Logo = await ImageManager.NewOrReplace(logo,
                Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", _configuration.GetValue<string>("LogosPath")));
        }

        if (banner != null)
        {
            if (!ImageManager.IsValidFile(banner))
            {
                return BadRequest("Invalid file type for logo. Only image files are allowed.");
            }

            club.Banner = await ImageManager.NewOrReplace(banner,
                Path.Combine(Directory.GetCurrentDirectory(), "wwwroot",
                    _configuration.GetValue<string>("BannersPath")));
        }

        _context.Entry(club).State = EntityState.Modified;
        await _context.SaveChangesAsync();

        return NoContent();
    }

    // DELETE: api/Clubs/{name}
    [HttpDelete("{name}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> DeleteClub(string name)
    {
        var club = await _context.Clubs.FirstOrDefaultAsync(c => c.Name == name);
        if (club == null)
        {
            return NotFound();
        }

        ImageManager.Delete(Path.Combine(Directory.GetCurrentDirectory(), "wwwroot",
            _configuration.GetValue<string>("BannersPath"), club.Banner));
        ImageManager.Delete(Path.Combine(Directory.GetCurrentDirectory(), "wwwroot",
            _configuration.GetValue<string>("LogosPath"), club.Logo));

        _context.Clubs.Remove(club);
        await _context.SaveChangesAsync();

        return NoContent();
    }

    private IEnumerable<ClubDto> mapClubs(IEnumerable<Club> clubs)
    {
        return clubs.Select(club => new ClubDto(club, _configuration)).ToList();
    }
}