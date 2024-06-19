namespace clubescom.manager.models;

public class PostCalendarLinksDto
{
    public Guid ID { get; set; }
    public Guid PostID { get; set; }
    public Guid CalendarID { get; set; }

    public PostCalendarLinksDto(PostCalendarLinks postCalendarLinks)
    {
        ID = postCalendarLinks.ID;
        PostID = postCalendarLinks.Post.ID;
        CalendarID = postCalendarLinks.Calendar.ID;
    }
}