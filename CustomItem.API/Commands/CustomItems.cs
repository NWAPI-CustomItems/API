using CommandSystem;
using System;
using System.Collections.Generic;
using System.Text;

namespace NWAPI.CustomItems.Commands
{
    public class CustomItems : ParentCommand
    {
        public CustomItems() => LoadGeneratedCommands();

        /// <inheritdoc/>
        public override string Command => "customitems";

        /// <inheritdoc/>
        public override string[] Aliases => new string[] { "ci", "cis" };

        /// <inheritdoc/>
        public override string Description => "Main command for customitems";

        private readonly Dictionary<string, string[]> CommandInfo = new();
        private string _cachedInfo = string.Empty;
        /// <inheritdoc/>
        public override void LoadGeneratedCommands()
        {
            RegisterCommand(SubCommands.Give.Instance);
            CommandInfo.Add(SubCommands.Give.Instance.Command, SubCommands.Give.Instance.Aliases);

            RegisterCommand(SubCommands.Info.Instance);
            CommandInfo.Add(SubCommands.Info.Instance.Command, SubCommands.Info.Instance.Aliases);

            RegisterCommand(SubCommands.Spawn.Instance);
            CommandInfo.Add(SubCommands.Spawn.Instance.Command, SubCommands.Spawn.Instance.Aliases);
        }

        /// <inheritdoc/>
        protected override bool ExecuteParent(ArraySegment<string> arguments, ICommandSender sender, out string response)
        {
            response = GetInfo();
            return false;
        }

        private string GetInfo()
        {
            if (string.IsNullOrEmpty(_cachedInfo))
            {
                var builder = new StringBuilder();
                builder.AppendLine("Invalid subcommand. Available subcommands are:");
                builder.AppendLine("| Command | Aliases |");

                foreach (var command in CommandInfo)
                {
                    var aliases = command.Value != null && command.Value.Length > 0
                        ? string.Join(", ", command.Value)
                        : "NA";
                    builder.AppendLine($"- {command.Key} | {aliases}");
                }

                _cachedInfo = builder.ToString();
            }

            return _cachedInfo;
        }

    }
}
