using McMaster.Extensions.CommandLineUtils;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text.RegularExpressions;
using System.Threading;
using THNETII.Common;

namespace THNETII.AppFramework.Cli
{
    /// <summary>
    /// A CLI command that uses Assembly Reflection to provide a default implementation for printing
    /// assembly and runtime information for the exceutable that is being executed.
    /// </summary>
    public class AboutCliCommand : CliCommand
    {
        private static readonly Regex pascalCaseBoundaryMatcher = new Regex(@"(?<=[A-Za-z])(?=[A-Z][a-z])|(?<=[a-z0-9])(?=[0-9]?[A-Z])");
        private readonly Assembly executeAssembly;

        public CommandLineApplication CommandLineApplication { get; }

        public ILogger Logger { get; set; }

        /// <summary>
        /// Creates a new About CLI command instance for the specified executing Assembly.
        /// </summary>
        /// <param name="executeAssembly">The executing assembly containing the main entry point of the CLI application.</param>
        public AboutCliCommand(Assembly executeAssembly) : base()
        {
            this.executeAssembly = executeAssembly
                ?? typeof(AboutCliCommand).GetTypeInfo().Assembly;
        }

        /// <summary>
        /// Creates a new About CLI command instance for the specified executing Assembly.
        /// </summary>
        /// <param name="executeAssembly">The executing assembly containing the main entry point of the CLI application.</param>
        public AboutCliCommand(Assembly executeAssembly,
            CommandLineApplication app) : this(executeAssembly)
        {
            CommandLineApplication = app.ThrowIfNull(nameof(app));
        }

        /// <summary>
        /// Creates a new About CLI command instance for the specified executing Assembly.
        /// </summary>
        /// <param name="executeAssembly">The executing assembly containing the main entry point of the CLI application.</param>
        public AboutCliCommand(Assembly executeAssembly, ILogger logger)
            : this(executeAssembly)
        {
            Logger = logger;
        }

        /// <summary>
        /// Creates a new About CLI command instance for the specified executing Assembly.
        /// </summary>
        /// <param name="executeAssembly">The executing assembly containing the main entry point of the CLI application.</param>
        public AboutCliCommand(Assembly executeAssembly,
            ILogger<AboutCliCommand> logger)
            : this(executeAssembly, logger as ILogger) { }

        /// <summary>
        /// Creates a new About CLI command instance for the specified executing Assembly.
        /// </summary>
        /// <param name="executeAssembly">The executing assembly containing the main entry point of the CLI application.</param>
        public AboutCliCommand(Assembly executeAssembly,
            CommandLineApplication app, ILogger logger)
            : this(executeAssembly, app)
        {
            Logger = logger;
        }

        /// <summary>
        /// Creates a new About CLI command instance for the specified executing Assembly.
        /// </summary>
        /// <param name="executeAssembly">The executing assembly containing the main entry point of the CLI application.</param>
        public AboutCliCommand(Assembly executeAssembly,
            CommandLineApplication app, ILogger<AboutCliCommand> logger)
            : this(executeAssembly, app, logger as ILogger) { }

        private static void WriteKeyValuePairFormatted(TextWriter writer,
            IEnumerable<KeyValuePair<string, object>> keyValuePairs)
        {
            var keyValuePairsList = new List<KeyValuePair<string, object>>();
            var longestKeyLength = 0;
            foreach (var kvp in keyValuePairs)
            {
                longestKeyLength = Math.Max(longestKeyLength, kvp.Key.Length + 1);
                keyValuePairsList.Add(kvp);
            }

            foreach (var kvp in keyValuePairsList)
                writer.WriteLine($"{$"{kvp.Key}:".PadRight(longestKeyLength)} {kvp.Value}");
        }

        /// <inheritdoc />
        public override int Run()
        {
            var app = CommandLineApplication;
            app.ShowRootCommandFullNameAndVersion();

            var writer = app?.Out ?? Console.Out;

            KeyValuePair<string, object> MemberInfoToKvp(PropertyInfo propInfo, object instance)
            {
                string key = pascalCaseBoundaryMatcher.Replace(propInfo.Name, " ");
                return new KeyValuePair<string, object>(key, propInfo.GetValue(instance));
            }
            KeyValuePair<string, object> StaticMemberInfoToKvp(PropertyInfo propInfo) => MemberInfoToKvp(propInfo, null);

            writer.WriteLine("Executable Assembly Attributes:");
            var execAssemblyName = executeAssembly.GetName();
            WriteKeyValuePairFormatted(writer, typeof(AssemblyName).GetProperties(BindingFlags.Instance | BindingFlags.Public)
                .Select(propInfo => MemberInfoToKvp(propInfo, executeAssembly.GetName())));
            WriteKeyValuePairFormatted(writer, executeAssembly.GetCustomAttributes().Select(attr =>
            {
                Type type = attr.GetType();
                string key = type.Name;
                if (key.StartsWith("Assembly", StringComparison.OrdinalIgnoreCase))
                    key = key.Substring("Assembly".Length);
                if (key.EndsWith(nameof(Attribute), StringComparison.OrdinalIgnoreCase))
                    key = key.Substring(0, key.Length - nameof(Attribute).Length);
                var propInfo = type.GetProperty(key, BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.Instance);
                if (propInfo == null)
                    return default(KeyValuePair<string, object>);
                key = pascalCaseBoundaryMatcher.Replace(key, " ");
                var value = propInfo.GetValue(attr);
                return new KeyValuePair<string, object>(key, value);
            }).Where(kvp => !string.IsNullOrWhiteSpace(kvp.Key)));
            writer.WriteLine();

            writer.WriteLine("Execution environment:");
            WriteKeyValuePairFormatted(writer, typeof(RuntimeInformation).GetProperties(BindingFlags.Public | BindingFlags.Static)
                .Select(StaticMemberInfoToKvp));
            WriteKeyValuePairFormatted(writer, new Dictionary<string, object>
            {
                [nameof(Environment.MachineName)] = Environment.MachineName,
                [nameof(Environment.ProcessorCount)] = Environment.ProcessorCount,
#if !NETSTANDARD1_6
                [nameof(Environment.SystemDirectory)] = Environment.SystemDirectory,
                [nameof(Environment.SystemPageSize)] = Environment.SystemPageSize,
                [nameof(Environment.CommandLine)] = Environment.CommandLine,
                [nameof(Environment.CurrentDirectory)] = Environment.CurrentDirectory,
                [nameof(Environment.OSVersion)] = Environment.OSVersion,
                ["Is 64-bit OS"] = Environment.Is64BitOperatingSystem,
                ["Is 64-bit Process"] = Environment.Is64BitProcess,
                [nameof(Environment.UserDomainName)] = Environment.UserDomainName,
                [nameof(Environment.UserName)] = Environment.UserName,
                [nameof(Environment.UserInteractive)] = Environment.UserInteractive,
                ["CLR Version"] = Environment.Version
#endif
            }.Select(kvp => new KeyValuePair<string, object>(pascalCaseBoundaryMatcher.Replace(kvp.Key, " "), kvp.Value)));
            writer.WriteLine();

            writer.WriteLine("Current Thread:");
            WriteKeyValuePairFormatted(writer, Thread.CurrentThread.GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance)
                .Where(pi => pi.GetCustomAttribute<ObsoleteAttribute>() == null)
                .Where(pi => pi.PropertyType.GetMethod(nameof(ToString), Type.EmptyTypes).DeclaringType != typeof(object))
                .Select(pi => new KeyValuePair<string, object>(pi.Name, pi.GetValue(Thread.CurrentThread)))
                );
            writer.WriteLine();

            return 0;
        }
    }
}
