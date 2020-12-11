using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;

namespace PlaylistGrabber.Test
{
    [TestClass]
    public class DestinationPathBuilderTest
    {
        private DestinationPathBuilder destinationPathBuilder;

        private readonly Mock<IConfiguration> mockConfiguration = new Mock<IConfiguration>();
        private readonly Mock<IDirectoryWrapper> mockDirectoryWrapper = new Mock<IDirectoryWrapper>();
        private readonly Mock<IFileWrapper> mockFileWrapper = new Mock<IFileWrapper>();

        [TestInitialize]
        public void TestInitialize()
        {
            destinationPathBuilder = new DestinationPathBuilder(
                mockConfiguration.Object,
                mockDirectoryWrapper.Object,
                mockFileWrapper.Object);
        }

        [TestMethod]
        public void ValidatesArgs()
        {
            Assert.ThrowsException<ArgumentNullException>(() => destinationPathBuilder = new DestinationPathBuilder(null, mockDirectoryWrapper.Object, mockFileWrapper.Object));
            Assert.ThrowsException<ArgumentNullException>(() => destinationPathBuilder = new DestinationPathBuilder(mockConfiguration.Object, null, mockFileWrapper.Object));
            Assert.ThrowsException<ArgumentNullException>(() => destinationPathBuilder = new DestinationPathBuilder(mockConfiguration.Object, mockDirectoryWrapper.Object, null)); 
            Assert.IsNotNull(destinationPathBuilder = new DestinationPathBuilder(mockConfiguration.Object, mockDirectoryWrapper.Object, mockFileWrapper.Object));
        }

        [TestMethod]
        public void WhenUriNull_Throws()
        {
            Assert.ThrowsException<ArgumentNullException>(() => destinationPathBuilder.CreateDestinationPath(null));
        }

        [DataTestMethod]
        [DataRow("https://www.mock.com/mock-album-name/track01.flac", @"\mock-album-name\track01.flac")]
        [DataRow("https://www.mock.com/mock-album-name/track02.flac", @"\mock-album-name\track02.flac")]
        public void ReturnsExpectedDestinationPath(string sourcePath, string expectedDestinationPath)
        {
            var uri = new Uri(sourcePath);

            var actualDestinationPath = destinationPathBuilder.CreateDestinationPath(uri);

            Assert.AreEqual(expectedDestinationPath, actualDestinationPath);
        }

        [TestMethod]
        public void MakesCertainDestinationDirectoryExists()
        {
            // arrange
            const string mockDestinationPathBase = @"C:\";
            const string mockDirectoryName = "mock-album-name";
            mockConfiguration.Setup(m => m.DestinationPathBase).Returns(mockDestinationPathBase);
            var uri = new Uri($@"https://www.mock.com/{mockDirectoryName}/track01.flac");

            // act
            var actualDestinationPath = destinationPathBuilder.CreateDestinationPath(uri);

            // assert
            mockDirectoryWrapper.Verify(m => m.CreateDirectory($@"{mockDestinationPathBase}\{mockDirectoryName}"), Times.Once);
        }

        [TestMethod]
        public void WhenFileAlreadyExists_DeletesFile()
        {
            // arrange
            mockFileWrapper.Setup(m => m.Exists(It.IsAny<string>())).Returns(true);
            var uri = new Uri($@"https://www.mock.com/mock-album-name/track01.flac");

            // act
            var actualDestinationPath = destinationPathBuilder.CreateDestinationPath(uri);

            // assert
            mockFileWrapper.Verify(m => m.Delete(actualDestinationPath), Times.Once);
        }

        [TestMethod]
        public void WhenFileDoesNotAlreadyExist_DoesNotAttemptToDeleteFile()
        {
            // arrange
            mockFileWrapper.Setup(m => m.Exists(It.IsAny<string>())).Returns(false);
            var uri = new Uri($@"https://www.mock.com/mock-album-name/track01.flac");

            // act
            var actualDestinationPath = destinationPathBuilder.CreateDestinationPath(uri);

            // assert
            mockFileWrapper.Verify(m => m.Delete(actualDestinationPath), Times.Never);
        }
    }
}
