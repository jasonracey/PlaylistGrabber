using System;
using System.Net;
using System.Threading.Tasks;

namespace PlaylistGrabber
{
    public interface IWebClientWrapper
    {
        Task DownloadFileTaskAsync(Uri uri, string destinationPath);
    }

    public class WebClientWrapper : IWebClientWrapper
    {
        public async Task DownloadFileTaskAsync(Uri uri, string destinationPath)
        {
            using var webClient = new WebClient();
            await webClient.DownloadFileTaskAsync(uri, destinationPath).ConfigureAwait(false);
        }
    }
}
