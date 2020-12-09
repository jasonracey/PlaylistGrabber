using System;

namespace PlaylistGrabber
{
    public enum DownloadResultType
    {
        None = 0,
        Success = 1,
        Failure = 2
    }

    public class DownloadResult
    {
        public Uri DownloadUri { get; private set; }

        public DownloadResultType DownloadResultType { get; private set; }

        public string DownloadResultMessage { get; private set; }

        public DownloadResult(
            Uri downloadUri,
            DownloadResultType downloadResultType,
            string downloadResultMessage)
        {
            this.DownloadUri = downloadUri;
            this.DownloadResultType = downloadResultType;
            this.DownloadResultMessage = downloadResultMessage;
        }
    }
}
