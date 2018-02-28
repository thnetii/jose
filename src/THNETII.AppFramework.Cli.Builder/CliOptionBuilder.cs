using McMaster.Extensions.CommandLineUtils;
using System;
using THNETII.Common;

namespace THNETII.AppFramework.Cli
{
    public class CliOptionBuilder
    {
        private string template;

        public CliOptionBuilder(string template) => WithTemplate(template);

        public string Template
        {
            get => template;
            set => template = value.ThrowIfNullOrWhiteSpace(nameof(value));
        }

        public CliOptionBuilder WithTemplate(string template)
        {
            Template = template;
            return this;
        }

        public string Description { get; set; }

        public CliOptionBuilder WithDescription(string description)
        {
            Description = description;
            return this;
        }

        public CommandOptionType OptionType { get; set; }
            = CommandOptionType.SingleValue;

        public CliOptionBuilder WithOptionType(CommandOptionType optionType)
        {
            OptionType = optionType;
            return this;
        }

        public bool? Inherited { get; set; }

        public CliOptionBuilder EnableInheritance(bool inherited = true)
        {
            Inherited = inherited;
            return this;
        }

        public Action<CommandOption> ConfigureOption { get; set; }

        public CliOptionBuilder UseOption(Action<CommandOption> configureOption)
        {
            ConfigureOption += configureOption;
            return this;
        }

        public CliOptionBuilder HideFromHelpText(bool hideFromHelpText = true)
            => UseOption(opt => opt.ShowInHelpText = !hideFromHelpText);
    }
}
