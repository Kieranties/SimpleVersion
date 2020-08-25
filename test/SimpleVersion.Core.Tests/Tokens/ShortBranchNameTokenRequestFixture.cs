// Licensed under the MIT license. See https://kieranties.mit-license.org/ for full license information.

using System;
using FluentAssertions;
using SimpleVersion.Tokens;
using Xunit;

namespace SimpleVersion.Core.Tests.Tokens
{
    public class ShortBranchNameTokenRequestFixture
    {
        private readonly ShortBranchNameTokenRequest _sut;

        public ShortBranchNameTokenRequestFixture()
        {
            _sut = new ShortBranchNameTokenRequest();
        }

        [Fact]
        public void HasTokenKey_Set()
        {
            // Arrange / Act
            var attr = TokenAttribute.ResolveFor(_sut.GetType());

            // Assert
            attr.Key.Should().Be("shortbranchname");
            attr.Description.Should().Be("Provides parsing of the branch name.");
        }

        [Fact]
        public void Ctor_SetsDefaults()
        {
            _sut.BranchName.Should().Be(BranchNameOption.Short);
        }

        [Fact]
        public void BranchName_Setter_Throws()
        {
            // Arrange
            Action action = () => _sut.BranchName = BranchNameOption.Suffix;

            // Act / Assert
            action.Should().Throw<InvalidOperationException>()
                .WithMessage($"{nameof(ShortBranchNameTokenRequest)} does not support changing options.");
        }

        [Fact]
        public void Parse_Throws()
        {
            // Arrange
            Action action = () => _sut.Parse("This is invalid");

            // Act / Assert
            action.Should().Throw<InvalidOperationException>()
                .WithMessage($"{nameof(ShortBranchNameTokenRequest)} does not support options.");
        }
    }
}
