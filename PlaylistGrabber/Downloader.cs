using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PlaylistGrabber
{
    public class Downloader
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

        public void DownloadFiles(IList<string> sourcePaths)
        {
            State = $"Downloading...";
            TotalFiles = sourcePaths.Count;
            Task.WaitAll(sourcePaths.Select(sourcePath => DownloadFileAsync(sourcePath)).ToArray());
        }

        private async Task DownloadFileAsync(string sourcePath)
        {
            var destinationPath = this.destinationPathBuilder.GetDestinationPath(sourcePath);
            await this.webClientWrapper.DownloadFileTaskAsync(sourcePath, destinationPath).ConfigureAwait(false);
            DownloadedFiles++;
        }
    }
}
