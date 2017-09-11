using System.Threading.Tasks;
using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Interactivity;
using System;
using DSharpPlus.Entities;
using CSharpOsu;
using Saiko.Helpers;
using System.Linq;
using System.Collections.Generic;

namespace Saiko.Commands
{
    [Group("tag"), Aliases("t")]
    public class Tag
    {
        public async Task ExecuteGroup(CommandContext ctx, string Name)
        {
            SaikoTag t = await ctx.Dependencies.GetDependency<SaikoBot>().Database.GetTag(Name);
            var User = (DiscordMember)CommandsNextUtilities.ConvertArgument<DiscordMember>(t.Owner.ToString(), ctx);
            var eb = new DiscordEmbedBuilder()
            {
                Title = t.Name,
                Description = t.Contents,
            }.WithAuthor(User.Username + "#" + User.Discriminator, icon_url: User.AvatarUrl)
            .WithImageUrl(string.IsNullOrEmpty(t.Attachment) ? null : t.Attachment)
            .WithFooter($"ID: {t.ID}");
            await ctx.RespondAsync("", embed: eb.Build());
        }

        [Command("delete")]
        public async Task Delete(CommandContext ctx, string Name)
        {
            await ctx.Dependencies.GetDependency<SaikoBot>().Database.DeleteTag(Name, ctx.User.Id);
            await ctx.RespondAsync("I tried " + DiscordEmoji.FromName(ctx.Client, ":ok_hand:").Name);
        }

        [Command("set")]
        public async Task Set(CommandContext ctx, string Name, [RemainingText]string Content)
        {
            string at = "";
            if (ctx.Message.Attachments.Count > 0)
            {
                at = ctx.Message.Attachments.First().Url.IsImageUrl() ? ctx.Message.Attachments.First().Url : "";
            }

            SaikoTag t = new SaikoTag();
            t.Name = Name;
            t.Contents = string.IsNullOrEmpty(Content) ? "" : Content;
            t.Owner = ctx.User.Id;
            t.Attachment = at;

            await ctx.Dependencies.GetDependency<SaikoBot>().Database.SetTag(t);
            await ctx.RespondAsync("I tried " + DiscordEmoji.FromName(ctx.Client, ":ok_hand:").Name);
        }

        [Command("list")]
        public async Task List(CommandContext ctx)
        {
            List<SaikoTag> ts = await ctx.Dependencies.GetDependency<SaikoBot>().Database.GetTags();
            var b = new DiscordEmbedBuilder();
            string tags = string.Join("\n", ts.Select(x => $"{x.ID}:{x.Name} "));
            var ps = ctx.Client.GetInteractivityModule().GeneratePagesInEmbeds(tags);
            await ctx.Client.GetInteractivityModule().SendPaginatedMessage(ctx.Channel, ctx.User, ps, TimeSpan.FromSeconds(10), TimeoutBehaviour.Delete);
        }
    }
}
