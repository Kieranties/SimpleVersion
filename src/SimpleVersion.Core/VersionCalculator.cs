// Licensed under the MIT license. See https://kieranties.mit-license.org/ for full license information.

using System;
using System.IO;
using LibGit2Sharp;
using SimpleVersion.Pipeline;
using SimpleVersion.Pipeline.Formatting;

namespace SimpleVersion
{
    /// <summary>
    /// Entry point for version calculation.
    /// </summary>
    public class VersionCalculator : IVersionCalculator
    {
        /// <summary>
        /// Default calculator instance.
        /// </summary>
        /// <returns>An instance of <see cref="VersionCalculator"/>.</returns>
        public static VersionCalculator Default() => new VersionCalculator();

        /// <inheritdoc/>
        public VersionResult GetResult(string path)
        {
            var resolvedPath = ResolveRepoPath(path);

            using (var repo = new Repository(resolvedPath))
            {
                // Initialize context
                var ctx = new VersionContext(repo);
                ctx.Result.RepositoryPath = Directory.GetParent(resolvedPath).Parent.FullName;

                // Resolve configuration
                ApplyProcessor<RepositoryConfigurationContextProcessor>(ctx);

                // Resolve Formats
                ApplyProcessor<VersionFormatProcess>(ctx);
                ApplyProcessor<Semver1FormatProcess>(ctx);
                ApplyProcessor<Semver2FormatProcess>(ctx);

                return ctx.Result;
            }
        }

        private string ResolveRepoPath(string path)
        {
            if (string.IsNullOrWhiteSpace(path))
            {
                throw new ArgumentException(Resources.Exception_PathMustBeProvided, nameof(path));
            }

            var resolvedPath = Repository.Discover(path);

            if (string.IsNullOrWhiteSpace(resolvedPath))
            {
                throw new DirectoryNotFoundException(Resources.Exception_CouldNotFindGitRepository(path));
            }

            return resolvedPath;
        }

        private void ApplyProcessor<T>(IVersionContext context)
            where T : IVersionContextProcessor, new()
        {
            var instance = new T();
            instance.Apply(context);
        }
    }
}
