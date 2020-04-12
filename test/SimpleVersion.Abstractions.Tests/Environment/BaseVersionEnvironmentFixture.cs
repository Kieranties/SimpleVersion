// Licensed under the MIT license. See https://kieranties.mit-license.org/ for full license information.

using System;
using FluentAssertions;
using NSubstitute;
using SimpleVersion.Environment;
using Xunit;

namespace SimpleVersion.Abstractions.Tests.Environment
{
    public class BaseVersionEnvironmentFixture
    {
        [Fact]
        public void Ctor_NullAccessor_Throws()
        {
            // Arrange
            Action action = () => new StubVersionEnvironment(null);

            // Act / Assert
            action.Should().Throw<ArgumentNullException>()
                .And.ParamName.Should().Be("accessor");
        }

        [Fact]
        public void Ctor_WithAccessor_SetsProperty()
        {
            // Arrange
            var accessor = Substitute.For<IEnvironmentVariableAccessor>();

            // Act
            var instance = new StubVersionEnvironment(accessor);

            // Assert
            instance.Variables.Should().BeSameAs(accessor);
        }

        private class StubVersionEnvironment : BaseVersionEnvironment
        {
            public StubVersionEnvironment(IEnvironmentVariableAccessor accessor) : base(accessor)
            {
            }

            public override string CanonicalBranchName => throw new NotImplementedException("Should not be tested");

            public override string BranchName => throw new NotImplementedException("Should not be tested");

            public override bool IsValid => throw new NotImplementedException("Should not be tested");
        }
    }
}
