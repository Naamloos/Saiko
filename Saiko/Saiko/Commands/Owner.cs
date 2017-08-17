using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.Interactivity;
using DSharpPlus.CommandsNext.Attributes;
using System.Net;
using System.IO;

namespace Saiko.Commands
{
    [Group("owner"), Aliases("o"), Description("Commands only allowe dot be used by bot's owner."), RequireOwner]
    public class Owner
    {
        [Command("setavatar"), Aliases("sa"), Description("Sets bot avatar")]
        public async Task SetAvatar(CommandContext ctx, [Description("New avatar (can be empty)"), RemainingText]string ImageUrl)
        {
            if (string.IsNullOrEmpty(ImageUrl))
                await ctx.RespondAsync($"No image??");
            else
            {
                using (WebClient wc = new WebClient())
                {
                    using (MemoryStream ms = new MemoryStream())
                    {
                        var bs = wc.DownloadData(ImageUrl);
                        ms.Write(bs, 0, bs.Length);
                        ms.Position = 0;
                        await ctx.Client.EditCurrentUserAsync(avatar: ms, avatar_format: ImageFormat.Png);
                        await ctx.RespondAsync("Avatar set!");
                    }
                }
            }

        }

        [Command("leaveguild"), Description("leaves guilds")]
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
    }
}
