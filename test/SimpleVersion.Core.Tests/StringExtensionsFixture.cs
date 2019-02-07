using FluentAssertions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
    }
}
