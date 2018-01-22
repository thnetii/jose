using McMaster.Extensions.CommandLineUtils;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.Threading;
using System.Threading.Tasks;

namespace THNETII.AppFramework.Cli
{
    /// <summary>
    /// Represents a CLI command that is executed asynchronously by the application.
    /// </summary>
    public abstract class CliAsyncCommand : CliCommand
    {
        /// <inheritdoc />
        public CliAsyncCommand() : base() { }

        /// <inheritdoc />
        public CliAsyncCommand(ILogger<CliAsyncCommand> logger) : base(logger) { }

        /// <inheritdoc />
        public CliAsyncCommand(ILogger logger) : base(logger) { }

        /// <inheritdoc />
        public CliAsyncCommand(IConfiguration configuration, ILogger<CliAsyncCommand> logger = null) : base(configuration, logger) { }

        /// <summary>
        /// Runs the CLI command asynchronously using the specified Command-line application instance.
        /// </summary>
        /// <param name="app">The command-line application instance that executes the command.</param>
        /// <returns>A task instance that upon completion contains the Operating System exit code that indicated whether the command was executed successfully.</returns>
        public virtual Task<int> RunAsync(CommandLineApplication app, CancellationToken cancellationToken = default)
            => Task.FromResult(base.Run(app));

        /// <inheritdoc />
        public override sealed int Run(CommandLineApplication app)
            => RunAsync(app)?.GetAwaiter().GetResult() ?? ProcessExitCode.ExitFailure;
    }
}
