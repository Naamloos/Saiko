using System.Collections.Generic;
using System.Linq;
using System.Text;
using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Converters;
using DSharpPlus.CommandsNext.Entities;
using DSharpPlus.Entities;

namespace SaiCore
{
	/// <summary>
	/// Default CommandsNext help formatter.
	/// </summary>
	public class HelpFormatter : BaseHelpFormatter
	{
		public DiscordEmbedBuilder EmbedBuilder { get; }
		private Command Command { get; set; }

		/// <summary>
		/// Creates a new default help formatter.
		/// </summary>
		/// <param name="cnext">CommandsNext instance this formatter is for.</param>
		public HelpFormatter(CommandsNextExtension cnext)
			: base(cnext)
		{
			this.EmbedBuilder = new DiscordEmbedBuilder()
				.WithTitle("Help")
				.WithColor(((Saiko)cnext.Services.GetService(typeof(Saiko)))._config.Color);
		}

		/// <summary>
		/// Sets the command this help message will be for.
		/// </summary>
		/// <param name="command">Command for which the help message is being produced.</param>
		/// <returns>This help formatter.</returns>
		public override BaseHelpFormatter WithCommand(Command command)
		{
			this.Command = command;

			this.EmbedBuilder
				.WithDescription($"{Formatter.InlineCode(command.Name)}: " +
				 $"{(command.Description.Contains(']') ? command.Description.Substring(command.Description.IndexOf(']') + 1) : command.Description) ?? "No description provided."}");

			if (command is CommandGroup cgroup && cgroup.IsExecutableWithoutSubcommands)
				this.EmbedBuilder.WithDescription($"{this.EmbedBuilder.Description}\n\nThis group can be executed as a standalone command.");

			if (command.Aliases?.Any() == true)
				this.EmbedBuilder.AddField("Aliases", string.Join(", ", command.Aliases.Select(Formatter.InlineCode)), false);

			if (command.Overloads?.Any() == true)
			{
				var sb = new StringBuilder();

				foreach (var ovl in command.Overloads.OrderByDescending(x => x.Priority))
				{
					sb.Append('`').Append(command.QualifiedName);

					foreach (var arg in ovl.Arguments)
						sb.Append(arg.IsOptional || arg.IsCatchAll ? " [" : " <").Append(arg.Name).Append(arg.IsCatchAll ? "..." : "").Append(arg.IsOptional || arg.IsCatchAll ? ']' : '>');

					sb.Append("`\n");

					foreach (var arg in ovl.Arguments)
						sb.Append('`').Append(arg.Name).Append(" (").Append(this.CommandsNext.GetUserFriendlyTypeName(arg.Type)).Append(")`: ")
							.Append(arg.Description ?? "No description provided.").Append('\n');

					sb.Append('\n');
				}

				this.EmbedBuilder.AddField("Arguments", sb.ToString().Trim(), false);
			}

			return this;
		}

		/// <summary>
		/// Sets the subcommands for this command, if applicable. This method will be called with filtered data.
		/// </summary>
		/// <param name="subcommands">Subcommands for this command group.</param>
		/// <returns>This help formatter.</returns>
		public override BaseHelpFormatter WithSubcommands(IEnumerable<Command> subcommands)
		{
			List<(string, string)> subgroups = new List<(string, string)>();
			foreach (var cmd in subcommands)
			{
				if (cmd.Description != null && cmd.Description.StartsWith('[') && cmd.Description.Contains(']'))
				{
					string subgroup = cmd.Description.Substring(1);
					subgroup = subgroup.Remove(subgroup.IndexOf(']'));
					subgroups.Add((subgroup, cmd.Name));
				}
				else
				{
					subgroups.Add(("Uncategorized", cmd.Name));
				}
			}

			foreach (var sg in subgroups.Select(x => x.Item1).Distinct())
			{
				this.EmbedBuilder.AddField(this.Command != null ? $"{sg} (subcommands)" : $"{sg}", string.Join(", ", subgroups.Where(x => x.Item1 == sg).Select(x => Formatter.InlineCode(x.Item2))), false);
			}

			return this;
		}

		/// <summary>
		/// Construct the help message.
		/// </summary>
		/// <returns>Data for the help message.</returns>
		public override CommandHelpMessage Build()
		{
			if (this.Command == null)
				this.EmbedBuilder.WithDescription("Listing all top-level commands and groups. Specify a command to see more information.");

			return new CommandHelpMessage(embed: this.EmbedBuilder.Build());
		}
	}
}