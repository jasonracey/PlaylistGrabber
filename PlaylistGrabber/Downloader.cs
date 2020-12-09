using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PlaylistGrabber
{
    public interface IDownloader
    {
        int CompletedDownloadAttempts { get; }
        string State { get; }
        int TotalFiles { get; }

        Task<IEnumerable<DownloadResult>> DownloadFilesAsync(IEnumerable<Uri> uris);
    }

    public partial class Downloader : IDownloader
    {
        private readonly IDestinationPathBuilder destinationPathBuilder;
        private readonly IWebClientWrapper webClientWrapper;

        public string State { get; private set; }

        public int CompletedDownloadAttempts { get; private set; }

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

        public async Task<IEnumerable<DownloadResult>> DownloadFilesAsync(IEnumerable<Uri> uris)
        {
            if (uris == null)
                throw new ArgumentNullException(nameof(uris));

            State = $"Downloading...";
            TotalFiles = uris.Count();

            var results = new List<DownloadResult>();

            await Task.WhenAll(uris.Select(async uri =>
            {
                try
                {
                    await DownloadFileAsync(uri).ConfigureAwait(false);
                    results.Add(new DownloadResult(uri, DownloadResultType.Success, DownloadResultType.Success.ToString()));
                }
                catch (Exception ex)
                {
                    results.Add(new DownloadResult(uri, DownloadResultType.Failure, ex.Message));
                }
                finally
                {
                    CompletedDownloadAttempts++;
                }
            }));

            return results;
        }

        private async Task DownloadFileAsync(Uri uri)
        {
            var destinationPath = this.destinationPathBuilder.CreateDestinationPath(uri);
            await this.webClientWrapper.DownloadFileTaskAsync(uri, destinationPath).ConfigureAwait(false);
        }
    }
}
