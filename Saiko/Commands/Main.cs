using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DSharpPlus.Commands;
using DSharpPlus.Commands.Processors.SlashCommands;
using DSharpPlus.Commands.Trees.Attributes;
using DSharpPlus.Entities;

namespace Saiko.Commands
{
    public class Main
    {
        [Command("about"), Description("about Saiko")]
        public static ValueTask AboutAsync(SlashCommandContext ctx)
        {
            return ctx.RespondAsync(new DiscordMessageBuilder()
                .WithContent("Skibidi Toilet?"));
        }
    }
}
