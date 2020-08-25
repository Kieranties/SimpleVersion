// Licensed under the MIT license. See https://kieranties.mit-license.org/ for full license information.

using System;
using FluentAssertions;
using SimpleVersion.Tokens;
using Xunit;

namespace SimpleVersion.Core.Tests.Tokens
{
    public class HeightTokenRequestFixture
    {
        private readonly HeightTokenRequest _sut;

        public HeightTokenRequestFixture()
        {
            _sut = new HeightTokenRequest();
        }

        [Fact]
        public void HasTokenKey_Set()
        {
            // Arrange / Act
            var attr = TokenAttribute.ResolveFor(_sut.GetType());

            // Assert
            attr.Key.Should().Be("*");
            attr.Description.Should().Be("Provides parsing of the commit height.");
        }

        [Fact]
        public void Ctor_SetsDefaults()
        {
            _sut.Padding.Should().Be(0);
        }

        [Fact]
        public void Padding_InvalidValue_Throws()
        {
            // Arrange
            Action action = () => _sut.Padding = -10;

            // Act / Assert
            action.Should().Throw<InvalidOperationException>()
                .WithMessage("Value must be zero or greater");

        }

        [Theory]
        [InlineData("500", 500)]
        [InlineData("050", 50)]
        [InlineData("005", 5)]
        [InlineData(null, 0)]
        [InlineData("", 0)]
        [InlineData("\t\t  ", 0)]
        public void Parse_ValidValues_SetsPadding(string value, int expected)
        {
            // Act
            _sut.Parse(value);

            // Assert
            _sut.Padding.Should().Be(expected);
        }

        [Theory]
        [InlineData("-10", "Value must be zero or greater")]
        [InlineData("lies", "Invalid value")]
        public void Parse_InvalidValues_Throws(string value, string message)
        {
            // Arrange
            Action action = () => _sut.Parse(value);

            // Act / Assert
            action.Should().Throw<InvalidOperationException>()
                .WithMessage(message);
        }
    }
}
