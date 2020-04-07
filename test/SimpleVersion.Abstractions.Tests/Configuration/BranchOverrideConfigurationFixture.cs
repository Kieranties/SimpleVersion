// Licensed under the MIT license. See https://kieranties.mit-license.org/ for full license information.

using FluentAssertions;
using SimpleVersion.Configuration;
using Xunit;

namespace SimpleVersion.Abstractions.Tests.Configuration
{
    public class BranchOverrideConfigurationFixture
    {
        [Fact]
        public void Ctor_SetsDefaults()
        {
            // Arrange / Act
            var sut = new BranchOverrideConfiguration();

            // Assert
            sut.Label.Should().BeNull();
            sut.PrefixLabel.Should().BeNull();
            sut.PostfixLabel.Should().BeNull();
            sut.InsertLabel.Should().BeNull();
            sut.Match.Should().BeEmpty();
            sut.Metadata.Should().BeNull();
            sut.PrefixMetadata.Should().BeNull();
            sut.PostfixMetadata.Should().BeNull();
            sut.InsertMetadata.Should().BeNull();
        }
    }
}
