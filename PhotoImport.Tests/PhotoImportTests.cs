using System.Collections.Generic;
using FakeItEasy;
using Xunit;

namespace PhotoImport.Tests
{
    public class PhotoImportTests
    {
        private readonly IImageDiscovery _imageDiscovery;
        private readonly PhotoImport _photoImport;
        private readonly string baseDirectory;
        private readonly IUserRespondsPositively _userRespondsPositively;
        private readonly IUserIO _userIO;

        public PhotoImportTests()
        {
            _imageDiscovery = A.Fake<IImageDiscovery>();
            _userRespondsPositively = A.Fake<IUserRespondsPositively>();
            _userIO = A.Fake<IUserIO>();
            _photoImport = new PhotoImport(_userIO,_userRespondsPositively, _imageDiscovery);
            baseDirectory = "/TestDirectory/";
        }

        [Fact]
        public void TestThatPhotoImportGetsListOfDirectories()
        {
            //arrange
            //act
            _photoImport.Run(baseDirectory, string.Empty);
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
            _photoImport.Run(baseDirectory, string.Empty);
            //assert
            A.CallTo(() => _userRespondsPositively.ToQuestion(A<string>.Ignored)).MustHaveHappened(Repeated.Exactly.Twice);
        }

        [Fact]
        public void TestThatWhenUserWantsToProcessTheDirectoryItAsksForADescription()
        {
            //arrange
            A.CallTo(() => _userRespondsPositively.ToQuestion(A<string>.Ignored)).Returns(true);
            //act
            _photoImport.ProcessImageDirectory(baseDirectory+"SubDirectory", string.Empty);
            //assert
            A.CallTo(() => _userIO.WriteLine(A<string>.That.Matches(s => s.ToLower().Contains("enter a description"))))
                .MustHaveHappened();
            A.CallTo(() => _userIO.ReadLine())
                .MustHaveHappened();
        }
        
        [Fact]
        public void TestThatWhenUserDoesntWantToProcessTheDirectoryItDoesntAskForADescription()
        {
            //arrange
            A.CallTo(() => _userRespondsPositively.ToQuestion(A<string>.Ignored)).Returns(false);
            //act
            _photoImport.ProcessImageDirectory(baseDirectory+"SubDirectory", string.Empty);
            //assert
            A.CallTo(() => _userIO.WriteLine(A<string>.Ignored))
                .MustNotHaveHappened();
            A.CallTo(() => _userIO.ReadLine())
                .MustNotHaveHappened();
        }
        
  
    }
}