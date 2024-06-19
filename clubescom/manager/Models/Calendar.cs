namespace clubescom.manager.models;

public class Calendar
{
    public Guid ID { get; set; }
    public Club Club { get; set; }
    public string Title { get; set; }
    public String Description { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public bool RepeatEveryWeek { get; set; }
    public CalendarEventType type { get; set; }
}