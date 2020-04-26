// Licensed under the MIT license. See https://kieranties.mit-license.org/ for full license information.

using System.Collections.Generic;
using System.IO;
using FluentAssertions;
using SimpleVersion.Configuration;
using SimpleVersion.Serialization;
using Xunit;

namespace SimpleVersion.Core.Tests.Serialization
{
    public class SerializationFixture
    {
        private readonly Serializer _sut;

        public SerializationFixture()
        {
            _sut = new Serializer();
        }

        [Fact]
        public void Serialize_Configuration_MatchesExpected()
        {
            // Arrange
            var expected = File.ReadAllText(Path.Combine("Assets", "Expectations", "FullConfiguration.json"));
            var config = GetConfiguration();

            // Act
            var json = _sut.Serialize(config);

            // Assert
            json.Should().Be(expected);
        }

        [Fact]
        public void Deserialize_Configuration_MatchesExpected()
        {
            // Arrange
            var json = File.ReadAllText(Path.Combine("Assets", "Expectations", "FullConfiguration.json"));
            var expected = GetConfiguration();

            // Act
            var result = _sut.Deserialize<RepositoryConfiguration>(json);

            // Assert
            result.Should().BeEquivalentTo(expected);
        }

        private static RepositoryConfiguration GetConfiguration()
        {
            return new RepositoryConfiguration
            {
                Version = "1.2.3.4",
                Label = { "one", "two", "three" },
                Metadata = { "this", "and", "that" },
                OffSet = 10,
                Branches =
                {
                    Release = { "master", "release"},
                    Overrides =
                    {
                        new BranchOverrideConfiguration
                        {
                            Label = new List<string> { "a", "b" },
                            InsertLabel = new Dictionary<int, string>
                            {
                                [1] = "first",
                                [3] = "third"
                            },
                            PrefixLabel = new List<string> { "1", "2" },
                            PostfixLabel = new List<string> { "11", "22" },
                            Metadata = new List<string> { "ma", "mb" },
                            InsertMetadata = new Dictionary<int, string>
                            {
                                [1] = "mfirst",
                                [3] = "mthird"
                            },
                            PrefixMetadata = new List<string> { "m1", "m2" },
                            PostfixMetadata = new List<string> { "m11", "m22" },
                            Match = "master"
                        }
                    }
                }
            };
        }
    }
}
