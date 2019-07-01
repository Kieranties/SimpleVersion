// Licensed under the MIT license. See https://kieranties.mit-license.org/ for full license information.

using FluentAssertions;
using Xunit;

namespace SimpleVersion.Core.Tests
{
    public class StringExtensionsFixture
    {
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

        [Fact]
        public void FormatWith_GivenValues_FormatsString()
        {
            // Arrange
            var format = "This {0} is {1} formatted. {0}";
            var values = new[] { "Zero", "One" };

            // Act
            var result = StringExtensions.FormatWith(format, values);

            // Assert
            result.Should().Be("This Zero is One formatted. Zero");
        }
    }
}
