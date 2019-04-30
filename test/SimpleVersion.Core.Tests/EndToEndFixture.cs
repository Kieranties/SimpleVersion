using FluentAssertions;
using GitTools.Testing;
using SimpleVersion.Pipeline;
using SimpleVersion.Pipeline.Formatting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace SimpleVersion.Core.Tests
{
#pragma warning disable CA1063 // Implement IDisposable Correctly
    public class EndToEndFixture : IDisposable
#pragma warning restore CA1063 // Implement IDisposable Correctly
    {
        private readonly EmptyRepositoryFixture _repo;

        public EndToEndFixture()
        {
            _repo = new EmptyRepositoryFixture();
        }

#pragma warning disable CA1063 // Implement IDisposable Correctly
#pragma warning disable CA1816 // Dispose methods should call SuppressFinalize
        public void Dispose() => _repo?.Dispose();
#pragma warning restore CA1816 // Dispose methods should call SuppressFinalize
#pragma warning restore CA1063 // Implement IDisposable Correctly

        // https://github.com/Kieranties/SimpleVersion/issues/71
        [Fact]
        public void Override_Branches_Do_Not_Work_If_Asterisk_Used_In_Label()
        {
            // Arrange

            // Create the configuration model
            var config = new Model.Configuration
            {
                Version = "1.0.0",
                Label = { "r*" },
                Branches =
                {
                    Release = { "^refs/heads/master$", "^refs/heads/release/.+$", "^refs/heads/feature/.+$" },
                    Overrides =
                    {
                        new Model.BranchConfiguration
                        {
                            Match = "^refs/heads/feature/.+$",
                            Label = new List<string> { "{shortbranchname}" }
                        }
                    }
                }
            };
            Utils.WriteConfiguration(config, _repo);

            // Make some extra commits on master
            _repo.MakeACommit();
            _repo.MakeACommit();
            _repo.MakeACommit();

            // branch to a feature branch
            _repo.BranchTo("feature/PBI-319594-GitVersionDeprecation");
            _repo.MakeACommit();
            _repo.MakeACommit();
            _repo.MakeACommit();

            // Act
            var result = GetResult(_repo);

            // Assert
            result.Formats[Semver1FormatProcess.FormatKey].Should().Be("1.0.0-featurePBI319594GitVersionDeprecation-0007");
            result.Formats[Semver2FormatProcess.FormatKey].Should().Be("1.0.0-featurePBI319594GitVersionDeprecation.7");
        }

        private static Model.VersionResult GetResult(RepositoryFixtureBase repo) => VersionCalculator.Default().GetResult(repo.RepositoryPath);
    }
}
