using System;

namespace PlaylistGrabber
{
    public interface IDestinationPathBuilder
    {
        string CreateDestinationPath(string sourcePath);
    }

    public class DestinationPathBuilder : IDestinationPathBuilder
    {
        private readonly IConfiguration configuration;
        private readonly IDirectoryWrapper directoryWrapper;
        private readonly IFileWrapper fileWrapper;

        public DestinationPathBuilder(
            IConfiguration configuration,
            IDirectoryWrapper directoryWrapper,
            IFileWrapper fileWrapper)
        {
            this.configuration = configuration ??
                throw new ArgumentNullException(nameof(configuration));

            this.directoryWrapper = directoryWrapper ??
                throw new ArgumentNullException(nameof(directoryWrapper));

            this.fileWrapper = fileWrapper ??
                throw new ArgumentNullException(nameof(fileWrapper));
        }

        public string CreateDestinationPath(string sourcePath)
        {
            if (string.IsNullOrWhiteSpace(sourcePath))
                throw new ArgumentException("Must not be null or white space", nameof(sourcePath));

            if (!Uri.IsWellFormedUriString(sourcePath, UriKind.Absolute))
                throw new ArgumentException("Must be a well-formed absolute uri string", nameof(sourcePath));

            var parts = sourcePath.Split('/');
            var directoryName = parts[^2];
            var fileName = parts[^1];

            var destinationDirectory = $@"{configuration.DestinationPathBase}\{directoryName}";

            // only creates dir if it doesn't already exist
            this.directoryWrapper.CreateDirectory(destinationDirectory);

            var destinationPath = $@"{destinationDirectory}\{fileName}";

            if (this.fileWrapper.Exists(destinationPath))
            {
                this.fileWrapper.Delete(destinationPath);
            }

            return destinationPath;
        }
    }
}
