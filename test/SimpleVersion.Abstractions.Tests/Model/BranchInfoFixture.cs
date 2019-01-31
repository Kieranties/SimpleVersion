using FluentAssertions;
using SimpleVersion.Model;
using Xunit;

namespace SimpleVersion.Abstractions.Tests.Model
{
    public class BranchInfoFixture
    {
        [Fact]
        public void Ctor_PopulatesEmpty_Release()
        {
            // Arrange / Act
            var sut = new BranchInfo();

            // Assert
            sut.Release.Should().BeEmpty();
        }
    }
}
