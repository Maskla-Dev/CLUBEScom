namespace clubescom.manager.models;

public class ClubDto
{
    public Guid ID { get; set; }
    public string President { get; set; }
    public string PresidentEmail { get; set; }
    public string PresidentPhoneNumber { get; set; }
    public string OptionalPhoneNumber { get; set; }
    public string OptionalEmail { get; set; }
    public string Logo { get; set; }
    public string Name { get; set; }
    public string Banner { get; set; }
    
    public ClubDto(Club club)
    {
        ID = club.ID;
        President = club.President.Name;
        PresidentEmail = club.President.Email;
        PresidentPhoneNumber = club.President.PhoneNumber;
        OptionalPhoneNumber = club.OptionalPhoneNumber;
        OptionalEmail = club.OptionalEmail;
        Logo = club.Logo;
        Name = club.Name;
        Banner = club.Banner;
    }
}