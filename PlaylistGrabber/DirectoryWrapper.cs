using System.IO;

namespace PlaylistGrabber
{
    public interface IDirectoryWrapper
    {
        DirectoryInfo CreateDirectory(string path);
    }

    public class DirectoryWrapper : IDirectoryWrapper
    {
        public DirectoryInfo CreateDirectory(string path)
        {
            return Directory.CreateDirectory(path);
        }
    }
}
