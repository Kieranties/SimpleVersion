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

            AssertUtils.AssertGetSetProperty(sut, nameof(sut.Branches), x => x.Should().NotBeNull(), new BranchConfiguration { Release = { "test" } });
        }
    }
}
