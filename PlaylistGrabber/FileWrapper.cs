using System.IO;

namespace PlaylistGrabber
{
    public interface IFileWrapper
    {
        void Delete(string path);

        bool Exists(string path);
    }

    public class FileWrapper : IFileWrapper
    {
        public void Delete(string path)
        {
            File.Delete(path);
        }

        public bool Exists(string path)
        {
            return File.Exists(path);
        }
    }
}
