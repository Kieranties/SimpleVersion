//using Xunit;
//using GitTools.Testing;
//using System.IO;
//using System;
//using LibGit2Sharp;

//namespace SimpleVersion.Git.Tests
//{
//    public class Playground
//    {
//        [Fact]
//        public void Can_Count_Commits_To_File()
//        {
//            var sut = new HeightCalculator();
//            using(var fixture = new EmptyRepositoryFixture())
//            {
//                // Create the file
//                var versionFileName = "version.json";
//                var versionFile = Path.Combine(fixture.RepositoryPath, versionFileName);
//                File.WriteAllText(versionFile, "{ \"version\": \"1.2.3.4\" }");

//                fixture.Repository.Index.Add(versionFileName);
//                fixture.Repository.Index.Write();
//                fixture.MakeACommit();
//                fixture.MakeACommit();
//                fixture.MakeACommit();

//                File.WriteAllText(versionFile, "{ \"version\": \"1.2.3\" }");
//                fixture.Repository.Index.Add(versionFileName);
//                fixture.Repository.Index.Write();
//                fixture.MakeACommit();
//                fixture.MakeACommit();

//                var result = sut.Count(fixture.Repository);

//                Assert.Equal(3, result);
//            }
//        }

//        [Fact]
//        public void Process()
//        {
//            using (var fixture = new EmptyRepositoryFixture())
//            {
//                // Create the file
//                var versionFileName = "version.json";
//                var versionFile = Path.Combine(fixture.RepositoryPath, versionFileName);
//                File.WriteAllText(versionFile, "{ \"version\": \"0.1.0\" }");

//                fixture.Repository.Index.Add(versionFileName);
//                fixture.Repository.Index.Write();
//                Commit(fixture, "Bump version");
//                var sut = new HeightCalculator();
//                var height = sut.Count(fixture.Repository);
//                fixture.ApplyTag($"v0.1.0+{height}");
//                Commit(fixture);
//                Commit(fixture);
//                Commit(fixture);
//                height = sut.Count(fixture.Repository);
//                fixture.ApplyTag($"v0.1.0+{height}");

//                File.WriteAllText(versionFile, "{ \"version\": \"1.0.0\" }");
//                fixture.Repository.Index.Add(versionFileName);
//                fixture.Repository.Index.Write();
//                Commit(fixture, "Bump version");
//                height = sut.Count(fixture.Repository);
//                fixture.ApplyTag($"v1.0.0+{height}");
//                Commit(fixture);
//                Commit(fixture);
//                Commit(fixture);
//                Commit(fixture);
//                Commit(fixture);
//                Commit(fixture);
//                Commit(fixture);
//                Commit(fixture);

//                height = sut.Count(fixture.Repository);
//                fixture.ApplyTag($"v1.0.0+{height}");
//            }
//        }

//        private void Commit(RepositoryFixtureBase fixture, string message = "Example")
//        {
//            fixture.Repository.Commit(
//                  message,
//                  new Signature("kmn", "kmn@kmn.com", DateTime.UtcNow),
//                  new Signature("kmn", "kmn@kmn.com", DateTime.UtcNow),
//                  new CommitOptions { AllowEmptyCommit = true });
//        }
//    }
//}
