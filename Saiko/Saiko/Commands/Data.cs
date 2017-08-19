using System;
using System.Linq;
using System.Threading.Tasks;
using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Interactivity;

namespace Saiko.Commands
{
    [Group("data"), Aliases("d"), Description("Data / Informative Commands")]
    public class Data
    {
        [Command("avatar"), Aliases("a"), Description("Get avatar from a User")]
        public async Task AvatarAsync(CommandContext ctx, [Description("Users to get avatars from")]params DiscordUser[] Users)
        {
            if(!Users.Any())
            {
                await ctx.RespondAsync("You can't get \"no-one's\" avatar!");
                return;
            }
            string avatars = string.Join("\n", Users.Select(x => x.AvatarUrl));
            await ctx.RespondAsync($"**Avatar(s)**:\n{avatars}");
        }

        [Command("uptime"), Aliases("up", "u"), Description("Shows data about uptime")]
        public async Task UptimeAsync(CommandContext ctx)
        {
            TimeSpan BotUp = DateTimeOffset.Now.Subtract(Program.SaikoBot.BotStart);
            TimeSpan SocketUp = DateTimeOffset.Now.Subtract(Program.SaikoBot.SocketStart);

            var b = new DiscordEmbedBuilder();
            b.WithTitle("Saiko's Uptime")
                .AddField("Bot Uptime", String.Format(@"{0} days, {1}", BotUp.ToString(@"dd"), BotUp.ToString(@"hh\:mm\:ss")), true)
                .AddField("Socket Uptime", String.Format(@"{0} days, {1}", SocketUp.ToString(@"dd"), SocketUp.ToString(@"hh\:mm\:ss")), true)
                .AddField("Bot Start", Program.SaikoBot.BotStart.ToString("dd MMM yyyy hh:mm"), true)
                .AddField("Socket Start", Program.SaikoBot.SocketStart.ToString("d MMM yyyy hh:mm"), true)
                .WithColor(Program.SaikoBot.Color)
                .WithThumbnailUrl(ctx.Client.CurrentUser.AvatarUrl);

            await ctx.RespondAsync("", embed: b.Build());
        }

        [Command("guild"), Aliases("g"), Description("Gets data about current guild")]
        public async Task GuildAsync(CommandContext ctx)
        {
            var b = new DiscordEmbedBuilder();
            b.WithTitle($"{ctx.Guild.Name} ({ctx.Guild.Id})")
                .WithDescription($"Guild owned by {ctx.Guild.Owner.Username}#{ctx.Guild.Owner.Discriminator} (ID: {ctx.Guild.Owner.Id})")
                .WithThumbnailUrl(ctx.Guild.IconUrl)
                .AddField("Channel count", $"{ctx.Guild.Channels.Count}", true)
                .AddField("AFK Timeout", $"{ctx.Guild.AfkTimeout}", true)
                .AddField("Region", ctx.Guild.RegionId, true)
                .AddField("Role Count", $"{ctx.Guild.Roles.Count}", true)
                .AddField("Large?", ctx.Guild.IsLarge ? "Yes" : "No", true)
                .AddField("Verification Level", ctx.Guild.VerificationLevel.ToString(), false)
                .AddField("Icon Url", ctx.Guild.IconUrl, false)
                .WithFooter("Creation Date:", ctx.Guild.IconUrl)
                .WithTimestamp(ctx.Guild.CreationDate)
                .WithColor(Program.SaikoBot.Color);

            await ctx.RespondAsync("", embed: b.Build());
        }

        [Command("urban"), Aliases("ur"), Description("Urban Dictionary lookup")]
        public async Task UrbanAsync(CommandContext ctx, [RemainingText, Description("Word to look up")]string Word)
        {
            var dat = await Helpers.UrbanDict.GetDataAsync(Word);
            if (dat.Key == true)
            {
                var ps = dat.Value.List.Select(x => new Page()
                {
                    Content = "",
                    Embed = new DiscordEmbedBuilder().WithDescription(x.Definition.TryRemove(1000)).WithColor(Program.SaikoBot.Color).Build()
                });

                await ctx.Client.GetInteractivityModule().SendPaginatedMessage(ctx.Channel, ctx.User, ps, TimeSpan.FromSeconds(30), TimeoutBehaviour.Ignore);
            }
            else
            {
                await ctx.RespondAsync("💔 No results found!");
            }
        }
    }

    public static class ExtendedString
    {
        public static string TryRemove(this string input, int index)
        {
            if (input.Length > index)
                return input.Remove(index);
            return input;
        }
    }
}
