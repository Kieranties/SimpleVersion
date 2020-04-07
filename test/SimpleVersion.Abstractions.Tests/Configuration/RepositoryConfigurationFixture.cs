// Licensed under the MIT license. See https://kieranties.mit-license.org/ for full license information.

using FluentAssertions;
using SimpleVersion.Configuration;
using Xunit;

namespace SimpleVersion.Abstractions.Tests.Configuration
{
    public class RepositoryConfigurationFixture
    {
        [Fact]
        public void Ctor_SetsDefaults()
        {
            // Arrange / Act
            var sut = new RepositoryConfiguration();

            // Assert
            sut.Label.Should().BeEmpty();
            sut.Metadata.Should().BeEmpty();
            sut.Version.Should().BeEmpty();
            sut.OffSet.Should().Be(0);
            sut.Branches.Should().NotBeNull();
        }
    }
}
