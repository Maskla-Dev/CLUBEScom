using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using clubescom.manager.models;
using Microsoft.AspNetCore.Authorization;

namespace clubescom.manager.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CalendarController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly IConfiguration _configuration;

        public CalendarController(AppDbContext context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
        }

        // GET: api/Calendar
        [HttpGet]
        public async Task<ActionResult<IEnumerable<CalendarDto>>> GetCalendars()
        {
            var calendars = await _context.Calendars.ToListAsync();

            var calendarDtos = calendars.Select(calendar => new CalendarDto(calendar, _configuration)).ToList();

            return Ok(calendarDtos);
        }

        // GET: api/Calendar/{id}
        [HttpGet("{id}")]
        public async Task<ActionResult<CalendarDto>> GetCalendar(Guid id)
        {
            var calendar = await _context.Calendars.FindAsync(id);

            if (calendar == null)
            {
                return NotFound();
            }

            return Ok(new CalendarDto(calendar, _configuration));
        }

        // GET: api/Calendar/ByClub?clubId={clubId}
        [HttpGet("ByClub/{clubId}")]
        public async Task<ActionResult<IEnumerable<CalendarDto>>> GetCalendars([FromQuery] Guid clubId)
        {
            var calendars = await _context.Calendars.Where(c => c.Club.ID == clubId).ToListAsync();

            if (!calendars.Any())
            {
                return NotFound();
            }

            var calendarDtos = calendars.Select(calendar => new CalendarDto(calendar, _configuration)).ToList();

            return Ok(calendarDtos);
        }

        // POST: api/Calendar
        [HttpPost]
        [Authorize]
        public async Task<ActionResult<Calendar>> PostCalendar(Guid clubId, string title, string description,
            DateTime startDate, DateTime endDate, bool repeatEveryWeek, string type)
        {
            var club = await _context.Clubs.FindAsync(clubId);


            if (club == null)
            {
                return NotFound();
            }

            var authenticatedUser = await _context.Users.FindAsync(User.Identity.Name);

            if (authenticatedUser != null && authenticatedUser.Id != club.President.Id)
            {
                return Forbid();
            }

            var calendarType = await _context.CalendarTypes.FindAsync(type);

            if (calendarType == null)
            {
                return NotFound();
            }

            var calendar = new Calendar
            {
                Club = club,
                Title = title,
                Description = description,
                StartDate = startDate,
                EndDate = endDate,
                RepeatEveryWeek = repeatEveryWeek,
                type = calendarType
            };

            _context.Calendars.Add(calendar);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetCalendar", new { id = calendar.ID }, new CalendarDto(calendar, _configuration));
        }

        // PUT: api/Calendar/{id}
        [HttpPut("{id}")]
        [Authorize]
        public async Task<IActionResult> PutCalendar(Guid id, Guid clubId, string title, string description,
            DateTime startDate, DateTime endDate, bool repeatEveryWeek, string type)
        {
            var club = await _context.Clubs.FindAsync(clubId);


            if (club == null)
            {
                return NotFound();
            }

            var authenticatedUser = await _context.Users.FindAsync(User.Identity.Name);

            if (authenticatedUser != null && authenticatedUser.Id != club.President.Id)
            {
                return Forbid();
            }

            var calendar = await _context.Calendars.FindAsync(id);

            if (calendar == null)
            {
                return NotFound();
            }

            var calendarType = await _context.CalendarTypes.FindAsync(type);

            if (calendarType == null)
            {
                return NotFound();
            }

            calendar.Club = club;
            calendar.Title = title;
            calendar.Description = description;
            calendar.StartDate = startDate;
            calendar.EndDate = endDate;
            calendar.RepeatEveryWeek = repeatEveryWeek;
            calendar.type = calendarType;

            await _context.SaveChangesAsync();

            return NoContent();
        }

        // DELETE: api/Calendar/{id}
        [HttpDelete("{id}")]
        [Authorize]
        public async Task<IActionResult> DeleteCalendar(Guid id)
        {
            var calendar = await _context.Calendars.FindAsync(id);
            var club = calendar.Club;


            if (club == null)
            {
                return NotFound();
            }

            var authenticatedUser = await _context.Users.FindAsync(User.Identity.Name);

            if (authenticatedUser != null && authenticatedUser.Id != club.President.Id)
            {
                return Forbid();
            }

            if (calendar == null)
            {
                return NotFound();
            }

            _context.Calendars.Remove(calendar);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        // GET: api/Calendar/Types
        [HttpGet("Types")]
        public async Task<ActionResult<IEnumerable<CalendarEventType>>> GetCalendarTypes()
        {
            var calendarTypes = await _context.CalendarTypes.ToListAsync();

            return Ok(calendarTypes);
        }

        // GET: api/Calendar/Links?PostId={PostId}
        [HttpGet ("Links")]
        public async Task<ActionResult<IEnumerable<PostCalendarLinksDto>>> GetPostCalendarLinks([FromQuery] Guid PostId)
        {
            var postCalendarLinks = await _context.PostCalendarlinks.Where(pcl => pcl.Post.ID == PostId).ToListAsync();

            if (!postCalendarLinks.Any())
            {
                return NotFound();
            }

            var postCalendarLinksDto =
                postCalendarLinks.Select(pcl => new PostCalendarLinksDto(pcl)).ToList();

            return Ok(postCalendarLinksDto);
        }
    }
}