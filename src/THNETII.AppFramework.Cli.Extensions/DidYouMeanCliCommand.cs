using McMaster.Extensions.CommandLineUtils;
using Microsoft.Extensions.Logging;
using System;
using System.Linq;
using THNETII.Common;

namespace THNETII.AppFramework.Cli
{
    public class DidYouMeanCliCommand : CliCommand
    {
        public DidYouMeanCliCommand(CommandLineApplication app)
        {
            CommandLineApplication = app;
        }

        public DidYouMeanCliCommand(CommandLineApplication app, ILogger logger)
            : this(app)
        {
            Logger = logger;
        }

        public DidYouMeanCliCommand(CommandLineApplication app,
            ILogger<DidYouMeanCliCommand> logger) : this(app, logger as ILogger)
        { }

        public CommandLineApplication CommandLineApplication { get; }

        public ILogger Logger { get; }

        /// <summary>
        /// Redirects execution to a matching command in the specified <see cref="CommandLineApplication"/> instance.
        /// <para>Command matching is done by comparing the command-line argument against the beginning of declared command names.</para>
        /// </summary>
        /// <param name="app">The command line application being executed. Must not be <c>null</c>.</param>
        /// <param name="unrecognizedCommand">The unrecognized command name that should be matched agains known command names.</param>
        /// <param name="commandIdx">The index in the <see cref="CommandLineApplication.RemainingArguments"/> of the specified command-line application where the unrecognized command is located.</param>
        /// <returns>The return value of executing the redirected command, if a unique match was found; otherwise <c>1</c>.</returns>
        protected int UnrecognizedCommandFallback(CommandLineApplication app,
            string unrecognizedCommand, int commandIdx)
            => UnrecognizedCommandFallback(app, unrecognizedCommand,
                unrecognizedCommand, commandIdx);

        private int UnrecognizedCommandFallback(CommandLineApplication app,
            string originalUnrecognizedCommand, string unrecognizedCommand,
            int commandIdx)
        {
            var matchCommands = app.Commands
                .Where(c => c.Name.StartsWith(unrecognizedCommand, StringComparison.OrdinalIgnoreCase))
                .ToList();
            switch (matchCommands.Count)
            {
                case 0:
                    if (unrecognizedCommand.Length > 1)
                    {
                        return UnrecognizedCommandFallback(app,
                            originalUnrecognizedCommand,
                            unrecognizedCommand.Substring(0, unrecognizedCommand.Length - 1),
                            commandIdx);
                    }
                    break;
                case 1:
                    if (originalUnrecognizedCommand != unrecognizedCommand)
                        break;
                    var matchingCommand = matchCommands[0];
                    Logger?.LogDebug($"Matching shortened command '{{{nameof(unrecognizedCommand)}}}' to command '{{{nameof(matchingCommand)}}}'",
                        unrecognizedCommand, matchingCommand.Name);
                    var commandArgs = new string[app.RemainingArguments.Count - 1];
                    for (int i = 0; i < app.RemainingArguments.Count; i++)
                    {
                        if (i < commandIdx)
                            commandArgs[i] = app.RemainingArguments[i];
                        else if (i > commandIdx)
                            commandArgs[i - 1] = app.RemainingArguments[i];
                    }
                    return matchCommands[0].Execute(commandArgs);
            }

            Logger?.LogError($"Unrecognized Command: {{{nameof(unrecognizedCommand)}}}",
                originalUnrecognizedCommand);
            if (matchCommands.Any())
                app.Out.WriteLine("Did you mean:");
            foreach (var mc in matchCommands)
                app.Out.WriteLine("\t" + mc.Name);
            app.Out.WriteLine();
            app.ShowHint();
            return ProcessExitCode.ExitFailure;
        }

        /// <summary>
        /// Gets the first arguments in the list of remaining arguments in the executing command line application that does not start with an option-indicating hyphen (<c>'-'</c>) character.
        /// </summary>
        /// <param name="app">The command-line application currently being executed.</param>
        /// <returns>The first command-line argument that might be a command, or <c>null</c> if no command-like argument could be found.</returns>
        protected static string FindUnrecognizedCommand(CommandLineApplication app)
            => FindUnrecognizedCommand(app, out var _);

        /// <summary>
        /// Gets the first arguments in the list of remaining arguments in the executing command line application that does not start with an option-indicating hyphen (<c>'-'</c>) character.
        /// </summary>
        /// <param name="app">The command-line application currently being executed.</param>
        /// <param name="argumentIndex">An out variable that receives the index in the list of remaining arguments where the unrecognized command is located. Set to <c>-1</c> if no argument was found.</param>
        /// <returns>The first command-line argument that might be a command, or <c>null</c> if no command-like argument could be found.</returns>
        protected static string FindUnrecognizedCommand(CommandLineApplication app, out int argumentIndex)
        {
            app.ThrowIfNull(nameof(app));
            for (int i = 0; i < app.RemainingArguments.Count; i++)
            {
                if (!app.RemainingArguments[i].StartsWith("-", StringComparison.Ordinal))
                {
                    argumentIndex = i;
                    return app.RemainingArguments[i];
                }
            }

            argumentIndex = -1;
            return null;
        }

        /// <inheritdoc />
        public override int Run()
        {
            var app = CommandLineApplication;
            string unrecognizedCommand = FindUnrecognizedCommand(app, out int i);

            if (unrecognizedCommand == null)
            {
                Logger?.LogError("Missing required command argument.");
                app.ShowHint();
                return ProcessExitCode.ExitFailure;
            }

            return UnrecognizedCommandFallback(app, unrecognizedCommand, i);
        }
    }
}
