// Licensed under the MIT license. See https://kieranties.mit-license.org/ for full license information.

using FluentAssertions;
using SimpleVersion.Pipeline;
using Xunit;

namespace SimpleVersion.Abstractions.Tests.Pipeline
{
    public class VersionContextFixture
    {
        [Fact]
        public void Ctor_SetsDefaults()
        {
            // Arrange / Act
            var sut = new VersionContext();

            // Assert
            sut.Path.Should().BeEmpty();
            sut.RepositoryPath.Should().BeEmpty();
            sut.Result.Should().NotBeNull();
            sut.Configuration.Should().NotBeNull();
        }
    }
}
