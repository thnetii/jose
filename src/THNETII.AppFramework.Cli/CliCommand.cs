using McMaster.Extensions.CommandLineUtils;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Linq;

namespace THNETII.AppFramework.Cli
{
    /// <summary>
    /// Represents a CLI command that is executing by the application.
    /// <para>The default CLI command implementation provides an automatic fallback command that suggests possible commands to the user.</para>
    /// </summary>
    public class CliCommand
    {
        /// <summary>
        /// Gets the Configuration instance containing application configuration data for this command.
        /// <para>Property value may be <c>null</c>.</para>
        /// </summary>
        public virtual IConfiguration Configuration { get; }

        /// <summary>
        /// Gets a logger instance for this command instance that can be used to write formatted log output to one or more configured logging providers.
        /// <para>Property value may be <c>null</c>.</para>
        /// </summary>
        public virtual ILogger Logger { get; }

        /// <summary>
        /// Creates a new default CLI command instance that does not use any configuration data or logger instances.
        /// </summary>
        public CliCommand() { }

        /// <summary>
        /// Creates a new default CLI command instance using the specified logger.
        /// </summary>
        /// <param name="logger">An optional logger instance for this command. May be <c>null</c>.</param>
        public CliCommand(ILogger<CliCommand> logger) : this()
        {
            Logger = logger;
        }

        /// <summary>
        /// Creates a new default CLI command instance using the specified logger.
        /// </summary>
        /// <param name="logger">An optional logger instance to use for logging. May be <c>null</c>.</param>
        public CliCommand(ILogger logger) : this()
        {
            Logger = logger;
        }

        /// <summary>
        /// Creates a new default CLI command instance using the specified configuration data and logger instance.
        /// </summary>
        /// <param name="configuration">The configuration data that the CLI command should use. May be <c>null</c>.</param>
        /// <param name="logger">An optional logger instance for this command. May be <c>null</c>.</param>
        public CliCommand(IConfiguration configuration, ILogger<CliCommand> logger = null) : this(logger)
        {
            Configuration = configuration;
        }

        /// <summary>
        /// Redirects execution to a matching command in the specified <see cref="CommandLineApplication"/> instance.
        /// <para>Command matching is done by comparing the command-line argument against the beginning of declared command names.</para>
        /// </summary>
        /// <param name="app">The command line application being executed. Must not be <c>null</c>.</param>
        /// <param name="unrecognizedCommand">The unrecognized command name that should be matched agains known command names.</param>
        /// <param name="commandIdx">The index in the <see cref="CommandLineApplication.RemainingArguments"/> of the specified command-line application where the unrecognized command is located.</param>
        /// <returns>The return value of executing the redirected command, if a unique match was found; otherwise <c>1</c>.</returns>
        protected int UnrecognizedCommandFallback(CommandLineApplication app, string unrecognizedCommand, int commandIdx)
            => UnrecognizedCommandFallback(app, unrecognizedCommand, unrecognizedCommand, commandIdx);

        private int UnrecognizedCommandFallback(CommandLineApplication app, string originalUnrecognizedCommand, string unrecognizedCommand, int commandIdx)
        {
            var matchCommands = app.Commands.Where(c => c.Name.StartsWith(unrecognizedCommand, StringComparison.OrdinalIgnoreCase)).ToList();
            switch (matchCommands.Count)
            {
                case 0:
                    if (unrecognizedCommand.Length > 1)
                        return UnrecognizedCommandFallback(app, originalUnrecognizedCommand, unrecognizedCommand.Substring(0, unrecognizedCommand.Length - 1), commandIdx);
                    break;
                case 1:
                    if (originalUnrecognizedCommand != unrecognizedCommand)
                        break;
                    var matchingCommand = matchCommands[0];
                    Logger?.LogDebug($"Matching shortened command '{{{nameof(unrecognizedCommand)}}}' to command '{{{nameof(matchingCommand)}}}'", unrecognizedCommand, matchingCommand.Name);
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

            Logger?.LogError($"Unrecognized Command: {{{nameof(unrecognizedCommand)}}}", originalUnrecognizedCommand);
            if (matchCommands.Any())
                app.Out.WriteLine("Did you mean:");
            foreach (var mc in matchCommands)
                app.Out.WriteLine("\t" + mc.Name);
            app.Out.WriteLine();
            app.ShowHint();
            return 1;
        }

        /// <summary>
        /// Gets the first arguments in the list of remaining arguments in the executing command line application that does not start with an option-indicating hyphen (<c>'-'</c>) character.
        /// </summary>
        /// <param name="app">The command-line application currently being executed.</param>
        /// <returns>The first command-line argument that might be a command, or <c>null</c> if no command-like argument could be found.</returns>
        protected static string FindUnrecognizedCommand(CommandLineApplication app) => FindUnrecognizedCommand(app, out var _);

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

        public virtual int Run(CommandLineApplication app)
        {
            string unrecognizedCommand = FindUnrecognizedCommand(app, out int i);

            if (unrecognizedCommand == null)
            {
                Logger?.LogError("Missing required command argument.");
                app.ShowHint();
                return 1;
            }

            return UnrecognizedCommandFallback(app, unrecognizedCommand, i);
        }
    }
}
