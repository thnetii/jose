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
        private delegate void ConfigureApplicationSpecialized((CommandLineApplication app,
            ICollection<ApplicationConfigurationCallback> configurationCallbacks,
            bool asyncExecute, Action<Task<int>> resultCallback, CancellationToken cancelToken) context);

        private Action<CommandLineApplication> configureApplication;
        private int configureApplicationSpecializedCount = 0;
        private ConfigureApplicationSpecialized configureApplicationSpecialized;
        private Action<IConfigurationBuilder> configureConfiguration;
        private Action<IServiceCollection, IConfiguration> configureServices;

        protected CliCommandBuilder() { }

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
            if (configureServices != null)
                return UseServices((services, _) => configureServices(services));
            return this;
        }

        public CliCommandBuilder<TCommand> UseServices(
            Action<IServiceCollection, IConfiguration> configureServices)
        {
            this.configureServices += configureServices;
            return this;
        }

        public CliCommandBuilder<TCommand> AddArgument(string name,
            Action<CliArgumentBuilder> configureArgument)
        {
            configureApplicationSpecialized += ctx =>
            {
                var app = ctx.app;
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
            configureApplicationSpecialized += ctx =>
            {
                var app = ctx.app;
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
            configureApplicationSpecialized += ctx =>
            {
                var (app, _, async, resultCallback, cancelToken) = ctx;
                var cmdBuilder = new SubCliCommandBuilder<TSubCommand>(this);
                configureCommand?.Invoke(cmdBuilder);

                app.Command(name, cmd =>
                {
                    cmdBuilder.PrepareExecute(cmd, async, resultCallback,
                        cancelToken);
                }, app.ThrowOnUnexpectedArgument);
            };
            return this;
        }

        private class SubCliCommandBuilder<TSubCommand> : CliCommandBuilder<TSubCommand>
            where TSubCommand : CliCommand
        {
            public CliCommandBuilder<TCommand> Parent { get; }

            public SubCliCommandBuilder(CliCommandBuilder<TCommand> parent)
                : base()
            {
                Parent = parent.ThrowIfNull(nameof(parent));
                configureConfiguration = parent.configureConfiguration;
                configureServices = parent.configureServices;
            }
        }

        private Task<int> OnExecute(CommandLineApplication app, bool async,
            ApplicationConfigurationCallback applicationConfigurationCallback,
            CancellationToken cancellationToken = default)
        {
            var configBuilder = new ConfigurationBuilder();
            configureConfiguration?.Invoke(configBuilder);
            var commandLineArgumentConfigurationDictionary =
                new Dictionary<string, string>(StringComparer.Ordinal);
            applicationConfigurationCallback?.Invoke(configBuilder,
                commandLineArgumentConfigurationDictionary);
            var serviceCollection = new ServiceCollection();
            IConfiguration config = configBuilder.Build();
            serviceCollection.AddSingleton(config);
            configureServices?.Invoke(serviceCollection, config);

            serviceCollection.AddSingleton<TCommand>();
            var serviceProvider = serviceCollection.Build();
            using (var disposable = serviceProvider as IDisposable)
            {
                var cmd = serviceProvider.GetRequiredService<TCommand>();
                if (async)
                {
                    if (cmd is CliAsyncCommand asyncCmd)
                        return asyncCmd.RunAsync(cancellationToken);
                    else
                        return Task.Run((Func<int>)cmd.Run, cancellationToken);
                }
                else
                    return Task.FromResult(cmd.Run());
            }
        }

        protected void PrepareExecute(CommandLineApplication app, bool async,
            Action<Task<int>> resultCallback, CancellationToken cancellationToken = default)
        {
            var configureApplicationCallbacks = new List<ApplicationConfigurationCallback>(
                configureApplicationSpecializedCount);
            configureApplicationSpecialized?.Invoke(
                (
                    app,
                    configureApplicationCallbacks,
                    async,
                    resultCallback,
                    cancellationToken
                ));
            configureApplication?.Invoke(app);

            var configAppCallback = configureApplicationCallbacks
                .Aggregate(default(ApplicationConfigurationCallback),
                    (accumulated, element) => accumulated + element);
            app.OnExecute(() =>
            {
                var resultTask = OnExecute(app, async, configAppCallback,
                    cancellationToken);
                resultCallback(resultTask);
            });
        }
    }
}
