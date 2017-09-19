using System.Threading.Tasks;
using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Interactivity;
using System;
using DSharpPlus.Entities;
using DSharpPlus.VoiceNext;
using System.IO;
using System.Diagnostics;

namespace Saiko.Commands
{
    [Group("voice"), Aliases("v", "vc"), Description("Moosic")]
    public class Voice
    {
        [Command("join")]
        public async Task JoinAsync(CommandContext ctx)
        {
            var vnext = ctx.Client.GetVoiceNextClient();

            var vnc = vnext.GetConnection(ctx.Guild);
            if (vnc != null)
                await ctx.RespondAsync("I'm already connected!!1");

            var chn = ctx.Member?.VoiceState?.Channel;
            if (chn == null)
                await ctx.RespondAsync("Connect? to what voice channel?!");

            vnc = await vnext.ConnectAsync(chn);
            await ctx.RespondAsync("Connected to your voice channel!");
        }

        [Command("leave"), Aliases("gtfo")]
        public async Task LeaveAsync(CommandContext ctx)
        {
            var vnext = ctx.Client.GetVoiceNextClient();

            var vnc = vnext.GetConnection(ctx.Guild);
            if (vnc == null)
                await ctx.RespondAsync("But ehh.. I'm already disconnected?");

            vnc.Disconnect();
            await ctx.RespondAsync("I'm out c:");
        }

        [Command("play")]
        public async Task PlayAsync(CommandContext ctx, [RemainingText] string file)
        {
            var vnext = ctx.Client.GetVoiceNextClient();

            var vnc = vnext.GetConnection(ctx.Guild);
            if (vnc == null)
                throw new InvalidOperationException("Not connected in this guild.");

            if (!File.Exists(file))
                throw new FileNotFoundException("File was not found.");

            await ctx.RespondAsync("👌");
            await vnc.SendSpeakingAsync(true); // send a speaking indicator

            var psi = new ProcessStartInfo
            {
                FileName = "ffmpeg",
                Arguments = $@"-i ""{file}"" -af volume=0.5 -ac 2 -f s16le -ar 48000 pipe:1",
                RedirectStandardOutput = true,
                UseShellExecute = false
            };
            var ffmpeg = Process.Start(psi);
            var ffout = ffmpeg.StandardOutput.BaseStream;

            var buff = new byte[3840];
            var br = 0;
            while ((br = ffout.Read(buff, 0, buff.Length)) > 0)
            {
                if (br < buff.Length) // not a full sample, mute the rest
                    for (var i = br; i < buff.Length; i++)
                        buff[i] = 0;

                await vnc.SendAsync(buff, 20);
            }

            await vnc.SendSpeakingAsync(false); // we're not speaking anymore
        }
    }
}
