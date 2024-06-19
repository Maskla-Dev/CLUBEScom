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

    public PostDto(Post post, IConfiguration _configuration)
    {
        ID = post.ID;
        Title = post.Title;
        Content = post.Content;
        PostType = post.PostType;
        Club = new ClubDto(post.Club, _configuration);
        Date = post.Date;
        Image = post.Image;
        Author = new AppUserDto(post.Author, _configuration);
        Preview = post.Preview;
    }
}