using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;

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
        public void WhenSourcePathsNull_Throws()
        {
            Assert.ThrowsException<ArgumentNullException>(() => downloader.DownloadFiles(null));
        }

        [TestMethod]
        public void WhenSourcePathsEmpty_Skipped()
        {
            //arrange
            var uris = new List<Uri>();

            // act
            downloader.DownloadFiles(uris);

            // assert
            Assert.AreEqual("Downloading...", downloader.State);
            Assert.AreEqual(0, downloader.TotalFiles);
            Assert.AreEqual(0, downloader.DownloadedFiles);
        }

        [TestMethod]
        public void WhenSourcePathsContainsValidUriString_DownloadsFile_AndUpdatesCount()
        {
            //arrange
            var uris = new List<Uri>
            {
                new Uri("https://www.contoso.com/path/file1"),
                new Uri("https://www.contoso.com/path/file2"),
                new Uri("https://www.contoso.com/path/file3"),
            };

            // act
            downloader.DownloadFiles(uris);

            // assert
            Assert.AreEqual("Downloading...", downloader.State);
            Assert.AreEqual(uris.Count(), downloader.TotalFiles);
            Assert.AreEqual(uris.Count(), downloader.DownloadedFiles);
        }
    }
}
