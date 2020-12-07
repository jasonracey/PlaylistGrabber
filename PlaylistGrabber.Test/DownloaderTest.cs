using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.Collections.Generic;

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
            var sourcePaths = new List<string>();

            // act
            downloader.DownloadFiles(sourcePaths);

            // assert
            Assert.AreEqual("Downloading...", downloader.State);
            Assert.AreEqual(0, downloader.TotalFiles);
            Assert.AreEqual(0, downloader.DownloadedFiles);
        }

        [DataTestMethod]
        [DataRow(null)]
        [DataRow(" ")]
        [DataRow("www.contoso.com/path/file")]
        public void WhenSourcePathsContainsInvalidPath_Skipped(string invalidPath)
        {
            //arrange
            var sourcePaths = new List<string>
            {
                invalidPath
            };

            // act
            downloader.DownloadFiles(sourcePaths);

            // assert
            Assert.AreEqual("Downloading...", downloader.State);
            Assert.AreEqual(0, downloader.TotalFiles);
            Assert.AreEqual(0, downloader.DownloadedFiles);
        }

        [TestMethod]
        public void WhenSourcePathsContainsValidUriString_DownloadsFile_AndUpdatesCount()
        {
            //arrange
            var sourcePaths = new List<string>
            {
                null,
                "https://www.contoso.com/path/file1",
                string.Empty,
                "https://www.contoso.com/path/file2",
                "www.contoso.com/path/file",
                "https://www.contoso.com/path/file3",
                "https://www.contoso.com/path/file4",
            };

            // act
            downloader.DownloadFiles(sourcePaths);

            // assert
            Assert.AreEqual("Downloading...", downloader.State);
            Assert.AreEqual(4, downloader.TotalFiles);
            Assert.AreEqual(4, downloader.DownloadedFiles);
        }
    }
}
