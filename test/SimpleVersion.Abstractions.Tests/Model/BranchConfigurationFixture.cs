// Licensed under the MIT license. See https://kieranties.mit-license.org/ for full license information.

using FluentAssertions;
using SimpleVersion.Model;
using Xunit;

namespace SimpleVersion.Abstractions.Tests.Model
{
    public class BranchConfigurationFixture
    {
        [Fact]
        public void Ctor_SetsDefaults()
        {
            // Arrange / Act
            var sut = new BranchConfiguration();

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
