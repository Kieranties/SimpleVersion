// Licensed under the MIT license. See https://kieranties.mit-license.org/ for full license information.

using FluentAssertions;
using SimpleVersion.Configuration;
using Xunit;

namespace SimpleVersion.Abstractions.Tests.Configuration
{
    public class BranchConfigurationFixture
    {
        [Fact]
        public void Ctor_PopulatesEmpty_Release()
        {
            // Arrange / Act
            var sut = new BranchConfiguration();

            // Assert
            sut.Release.Should().BeEmpty();
            sut.Overrides.Should().BeEmpty();
        }
    }
}
