namespace clubescom.manager.models;

public class AppUserDto
{
    public string Name { get; set; }
    public string Email { get; set; }
    public string PhoneNumber { get; set; }
    public string ProfileImagePath { get; set; }
    public string Role { get; set; }

    public AppUserDto(AppUser user, IConfiguration configuration)
    {
        Name = user.Name;
        Email = user.Email;
        PhoneNumber = user.PhoneNumber;
        ProfileImagePath = Path.Combine(configuration.GetValue<string>("URIBasePath"),
            configuration.GetValue<string>("ProfileImagesPath"), user.ProfileImagePath);
        Role = user.Roles.Name;
    }
}