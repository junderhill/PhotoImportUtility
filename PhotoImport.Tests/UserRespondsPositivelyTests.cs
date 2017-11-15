using FakeItEasy;
using Xunit;

namespace PhotoImport.Tests
{
    public class UserRespondsPositivelyTests
    {
        private IUserIO _userIO;
        private UserRespondsPositively sut;

        public UserRespondsPositivelyTests()
        {
            sut = new UserRespondsPositively();
            _userIO = A.Fake<IUserIO>();
            sut.UserIO = _userIO;
        }
        
        [Theory]
        [InlineData("Y", true)]
        [InlineData("y", true)]
        [InlineData("yes", true)]
        [InlineData("YES", true)]
        [InlineData("Yes", true)]
        [InlineData("N", false)]
        [InlineData("n", false)]
        [InlineData("no", false)]
        [InlineData("NO", false)]
        [InlineData("No", false)]
        [InlineData("1", true)]
        [InlineData("0", false)]
        [InlineData("", false)]
        [InlineData("    Y", true)]
        [InlineData("y    ", true)]
        [InlineData("   yes  ", true)]
        [InlineData(" YES  ", true)]
        [InlineData("                    Yes", true)]
        [InlineData("N  ", false)]
        [InlineData("  n     ", false)]
        [InlineData("      no", false)]
        [InlineData("   N  O  ", false)]
        [InlineData(" No ", false)]
        [InlineData(" 1   ", true)]
        [InlineData("               0", false)]
        [InlineData("               ", false)]
        public void TestCorrectResponseParsing(string userResponse, bool expectedResult)
        {
           //arrange
            A.CallTo(() => _userIO.ReadLine()).Returns(userResponse);
            //act
            var actualResult = sut.ToQuestion("test");
            //assert
            Assert.Equal(expectedResult, actualResult);
        } 
    }
}