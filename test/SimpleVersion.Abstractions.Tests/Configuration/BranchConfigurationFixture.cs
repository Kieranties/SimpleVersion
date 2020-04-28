// Licensed under the MIT license. See https://kieranties.mit-license.org/ for full license information.

using System.Collections.Generic;
using FluentAssertions;
using SimpleVersion.Configuration;
using Xunit;

namespace SimpleVersion.Abstractions.Tests.Configuration
{
    public class BranchConfigurationFixture
    {
        [Fact]
        public void Ctor_Property_Expectations()
        {
            var sut = new BranchConfiguration();
            AssertUtils.AssertGetSetProperty(sut, nameof(sut.Release), x => x.Should().BeEmpty(), (List<string>)null);
            AssertUtils.AssertGetSetProperty(sut, nameof(sut.Overrides), x => x.Should().BeEmpty(), (List<BranchOverrideConfiguration>)null);
        }
    }
}
