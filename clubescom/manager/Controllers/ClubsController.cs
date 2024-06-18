using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;
using clubescom.manager;
using clubescom.manager.models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;

namespace clubescom.controllers;

[ApiController]
[Route("api/[controller]")]
public class ClubsController : ControllerBase
{
    private readonly AppDbContext _context;

    public ClubsController(AppDbContext context)
    {
        _context = context;
    }

    // Get: api/Clubs
    [HttpGet]
    public async Task<ActionResult<IEnumerable<ClubDto>>> GetClubs()
    {
        // Retreive all clubs from database
        var clubs = await _context.Clubs.ToListAsync();
        var clubDtos = mapClubs(clubs);
        return Ok(clubDtos);
    }

    // Get: api/Clubs/{name}
    [HttpGet("{name}")]
    public async Task<ActionResult<ClubDto>> GetClub(string name)
    {
        // Retreive club from database
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

        if (logo != null)
        {
            var logoContentType = logo.ContentType;
            var isLogoImage = logoContentType.StartsWith("image/");
            if (!isLogoImage)
            {
                return BadRequest("Invalid file type for logo. Only image files are allowed.");
            }

            var logoUploads = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "clubs", "logos");
            if (!Directory.Exists(logoUploads))
            {
                Directory.CreateDirectory(logoUploads);
            }

            var logoFilePath = Path.Combine(logoUploads, logo.FileName);
            using (var fileStream = new FileStream(logoFilePath, FileMode.Create))
            {
                await logo.CopyToAsync(fileStream);
            }

            club.Logo = logoFilePath;
        }

        if (banner != null)
        {
            var bannerContentType = banner.ContentType;
            var isBannerImage = bannerContentType.StartsWith("image/");
            if (!isBannerImage)
            {
                return BadRequest("Invalid file type for banner. Only image files are allowed.");
            }

            var bannerUploads = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "clubs", "banners");
            if (!Directory.Exists(bannerUploads))
            {
                Directory.CreateDirectory(bannerUploads);
            }

            var bannerFilePath = Path.Combine(bannerUploads, banner.FileName);
            using (var fileStream = new FileStream(bannerFilePath, FileMode.Create))
            {
                await banner.CopyToAsync(fileStream);
            }

            club.Banner = bannerFilePath;
        }

        _context.Clubs.Add(club);
        await _context.SaveChangesAsync();

        return CreatedAtAction(nameof(GetClub), new { name = club.Name }, club);
    }

    // PUT: api/Clubs/{name}
    [HttpPut("{name}")]
    [Authorize(Roles = "Admin, President")]
    public async Task<IActionResult> PutClub(string name, string optionalPhoneNumber, string optionalEmail,
        IFormFile logo, IFormFile banner)
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
            var logoContentType = logo.ContentType;
            var isLogoImage = logoContentType.StartsWith("image/");
            if (!isLogoImage)
            {
                return BadRequest("Invalid file type for logo. Only image files are allowed.");
            }

            // Delete the existing logo file
            if (System.IO.File.Exists(club.Logo))
            {
                System.IO.File.Delete(club.Logo);
            }

            // Save the new logo file
            var logoUploads = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "clubs", "logos");
            if (!Directory.Exists(logoUploads))
            {
                Directory.CreateDirectory(logoUploads);
            }

            var logoFilePath = Path.Combine(logoUploads, logo.FileName);
            using (var fileStream = new FileStream(logoFilePath, FileMode.Create))
            {
                await logo.CopyToAsync(fileStream);
            }

            club.Logo = logoFilePath;
        }

        if (banner != null)
        {
            var bannerContentType = banner.ContentType;
            var isBannerImage = bannerContentType.StartsWith("image/");
            if (!isBannerImage)
            {
                return BadRequest("Invalid file type for banner. Only image files are allowed.");
            }

            // Delete the existing banner file
            if (System.IO.File.Exists(club.Banner))
            {
                System.IO.File.Delete(club.Banner);
            }

            // Save the new banner file
            var bannerUploads = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "clubs", "banners");
            if (!Directory.Exists(bannerUploads))
            {
                Directory.CreateDirectory(bannerUploads);
            }

            var bannerFilePath = Path.Combine(bannerUploads, banner.FileName);
            using (var fileStream = new FileStream(bannerFilePath, FileMode.Create))
            {
                await banner.CopyToAsync(fileStream);
            }

            club.Banner = bannerFilePath;
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

        _context.Clubs.Remove(club);
        await _context.SaveChangesAsync();

        return NoContent();
    }

    private IEnumerable<ClubDto> mapClubs(IEnumerable<Club> clubs)
    {
        return clubs.Select(club => new ClubDto(club)).ToList();
    }
}