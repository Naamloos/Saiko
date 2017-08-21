using DSharpPlus.CommandsNext.Converters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Entities;
using DSharpPlus;

namespace Saiko
{
    class SaikoHelpFormatter : IHelpFormatter
    {
        public static DiscordColor HelpColor;
        private string _name = null, _desc = null, _args = null, _aliases = null, _subcs = null;
        private bool _gexec = false;

        public CommandHelpMessage Build()
        {
            var b = new DiscordEmbedBuilder();
            if (this._name == null)
            {
                if (this._subcs != null)
                    b.WithTitle("Displaying all available commands.");
                else
                    b.WithTitle("No commands are available.");
            }
            else
            {
                b.WithTitle(this._name + "\n\n");

                b.WithDescription(this._desc ?? "No description available.");
                if(_args != null)
                    b.AddField("Arguments required", this._args);
                if(_aliases != null)
                    b.AddField("Aliases", this._aliases);
            }

            if(_subcs != null)
                b.AddField("Subcommands", _subcs);
            b.WithColor(HelpColor);
            return new CommandHelpMessage("", b.Build());
        }

        public IHelpFormatter WithAliases(IEnumerable<string> aliases)
        {
            if (aliases.Any())
                this._aliases = string.Join(", ", aliases.Select(x => $"`{x}`"));
            return this;
        }

        public IHelpFormatter WithArguments(IEnumerable<CommandArgument> arguments)
        {
            if (arguments.Any())
                this._args = string.Join("\n", arguments.Select(xa => $"**{xa.Name}:** `{xa.Type.ToUserFriendlyName()}{(xa.DefaultValue != null? $" = \"{xa.DefaultValue.ToString()}\"" : "")}`" +
                $"\n{(xa.Description == null? "" : $"*{xa.Description}*\n")}" +
                $"{(xa.IsOptional ? "- Is optional\n" : "")}{(xa.IsCatchAll ? "- Is remaining text" : "")}"));
            return this;
        }

        public IHelpFormatter WithCommandName(string name)
        {
            _name = name;
            return this;
        }

        public IHelpFormatter WithDescription(string description)
        {
            _desc = description;
            return this;
        }

        public IHelpFormatter WithGroupExecutable()
        {
            _gexec = true;
            return this;
        }

        public IHelpFormatter WithSubcommands(IEnumerable<Command> subcommands)
        {
            if (subcommands.Any())
            {
                this._subcs = string.Join(", ", subcommands.Select(x => $"`{x.Name}`"));
            }
            return this;
        }
    }
}
