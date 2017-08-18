using Microsoft.Extensions.CommandLineUtils;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text.RegularExpressions;

namespace THNETII.Common.Cli
{
    /// <summary>
    /// A CLI command that uses Assembly Reflection to provide a default implementation for printing
    /// assembly and runtime information for the exceutable that is being executed.
    /// </summary>
    public class AboutCliCommand : CliCommand
    {
        private static readonly Regex pascalCaseBoundaryMatcher = new Regex(@"(?<=[A-Za-z])(?=[A-Z][a-z])|(?<=[a-z0-9])(?=[0-9]?[A-Z])");
        private readonly Assembly executeAssembly;

        /// <summary>
        /// Creates a new About CLI command instance for the specified executing Assembly.
        /// </summary>
        /// <param name="executeAssembly">The executing assembly containing the main entry point of the CLI application.</param>
        /// <param name="configuration" />
        /// <param name="logger" />
        protected AboutCliCommand(
            Assembly executeAssembly,
            IConfiguration configuration, 
            ILogger<AboutCliCommand> logger) 
            : base(configuration, logger)
        {
            this.executeAssembly = executeAssembly ?? typeof(AboutCliCommand).GetTypeInfo().Assembly;
        }

        /// <summary>
        /// Creates a new About CLI command instance for the specified executing Assembly.
        /// </summary>
        /// <param name="executeAssembly">The executing assembly containing the main entry point of the CLI application.</param>
        public AboutCliCommand(Assembly executeAssembly) : this(executeAssembly, null, null) { }

        private static void WriteKeyValuePairFormatted(TextWriter writer, IEnumerable<KeyValuePair<string, object>> keyValuePairs)
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
        public override int Run(CommandLineApplication app)
        {
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

            return 0;
        }
    }
}
