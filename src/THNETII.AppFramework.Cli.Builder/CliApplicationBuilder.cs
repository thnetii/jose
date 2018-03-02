using McMaster.Extensions.CommandLineUtils;
using System;
using System.Threading;
using System.Threading.Tasks;
using THNETII.Common;

namespace THNETII.AppFramework.Cli
{
    public class CliApplicationBuilder<TCommand> : CliCommandBuilder<TCommand>
        where TCommand : CliCommand
    {
        private Func<CommandLineApplication> appFactory;

        public CliApplicationBuilder() : base() { }

        public CliCommandBuilder<TCommand> UseApplicationInitializer(
            Func<CommandLineApplication> appFactory)
        {
            this.appFactory = appFactory;
            return this;
        }

        public Task<int> ExecuteAsync(string[] args,
            CancellationToken cancellationToken = default)
            => ExecuteImpl(args, async: true, cancellationToken);

        private Task<int> ExecuteImpl(string[] args, bool async = false,
            CancellationToken cancelToken = default)
        {
            var app = appFactory?.Invoke() ?? new CommandLineApplication();
            Task<int> resultTask = default;
            PrepareExecute(app, async, t => resultTask = t, cancelToken);
            app.Execute(args.ZeroLengthIfNull());
            return resultTask;
        }

        public int Execute(string[] args) => ExecuteImpl(args, async: false)
            .GetAwaiter().GetResult();
    }
}
