namespace clubescom.manager.models;

public class Club
{
    public Guid ID { get; set; }
    public User President { get; set; }
    public string PhoneNumber { get; set; }
    public string Email { get; set; }
    public Image Logo { get; set; }
    public string Name { get; set; }
    public Image Banner { get; set; }
}