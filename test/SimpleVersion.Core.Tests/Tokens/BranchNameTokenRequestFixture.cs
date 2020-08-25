// Licensed under the MIT license. See https://kieranties.mit-license.org/ for full license information.

using System;
using FluentAssertions;
using SimpleVersion.Tokens;
using Xunit;

namespace SimpleVersion.Core.Tests.Tokens
{
    public class BranchNameTokenRequestFixture
    {
        private readonly BranchNameTokenRequest _sut;

        public BranchNameTokenRequestFixture()
        {
            _sut = new BranchNameTokenRequest();
        }

        [Fact]
        public void HasTokenKey_Set()
        {
            // Arrange / Act
            var attr = TokenAttribute.ResolveFor(_sut.GetType());

            // Assert
            attr.Key.Should().Be("branchname");
            attr.Description.Should().Be("Provides parsing of the branch name.");
        }

        [Fact]
        public void Ctor_SetsDefaults()
        {
            _sut.BranchName.Should().Be(BranchNameOption.Canonical);
        }

        [Theory]
        [InlineData("CANONICAL", BranchNameOption.Canonical)]
        [InlineData("canonical", BranchNameOption.Canonical)]
        [InlineData("CaNoNiCaL", BranchNameOption.Canonical)]
        [InlineData("SHORT", BranchNameOption.Short)]
        [InlineData("short", BranchNameOption.Short)]
        [InlineData("ShOrT", BranchNameOption.Short)]
        [InlineData("SUFFIX", BranchNameOption.Suffix)]
        [InlineData("suffix", BranchNameOption.Suffix)]
        [InlineData("SuFfIx", BranchNameOption.Suffix)]
        public void Parse_ValidValues_SetsBranchName(string value, BranchNameOption expected)
        {
            // Arrange / Act
            _sut.Parse(value);

            // Assert
            _sut.BranchName.Should().Be(expected);
        }

        [Fact]
        public void Parse_InvalidValue_Throws()
        {
            // Arrange
            Action action = () => _sut.Parse("This is invalid");

            // Act / Assert
            action.Should().Throw<ArgumentException>()
                .WithMessage("Invalid option 'This is invalid' - Valid options are 'canonical', 'short', 'suffix'.");
        }
    }
}
