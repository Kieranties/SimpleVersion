// Licensed under the MIT license. See https://kieranties.mit-license.org/ for full license information.

using FluentAssertions;
using SimpleVersion.Extensions;
using Xunit;

namespace SimpleVersion.Core.Tests.Extensions
{
    public class StringExtensionsFixture
    {
        [Theory]
        [InlineData(null, "default")]
        [InlineData("", "default")]
        [InlineData("\t\t\t   ", "default")]
        public void DefaultIfNullOrWhiteSpace_NullOrWhitespaceValue_ReturnsDefault(string value, string @default)
        {
            // Arrange / Act
            var result = StringExtensions.DefaultIfNullOrWhiteSpace(value, @default);

            // Assert
            result.Should().Be(@default);
        }

        [Theory]
        [InlineData("Hi!", "default")]
        public void DefaultIfNullOrWhiteSpace_Non_NullOrWhitespaceValue_ReturnsValue(string value, string @default)
        {
            // Arrange / Act
            var result = StringExtensions.DefaultIfNullOrWhiteSpace(value, @default);

            // Assert
            result.Should().Be(value);
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData(" ")]
        [InlineData("\t\t")]
        [InlineData("FALSE")]
        [InlineData("false")]
        [InlineData("False")]
        [InlineData("Nope")]
        [InlineData("1")]
        [InlineData("-1")]
        [InlineData("00001")]
        [InlineData("0.1")]
        public void ToBool_FalseValues_ReturnsFalse(string value)
        {
            // Arrange / Act
            var result = StringExtensions.ToBool(value);

            // Assert
            result.Should().BeFalse();
        }

        [Theory]
        [InlineData("TRUE")]
        [InlineData("true")]
        [InlineData("True")]
        [InlineData("0")]
        [InlineData("-0")]
        [InlineData("00000")]
        public void ToBool_TrueValues_ReturnsTrue(string value)
        {
            // Arrange / Act
            var result = StringExtensions.ToBool(value);

            // Assert
            result.Should().BeTrue();
        }
    }
}
