// Licensed under the MIT license. See https://kieranties.mit-license.org/ for full license information.

using System;
using FluentAssertions;
using NSubstitute;
using SimpleVersion.Environment;
using Xunit;

namespace SimpleVersion.Core.Tests.Environment
{
    public class DefaultVersionEnvironmentFixture
    {
        private readonly IEnvironmentVariableAccessor _env;

        public DefaultVersionEnvironmentFixture()
        {
            _env = Substitute.For<IEnvironmentVariableAccessor>();
        }

        [Fact]
        public void Ctor_NullAccessor_Throws()
        {
            // Arrange
            Action action = () => new DefaultVersionEnvironment(null);

            // Act / Assert
            action.Should().Throw<ArgumentNullException>()
                .And.ParamName.Should().Be("accessor");
        }

        [Fact]
        public void IsValid_Environment_ResturnsTrue()
        {
            // Arrange
            var sut = new DefaultVersionEnvironment(_env);

            // Act
            var result = sut.IsValid;

            // Assert
            result.Should().BeTrue();
        }

        [Fact]
        public void CanonicalBranchName_ReturnsNull()
        {
            // Arrange
            var sut = new DefaultVersionEnvironment(_env);

            // Act
            var result = sut.CanonicalBranchName;

            // Assert
            result.Should().BeNull();
        }

        [Fact]
        public void BranchName_ReturnsNull()
        {
            // Arrange
            var sut = new DefaultVersionEnvironment(_env);

            // Act
            var result = sut.BranchName;

            // Assert
            result.Should().BeNull();
        }

        [Fact]
        public void BranchName_OverrideSet_ReturnsOverride()
        {
            // Arrange
            var expectedName = "OVERRIDE";
            _env.GetVariable("simpleversion.sourcebranch").Returns(expectedName);
            var sut = new DefaultVersionEnvironment(_env);

            // Act
            var result = sut.BranchName;

            // Assert
            result.Should().Be(expectedName);
        }
    }
}
