using System;

namespace PlaylistGrabber
{
    public interface IDestinationPathBuilder
    {
        string GetDestinationPath(string sourcePath);
    }

    public class DestinationPathBuilder : IDestinationPathBuilder
    {
        private readonly IDirectoryWrapper directoryWrapper;
        private readonly IFileWrapper fileWrapper;

        public DestinationPathBuilder(
            IDirectoryWrapper directoryWrapper,
            IFileWrapper fileWrapper)
        {
            this.directoryWrapper = directoryWrapper ??
                throw new ArgumentNullException(nameof(directoryWrapper));

            this.fileWrapper = fileWrapper ??
                throw new ArgumentNullException(nameof(fileWrapper));
        }

        public string GetDestinationPath(string sourcePath)
        {
            var parts = sourcePath.Split('/');
            var directoryName = parts[^2];
            var fileName = parts[^1];

            string destinationDirectory = $@"Z:\Downloads\{directoryName}";

            // only creates dir if it doesn't already exist
            this.directoryWrapper.CreateDirectory(destinationDirectory);

            string destinationPath = $@"{destinationDirectory}\{fileName}";

            if (this.fileWrapper.Exists(destinationPath))
            {
                this.fileWrapper.Delete(destinationPath);
            }

            return destinationPath;
        }
    }
}
