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
    [Group("hentai", CanInvokeWithoutSubcommand = true), Aliases("h"), Description("You want the tiddy? Hop into an NSFW channel first please."), Hidden]
    public class Hentai
    {
        [Command("konachan"), Description("Gets a random image from Konachan.com.\nNSFW.")]
        public async Task Konachan(CommandContext ctx, [Description("Search query")]params string[] query)
        {
            if (ctx.Channel.IsNSFW)
            {
                string args = string.Join(" ", query);
                await ctx.RespondAsync(await Helpers.Pervert.GetKonachanImageLink(args));
            }
            else
                await ctx.RespondAsync("This command is only allowed in NSFW channels!");
        }

        [Command("danbooru"), Description("Gets a random image from Danbooru.donmai.us.\nNSFW.")]
        public async Task Danbooru(CommandContext ctx, [Description("Search query")]params string[] query)
        {
            if (ctx.Channel.IsNSFW)
            {
                await ctx.RespondAsync(await Helpers.Pervert.GetDanbooruImageLink(query));
            }
            else
                await ctx.RespondAsync("This command is only allowed in NSFW channels!");
        }

        [Command("gelbooru"), Description("Gets a random image from gelbooru.com.\nNSFW.")]
        public async Task Gelbooru(CommandContext ctx, [Description("Search query")]params string[] query)
        {
            if (ctx.Channel.IsNSFW)
            {
                await ctx.RespondAsync(await Helpers.Pervert.GetGelbooruImageLink(query));
            }
            else
                await ctx.RespondAsync("This command is only allowed in NSFW channels!");
        }

        [Command("r34"), Description("Gets a random image from r34.xxx.\nNSFW.")]
        public async Task R34(CommandContext ctx, [Description("Search query")]params string[] query)
        {
            if (ctx.Channel.IsNSFW)
            {
                await ctx.RespondAsync(await Helpers.Pervert.GetR34ImageLink(query));
            }
            else
                await ctx.RespondAsync("This command is only allowed in NSFW channels!");
        }

        [Command("cureninja"), Description("Gets a random image from cure.ninja.\nNSFW.")]
        public async Task CureNinja(CommandContext ctx, [Description("Search query")]params string[] query)
        {
            if (ctx.Channel.IsNSFW)
            {
                await ctx.RespondAsync(await Helpers.Pervert.GetCureninjaImageLink(query));
            }
            else
                await ctx.RespondAsync("This command is only allowed in NSFW channels!");
        }

        [Command("yandere"), Description("Gets a random image from Yande.re.\nNSFW.")]
        public async Task Yandere(CommandContext ctx, [Description("Search query")]params string[] query)
        {
            if (ctx.Channel.IsNSFW)
            {
                await ctx.RespondAsync(await Helpers.Pervert.GetKonachanImageLink(query));
            }
            else
                await ctx.RespondAsync("This command is only allowed in NSFW channels!");
        }
    }
}
