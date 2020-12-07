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

        [DataTestMethod]
        [DataRow(null)]
        [DataRow(" ")]
        [DataRow("www.contoso.com/path/file")]
        public void WhenSourcePathInvalid_Throws(string sourcePath)
        {
            Assert.ThrowsException<ArgumentException>(() => destinationPathBuilder.CreateDestinationPath(sourcePath));
        }
    }
}
