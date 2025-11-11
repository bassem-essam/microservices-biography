namespace AvatarService.Services;

public interface IAvatarStore
{
    string NewAvatarPath(string extension);
    public string GetAvatarFileName(string avatarPath);
    bool Exists(string avatarPath);
    void Delete(string avatarPath);
    string Upload(IFormFile formFile);
}

public class AvatarStore : IAvatarStore
{
    private readonly string avatarDirectory = Path.Combine(Directory.GetCurrentDirectory(), "public/avatars");

    public string NewAvatarPath(string extension)
    {
        return $"{Guid.NewGuid()}.{extension}";
    }


    public string GetAvatarFileName(string avatarPath)
    {
        return Path.Combine(avatarDirectory, avatarPath);
    }

    public bool Exists(string avatarPath)
    {
        return File.Exists(GetAvatarFileName(avatarPath));
    }

    public void Delete(string avatarPath)
    {
        var fileName = GetAvatarFileName(avatarPath);
        if (!File.Exists(fileName)) return;

        File.Delete(fileName);
    }

    public string Upload(IFormFile formFile)
    {
        var fileName = NewAvatarPath(formFile.FileName.Split('.').Last());
        var filePath = GetAvatarFileName(fileName);
        formFile.CopyTo(new FileStream(filePath, FileMode.Create));
        return fileName;
    }
}