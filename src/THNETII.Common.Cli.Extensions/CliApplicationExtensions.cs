using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;

namespace THNETII.Common.Cli
{
    public static class CliApplicationExtensions
    {
        public static CliBuilder<TCommand> CreateDefault<TCommand>(this CliBuilder<TCommand> cliBuilder, bool debug = false)
            where TCommand : CliCommand
        {
            if (cliBuilder == null)
                throw new ArgumentNullException(nameof(cliBuilder));

            cliBuilder.Configuration(configBuilder => DefaultConfigurationBuilder(configBuilder, cliBuilder.ExecutingAssembly, debug));
            cliBuilder.ConfigureServices(DefaultConfigureServices);
            cliBuilder.PreRunCommand((app, serviceProvider) => DefaultAlwaysRun(serviceProvider, debug));

            return cliBuilder;
        }

        internal const string LoggingSectionConfigKey = "Logging";
        internal static readonly string LogLevelConfigKey = ConfigurationPath.Combine(LoggingSectionConfigKey, nameof(LogLevel), "Default");

        public static void DefaultConfigurationBuilder(IConfigurationBuilder configBuilder, Assembly executingAssembly, bool debug = false)
        {
            configBuilder.AddInMemoryCollection(new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
            {
                [LogLevelConfigKey] = debug ? nameof(LogLevel.Information) : nameof(LogLevel.Warning),
            });
            if (executingAssembly != null)
            {
                var assemblyFileName = executingAssembly.Location;
                string executableDirectory;
                try { executableDirectory = Path.GetDirectoryName(assemblyFileName); }
                catch (ArgumentException) { executableDirectory = null; }
                catch (PathTooLongException) { executableDirectory = null; }
                if (!string.IsNullOrWhiteSpace(executableDirectory) && !string.Equals(Directory.GetCurrentDirectory(), executableDirectory, StringComparison.OrdinalIgnoreCase))
                {
                    configBuilder.AddJsonFile(Path.Combine(executableDirectory, "appsettings.json"), optional: true);
                    if (debug)
                        configBuilder.AddJsonFile(Path.Combine(executableDirectory, "appsettings.Debug.json"), optional: true);
                }
            }
            configBuilder.AddJsonFile("appsettings.json", optional: true);
            if (debug)
                configBuilder.AddJsonFile("appsettings.Debug.json", optional: true);
            configBuilder.AddEnvironmentVariables();
        }

        public static void DefaultConfigureServices(IServiceCollection services)
        {
            services.AddLogging();
        }

        public static void DefaultAlwaysRun(IServiceProvider serviceProvider, bool debug = false)
        {
            var loggerFactory = serviceProvider.ThrowIfNull(nameof(serviceProvider)).GetService<ILoggerFactory>();
            loggerFactory?
                .AddDebug(debug ? LogLevel.Trace : LogLevel.Information)
                .AddConsole(serviceProvider.GetService<IConfiguration>()?.GetSection(LoggingSectionConfigKey))
                ;
            var cliLogger = loggerFactory?.CreateLogger<CliCommand>();
            Console.CancelKeyPress += (sender, e) => cliLogger.LogDebug("Cancel Key press detected: {cancelKey}", e.SpecialKey);
        }
    }
}
