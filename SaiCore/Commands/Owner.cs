using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Scripting;
using Microsoft.CodeAnalysis.Scripting;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.IO;
using System.Web;
using System.Threading.Tasks;
using System.Net;

namespace SaiCore.Commands
{
	public class Owner : BaseCommandModule
	{
		private Saiko bot { get; set; }
		public Owner(Saiko bot)
		{
			this.bot = bot;
		}


		[Command("shutdown")]
		[RequireOwner]
		public async Task ShutdownAsync(CommandContext ctx)
		{
			await ctx.RespondAsync("Good night!");
			bot._cts.Cancel();
		}

		[Command("sudo")]
		[Description("[Owner]Execute a command as if you're another user")]
		[RequireOwner]
		public async Task SudoAsync(CommandContext ctx, [Description("User to Sudo")]DiscordUser user, [RemainingText, Description("Command to execute")] string command = "help")
		{
			await ctx.CommandsNext.SudoAsync(user, ctx.Channel, command);
		}

		// stole- borrowed from emzi
		[Command("eval")]
		[Description("[Owner]Evaluates a snippet of C# code, in context.")]
		[RequireOwner]
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
				Color = bot._config.Color
			};
			var msg = await ctx.RespondAsync("", embed: embed.Build()).ConfigureAwait(false);

			var globals = new EvaluationEnvironment(ctx, bot);
			var sopts = ScriptOptions.Default
				.WithImports("System", "System.Collections.Generic", "System.Linq", "System.Net.Http", "System.Net.Http.Headers", "System.Reflection", "System.Text", "System.Threading.Tasks",
					"DSharpPlus", "DSharpPlus.CommandsNext", "DSharpPlus.Interactivity", "SaiCore.Helpers")
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
					Color = bot._config.Color
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
				await msg.ModifyAsync(embed: embed.Build()).ConfigureAwait(false);
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
					Color = bot._config.Color,
				};
				await msg.ModifyAsync(embed: embed.Build()).ConfigureAwait(false);
				return;
			}

			// execution succeeded
			embed = new DiscordEmbedBuilder
			{
				Title = "Evaluation successful",
				Color = bot._config.Color,
			};

			embed.AddField("Result", css.ReturnValue != null ? css.ReturnValue.ToString() : "No value returned", false)
				.AddField("Compilation time", string.Concat(sw1.ElapsedMilliseconds.ToString("#,##0"), "ms"), true)
				.AddField("Execution time", string.Concat(sw2.ElapsedMilliseconds.ToString("#,##0"), "ms"), true);

			if (css.ReturnValue != null)
				embed.AddField("Return type", css.ReturnValue.GetType().ToString(), true);

			await msg.ModifyAsync(embed: embed.Build()).ConfigureAwait(false);
		}

		[Command("setavatar")]
		[Description("[Owner]Sets bot avatar")]
		[RequireOwner]
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
						await ctx.Client.UpdateCurrentUserAsync(avatar: ms);
						await ctx.RespondAsync("Avatar set!");
					}
				}
			}

		}
	}
}
