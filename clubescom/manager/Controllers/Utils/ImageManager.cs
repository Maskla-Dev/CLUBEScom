namespace clubescom.manager.Controllers.Utils;

public class ImageManager
{
    public static bool IsValidFile(IFormFile? file)
    {
        return file != null && file.ContentType.StartsWith("image/");
    }

    public static async Task<String> NewOrReplace(IFormFile file, string savePath)
    {
        var fullFilePath = Path.Combine(savePath, file.FileName);
        ImageManager.Delete(fullFilePath);
        return await ImageManager.New(file, fullFilePath);
    }

    public static async Task<String> New(IFormFile file, string savePath)
    {
        if (!Directory.Exists(savePath))
        {
            Directory.CreateDirectory(savePath);
        }

        using (var fileStream = new FileStream(savePath, FileMode.Create))
        {
            await file.CopyToAsync(fileStream);
        }

        return file.FileName;
    }

    public static void Delete(string path)
    {
        if (!System.IO.File.Exists(path)) return;
        System.IO.File.Delete(path);
        return;
    }
}