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
            // TODO: arguments and options
            var path = System.IO.Directory.GetCurrentDirectory();
            if (args?.Length > 0)
            {
                path = args[0];
            }

            SchemaGenerator.Generate(typeof(RepositoryConfiguration), Path.Combine(path, "schema.json"));
        }
    }
}
