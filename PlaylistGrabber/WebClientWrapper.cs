using System.Net;
using System.Threading.Tasks;

namespace PlaylistGrabber
{
    public interface IWebClientWrapper
    {
        Task DownloadFileTaskAsync(string sourcePath, string destinationPath);
    }

    public class WebClientWrapper : IWebClientWrapper
    {
        public async Task DownloadFileTaskAsync(string sourcePath, string destinationPath)
        {
            using var webClient = new WebClient();
            await webClient.DownloadFileTaskAsync(sourcePath, destinationPath).ConfigureAwait(false);
        }
    }
}
