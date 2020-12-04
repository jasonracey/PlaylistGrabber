using System;
using System.Collections.Generic;
using System.IO;
using System.Net;

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
            TotalFiles = uris.Count;
            foreach (var uri in uris)
            {
                DownloadFile(uri);
                DownloadedFiles++;
            }
        }

        private void DownloadFile(Uri uri)
        {
            State = $"Downloading {uri} ...";

            var webClient = new WebClient();
            var destinationPath = GetDestinationPath(uri);
            webClient.DownloadFile(uri, destinationPath);
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
