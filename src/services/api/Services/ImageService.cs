using System.Globalization;
using System.Security.Cryptography;
using System.Text.RegularExpressions;

namespace API.Services;

public class ImageService
{
    private static readonly string _storagePath = "/data/images";
    private static readonly string[] permittedExtensions = { ".jpg", ".jpeg", ".png" };
    private static readonly Dictionary<string, List<byte[]>> _fileSignature = new()
    {
        [".jpg"] = new List<byte[]>
        {
            new byte[] { 0xFF, 0xD8, 0xFF, 0xE0 },
            new byte[] { 0xFF, 0xD8, 0xFF, 0xE1 },
            new byte[] { 0xFF, 0xD8, 0xFF, 0xE8 },
        },

        [".jpeg"] = new List<byte[]>
        {
            new byte[] { 0xFF, 0xD8, 0xFF, 0xE0 },
            new byte[] { 0xFF, 0xD8, 0xFF, 0xE2 },
            new byte[] { 0xFF, 0xD8, 0xFF, 0xE3 },
        },

        [".png"] = new List<byte[]>
        {
            new byte[] { 0x89, 0x50, 0x4E, 0x47, 0x0D, 0x0A, 0x1A, 0x0A }
        }
    };

    private static readonly Regex _imageNamePattern = new(@"^[a-zA-Z0-9.]*$");

    public static bool IsValid(string fileName, Stream file)
    {
        var ext = Path.GetExtension(fileName).ToLowerInvariant();

        if (string.IsNullOrEmpty(ext) || !permittedExtensions.Contains(ext))
        {
            return false;
        }

        using var reader = new BinaryReader(file);
        var signatures = _fileSignature[ext];
        var headerBytes = reader.ReadBytes(signatures.Max(m => m.Length));

        return signatures.Any(signature =>
            headerBytes.Take(signature.Length).SequenceEqual(signature)
        );
    }

    public static async Task<string> Persist(
        Guid subjectId,
        string sourceFileName,
        Func<Stream, Task> lambda
    )
    {
        string imageName = RandomNumberGenerator
            .GetInt32(int.MaxValue)
            .ToString("D10", CultureInfo.InvariantCulture) 
            + Path.GetExtension(sourceFileName);

        string fileName = $"{subjectId:N}{imageName}";

        using var fs = File.Create(Path.Combine(_storagePath, fileName));
        await lambda.Invoke(fs);
        await fs.FlushAsync();
        fs.Close();

        return imageName;
    }

    public static string GetPath(Guid subjectId, string image)
    {
        if (!_imageNamePattern.IsMatch(image)) return null;

        string fileName = subjectId.ToString("N") + image;
        string path = Path.Combine(_storagePath, fileName);

        return File.Exists(path) ? path : null;
    }
}