// Licensed under the MIT license. See https://kieranties.mit-license.org/ for full license information.

using System.Collections.Generic;
using System.Linq;
using FluentAssertions;
using GitTools.Testing;
using SimpleVersion.Pipeline.Formatting;
using Xunit;

namespace SimpleVersion.Core.Tests
{
    public class EndToEndFixture
    {
        // https://github.com/Kieranties/SimpleVersion/issues/71
        [Fact]
        public void Override_Branches_Do_Not_Work_If_Asterisk_Used_In_Label()
        {
            // Create the configuration model
            var config = new Model.Settings
            {
                Version = "1.0.0",
                Label = { "r*" },
                Branches =
                    {
                        Release =
                        {
                            "^refs/heads/master$",
                            "^refs/heads/release/.+$",
                            "^refs/heads/feature/.+$"
                        },
                        Overrides =
                        {
                            new Model.BranchSettings
                            {
                                Match = "^refs/heads/feature/.+$",
                                Label = new List<string> { "{shortbranchname}" }
                            }
                        }
                    }
            };

            // Arrange
            using (var fixture = new SimpleVersionRepositoryFixture(config))
            using (EnvironmentContext.NoBuildServer())
            {
                // Make some extra commits on master
                fixture.MakeACommit();
                fixture.MakeACommit();
                fixture.MakeACommit();

                // branch to a feature branch
                fixture.BranchTo("feature/PBI-319594-GitVersionDeprecation");
                fixture.MakeACommit();
                fixture.MakeACommit();
                fixture.MakeACommit();

                // Act
                var result = GetResult(fixture);
                var semver1 = result.Formats[Semver1FormatProcess.FormatKey];
                var semver2 = result.Formats[Semver2FormatProcess.FormatKey];

                // Assert
                semver1.Should().Be("1.0.0-featurePBI319594GitVersionDeprecation-0007");
                semver2.Should().Be("1.0.0-featurePBI319594GitVersionDeprecation.7");
            }
        }

        private static Model.VersionResult GetResult(RepositoryFixtureBase repo) => VersionCalculator.Default().GetResult(repo.RepositoryPath);
    }
}
