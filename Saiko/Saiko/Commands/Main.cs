using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.Interactivity;
using DSharpPlus.CommandsNext.Attributes;

namespace Saiko.Commands
{
    public class Main
    {
        [Command("info"), Aliases("i", "about", "saiko", "?", "wtf", "uhh"), Description("Info about Saiko")]
        public async Task InfoAsync(CommandContext ctx)
        {
            var b = new DiscordEmbedBuilder();
            b.WithTitle("Saiko on Github")
                .WithDescription("Saiko is an open-source Discord bot written using the DSharpPlus Library.\nRead more about her in her Wiki.")
                .WithImageUrl(ctx.Client.CurrentUser.AvatarUrl)
                .WithColor(Program.SaikoBot.Color)
                .WithFooter("❤❤❤ Thank you for using Saiko! ❤❤❤")
                .WithUrl("https://github.com/NaamloosDT/Saiko");

            await ctx.RespondAsync("", embed: b.Build());
        }

        [Command("ping"), Aliases("p"), Description("What is a bot without a ping command?!")]
        public async Task PingAsync(CommandContext ctx)
        {
            await ctx.RespondAsync($"Pong! {ctx.Client.Ping} ms. (ahh.. this never gets old ^3^)");
        }
    }
}
