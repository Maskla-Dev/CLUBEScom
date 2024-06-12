namespace clubescom.manager.models;

public class Image
{
    public Guid ID { get; set; }
    public string FileName { get; set; }
    public byte[] Data { get; set; }
}