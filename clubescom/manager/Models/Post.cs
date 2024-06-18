namespace clubescom.manager.models;

public class Post
{
    public Guid ID { get; set; }
    public string Title { get; set; }
    public byte[] Content { get; set; }
    public PostType PostType { get; set; }
    public Club Club { get; set; }
    public DateTime Date { get; set; }
    public string Image { get; set; }
    public AppUser Author { get; set; }
    public bool enabled { get; set; }
    public string Preview { get; set; }
}