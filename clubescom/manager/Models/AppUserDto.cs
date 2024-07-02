namespace clubescom.manager.models;

public class AppUserDto
{
    public string Name { get; set; }
    public string Email { get; set; }
    public string PhoneNumber { get; set; }
    public string ProfileImagePath { get; set; }

    public AppUserDto(AppUser user, IConfiguration configuration)
    {
        Name = user.Name;
        Email = user.Email;
        PhoneNumber = user.PhoneNumber;
        ProfileImagePath = "http://localhost:5274/" + "users/avatars/" + user.ProfileImagePath;
    }
}