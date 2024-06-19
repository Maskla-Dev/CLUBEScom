namespace clubescom.manager.models;

public class CalendarDto
{
    public Guid ID { get; set; }
    public ClubDto Club { get; set; }
    public string Title { get; set; }
    public String Description { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public bool RepeatEveryWeek { get; set; }
    public string ClubName { get; set; }
    public string type { get; set; }

    public CalendarDto(Calendar calendar, IConfiguration configuration)
    {
        ID = calendar.ID;
        Club = new ClubDto(calendar.Club, configuration);
        Title = calendar.Title;
        Description = calendar.Description;
        StartDate = calendar.StartDate;
        EndDate = calendar.EndDate;
        RepeatEveryWeek = calendar.RepeatEveryWeek;
        ClubName = calendar.Club.Name;
        type = calendar.type.Name;
    }
}