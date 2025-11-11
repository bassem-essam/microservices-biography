using AvatarService.Services;
using MediatR;

public class UploadAvatarHandler : IRequestHandler<UploadAvatar, UploadAvatarResult>
{
    private readonly IAvatarStore _avatarStore;
    public UploadAvatarHandler(IAvatarStore avatarStore)
    {
        _avatarStore = avatarStore;
    }

    public Task<UploadAvatarResult> Handle(UploadAvatar request, CancellationToken cancellationToken)
    {
        if (!validateFile(request.Avatar))
        {
            return Task.FromResult(new UploadAvatarResult { Created = false, AvatarPath = "", Error = "Invalid file" });
        }

        var path = _avatarStore.Upload(request.Avatar);
        return Task.FromResult(new UploadAvatarResult { Created = true, AvatarPath = path, Error = "" });
    }

    private static readonly Dictionary<string, List<byte[]>> _fileSignature =
    new Dictionary<string, List<byte[]>>
    {
        { ".jpeg", new List<byte[]>
            {
                new byte[] { 0xFF, 0xD8, 0xFF, 0xE0 },
            }
        },
        { ".jpg", new List<byte[]>
            {
                new byte[] { 0xFF, 0xD8, 0xFF, 0xE0 },
            }
        },
        { ".png", new List<byte[]>
            {
                new byte[] { 0x89, 0x50, 0x4E, 0x47, 0x0D, 0x0A, 0x1A, 0x0A },
            }
        },
    };

    private static bool validateFile(IFormFile formFile)
    {
        var ext = Path.GetExtension(formFile.FileName);
        if (!_fileSignature.ContainsKey(ext))
            return false;

        using (var reader = new BinaryReader(formFile.OpenReadStream()))
        {
            var signatures = _fileSignature[ext];
            var headerBytes = reader.ReadBytes(signatures.Max(m => m.Length));

            return signatures.Any(signature =>
                headerBytes.Take(signature.Length).SequenceEqual(signature));
        }
    }

}
