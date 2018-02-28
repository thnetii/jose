using McMaster.Extensions.CommandLineUtils;
using System;
using THNETII.Common;

namespace THNETII.AppFramework.Cli
{
    public class CliArgumentBuilder
    {
        private string name;

        public string Name
        {
            get => name;
            set => name = value.ThrowIfNullOrWhiteSpace(nameof(value));
        }

        public string Description { get; set; }
        public bool? MultipleValues { get; set; }
        public Action<CommandArgument> ConfigureArgument { get; set; }

        public CliArgumentBuilder(string name) => WithName(name);

        public CliArgumentBuilder WithName(string name)
        {
            Name = name;
            return this;
        }

        public CliArgumentBuilder WithDescription(string description)
        {
            Description = description;
            return this;
        }

        public CliArgumentBuilder WithMultipleValues(bool multipleValues)
        {
            MultipleValues = multipleValues;
            return this;
        }

        public CliArgumentBuilder HideFromHelpText(bool hideInHelpText = true)
            => UseArgument(arg => arg.ShowInHelpText = !hideInHelpText);

        public CliArgumentBuilder UseArgument(Action<CommandArgument> configureArgument)
        {
            ConfigureArgument += configureArgument;
            return this;
        }
    }
}
