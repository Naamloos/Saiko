using System.Threading.Tasks;
using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Interactivity;
using System;
using DSharpPlus.Entities;
using CSharpOsu;
using Saiko.Helpers;

namespace Saiko.Commands
{
    [Group("osu")]
    public class Osu
    {
        [Command("user")]
        public async Task GetUserAsync(CommandContext ctx, string id)
        {
            var saiko = ctx.Dependencies.GetDependency<SaikoBot>();
            var osu = saiko.Osu;
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
    }
}
