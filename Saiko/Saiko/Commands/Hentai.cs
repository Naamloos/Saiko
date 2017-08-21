using System.Threading.Tasks;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;

namespace Saiko.Commands
{
    [Group("hentai"), Aliases("h"), Description("You want the tiddy? Hop into an NSFW channel first please."), Hidden]
    public class Hentai
    {
        [Command("konachan"), Description("Gets a random image from Konachan.com.\nNSFW.")]
        public async Task Konachan(CommandContext ctx, [RemainingText, Description("Search query")]string Query)
        {
            if (ctx.Channel.IsNSFW)
            {
                await ctx.RespondAsync(await Helpers.Pervert.GetKonachanImageLink(Query));
            }
            else
                await ctx.RespondAsync("This command is only allowed in NSFW channels!");
        }

        [Command("danbooru"), Description("Gets a random image from Danbooru.donmai.us.\nNSFW.")]
        public async Task Danbooru(CommandContext ctx, [RemainingText, Description("Search query")]string Query)
        {
            if (ctx.Channel.IsNSFW)
            {
                await ctx.RespondAsync(await Helpers.Pervert.GetDanbooruImageLink(Query));
            }
            else
                await ctx.RespondAsync("This command is only allowed in NSFW channels!");
        }

        [Command("gelbooru"), Description("Gets a random image from gelbooru.com.\nNSFW.")]
        public async Task Gelbooru(CommandContext ctx, [RemainingText, Description("Search query")]string Query)
        {
            if (ctx.Channel.IsNSFW)
            {
                await ctx.RespondAsync(await Helpers.Pervert.GetGelbooruImageLink(Query));
            }
            else
                await ctx.RespondAsync("This command is only allowed in NSFW channels!");
        }

        [Command("r34"), Description("Gets a random image from r34.xxx.\nNSFW.")]
        public async Task R34(CommandContext ctx, [RemainingText, Description("Search query")]string Query)
        {
            if (ctx.Channel.IsNSFW)
            {
                await ctx.RespondAsync(await Helpers.Pervert.GetR34ImageLink(Query));
            }
            else
                await ctx.RespondAsync("This command is only allowed in NSFW channels!");
        }

        [Command("cureninja"), Description("Gets a random image from cure.ninja.\nNSFW.")]
        public async Task CureNinja(CommandContext ctx, [RemainingText, Description("Search query")]string Query)
        {
            if (ctx.Channel.IsNSFW)
            {
                await ctx.RespondAsync(await Helpers.Pervert.GetCureninjaImageLink(Query));
            }
            else
                await ctx.RespondAsync("This command is only allowed in NSFW channels!");
        }

        [Command("yandere"), Description("Gets a random image from Yande.re.\nNSFW.")]
        public async Task Yandere(CommandContext ctx, [RemainingText, Description("Search query")]string Query)
        {
            if (ctx.Channel.IsNSFW)
            {
                await ctx.RespondAsync(await Helpers.Pervert.GetKonachanImageLink(Query));
            }
            else
                await ctx.RespondAsync("This command is only allowed in NSFW channels!");
        }
    }
}
