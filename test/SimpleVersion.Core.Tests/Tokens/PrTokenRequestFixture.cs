// Licensed under the MIT license. See https://kieranties.mit-license.org/ for full license information.

using System;
using FluentAssertions;
using SimpleVersion.Tokens;
using Xunit;

namespace SimpleVersion.Core.Tests.Tokens
{
    public class PrTokenRequestFixture
    {
        private readonly PrTokenRequest _sut;

        public PrTokenRequestFixture()
        {
            _sut = new PrTokenRequest();
        }

        [Fact]
        public void HasTokenKey_Set()
        {
            // Arrange / Act
            var attr = TokenAttribute.ResolveFor(_sut.GetType());

            // Assert
            attr.Key.Should().Be("pr");
            attr.Description.Should().Be("Provides parsing of the pull-request number.");
        }

        [Fact]
        public void Parse_Throws()
        {
            // Arrange
            Action action = () => _sut.Parse(null);

            // Act / Assert
            action.Should().Throw<InvalidOperationException>()
                .WithMessage("Option values not supported.");
        }
    }
}
