using McMaster.Extensions.CommandLineUtils;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using THNETII.Common.Collections.Generic;
using THNETII.Common;

namespace THNETII.AppFramework.Cli
{
    using CommandLineApplicationComparer = WeakReferenceEqualityComparer<CommandLineApplication>;
    using CommandLineApplicationWR = WeakReference<CommandLineApplication>;
    using ConfigurationBuilderAction = Action<IConfigurationBuilder>;
    using ConfigurationBuilderDictionary = ConcurrentDictionary<
        WeakReference<CommandLineApplication>,
        Action<IConfigurationBuilder>
        >;
    using ConfigureArgumentAction = Action<CommandArgument>;
    using ConfigureCommandAction = Action<CommandLineApplication>;
    using ConfigureOnExecuteDictionary = ConcurrentDictionary<
        WeakReference<CommandLineApplication>,
        Action<Action<CliCommand>>
        >;
    using ConfigureOptionAction = Action<CommandOption>;
    using ConfigureServicesAction = Action<IConfiguration, IServiceCollection>;
    using ConfigureServicesDictionary = ConcurrentDictionary<
        WeakReference<CommandLineApplication>,
        Action<IConfiguration, IServiceCollection>
        >;

    public static class CommandLineApplicationExtensions
    {
        private static readonly CommandLineApplicationComparer comp = CommandLineApplicationComparer.Default;
        private static readonly ConfigurationBuilderDictionary configureConfigurationDict =
            new ConfigurationBuilderDictionary(comp);
        private static readonly ConfigureServicesDictionary configureServicesDict =
            new ConfigureServicesDictionary(comp);
        private static readonly ConfigureOnExecuteDictionary configureOnExecuteDict =
            new ConfigureOnExecuteDictionary(comp);

        private static void ClearDisposedKeys<TRef, TValue>(this ConcurrentDictionary<WeakReference<TRef>, TValue> dictionary)
            where TRef : class
        {
            var removeList = new List<WeakReference<TRef>>(dictionary.Count);
            foreach (var wRef in dictionary.Keys)
            {
                if (wRef is null || !wRef.TryGetTarget(out var _))
                    removeList.Add(wRef);
            }
            foreach (var wRef in removeList)
                dictionary.TryRemove(wRef, out var _);
        }

        private static void ClearDisposed()
        {
            configureServicesDict.ClearDisposedKeys();
        }

        public static CommandLineApplication WithConfigurationBuilder(
            this CommandLineApplication app,
            ConfigurationBuilderAction configureConfiguration)
        {
            var weak = new CommandLineApplicationWR(
                app.ThrowIfNull(nameof(app)));
            configureConfigurationDict.AddOrUpdate(
                weak, configureConfiguration,
                (_, p) => p + configureConfiguration);
            return app;
        }

        public static CommandLineApplication WithServiceCollection(
            this CommandLineApplication app, 
            ConfigureServicesAction configureServices)
        {
            var weak = new CommandLineApplicationWR(
                app.ThrowIfNull(nameof(app)));
            configureServicesDict.AddOrUpdate(
                weak, configureServices,
                (_, p) => p + configureServices);
            return app;
        }

        public static CommandLineApplication WithOption(
            this CommandLineApplication app,
            string template, CommandOptionType optionType,
            ConfigureOptionAction configureOption = null)
        {
            app.ThrowIfNull(nameof(app)).Option(
                template,
                null, // description
                optionType,
                configureOption ?? (_ => { })
                );
            return app;
        }

        public static CommandLineApplication WithArgument(
            this CommandLineApplication app,
            string name,
            ConfigureArgumentAction configureArgument = null)
        {
            app.ThrowIfNull(nameof(app)).Argument(
                name,
                null, // description
                configureArgument ?? (_ => { })
                );
            return app;
        }

        public static CommandLineApplication WithSubCommand<TCommand>(this CommandLineApplication app,
            string name, ConfigureCommandAction configureCommand)
            where TCommand : CliCommand
        {
            var subApp = app.ThrowIfNull(nameof(app)).Command(
                name,
                configureCommand ?? (_ => { }),
                app.ThrowOnUnexpectedArgument
                );
            var weak = new CommandLineApplicationWR(subApp);
            configureOnExecuteDict[weak] = GetOnExecute<TCommand>(weak);
            return app;
        }

        public static CommandLineApplication WithArgumentSeparator(this CommandLineApplication app,
            bool allowArgumentSeparator = true)
        {
            app.ThrowIfNull(nameof(app)).AllowArgumentSeparator = allowArgumentSeparator;
            return app;
        }

        public static CommandLineApplication WithDescription(this CommandLineApplication app,
            string description)
        {
            app.ThrowIfNull(nameof(app)).Description = description;
            return app;
        }

        public static CommandLineApplication WithErrorWriter(this CommandLineApplication app,
            TextWriter errorWriter)
        {
            app.ThrowIfNull(nameof(app)).Error = errorWriter ?? Console.Error;
            return app;
        }

        public static int ExecuteCommand<T>(
            this CommandLineApplication app,
            params string[] args)
            where T : CliCommand
            => DoExecuteCommand<T, int>(app, args,
                cmd => cmd.Run(app));

        public static Task<int> ExecuteCommandAsync<T>(
            this CommandLineApplication app,
            params string[] args)
            where T : CliAsyncCommand
            => ExecuteCommandAsync<T>(app, args, default);

        public static Task<int> ExecuteCommandAsync<T>(
            this CommandLineApplication app,
            string[] args,
            CancellationToken cancellationToken)
            where T : CliAsyncCommand
            => DoExecuteCommand<T, Task<int>>(app, args, cmd =>
            {
                if (cmd is CliAsyncCommand asyncCmd)
                    return asyncCmd.RunAsync(app, cancellationToken);
                return Task.Run(() => cmd.Run(app), cancellationToken);
            });

        private static Action<Action<CliCommand>> GetOnExecute<TCommand>(CommandLineApplicationWR weak)
            where TCommand : CliCommand
        {
            return commandExecute =>
            {
                var configBuilder = new ConfigurationBuilder();
                if (configureConfigurationDict.TryGetValue(weak, out var configureConfigurationBuilder))
                    configureConfigurationBuilder(configBuilder);
                var config = configBuilder.Build();
                var serviceCollection = new ServiceCollection();
                serviceCollection.AddSingleton<IConfiguration>(config);
                if (configureServicesDict.TryGetValue(weak, out var configureServices))
                    configureServices(config, serviceCollection);
                serviceCollection.AddSingleton<TCommand>();
                var serviceProvider = serviceCollection.BuildServiceProvider();
                using (serviceProvider)
                {
                    var cmd = serviceProvider.GetRequiredService<TCommand>();
                    commandExecute(cmd);
                }
            };
        }

        private static TResult DoExecuteCommand<TCommand, TResult>(
            CommandLineApplication app,
            string[] args,
            Func<CliCommand, TResult> commandExecute)
            where TCommand : CliCommand
        {
            if (app is null)
                throw new ArgumentNullException(nameof(app));

            TResult result = default;
            void commandExecuteAction(CliCommand cmd) => result = commandExecute(cmd);

            foreach (var subcmd in app.Commands)
            {
                var subWeak = new CommandLineApplicationWR(subcmd);
                if (configureOnExecuteDict.TryGetValue(subWeak, out var subOnExecute))
                    subcmd.OnExecute(() => subOnExecute(commandExecuteAction));
            }
            var onExecute = configureOnExecuteDict.GetOrAdd(
                new CommandLineApplicationWR(app),
                GetOnExecute<TCommand>);
            app.OnExecute(() => onExecute(commandExecuteAction));
            app.Execute(args ?? Array.Empty<string>());
            return result;
        }
    }
}
