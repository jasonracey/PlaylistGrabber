using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PlaylistGrabber
{
    [TestClass]
    public class DownloaderTest
    {
        private readonly Mock<IDestinationPathBuilder> mockDestinationPathBuilder = new Mock<IDestinationPathBuilder>();
        private readonly Mock<IWebClientWrapper> mockWebClientWrapper = new Mock<IWebClientWrapper>();

        private Downloader downloader;

        [TestInitialize]
        public void TestInitialize()
        {
            downloader = new Downloader(mockDestinationPathBuilder.Object, mockWebClientWrapper.Object);
        }

        [TestMethod]
        public void WhenConstructorArgNull_Throws()
        {
            Assert.ThrowsException<ArgumentNullException>(() => new Downloader(null, mockWebClientWrapper.Object));
            Assert.ThrowsException<ArgumentNullException>(() => new Downloader(mockDestinationPathBuilder.Object, null));
            Assert.IsNotNull(new Downloader(mockDestinationPathBuilder.Object, mockWebClientWrapper.Object));
        }

        [TestMethod]
        public async Task WhenSourcePathsNull_Throws()
        {
            await Assert.ThrowsExceptionAsync<ArgumentNullException>(() => downloader.DownloadFilesAsync(null));
        }

        [TestMethod]
        public async Task WhenSourcePathsEmpty_Skipped()
        {
            //arrange
            var uris = new List<Uri>();

            // act
            await downloader.DownloadFilesAsync(uris);

            // assert
            Assert.AreEqual("Downloading...", downloader.State);
            Assert.AreEqual(0, downloader.TotalFiles);
            Assert.AreEqual(0, downloader.CompletedDownloadAttempts);
        }

        [TestMethod]
        public async Task WhenSourcePathsContainsValidUriString_DownloadsFile_AndUpdatesCount()
        {
            //arrange
            var uris = new List<Uri>
            {
                new Uri("https://www.contoso.com/path/file1"),
                new Uri("https://www.contoso.com/path/file2"),
                new Uri("https://www.contoso.com/path/file3"),
            };

            // act
            await downloader.DownloadFilesAsync(uris);

            // assert
            Assert.AreEqual("Downloading...", downloader.State);
            Assert.AreEqual(uris.Count(), downloader.TotalFiles);
            Assert.AreEqual(uris.Count(), downloader.CompletedDownloadAttempts);
        }
    }
}
