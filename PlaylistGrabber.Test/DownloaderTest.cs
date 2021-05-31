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
        public async Task WhenUrisNull_Throws()
        {
            await Assert.ThrowsExceptionAsync<ArgumentNullException>(() => downloader.DownloadFilesAsync(null));
        }

        [TestMethod]
        public async Task WhenUrisEmpty_Skipped()
        {
            //arrange
            var uris = new List<Uri>();

            // act
            var downloadResults = await downloader.DownloadFilesAsync(uris);

            // assert
            Assert.AreEqual("Downloading...", downloader.State);
            Assert.AreEqual(0, downloader.TotalFiles);
            Assert.AreEqual(0, downloader.CompletedDownloadAttempts);
            Assert.IsNotNull(downloadResults);
            Assert.AreEqual(uris.Count, downloadResults.Count());
        }

        [TestMethod]
        public async Task WhenDownloadSucceeds_UpdatesCount_AndReturnsSuccessResult()
        {
            // arrange
            var uris = new List<Uri>
            {
                new Uri("https://www.contoso.com/path/file1"),
                new Uri("https://www.contoso.com/path/file2"),
                new Uri("https://www.contoso.com/path/file3"),
            };

            // act
            var downloadResults = await downloader.DownloadFilesAsync(uris);

            // assert
            Assert.AreEqual("Downloading...", downloader.State);
            Assert.AreEqual(uris.Count(), downloader.TotalFiles);
            Assert.AreEqual(uris.Count(), downloader.CompletedDownloadAttempts);
            Assert.IsNotNull(downloadResults);
            Assert.AreEqual(uris.Count, downloadResults.Count());
            foreach (var downloadResult in downloadResults)
            {
                Assert.AreEqual(DownloadResultType.Success.ToString(), downloadResult.DownloadResultMessage);
                Assert.AreEqual(DownloadResultType.Success, downloadResult.DownloadResultType);
                Assert.IsTrue(uris.Contains(downloadResult.DownloadUri));
            }
        }

        [TestMethod]
        public async Task WhenDownloadStarts_ResetsCount()
        {
            // arrange
            var uris = new List<Uri>
            {
                new Uri("https://www.contoso.com/path/file1"),
                new Uri("https://www.contoso.com/path/file2"),
                new Uri("https://www.contoso.com/path/file3"),
            };

            // act
            await downloader.DownloadFilesAsync(uris);
            await downloader.DownloadFilesAsync(uris);

            // assert
            Assert.AreEqual(uris.Count(), downloader.CompletedDownloadAttempts);
        }

        [TestMethod]
        public async Task WhenDownloadFails_UpdatesCount_AndReturnsFailureResult()
        {
            //arrange
            const string mockMessage = "mock error message";
            var uris = new List<Uri>
            {
                new Uri("https://www.contoso.com/path/file1"),
            };
            mockWebClientWrapper
                .Setup(m => m.DownloadFileTaskAsync(It.IsAny<Uri>(), It.IsAny<string>()))
                .ThrowsAsync(new Exception(mockMessage));

            // act
            var downloadResults = await downloader.DownloadFilesAsync(uris);

            // assert
            Assert.AreEqual("Downloading...", downloader.State);
            Assert.AreEqual(uris.Count(), downloader.TotalFiles);
            Assert.AreEqual(uris.Count(), downloader.CompletedDownloadAttempts);
            Assert.IsNotNull(downloadResults);
            Assert.AreEqual(uris.Count, downloadResults.Count());
            foreach (var downloadResult in downloadResults)
            {
                Assert.AreEqual(mockMessage, downloadResult.DownloadResultMessage);
                Assert.AreEqual(DownloadResultType.Failure, downloadResult.DownloadResultType);
                Assert.AreEqual(uris[0], downloadResult.DownloadUri);
            }
        }
    }
}
