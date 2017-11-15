using System.Collections.Generic;
using FakeItEasy;
using Xunit;

namespace PhotoImport.Tests
{
    public class PhotoImportTests
    {
        private IImageDiscovery _imageDiscovery;
        private PhotoImport _photoImport;
        private string baseDirectory;
        private IUserRespondsPositively _userRespondsPositively;

        public PhotoImportTests()
        {
            _imageDiscovery = A.Fake<IImageDiscovery>();
            _userRespondsPositively = A.Fake<IUserRespondsPositively>();
            _photoImport = new PhotoImport(_userRespondsPositively, _imageDiscovery);
            baseDirectory = "/TestDirectory/";
        }

        [Fact]
        public void TestThatPhotoImportGetsListOfDirectories()
        {
            //arrange
            //act
            _photoImport.Run(baseDirectory);
            //assert
            A.CallTo(() => _imageDiscovery.GetImageDirectories(baseDirectory)).MustHaveHappened();
        }

        [Fact]
        public void TestThatTheUserIsAskedForEachDirectoryIfItShouldBeProcessed()
        {
            //arrange
            A.CallTo(() => _imageDiscovery.GetImageDirectories(A<string>.Ignored))
                .Returns(new List<string> {"/TestDirectory/20170801/", "/TestDirectory/20170805"});
            //act
            _photoImport.Run(baseDirectory);
            //assert
            A.CallTo(() => _userRespondsPositively.ToQuestion(A<string>.Ignored)).MustHaveHappened(Repeated.Exactly.Twice);
        }
    }
}