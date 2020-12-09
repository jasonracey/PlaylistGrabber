using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PlaylistGrabber
{
    public interface IDownloader
    {
        int DownloadedFiles { get; }
        string State { get; }
        int TotalFiles { get; }

        void DownloadFiles(IEnumerable<Uri> uris);
    }

    public class Downloader : IDownloader
    {
        private readonly IDestinationPathBuilder destinationPathBuilder;
        private readonly IWebClientWrapper webClientWrapper;

        public string State { get; private set; }

        public int DownloadedFiles { get; private set; }

        public int TotalFiles { get; private set; }

        public Downloader(
            IDestinationPathBuilder destinationPathBuilder,
            IWebClientWrapper webClientWrapper)
        {
            this.destinationPathBuilder = destinationPathBuilder ??
                throw new ArgumentNullException(nameof(destinationPathBuilder));

            this.webClientWrapper = webClientWrapper ??
                throw new ArgumentNullException(nameof(webClientWrapper));

            State = string.Empty;
        }

        public void DownloadFiles(IEnumerable<Uri> uris)
        {
            if (uris == null)
                throw new ArgumentNullException(nameof(uris));

            State = $"Downloading...";
            TotalFiles = uris.Count();

            Task.WaitAll(uris.Select(uri => DownloadFileAsync(uri)).ToArray());
        }

        private async Task DownloadFileAsync(Uri uri)
        {
            var destinationPath = this.destinationPathBuilder.CreateDestinationPath(uri);
            await this.webClientWrapper.DownloadFileTaskAsync(uri, destinationPath).ConfigureAwait(false);
            DownloadedFiles++;
        }
    }
}
