// Licensed under the MIT license. See https://kieranties.mit-license.org/ for full license information.

using System;
using FluentAssertions;
using SimpleVersion.Tokens;
using Xunit;

namespace SimpleVersion.Core.Tests.Tokens
{
    public class SemverTokenRequestFixture
    {
        private readonly SemverTokenRequest _sut;

        public SemverTokenRequestFixture()
        {
            _sut = new SemverTokenRequest();
        }

        [Fact]
        public void HasTokenKey_Set()
        {
            // Arrange / Act
            var attr = TokenAttribute.ResolveFor(_sut.GetType());

            // Assert
            attr.Key.Should().Be("semver");
            attr.Description.Should().Be("Provides parsing of full semver compatible versions.");
        }

        [Fact]
        public void Ctor_SetsDefaults()
        {
            _sut.Version.Should().Be(2);
        }

        [Theory]
        [InlineData(0)]
        [InlineData(3)]
        public void Version_InvalidValue_Throws(int value)
        {
            // Arrange
            Action action = () => _sut.Version = value;

            // Act / Assert
            action.Should().Throw<InvalidOperationException>()
                .WithMessage("Value must be 1 or 2.");

        }

        [Theory]
        [InlineData("1", 1)]
        [InlineData("002", 2)]
        public void Parse_ValidValues_SetsVersion(string value, int expected)
        {
            // Act
            _sut.Parse(value);

            // Assert
            _sut.Version.Should().Be(expected);
        }

        [Theory]
        [InlineData("0", "Value must be 1 or 2.")]
        [InlineData("3", "Value must be 1 or 2.")]
        [InlineData("non-int", "Invalid value")]
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
