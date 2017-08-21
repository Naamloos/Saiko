using System;
using System.Linq;
using System.Threading.Tasks;
using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using System.Net;
using System.IO;
using Microsoft.CodeAnalysis.Scripting;
using System.Diagnostics;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Scripting;

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

        // stole- borrowed from emzi
        [Command("eval"), Description("Evaluates a snippet of C# code, in context."), RequireOwner]
        public async Task EvaluateAsync(CommandContext ctx, [RemainingText, Description("Code to evaluate.")] string code)
        {
            var cs1 = code.IndexOf("```") + 3;
            cs1 = code.IndexOf('\n', cs1) + 1;
            var cs2 = code.LastIndexOf("```");

            if (cs1 == -1 || cs2 == -1)
                throw new ArgumentException("You need to wrap the code into a code block.");

            code = code.Substring(cs1, cs2 - cs1);

            var embed = new DiscordEmbedBuilder
            {
                Title = "Evaluating...",
                Color = ctx.Dependencies.GetDependency<SaikoBot>().Color
            };
            var msg = await ctx.RespondAsync("", embed: embed.Build()).ConfigureAwait(false);

            var globals = new EvaluationEnvironment(ctx);
            var sopts = ScriptOptions.Default
                .WithImports("System", "System.Collections.Generic", "System.Linq", "System.Net.Http", "System.Net.Http.Headers", "System.Reflection", "System.Text", "System.Threading.Tasks",
                    "DSharpPlus", "DSharpPlus.CommandsNext", "DSharpPlus.Interactivity", "Saiko.Helpers")
                .WithReferences(AppDomain.CurrentDomain.GetAssemblies().Where(xa => !xa.IsDynamic && !string.IsNullOrWhiteSpace(xa.Location)));

            var sw1 = Stopwatch.StartNew();
            var cs = CSharpScript.Create(code, sopts, typeof(EvaluationEnvironment));
            var csc = cs.Compile();
            sw1.Stop();

            if (csc.Any(xd => xd.Severity == DiagnosticSeverity.Error))
            {
                embed = new DiscordEmbedBuilder
                {
                    Title = "Compilation failed",
                    Description = string.Concat("Compilation failed after ", sw1.ElapsedMilliseconds.ToString("#,##0"), "ms with ", csc.Length.ToString("#,##0"), " errors."),
                    Color = ctx.Dependencies.GetDependency<SaikoBot>().Color
                };
                foreach (var xd in csc.Take(3))
                {
                    var ls = xd.Location.GetLineSpan();
                    embed.AddField(string.Concat("Error at ", ls.StartLinePosition.Line.ToString("#,##0"), ", ", ls.StartLinePosition.Character.ToString("#,##0")), Formatter.InlineCode(xd.GetMessage()), false);
                }
                if (csc.Length > 3)
                {
                    embed.AddField("Some errors ommited", string.Concat((csc.Length - 3).ToString("#,##0"), " more errors not displayed"), false);
                }
                await msg.EditAsync(embed: embed.Build()).ConfigureAwait(false);
                return;
            }

            Exception rex = null;
            ScriptState<object> css = null;
            var sw2 = Stopwatch.StartNew();
            try
            {
                css = await cs.RunAsync(globals).ConfigureAwait(false);
                rex = css.Exception;
            }
            catch (Exception ex)
            {
                rex = ex;
            }
            sw2.Stop();

            if (rex != null)
            {
                embed = new DiscordEmbedBuilder
                {
                    Title = "Execution failed",
                    Description = string.Concat("Execution failed after ", sw2.ElapsedMilliseconds.ToString("#,##0"), "ms with `", rex.GetType(), ": ", rex.Message, "`."),
                    Color = ctx.Dependencies.GetDependency<SaikoBot>().Color,
                };
                await msg.EditAsync(embed: embed.Build()).ConfigureAwait(false);
                return;
            }

            // execution succeeded
            embed = new DiscordEmbedBuilder
            {
                Title = "Evaluation successful",
                Color = ctx.Dependencies.GetDependency<SaikoBot>().Color,
            };

            embed.AddField("Result", css.ReturnValue != null ? css.ReturnValue.ToString() : "No value returned", false)
                .AddField("Compilation time", string.Concat(sw1.ElapsedMilliseconds.ToString("#,##0"), "ms"), true)
                .AddField("Execution time", string.Concat(sw2.ElapsedMilliseconds.ToString("#,##0"), "ms"), true);

            if (css.ReturnValue != null)
                embed.AddField("Return type", css.ReturnValue.GetType().ToString(), true);

            await msg.EditAsync(embed: embed.Build()).ConfigureAwait(false);
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
        public SaikoBot Saiko { get { return Context.Dependencies.GetDependency<SaikoBot>(); } }

        public EvaluationEnvironment(CommandContext ctx)
        {
            this.Context = ctx;
        }
    }
}
