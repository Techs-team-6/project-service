using System.IO.Compression;
using Microsoft.Extensions.FileSystemGlobbing.Internal.PathSegments;
using ProjectService.Core.Interfaces;

namespace ProjectService.Core.Services;

public class ZipArchiver : IArchiver
{
    public Stream CompressStream(string path)
    {
        string tempPath = Path.GetTempPath();
        string fileName;

        do
        {
            fileName = $"{Guid.NewGuid()}.tmp";
        } while (File.Exists(Path.Combine(tempPath, fileName)));

        string file = Path.Combine(tempPath, fileName);

        ZipFile.CreateFromDirectory(
            path,
            file,
            CompressionLevel.Optimal,
            includeBaseDirectory: false);

        return new FileStream(file,
            FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.None,
            4096, FileOptions.RandomAccess | FileOptions.DeleteOnClose);
    }

    public void DecompressStream(string path, Stream archiveStream, bool overwriteFiles)
    {
        var archive = new ZipArchive(archiveStream, ZipArchiveMode.Read, leaveOpen:true);
        archive.ExtractToDirectory(path, overwriteFiles);
    }
}