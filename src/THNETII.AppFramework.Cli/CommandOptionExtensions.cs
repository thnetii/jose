using McMaster.Extensions.CommandLineUtils;

namespace THNETII.AppFramework.Cli
{
    public static class CommandOptionExtensions
    {
        public static CommandOption WithDescription(this CommandOption option, string description)
        {
            option.ThrowIfNull(nameof(option)).Description = description;
            return option;
        }

        public static CommandOption EnableInheritance(this CommandOption option, bool inherited = true)
        {
            option.ThrowIfNull(nameof(option)).Inherited = true;
            return option;
        }

        public static CommandOption HideFromHelpText(this CommandOption option, bool hideFromHelpText = true)
        {
            option.ThrowIfNull(nameof(option)).ShowInHelpText = !hideFromHelpText;
            return option;
        }
    }
}
