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
    [Group("admin"), Aliases("a"), Description("Administrative commands! Only for Admins!"), RequirePermissions(Permissions.Administrator)]
    public class Admin
    {
        [Command("shadowban"), Aliases("sb", "s"), Description("Bans someone by ID, doesn't require said user to be in the guild")]
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

        [Command("leave"), Aliases("l", "gtfo"), Description("Makes saiko leave your guild. Do you really want to do this? :(")]
        public async Task Leave(CommandContext ctx)
        {
            var inter = ctx.Client.GetInteractivityModule();
            await ctx.RespondAsync("Are you sure...?");
            var m = await inter.WaitForMessageAsync(x => x.Channel.Id == ctx.Channel.Id && x.Author.Id == ctx.User.Id && (x.Content.ToLower() == "yes" || x.Content.ToLower() == "no"), TimeSpan.FromSeconds(10));
            if (m != null)
            {
                if (m.Content == "yes")
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
    }
}
