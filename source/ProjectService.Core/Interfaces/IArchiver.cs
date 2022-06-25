namespace ProjectService.Core.Interfaces;

public interface IArchiver
{
    Stream CompressStream(string path);
    void DecompressStream(string path, Stream archiveStream, bool overwriteFiles = false);
}