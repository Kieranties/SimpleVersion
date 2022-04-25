// Licensed under the MIT license. See https://kieranties.mit-license.org/ for full license information.

using System;
using FluentAssertions;
using NSubstitute;
using SimpleVersion.Environment;
using SimpleVersion.Pipeline;
using Xunit;

namespace SimpleVersion.Core.Tests.Environment
{
    public class TeamCityEnvironmentFixture
    {
        private readonly IEnvironmentVariableAccessor _env;

        public TeamCityEnvironmentFixture()
        {
            _env = Substitute.For<IEnvironmentVariableAccessor>();
        }

        [Fact]
        public void Ctor_NullAccessor_Throws()
        {
            // Arrange
            Action action = () => new TeamCityEnvironment(null);

            // Act / Assert
            action.Should().Throw<ArgumentNullException>()
                .And.ParamName.Should().Be("accessor");
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("  ")]
        [InlineData("\t\t  ")]
        public void IsValid_EnvironmentNotSet_ReturnsFalse(string value)
        {
            // Arrange
            _env.GetVariable("TEAMCITY_VERSION").Returns(value);
            var sut = new TeamCityEnvironment(_env);

            // Act
            var result = sut.IsValid;

            // Assert
            result.Should().BeFalse();
        }

        [Theory]
        [InlineData("1.0.0", true)]
        [InlineData("2021.4.1", true)]
        [InlineData("False", true)]
        [InlineData("1", true)]
        [InlineData("TRUE", true)]
        [InlineData("true", true)]
        [InlineData("True", true)]
        [InlineData("0", true)]
        public void IsValid_EnvironmentSet_ReturnsExpected(string value, bool expected)
        {
            // Arrange
            _env.GetVariable("TEAMCITY_VERSION").Returns(value);
            var sut = new TeamCityEnvironment(_env);

            // Act
            var result = sut.IsValid;

            // Assert
            result.Should().Be(expected);
        }

        [Theory]
        [InlineData("refs/heads/master")]
        [InlineData("refs/heads/release/1.0/master")]
        [InlineData("refs/pull/10/merge")]
        public void CanonicalBranchName_ReadsCorrectVariable(string canonical)
        {
            // Arrange
            _env.GetVariable("BUILD_SOURCEBRANCH").Returns(canonical);
            var sut = new TeamCityEnvironment(_env);

            // Act
            var result = sut.CanonicalBranchName;

            // Assert
            result.Should().Be(canonical);
        }

        [Theory]
        [InlineData("refs/heads/master", "master")]
        [InlineData("refs/heads/release/1.0/master", "release/1.0/master")]
        [InlineData("refs/pull/10/merge", "pull/10/merge")]
        public void BranchName_ReadsCorrectVariable(string canonical, string branch)
        {
            // Arrange
            _env.GetVariable("BUILD_SOURCEBRANCH").Returns(canonical);
            var sut = new TeamCityEnvironment(_env);

            // Act
            var result = sut.BranchName;

            // Assert
            result.Should().Be(branch);
        }
    }
}
