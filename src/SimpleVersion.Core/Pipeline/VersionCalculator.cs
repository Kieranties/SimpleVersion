using SimpleVersion.Model;
using SimpleVersion.Pipeline.BuildServers;
using SimpleVersion.Pipeline.Formatting;
using System;
using System.Collections.Generic;

namespace SimpleVersion.Pipeline
{
    public class VersionCalculator : IVersionCalculator
    {
        public static IVersionCalculator Default() 
            => new VersionCalculator()
                .AddProcessor<ResolveRepositoryPathProcess>()
                .AddProcessor<ResolveConfigurationProcess>()
                .AddProcessor<AzureDevopsProcess>()
                .AddProcessor<VersionFormatProcess>()
                .AddProcessor<Semver1FormatProcess>()
                .AddProcessor<Semver2FormatProcess>();

        private readonly Queue<Lazy<ICalculatorProcess>> _queue = new Queue<Lazy<ICalculatorProcess>>();

        public IVersionCalculator AddProcessor<T>() where T : ICalculatorProcess, new()
        {
            _queue.Enqueue(new Lazy<ICalculatorProcess>(() => new T()));
            return this;
        }

        public VersionResult GetResult(string path)
        {
            var ctx = new VersionContext { Path = path };

            foreach (var processor in _queue)
                processor.Value.Apply(ctx);
            
            return ctx.Result;
        }
    }
}
