// Licensed under the MIT license. See https://kieranties.mit-license.org/ for full license information.

using System;
using FluentAssertions;
using GitTools.Testing;
using SimpleVersion.Exceptions;
using SimpleVersion.Pipeline;
using Xunit;

namespace SimpleVersion.Core.Tests.Pipeline
{
    public class VersionContextFixture
    {
        [Fact]
        public void Ctor_NullRepo_Throws()
        {
            // Arrange / Act
            Action action = () => new SimpleVersion.Pipeline.VersionContext(null);

            // Assert
            action.Should().Throw<ArgumentNullException>()
                .And.ParamName.Should().Be("repository");
        }

        [Fact]
        public void Ctor_WithEmptyRepo_Throws()
        {
            using (var fixture = new EmptyRepositoryFixture())
            {
                // Arrange
                Action action = () => new SimpleVersion.Pipeline.VersionContext(fixture.Repository);

                // Act / Assert
                action.Should().Throw<GitException>()
                    .WithMessage("Could not find the current branch tip. Unable to identify the current commit.");
            }
        }

        [Fact]
        public void Ctor_WithCommittedRepo_SetsProperties()
        {
            using (var fixture = new EmptyRepositoryFixture())
            {
                fixture.MakeACommit();
                fixture.MakeACommit();
                fixture.MakeACommit();

                // Arrange / Act
                var sut = new SimpleVersion.Pipeline.VersionContext(fixture.Repository);

                // Assert
                sut.Repository.Should().Be(fixture.Repository);
                sut.Configuration.Should().NotBeNull();
                sut.Result.BranchName.Should().Be(fixture.Repository.Head.FriendlyName);
                sut.Result.CanonicalBranchName.Should().Be(fixture.Repository.Head.CanonicalName);
                sut.Result.Sha.Should().Be(fixture.Repository.Head.Tip.Sha);
                sut.Result.Sha7.Should().Be(fixture.Repository.Head.Tip.Sha.Substring(0, 7));
            }
        }
    }
}
