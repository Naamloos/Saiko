using System.Threading.Tasks;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;

namespace Saiko.Commands
{
    [Group("weeb"), Aliases("w"), Description("Anime / Manga commands")]
    public class Weeb
    {
        [Command("anime"), Description("Search for an anime on MyAnimeList")]
        public async Task Anime(CommandContext ctx, [RemainingText, Description("Search query")]string Query)
        {
            await ctx.RespondAsync("", embed: await Helpers.Weeb.Anime(Query));
        }

        [Command("manga"), Description("Search for a manga on MyAnimeList")]
        public async Task Manga(CommandContext ctx, [RemainingText, Description("Search query")]string Query)
        {
            await ctx.RespondAsync("", embed: await Helpers.Weeb.Manga(Query));
        }

        [Command("safebooru"), Description("Gets a random image from Safebooru.org.\nGuaranteed SFW.")]
        public async Task Safebooru(CommandContext ctx, [Description("Search query")]params string[] query)
        {
            await ctx.RespondAsync(await Helpers.Pervert.GetSafebooruImageLink(query));
        }
    }
}
