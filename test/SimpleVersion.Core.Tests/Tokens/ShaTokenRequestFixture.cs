// Licensed under the MIT license. See https://kieranties.mit-license.org/ for full license information.

using System;
using FluentAssertions;
using SimpleVersion.Tokens;
using Xunit;

namespace SimpleVersion.Core.Tests.Tokens
{
    public class ShaTokenRequestFixture
    {
        private readonly ShaTokenRequest _sut;

        public ShaTokenRequestFixture()
        {
            _sut = new ShaTokenRequest();
        }

        [Fact]
        public void HasTokenKey_Set()
        {
            // Arrange / Act
            var attr = TokenAttribute.ResolveFor(_sut.GetType());

            // Assert
            attr.Key.Should().Be("sha");
            attr.Description.Should().Be("Provides parsing of the commit sha.");
        }

        [Fact]
        public void Ctor_SetsDefaults()
        {
            _sut.Length.Should().Be(40);
        }

        [Fact]
        public void Length_InvalidValue_Throws()
        {
            // Arrange
            Action action = () => _sut.Length = -10;

            // Act / Assert
            action.Should().Throw<InvalidOperationException>()
                .WithMessage("Must be greater than zero.");

        }

        [Theory]
        [InlineData("300", 40)]
        [InlineData("030", 30)]
        [InlineData("003", 3)]
        [InlineData(null, 40)]
        [InlineData("", 40)]
        [InlineData("\t\t  ", 40)]
        [InlineData("short", 7)]
        [InlineData("SHORT", 7)]
        [InlineData("ShOrT", 7)]
        [InlineData("full", 40)]
        [InlineData("FULL", 40)]
        [InlineData("FuLl", 40)]
        public void Parse_ValidValues_SetsPadding(string value, int expected)
        {
            // Act
            _sut.Parse(value);

            // Assert
            _sut.Length.Should().Be(expected);
        }

        [Theory]
        [InlineData("-10", "Must be greater than zero.")]
        [InlineData("lies", "Invalid value.")]
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
