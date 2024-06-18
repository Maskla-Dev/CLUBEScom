namespace clubescom.manager.models;

public class PostDto
{
    public Guid ID { get; set; }
    public string Title { get; set; }
    public byte[] Content { get; set; }
    public PostType PostType { get; set; }
    public ClubDto Club { get; set; }
    public DateTime Date { get; set; }
    public string Image { get; set; }
    public AppUserDto Author { get; set; }
    public string Preview { get; set; }

    public PostDto(Post post)
    {
        ID = post.ID;
        Title = post.Title;
        Content = post.Content;
        PostType = post.PostType;
        Club = new ClubDto(post.Club);
        Date = post.Date;
        Image = post.Image;
        Author = new AppUserDto(post.Author);
        Preview = post.Preview;
    }
}

public class AppUserDto
{
    public string Name { get; set; }
    public string Email { get; set; }
    public string PhoneNumber { get; set; }

    public AppUserDto(AppUser user)
    {
        Name = user.Name;
        Email = user.Email;
        PhoneNumber = user.PhoneNumber;
    }
}