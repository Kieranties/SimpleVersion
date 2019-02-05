using FluentAssertions;
using SimpleVersion.Model;
using SimpleVersion.Pipeline;
using Xunit;

namespace SimpleVersion.Core.Tests.Pipeline
{
    public class VersionCalculatorFixture
    {
        private readonly VersionCalculator _sut;

        public VersionCalculatorFixture()
        {
            _sut = new VersionCalculator();
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("   \t\t")]
        public void GetResult_EmptyPath_ReturnsResult(string path)
        {
            // Arrange / Act
            var result = _sut.GetResult(path);

            // Assert
            result.Should().BeEquivalentTo(new VersionResult());
        }
    }
}
