using CommandSystem;
using System;
using System.Collections.Generic;
using System.Text;

namespace NWAPI.CustomItems.Commands
{
    /// <summary>
    /// Main command for customitems.
    /// </summary>
    [CommandHandler(typeof(RemoteAdminCommandHandler))]
    [CommandHandler(typeof(GameConsoleCommandHandler))]
    public sealed class CustomItems : ParentCommand
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CustomItems"/> command class.
        /// </summary>
        public CustomItems() => LoadGeneratedCommands();

        /// <inheritdoc/>
        public override string Command => "customitems";

        /// <inheritdoc/>
        public override string[] Aliases => new string[] { "ci", "cis" };

        /// <inheritdoc/>
        public override string Description => "Main command for customitems";

        /// <inheritdoc/>
        public bool SanitizeResponse => true;

        private readonly Dictionary<string, (string, string[])> CommandInfo = new();

        private string _cachedInfo = string.Empty;
        /// <inheritdoc/>
        public override void LoadGeneratedCommands()
        {
            RegisterCommand(SubCommands.Give.Instance);
            CommandInfo.Add(SubCommands.Give.Instance.Command, (SubCommands.Give.Instance.Description, SubCommands.Give.Instance.Aliases));

            RegisterCommand(SubCommands.Info.Instance);
            CommandInfo.Add(SubCommands.Info.Instance.Command, (SubCommands.Info.Instance.Description, SubCommands.Info.Instance.Aliases));

            RegisterCommand(SubCommands.Spawn.Instance);
            CommandInfo.Add(SubCommands.Spawn.Instance.Command, (SubCommands.Spawn.Instance.Description, SubCommands.Spawn.Instance.Aliases));

            RegisterCommand(SubCommands.List.Instance);
            CommandInfo.Add(SubCommands.List.Instance.Command, (SubCommands.List.Instance.Description, SubCommands.List.Instance.Aliases));
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
                builder.AppendLine($"| Command | Aliases | Description |");

                foreach (var command in CommandInfo)
                {
                    var aliases = command.Value.Item2 != null && command.Value.Item2.Length > 0
                        ? string.Join(", ", command.Value.Item2)
                        : "NA";
                    builder.AppendLine($"- {command.Key,-5} | {aliases,-5} | {command.Value.Item1,-5}");
                }

                _cachedInfo = builder.ToString();
            }

            return _cachedInfo;
        }

    }
}
