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
        public static CliApplication CreateDefault(this CliApplication application, Type executingType) => CreateDefault(application, executingType.ThrowIfNull(nameof(executingType)).GetTypeInfo().Assembly);

        public static CliApplication CreateDefault(this CliApplication application, Assembly executingAssembly)
        {
            if (application == null)
                throw new ArgumentNullException(nameof(application));

            application.ConfigurationBuilder(configBuilder => DefaultConfigurationBuilder(configBuilder, executingAssembly));
            application.ConfigureServices(DefaultConfigureServices);
            application.AlwaysRun(DefaultAlwaysRun);

            return application;
        }

        internal static readonly string LoggingSectionConfigKey = "Logging";
        internal static readonly string LogLevelConfigKey = ConfigurationPath.Combine(LoggingSectionConfigKey, nameof(LogLevel), "Default");

        public static void DefaultConfigurationBuilder(IConfigurationBuilder configBuilder, Assembly executingAssembly)
        {
            configBuilder.AddInMemoryCollection(new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
            {
#if DEBUG
                [LogLevelConfigKey] = nameof(LogLevel.Information),
#else // !DEBUG
                [LogLevelConfigKey] = nameof(LogLevel.Warning),
#endif //  !DEBUG
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
#if DEBUG
                    configBuilder.AddJsonFile(Path.Combine(executableDirectory, "appsettings.Debug.json"), optional: true);
#endif // DEBUG
                }
            }
            configBuilder.AddJsonFile("appsettings.json", optional: true);
#if DEBUG
            configBuilder.AddJsonFile("appsettings.Debug.json", optional: true);
#endif // DEBUG
            configBuilder.AddEnvironmentVariables();
        }

        public static void DefaultConfigureServices(IServiceCollection services)
        {
            services.AddLogging();
        }

        public static void DefaultAlwaysRun(IServiceProvider serviceProvider)
        {
            var loggerFactory = serviceProvider.ThrowIfNull(nameof(serviceProvider)).GetService<ILoggerFactory>();
            loggerFactory?
#if DEBUG
                .AddDebug(LogLevel.Trace)
#else // !DEBUG
                .AddDebug(LogLevel.Information)
#endif // !DEBUG
                .AddConsole(serviceProvider.GetService<IConfiguration>()?.GetSection(LoggingSectionConfigKey))
                ;
            var cliLogger = loggerFactory?.CreateLogger<CliApplication>();
            Console.CancelKeyPress += (sender, e) => cliLogger.LogDebug("Cancel Key press detected: {cancelKey}", e.SpecialKey);
        }
    }
}
