using System;
using System.Collections.Generic;

namespace PlaylistGrabber
{
    public class Downloader
    {
        public string State { get; private set; }

        public int DownloadedFiles { get; private set; }

        public int TotalFiles { get; private set; }

        public void DownloadFiles(List<string> sourceFilePaths)
        {
            throw new NotImplementedException();
        }
    }
}
