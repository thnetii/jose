using McMaster.Extensions.CommandLineUtils;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

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
        {
            return DoExecute(args, cmd =>
            {
                switch (cmd)
                {
                    case CliAsyncCommand async:
                        return async.RunAsync(cancellationToken);
                    default:
                        Func<int> syncFallback = cmd.Run;
                        return Task.Run(syncFallback, cancellationToken);
                }
            });
        }

        public int Execute(string[] args) => DoExecute(args, cmd => cmd.Run());

        private TResult DoExecute<TResult>(string[] args,
            Func<TCommand, TResult> invokeCommand)
        {
            var appFactory = this.appFactory
                ?? (() => new CommandLineApplication());
            var app = appFactory();
            return DoExecute(app, args, invokeCommand);
        }
    }
}
