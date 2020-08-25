// Licensed under the MIT license. See https://kieranties.mit-license.org/ for full license information.

using System;
using FluentAssertions;
using SimpleVersion.Tokens;
using Xunit;

namespace SimpleVersion.Core.Tests.Tokens
{
    public class LabelTokenRequestFixture
    {
        private readonly LabelTokenRequest _sut;

        public LabelTokenRequestFixture()
        {
            _sut = new LabelTokenRequest();
        }

        [Fact]
        public void HasTokenKey_Set()
        {
            // Arrange / Act
            var attr = TokenAttribute.ResolveFor(_sut.GetType());

            // Assert
            attr.Key.Should().Be("label");
            attr.Description.Should().Be("Provides parsing of the version label.");
        }

        [Fact]
        public void Ctor_SetsDefaults()
        {
            _sut.Separator.Should().Be(".");
        }

        [Theory]
        [InlineData(null, "")]
        [InlineData("", "")]
        [InlineData("\t\t  ", "\t\t  ")]
        public void Parse_ValidValues_SetsPadding(string value, string expected)
        {
            // Act
            _sut.Parse(value);

            // Assert
            _sut.Separator.Should().Be(expected);
        }
    }
}
