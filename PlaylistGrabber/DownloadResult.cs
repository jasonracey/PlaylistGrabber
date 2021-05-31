using System;

namespace PlaylistGrabber
{
    public sealed record DownloadResult
    {
        public Uri DownloadUri { get; }

        public DownloadResultType DownloadResultType { get; }

        public string DownloadResultMessage { get; }

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
