using McMaster.Extensions.CommandLineUtils;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using THNETII.Common;
using THNETII.DependencyInjection;

namespace THNETII.AppFramework.Cli
{
    public class CliCommandBuilder<TCommand> where TCommand : CliCommand
    {
        private delegate void ApplicationConfigurationCallback(IConfigurationBuilder configurationBuilder,
            IDictionary<string, string> commandLineArgumentConfigurationDictionary);

        private Func<CommandLineApplication> appFactory;
        private Action<CommandLineApplication> configureApplication;
        private int configureApplicationSpecializedCount = 0;
        private Action<CommandLineApplication, ICollection<ApplicationConfigurationCallback>> configureApplicationSpecialized;
        private Action<IConfigurationBuilder> configureConfiguration;
        private Action<IServiceCollection> configureServices;

        protected CliCommandBuilder() { }

        public CliCommandBuilder<TCommand> UseApplicationInitializer(
            Func<CommandLineApplication> appFactory)
        {
            this.appFactory = appFactory;
            return this;
        }

        public CliCommandBuilder<TCommand> UseApplication(
            Action<CommandLineApplication> configureApplication)
        {
            this.configureApplication += configureApplication;
            return this;
        }

        public CliCommandBuilder<TCommand> UseConfiguration(
            Action<IConfigurationBuilder> configureConfiguration)
        {
            this.configureConfiguration += configureConfiguration;
            return this;
        }

        public CliCommandBuilder<TCommand> UseServices(
            Action<IServiceCollection> configureServices)
        {
            this.configureServices += configureServices;
            return this;
        }

        public CliCommandBuilder<TCommand> AddArgument(string name,
            Action<CliArgumentBuilder> configureArgument)
        {
            configureApplicationSpecialized += (app, _) =>
            {
                var argBuilder = new CliArgumentBuilder(name);
                configureArgument?.Invoke(argBuilder);

                CommandArgument arg;
                if (argBuilder.ConfigureArgument == null)
                {
                    if (argBuilder.MultipleValues.HasValue)
                        arg = app.Argument(argBuilder.Name, argBuilder.Description, argBuilder.MultipleValues.Value);
                    else
                        arg = app.Argument(argBuilder.Name, argBuilder.Description);
                }
                else
                {
                    if (argBuilder.MultipleValues.HasValue)
                        arg = app.Argument(argBuilder.Name, argBuilder.Description, argBuilder.ConfigureArgument, argBuilder.MultipleValues.Value);
                    else
                        arg = app.Argument(argBuilder.Name, argBuilder.Description, argBuilder.ConfigureArgument);
                }

            };
            configureApplicationSpecializedCount++;
            return this;
        }

        public CliCommandBuilder<TCommand> AddOption(string template,
            Action<CliOptionBuilder> configureOption)
        {
            configureApplicationSpecialized += (app, _) =>
            {
                var optBuilder = new CliOptionBuilder(template);
                configureOption?.Invoke(optBuilder);

                CommandOption opt;
                if (optBuilder.ConfigureOption == null)
                {
                    if (optBuilder.Inherited.HasValue)
                        opt = app.Option(optBuilder.Template, optBuilder.Description, optBuilder.OptionType, optBuilder.Inherited.Value);
                    else
                        opt = app.Option(optBuilder.Template, optBuilder.Description, optBuilder.OptionType);
                }
                else
                {
                    if (optBuilder.Inherited.HasValue)
                        opt = app.Option(optBuilder.Template, optBuilder.Description, optBuilder.OptionType, optBuilder.ConfigureOption, optBuilder.Inherited.Value);
                    else
                        opt = app.Option(optBuilder.Template, optBuilder.Description, optBuilder.OptionType, optBuilder.ConfigureOption);
                }
            };
            configureApplicationSpecializedCount++;
            return this;
        }

        public CliCommandBuilder<TCommand> AddSubCommand<TSubCommand>(
            string name, Action<CliCommandBuilder<TSubCommand>> configureCommand)
            where TSubCommand : CliCommand
        {
            return this;
        }

        public class SubCliCommandBuilder<TSubCommand> : CliCommandBuilder<TSubCommand>
            where TSubCommand : CliCommand
        {
            public CliCommandBuilder<TCommand> Parent { get; }

            public SubCliCommandBuilder(CliCommandBuilder<TCommand> parent)
                : base() => Parent = parent.ThrowIfNull(nameof(parent));
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

        private void OnExecute<TResult>(CommandLineApplication app,
            Func<TCommand, TResult> invokeCommand,
            ApplicationConfigurationCallback applicationConfigurationCallback,
            out TResult result)
        {
            var configBuilder = new ConfigurationBuilder();
            configureConfiguration?.Invoke(configBuilder);
            var commandLineArgumentConfigurationDictionary =
                new Dictionary<string, string>(StringComparer.Ordinal);
            applicationConfigurationCallback?.Invoke(configBuilder,
                commandLineArgumentConfigurationDictionary);
            var serviceCollection = new ServiceCollection();
            serviceCollection.AddSingleton<IConfiguration>(configBuilder.Build());
            configureServices?.Invoke(serviceCollection);

            serviceCollection.AddSingleton<TCommand>();
            var serviceProvider = serviceCollection.Build();

            var cmd = serviceProvider.GetRequiredService<TCommand>();
            result = invokeCommand(cmd);
        }

        private TResult DoExecute<TResult>(string[] args,
            Func<TCommand, TResult> invokeCommand)
        {
            var appFactory = this.appFactory
                ?? (() => new CommandLineApplication());
            var app = appFactory();
            var configureApplicationCallbacks = new List<ApplicationConfigurationCallback>(
                configureApplicationSpecializedCount);
            configureApplicationSpecialized?.Invoke(app, configureApplicationCallbacks);
            configureApplication?.Invoke(app);

            var configureApplicationCallback = configureApplicationCallbacks
                .Aggregate(default(ApplicationConfigurationCallback),
                    (accumulated, element) => accumulated + element);

            TResult result = default;
            app.OnExecute(() => OnExecute(app, invokeCommand,
                configureApplicationCallback, out result));
            app.Execute(args.ZeroLengthIfNull());
            return result;
        }
    }
}
