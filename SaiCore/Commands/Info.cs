using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;
using DSharpPlus.Interactivity;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Scripting;
using Microsoft.CodeAnalysis.Scripting;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace SaiCore.Commands
{
    public class Info : BaseCommandModule
    {
        private Saiko bot { get; set; }
        public Info(Saiko bot)
        {
            this.bot = bot;
        }

        [Command("info")]
        [Aliases("about", "saiko", "wtf")]
        [Description("[Info]Info about Saiko")]
        public async Task InfoAsync(CommandContext ctx)
        {
            var b = new DiscordEmbedBuilder();
            b.WithTitle("Saiko on Github")
                .WithDescription("Saiko is an open-source Discord bot written using the [DSharpPlus](https://github.com/NaamloosDT/DSharpPlus) Library.\nRead more about her in her [Wiki](https://github.com/NaamloosDT/Saiko/wiki).")
                .WithImageUrl(ctx.Client.CurrentUser.AvatarUrl)
                .WithColor(this.bot._config.Color)
                .WithFooter("❤❤❤ Thank you for using Saiko! ❤❤❤")
                .WithUrl("https://github.com/NaamloosDT/Saiko")
                .AddField("DSharpPlus version", ctx.Client.VersionString);

            await ctx.RespondAsync("", embed: b.Build());
        }

        [Command("ping")]
        [Description("[Info]What is a bot without a ping command?!")]
        public async Task PingAsync(CommandContext ctx)
        {
            await ctx.RespondAsync($"Pong! {ctx.Client.Ping} ms. (ahh.. this never gets old ^3^)");
        }

        [Command("report")]
        [Description("[Info]Report an issue, abuse or bug to Naamloos")]
        public async Task ReportAsync(CommandContext ctx, [RemainingText, Description("Issue you want to report")]string Issue)
        {
            var mm = await ctx.RespondAsync("Are you okay with your user info + ID, guild name, guild ID and guild owner info + ID getting sent for further inspection?" +
                "\n\n*(Please either respond with 'yes' or wait 5 seconds for the prompt to time out)*8");
            var i = bot._interactivity;
            var m = await i.WaitForMessageAsync(x => x.Author.Id == ctx.User.Id && x.Channel.Id == ctx.Channel.Id && x.Content == "yes", TimeSpan.FromSeconds(15));
            await mm.DeleteAsync();
            if (m != null)
            {
                //var dm = await ctx.Client.CreateDmAsync(ctx.Client.CurrentApplication.Owner);

                var b = new DiscordEmbedBuilder();
                b.WithTitle("Issue")
                    .WithDescription(Issue)
                    .WithAuthor($"{ctx.User.Username}#{ctx.User.Discriminator}", icon_url: ctx.User.AvatarUrl ?? ctx.User.DefaultAvatarUrl)
                    .AddField("Guild", $"{ctx.Guild.Name} ({ctx.Guild.Id}) owned by {ctx.Guild.Owner.Username}#{ctx.Guild.Owner.Discriminator}");

                //await dm.SendMessageAsync("A new issue has been reported!", embed: b.Build());
                await ctx.RespondAsync("Your issue has not been reported. Reporting is temporarily disabled.");
            }
            else
            {
                await ctx.RespondAsync("Your issue has not been reported.");
            }
        }

        [Command("avatar")]
        [Description("[Info]Get avatar from a User")]
        public async Task AvatarAsync(CommandContext ctx, [Description("Users to get avatars from")]params DiscordUser[] Users)
        {
            if (!Users.Any())
            {
                await ctx.RespondAsync("You can't get \"no-one's\" avatar!");
                return;
            }
            string avatars = string.Join("\n", Users.Select(x => x.AvatarUrl));
            await ctx.RespondAsync($"**Avatar(s)**:\n{avatars}");
        }

        [Command("uptime")]
        [Aliases("up")]
        [Description("[Info]Shows data about uptime")]
        public async Task UptimeAsync(CommandContext ctx)
        {
            TimeSpan BotUp = DateTimeOffset.Now.Subtract(bot.BotStart);
            TimeSpan SocketUp = DateTimeOffset.Now.Subtract(bot.SocketStart);

            var b = new DiscordEmbedBuilder();
            b.WithTitle("Saiko's Uptime")
                .AddField("Bot Uptime", String.Format(@"{0} days, {1}", BotUp.ToString(@"dd"), BotUp.ToString(@"hh\:mm\:ss")), true)
                .AddField("Socket Uptime", String.Format(@"{0} days, {1}", SocketUp.ToString(@"dd"), SocketUp.ToString(@"hh\:mm\:ss")), true)
                .AddField("Bot Start", bot.BotStart.ToString("dd MMM yyyy hh:mm"), true)
                .AddField("Socket Start", bot.SocketStart.ToString("d MMM yyyy hh:mm"), true)
                .WithColor(bot._config.Color)
                .WithThumbnailUrl(ctx.Client.CurrentUser.AvatarUrl);

            await ctx.RespondAsync("", embed: b.Build());
        }

        [Command("user")]
        [Description("[Info]Returns information about a specific user")]
        public async Task UserInfoAsync(CommandContext ctx, DiscordMember usr)
        {
            var embed = new DiscordEmbedBuilder()
                .WithColor(DiscordColor.MidnightBlue)
                .WithTitle($"@{usr.Username}#{usr.Discriminator} - ID: {usr.Id}");

            if (usr.IsBot) embed.Title += " __[BOT]__ ";
            if (usr.IsOwner) embed.Title += " __[OWNER]__ ";

            embed.Description =
                $"Registered on     : {usr.CreationTimestamp.DateTime.ToString(CultureInfo.InvariantCulture)}\n" +
                $"Joined Guild on  : {usr.JoinedAt.DateTime.ToString(CultureInfo.InvariantCulture)}";

            var roles = new StringBuilder();
            foreach (var r in usr.Roles) roles.Append($"[{r.Name}] ");
            if (roles.Length == 0) roles.Append("*None*");
            embed.AddField("Roles", roles.ToString());

            var permsobj = usr.PermissionsIn(ctx.Channel);
            var perms = permsobj.ToPermissionString();
            if (((permsobj & Permissions.Administrator) | (permsobj & Permissions.AccessChannels)) == 0)
                perms = "**[!] User can't see this channel!**\n" + perms;
            if (perms == String.Empty) perms = "*None*";
            embed.AddField("Permissions", perms);

            embed.WithFooter($"{ctx.Guild.Name} / #{ctx.Channel.Name} / {DateTime.Now}");

            await ctx.RespondAsync("", false, embed: embed);
        }

        [Command("guild")]
        [Description("[Info]Returns information about this guild")]
        public async Task GuildInfoAsync(CommandContext ctx)
        {
            await ctx.RespondAsync("The following embed might flood this channel. Do you want to proceed?");
            var m = await bot._interactivity.WaitForMessageAsync(x => x.Content.ToLower() == "yes" || x.Content.ToLower() == "no");
            if (m?.Message?.Content == "yes")
            {
                #region yes
                var g = ctx.Guild;
                var embed = new DiscordEmbedBuilder()
                    .WithColor(DiscordColor.PhthaloBlue)
                    .WithTitle($"{g.Name} ID: ({g.Id})")
                    .WithDescription($"Created on: {g.CreationTimestamp.DateTime.ToString(CultureInfo.InvariantCulture)}\n" +
                    $"Member count: {g.MemberCount}" +
                    $"Joined at: {g.JoinedAt.DateTime.ToString(CultureInfo.InvariantCulture)}");
                if (!string.IsNullOrEmpty(g.IconHash))
                    embed.WithThumbnailUrl(g.IconUrl);
                embed.WithAuthor($"Owner: {g.Owner.Username}#{g.Owner.Discriminator}", icon_url: string.IsNullOrEmpty(g.Owner.AvatarHash) ? null : g.Owner.AvatarUrl);
                var cs = new StringBuilder();
                #region channel list string builder
                foreach (var c in g.Channels)
                {
                    switch (c.Type)
                    {
                        case ChannelType.Text:
                            cs.Append($"[`#{c.Name} (💬)`]");
                            break;
                        case ChannelType.Voice:
                            cs.Append($"`[{c.Name} (🔈)]`");
                            break;
                        case ChannelType.Category:
                            cs.Append($"`[{c.Name.ToUpper()} (📁)]`");
                            break;
                        default:
                            cs.Append($"`[{c.Name} (❓)]`");
                            break;
                    }
                }
                #endregion
                embed.AddField("Channels", cs.ToString());

                var rs = new StringBuilder();
                #region role list string builder
                foreach (var r in g.Roles)
                {
                    rs.Append($"[`{r.Name}`] ");
                }
                #endregion
                embed.AddField("Roles", rs.ToString());

                var es = new StringBuilder();
                #region emoji list string builder
                foreach (var e in g.Emojis)
                {
                    es.Append($"[`{e.Name}`] ");
                }
                #endregion
                embed.AddField("Emotes", es.ToString());

                embed.AddField("Voice", $"AFK Channel: {(g.AfkChannel != null ? $"#{g.AfkChannel.Name}" : "None.")}\n" +
                    $"AFK Timeout: {g.AfkTimeout}\n" +
                    $"Region: {g.VoiceRegion.Name}");

                embed.AddField("Misc", $"Large: {(g.IsLarge ? "yes" : "no")}.\n" +
                    $"Default Notifications: {g.DefaultMessageNotifications}.\n" +
                    $"Explicit content filter: {g.ExplicitContentFilter}.\n" +
                    $"MFA Level: {g.MfaLevel}.\n" +
                    $"Verification Level: {g.VerificationLevel}");

                await ctx.RespondAsync("", false, embed: embed);
                #endregion
            }
            else
            {
                #region no or timeout
                await ctx.RespondAsync("Okay, I'm not sending the embed.");
                #endregion
            }
        }

        [Command("role")]
        [Description("[Info]Returns information about a specific role")]
        public async Task RoleInfoAsync(CommandContext ctx, DiscordRole role)
        {
            var embed = new DiscordEmbedBuilder();
            embed.WithTitle($"{role.Name} ID: ({role.Id})")
                .WithDescription($"Created at {role.CreationTimestamp.DateTime.ToString(CultureInfo.InvariantCulture)}")
                .AddField("Permissions", role.Permissions.ToPermissionString())
                .AddField("Data", $"Mentionable: {(role.IsMentionable ? "yes" : "no")}.\nHoisted: {(role.IsHoisted ? "yes" : "no")}.\nManaged: {(role.IsManaged ? "yes" : "no")}.")
                .WithColor(role.Color);

            await ctx.RespondAsync(embed: embed);
        }

        [Command("channel")]
        [Description("[Info]Returns information about a specific channel")]
        public async Task ChannelInfoAsync(CommandContext ctx, DiscordChannel channel)
        {
            var embed = new DiscordEmbedBuilder();
            embed.WithTitle($"#{channel.Name} ID: ({channel.Id})")
                .WithDescription($"Topic: {channel.Topic}\nCreated at: {channel.CreationTimestamp.DateTime.ToString(CultureInfo.InvariantCulture)}" +
                $"{(channel.ParentId != null ? $"\nChild of `{channel.Parent.Name.ToUpper()}` ID: ({channel.Parent.Id})" : "")}");
            if (channel.IsCategory)
            {
                var cs = new StringBuilder();
                #region channel list string builder
                foreach (var c in channel.Children)
                {
                    switch (c.Type)
                    {
                        case ChannelType.Text:
                            cs.Append($"[`#{c.Name} (💬)`]");
                            break;
                        case ChannelType.Voice:
                            cs.Append($"`[{c.Name} (🔈)]`");
                            break;
                        case ChannelType.Category:
                            cs.Append($"`[{c.Name.ToUpper()} (📁)]`");
                            break;
                        default:
                            cs.Append($"`[{c.Name} (❓)]`");
                            break;
                    }
                }
                #endregion
                embed.AddField("Children of category", cs.ToString());
            }
            if (channel.Type == ChannelType.Voice)
            {
                embed.AddField("Voice", $"Bit rate: {channel.Bitrate}\nUser limit: {(channel.UserLimit == 0 ? "Unlimited" : $"{channel.UserLimit}")}");
            }
            embed.AddField("Misc", $"NSFW: {(channel.IsNSFW ? "yes" : "no")}\n" +
                $"{(channel.Type == ChannelType.Text ? $"Last message ID: {channel.LastMessageId}" : "")}");

            await ctx.RespondAsync(embed: embed);
        }

        [Command("urban")]
        [Description("Urban Dictionary lookup")]
        public async Task UrbanAsync(CommandContext ctx, [RemainingText, Description("Word to look up")]string Word)
        {
            var dat = await Helpers.UrbanDict.GetDataAsync(Word);
            if (dat.Key == true)
            {
                var ps = dat.Value.List.Select(x => new Page()
                {
                    Content = "",
                    Embed = new DiscordEmbedBuilder().WithDescription(x.Definition.TryRemove(1000)).WithColor(bot._config.Color).Build()
                });

                await bot._interactivity.SendPaginatedMessage(ctx.Channel, ctx.User, ps, TimeSpan.FromSeconds(30), TimeoutBehaviour.Ignore);
            }
            else
            {
                await ctx.RespondAsync("💔 No results found!");
            }
        }

        [Command("shadowban")]
        [Description("Bans someone by ID, doesn't require said user to be in the guild")]
        [RequirePermissions(Permissions.BanMembers)]
        public async Task Shadowban(CommandContext ctx, ulong UserID, [RemainingText]string Reason)
        {
            if ((await ctx.Guild.GetMemberAsync(ctx.Client.CurrentUser.Id)).Roles.Where(x => x.CheckPermission(Permissions.BanMembers) == PermissionLevel.Allowed).Count() == 0)
            {
                await ctx.RespondAsync("I am not allowed to do that here!");
                return;
            }
            await ctx.Guild.BanMemberAsync(UserID, reason: Reason);
            await ctx.RespondAsync("Banned member. Check audit logs.");
        }

        [Command("leave")]
        [Aliases("gtfo")]
        [Description("Makes saiko leave your guild. Do you really want to do this? :(")]
        [RequirePermissions(Permissions.BanMembers)]
        public async Task Leave(CommandContext ctx)
        {
            var inter = bot._interactivity;
            await ctx.RespondAsync("Are you sure...?");
            var m = await inter.WaitForMessageAsync(x => x.Channel.Id == ctx.Channel.Id && x.Author.Id == ctx.User.Id && (x.Content.ToLower() == "yes" || x.Content.ToLower() == "no"), TimeSpan.FromSeconds(10));
            if (m != null)
            {
                if (m.Message.Content == "yes")
                {
                    await ctx.RespondAsync("Oh... I understand.. Goodbye :c");
                    await ctx.Guild.LeaveAsync();
                }
                else
                {
                    await ctx.RespondAsync("Fuck, jesus christ you scared the shit outta me boi");
                }
            }
            else
            {
                await ctx.RespondAsync("No response? Guess I'll stay then.");
            }
        }

        [Command("osu")]
        [Description("Gets data about a specific Osu member.")]
        public async Task GetUserAsync(CommandContext ctx, string id)
        {
            var osu = bot._osu;
            var ouser = osu.GetUser(id)[0];

            var b = new DiscordEmbedBuilder();
            b.WithTitle($"{ouser.username} ({ouser.user_id})")
            .WithDescription($"Level: {ouser.level}\nAccuracy: {ouser.accuracy}\nPlaycount: {ouser.playcount}")
            .WithUrl(ouser.url)
            .AddField("Rank", $"{ouser.pp_rank}", true)
            .AddField("Country Rank", $"{ouser.pp_country_rank}", true)
            .AddField("Total 300:", $"{ouser.count300}", true)
            .AddField("Total 100:", $"{ouser.count100}", true)
            .AddField("Total 50:", $"{ouser.count50}", true)
            .AddField("Total SS:", $"{ouser.count_rank_ss}", true)
            .AddField("Total S:", $"{ouser.count_rank_s}", true)
            .AddField("Total A:", $"{ouser.count_rank_a}", true)
            .AddField("Total score:", $"{ouser.total_score}", true)
            .AddField("Ranked score:", $"{ouser.ranked_score}", true)
            .WithThumbnailUrl($"{ouser.flag}")
            .WithImageUrl($"{ouser.image}");


            await ctx.RespondAsync(embed: b.Build());
        }

        [Command("leaveguild")]
        [Description("leaves guilds")]
        [RequireOwner]
        public async Task Leave(CommandContext ctx, [Description("Guild IDs to leave")]params ulong[] ids)
        {
            // old code and i'm lazy
            // but meh, it works
            string left = $"Left guilds:\n";
            foreach (ulong id in ids)
            {
                try
                {
                    var g = ctx.Client.Guilds[id];
                    await g.LeaveAsync();
                    left += $"{g.Name} owned by {g.Owner.Username}#{g.Owner.Discriminator}\n";
                }
                catch (Exception)
                {

                }
            }
            await ctx.RespondAsync(left);
        }

        [Command("bitmap")]
        [Description("Gets a random bitmap")]
        public async Task BitmapAsync(CommandContext ctx)
        {
            var s = new MemoryStream();
            Helpers.RandomBitmap.randombitmap().Save(s, System.Drawing.Imaging.ImageFormat.Png);
            s.Position = 0;
            await ctx.RespondWithFileAsync("random.png", s, "Random bitmap, comin up!");
        }

        [Command("big")]
        [Description("Converts text to emojis")]
        public async Task BigAsync(CommandContext ctx, [Description("Text input"), RemainingText]string Input)
        {
            Regex r = new Regex("[^A-Z^a-z]");
            var fw = r.Replace(Input, "");
            var fw2 = fw.Select(x => $":regional_indicator_{char.ToLower(x)}:");
            fw = string.Join(" ", fw2);
            await ctx.RespondAsync(fw);
        }

        [Command("fullwidth")]
        [Description("Converts text to fullwidth")]
        public async Task FullWidthAsync(CommandContext ctx, [Description("Text input"), RemainingText]string Input)
        {
            var fw = Input.Select(x => TextConsts.Normal.Contains(x) ? x : ' ');
            await ctx.RespondAsync(string.Join("", fw.Select(x => TextConsts.FullWidth[TextConsts.Normal.IndexOf(x)])));
        }

        [Command("regex")]
        [Description("Regex tester on input strings")]
        public async Task RegexTest(CommandContext ctx, [Description("Regex to use")]string pattern, [RemainingText, Description("Input string")]string input)
        {
            var ms = Regex.Matches(input, pattern);
            string matches = $"Found matches for regex: `{pattern}`";
            foreach (var m in ms)
            {
                matches += $"\n{m.ToString()}";
            }
            await ctx.RespondAsync(matches);
        }

        [Command("webregex")]
        [Description("Regex tester on webpages")]
        public async Task RegexWebTest(CommandContext ctx, [Description("Regex pattern")]string pattern, [Description("Url to grab HTML from")]string url)
        {
            string input = "";
            using (WebClient wc = new WebClient())
            {
                input = await wc.DownloadStringTaskAsync(url);
                wc.Dispose();
            }
            var ms = Regex.Matches(input, pattern);
            string matches = $"Found matches for: `{pattern}` at `{url}`.";
            foreach (var m in ms)
            {
                matches += $"\n{m.ToString()}";
            }
            await ctx.RespondAsync(matches);
        }

        [Command("anime")]
        [Description("Search for an anime on MyAnimeList")]
        public async Task Anime(CommandContext ctx, [RemainingText, Description("Search query")]string Query)
        {
            await ctx.RespondAsync("", embed: await Helpers.Weeb.Anime(Query, bot._config.MyAnimeList));
        }

        [Command("manga")]
        [Description("Search for a manga on MyAnimeList")]
        public async Task Manga(CommandContext ctx, [RemainingText, Description("Search query")]string Query)
        {
            await ctx.RespondAsync("", embed: await Helpers.Weeb.Manga(Query, bot._config.MyAnimeList));
        }
    }

    public sealed class EvaluationEnvironment
    {
        public CommandContext Context { get; }

        public DiscordMessage Message { get { return this.Context.Message; } }
        public DiscordChannel Channel { get { return this.Context.Channel; } }
        public DiscordGuild Guild { get { return this.Context.Guild; } }
        public DiscordUser User { get { return this.Context.User; } }
        public DiscordMember Member { get { return this.Context.Member; } }
        public DiscordClient Client { get { return this.Context.Client; } }
        public Saiko Saiko { get; }

        public EvaluationEnvironment(CommandContext ctx, Saiko bot)
        {
            this.Context = ctx;
            this.Saiko = bot;
        }
    }

    public class TextConsts
    {
        public const string Normal = "0123456789abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ!#$%&()*+,-./:;<=>?@[\\]^_`{|}~ ";
        public const string FullWidth = "０１２３４５６７８９ａｂｃｄｅｆｇｈｉｊｋｌｍｎｏｐｑｒｓｔｕｖｗｘｙｚＡＢＣＤＥＦＧＨＩＪＫＬＭＮＯＰＱＲＳＴＵＶＷＸＹＺ！＃＄％＆（）＊＋、ー。／：；〈＝〉？＠［\\］＾＿‘｛｜｝～ ";
    }
}