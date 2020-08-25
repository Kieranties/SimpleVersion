// Licensed under the MIT license. See https://kieranties.mit-license.org/ for full license information.

using System;
using FluentAssertions;
using SimpleVersion.Tokens;
using Xunit;

namespace SimpleVersion.Core.Tests.Tokens
{
    public class ShortShaTokenRequestFixture
    {
        private readonly ShortShaTokenRequest _sut;

        public ShortShaTokenRequestFixture()
        {
            _sut = new ShortShaTokenRequest();
        }

        [Fact]
        public void HasTokenKey_Set()
        {
            // Arrange / Act
            var attr = TokenAttribute.ResolveFor(_sut.GetType());

            // Assert
            attr.Key.Should().Be("shortsha");
            attr.Description.Should().Be("Provides parsing of the commit sha.");
        }

        [Fact]
        public void Ctor_SetsDefaults()
        {
            _sut.Length.Should().Be((int)ShaLengthOption.Short);
        }

        [Fact]
        public void Length_Setter_Throws()
        {
            // Arrange
            Action action = () => _sut.Length = 10;

            // Act / Assert
            action.Should().Throw<InvalidOperationException>()
                .WithMessage($"{nameof(ShortShaTokenRequest)} does not support changing 'Length'.");
        }

        [Fact]
        public void Parse_Throws()
        {
            // Arrange
            Action action = () => _sut.Parse("This is invalid");

            // Act / Assert
            action.Should().Throw<InvalidOperationException>()
                .WithMessage($"{nameof(ShortShaTokenRequest)} does not support options.");
        }
    }
}
