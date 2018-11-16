using McMaster.Extensions.CommandLineUtils;
using Microsoft.Extensions.DependencyInjection;
using SimpleVersion.Git;
using static System.IO.Directory;

namespace SimpleVersion.Command
{
    [HelpOption]
    public class Program
    {
        private readonly IConsole _console;
        private readonly IRepositoryResolver _repoResolver;
        private readonly IResultFormatter _formatter;

        public static int Main(string[] args)
        {
            var services = new ServiceCollection()
                .AddSingleton<IVersionModelReader, JsonVersionModelReader>()
                .AddSingleton<IRepositoryResolver, GitRepositoryResolver>()
                .AddSingleton<IResultFormatter, Semver2Formatter>()
                .BuildServiceProvider();

            var app = new CommandLineApplication<Program>();
            app.Conventions
                .UseDefaultConventions()
                .UseConstructorInjection(services);

            return app.Execute(args);
        }

        [Option(Description = "The path to the respository")]
        public string Path { get; } = GetCurrentDirectory();

        public Program(
            IConsole console,
            IRepositoryResolver repoResolver,
            IResultFormatter formatter)
        {
            _console = console;
            _repoResolver = repoResolver;
            _formatter = formatter;
        }
        
        public void OnExecute()
        {
            var repo = _repoResolver.Resolve(Path);
            var (height, model) = repo.GetResult();
            var formattedResult = _formatter.Format(height, model);

            _console.WriteLine(formattedResult.FullVersion);
        }
    }
}
