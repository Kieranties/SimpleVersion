// Licensed under the MIT license. See https://kieranties.mit-license.org/ for full license information.

using FluentAssertions;
using SimpleVersion.Configuration;
using System.Collections.Generic;
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
            AssertUtils.AssertGetSetProperty(sut, nameof(sut.Match), x => x.Should().BeEmpty(), "test");
            AssertUtils.AssertGetSetProperty(sut, nameof(sut.Label), x => x.Should().BeNull(), new List<string> { "test" });
            AssertUtils.AssertGetSetProperty(sut, nameof(sut.PrefixLabel), x => x.Should().BeNull(), new List<string> { "test" });
            AssertUtils.AssertGetSetProperty(sut, nameof(sut.PostfixLabel), x => x.Should().BeNull(), new List<string> { "test" });
            AssertUtils.AssertGetSetProperty(sut, nameof(sut.InsertLabel), x => x.Should().BeNull(), new Dictionary<int, string> { [1] = "test" });
            AssertUtils.AssertGetSetProperty(sut, nameof(sut.Metadata), x => x.Should().BeNull(), new List<string> { "test" });
            AssertUtils.AssertGetSetProperty(sut, nameof(sut.PrefixMetadata), x => x.Should().BeNull(), new List<string> { "test" });
            AssertUtils.AssertGetSetProperty(sut, nameof(sut.PostfixMetadata), x => x.Should().BeNull(), new List<string> { "test" });
            AssertUtils.AssertGetSetProperty(sut, nameof(sut.InsertMetadata), x => x.Should().BeNull(), new Dictionary<int, string> { [1] = "test" });
        }
    }
}
