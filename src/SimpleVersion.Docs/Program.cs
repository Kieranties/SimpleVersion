// Licensed under the MIT license. See https://kieranties.mit-license.org/ for full license information.

using System.IO;
using SimpleVersion.Configuration;

namespace SimpleVersion.Docs
{
    /// <summary>
    /// Command line entry point.
    /// </summary>
    public sealed class Program
    {
        /// <summary>
        /// Entry point for Docs invocation.
        /// </summary>
        /// <param name="args">The array of arguments.</param>
        public static void Main(string[] args)
        {
            var path = args[0];

            SchemaGenerator.Generate(typeof(RepositoryConfiguration), path);
        }
    }
}
