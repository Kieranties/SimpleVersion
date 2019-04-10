// Licensed under the MIT license. See https://kieranties.mit-license.org/ for full license information.

using LibGit2Sharp;
using System;
using System.IO;

namespace SimpleVersion.Pipeline
{
    public class ResolveRepositoryPathProcess : ICalculatorProcess
    {
        public void Apply(VersionContext context)
        {
            if (context == null)
                throw new ArgumentNullException(nameof(context));

            if (string.IsNullOrWhiteSpace(context.Path))
                throw new Exception("Context path should be populated");

            var resolvedPath = Repository.Discover(context.Path);

            if (string.IsNullOrWhiteSpace(resolvedPath))
                throw new DirectoryNotFoundException($"Could not find git repository at '{context.Path}' or any parent directory");

            // resolvedPath is full pth to .git folder.  We want the repo root.
            resolvedPath = resolvedPath.Trim(Path.DirectorySeparatorChar);
            context.RepositoryPath = Directory.GetParent(resolvedPath).FullName;
        }
    }
}
