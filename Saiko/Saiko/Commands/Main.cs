using System.Threading.Tasks;
using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Interactivity;
using System;
using DSharpPlus.Entities;

namespace Saiko.Commands
{
    public class Main
    {
        [Command("info"), Aliases("i", "about", "saiko", "?", "wtf", "uhh"), Description("Info about Saiko")]
        public async Task InfoAsync(CommandContext ctx)
        {
            var b = new DiscordEmbedBuilder();
            b.WithTitle("Saiko on Github")
                .WithDescription("Saiko is an open-source Discord bot written using the [DSharpPlus](https://github.com/NaamloosDT/DSharpPlus) Library.\nRead more about her in her [Wiki](https://github.com/NaamloosDT/Saiko/wiki).")
                .WithImageUrl(ctx.Client.CurrentUser.AvatarUrl)
                .WithColor(ctx.Dependencies.GetDependency<SaikoBot>().Color)
                .WithFooter("❤❤❤ Thank you for using Saiko! ❤❤❤")
                .WithUrl("https://github.com/NaamloosDT/Saiko")
                .AddField("DSharpPlus version", ctx.Client.VersionString);

            await ctx.RespondAsync("", embed: b.Build());
        }

        [Command("ping"), Aliases("p"), Description("What is a bot without a ping command?!")]
        public async Task PingAsync(CommandContext ctx)
        {
            await ctx.RespondAsync($"Pong! {ctx.Client.Ping} ms. (ahh.. this never gets old ^3^)");
        }

        [Command("report"), Description("Report an issue, abuse or bug to Naamloos")]
        public async Task ReportAsync(CommandContext ctx, [RemainingText, Description("Issue you want to report")]string Issue)
        {
            var mm = await ctx.RespondAsync("Are you okay with your user info + ID, guild name, guild ID and guild owner info + ID getting sent for further inspection?" +
                "\n\n*(Please either respond with 'yes' or wait 5 seconds for the prompt to time out)*8");
            var i = ctx.Client.GetInteractivityModule();
            var m = await i.WaitForMessageAsync(x => x.Author.Id == ctx.User.Id && x.Channel.Id == ctx.Channel.Id && x.Content == "yes", TimeSpan.FromSeconds(15));
            await mm.DeleteAsync();
            if (m != null)
            {
                var dm = await ctx.Client.CreateDmAsync(ctx.Client.CurrentApplication.Owner);

                var b = new DiscordEmbedBuilder();
                b.WithTitle("Issue")
                    .WithDescription(Issue)
                    .WithAuthor($"{ctx.User.Username}#{ctx.User.Discriminator}", icon_url: ctx.User.AvatarUrl ?? ctx.User.DefaultAvatarUrl)
                    .AddField("Guild", $"{ctx.Guild.Name} ({ctx.Guild.Id}) owned by {ctx.Guild.Owner.Username}#{ctx.Guild.Owner.Discriminator}");

                await dm.SendMessageAsync("A new issue has been reported!", embed: b.Build());
                await ctx.RespondAsync("Your issue has been reported.");
            }
            else
            {
                await ctx.RespondAsync("Your issue has not been reported.");
            }
        }
    }
}
