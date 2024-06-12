namespace clubescom.manager.models;

public class Club
{
    public Guid ID { get; set; }
    public AppUser President { get; set; }
    public string OptionalPhoneNumber { get; set; }
    public string OptionalEmail { get; set; }
    public string Logo { get; set; }
    public string Name { get; set; }
    public string Banner { get; set; }
}