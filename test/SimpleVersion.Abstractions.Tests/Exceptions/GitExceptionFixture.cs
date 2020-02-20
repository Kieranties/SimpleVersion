// Licensed under the MIT license. See https://kieranties.mit-license.org/ for full license information.

using System;
using System.Collections.Generic;
using FluentAssertions;
using SimpleVersion.Abstractions.Exceptions;
using Xunit;

namespace SimpleVersion.Abstractions.Tests.Exceptions
{
    public class GitExceptionFixture
    {
        public static IEnumerable<object[]> Inputs()
        {
            yield return new[] { string.Empty };
            yield return new[] { "\t\t\t   " };
            yield return new[] { "My Message" };
        }

        [Fact]
        public void Ctor_NoParams_DoesNotThrow()
        {
            // Arrange
            Action action = () => new GitException();

            // Act / Assert
            action.Should().NotThrow();
        }

        [Theory]
        [MemberData(nameof(Inputs))]
        public void Ctor_WithMessage_SetsMessage(string value)
        {
            // Arrange / Act
            var result = new GitException(value);

            // Assert
            result.Message.Should().Be(value);
        }

        [Fact]
        public void Ctor_WithNullMessage_ShouldGiveDefault()
        {
            // Arrange / ACt
            var result = new GitException(null);

            // Assert
            result.Message.Should().Be("Exception of type 'SimpleVersion.Abstractions.Exceptions.GitException' was thrown.");
        }

        [Theory]
        [MemberData(nameof(Inputs))]
        public void Ctor_WithMessage_And_Exception_SetsParameters(string value)
        {
            // Arrange
            var exception = new Exception(value);

            // Act
            var result = new GitException(value, exception);

            // Assert
            result.Message.Should().Be(value);
            result.InnerException.Should().BeSameAs(exception);
        }

        
    }
}
