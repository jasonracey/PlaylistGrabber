using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace PlaylistGrabber
{
    public class Downloader
    {
        public string State { get; private set; }

        public int DownloadedFiles { get; private set; }

        public int TotalFiles { get; private set; }

        public Downloader()
        {
            State = string.Empty;
        }

        public void DownloadFiles(List<Uri> uris)
        {
            State = $"Downloading...";
            TotalFiles = uris.Count;
            Task.WaitAll(uris.Select(uri => DownloadFileAsync(uri)).ToArray());
        }

        private async Task DownloadFileAsync(Uri uri)
        {
            var destinationPath = GetDestinationPath(uri);
            using var webClient = new WebClient();
            await webClient.DownloadFileTaskAsync(uri, destinationPath).ConfigureAwait(false);
            DownloadedFiles++;
        }

        private static string GetDestinationPath(Uri uri)
        {
            var parts = uri.ToString().Split('/');
            var directoryName = parts[^2];
            var fileName = parts[^1];

            string destinationDirectory = $@"Z:\Downloads\{directoryName}";

            // only creates dir if it doesn't already exist
            Directory.CreateDirectory(destinationDirectory);

            string destinationPath = $@"{destinationDirectory}\{fileName}";

            if (File.Exists(destinationPath))
            {
                File.Delete(destinationPath);
            }

            return destinationPath;
        }
    }
}
