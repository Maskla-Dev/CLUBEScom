namespace clubescom.manager.models;

public class User
{
    public Guid ID { get; set; }
    public string Name { get; set; }
    public Rol Rol { get; set; }
    public string PhoneNumber { get; set; }
    public string Email { get; set; }
    public Image ProfileImage { get; set; }
    public string Password { get; set; }
}