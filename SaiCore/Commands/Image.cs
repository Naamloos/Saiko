using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using System;
using System.Collections.Generic;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace SaiCore.Commands
{
	public class Image : BaseCommandModule
	{
		private Saiko bot { get; set; }
		public Image(Saiko bot)
		{
			this.bot = bot;
		}

		[Command("safebooru")]
		[Description("[Images]Gets a random image from Safebooru.org.\nGuaranteed SFW.")]
		public async Task Safebooru(CommandContext ctx, [RemainingText, Description("Search query")]string Query)
		{
			await ctx.RespondAsync(await Helpers.Pervert.GetSafebooruImageLink(Query));
		}

		// ancient, almost untouched commands i made long ago
		[Command("cat")]
		[Description("[Images]Gets a random cat image")]
		public async Task CatAsync(CommandContext ctx)
		{
			using (WebClient webclient = new WebClient())
			{
				var p = webclient.DownloadString("http://random.cat/meow");
				int pFrom = p.IndexOf("\\/i\\/") + "\\/i\\/".Length;
				int pTo = p.LastIndexOf("\"}");
				string cat = p.Substring(pFrom, pTo - pFrom);
				await ctx.RespondAsync(":cat: Nya! http://random.cat/i/" + cat);
			}
		}

		[Command("dog")]
		[Description("[Images]Gets a random dog image")]
		public async Task DogAsync(CommandContext ctx)
		{
			using (WebClient webclient = new WebClient())
			{
				var p = webclient.DownloadString("http://random.dog/woof");
				await ctx.RespondAsync(":dog: Bork! http://random.dog/" + p);
			}
		}

		[Command("konachan"), Description("[Images]Gets a random image from Konachan.com.\nNSFW.")]
		public async Task Konachan(CommandContext ctx, [RemainingText, Description("Search query")]string Query)
		{
			if (ctx.Channel.IsNSFW)
			{
				await ctx.RespondAsync(await Helpers.Pervert.GetKonachanImageLink(Query));
			}
			else
				await ctx.RespondAsync("This command is only allowed in NSFW channels!");
		}

		[Command("danbooru"), Description("[Images]Gets a random image from Danbooru.donmai.us.\nNSFW.")]
		public async Task Danbooru(CommandContext ctx, [RemainingText, Description("Search query")]string Query)
		{
			if (ctx.Channel.IsNSFW)
			{
				await ctx.RespondAsync(await Helpers.Pervert.GetDanbooruImageLink(Query));
			}
			else
				await ctx.RespondAsync("This command is only allowed in NSFW channels!");
		}

		[Command("gelbooru"), Description("[Images]Gets a random image from gelbooru.com.\nNSFW.")]
		public async Task Gelbooru(CommandContext ctx, [RemainingText, Description("Search query")]string Query)
		{
			if (ctx.Channel.IsNSFW)
			{
				await ctx.RespondAsync(await Helpers.Pervert.GetGelbooruImageLink(Query));
			}
			else
				await ctx.RespondAsync("This command is only allowed in NSFW channels!");
		}

		[Command("r34"), Description("[Images]Gets a random image from r34.xxx.\nNSFW.")]
		public async Task R34(CommandContext ctx, [RemainingText, Description("Search query")]string Query)
		{
			if (ctx.Channel.IsNSFW)
			{
				await ctx.RespondAsync(await Helpers.Pervert.GetR34ImageLink(Query));
			}
			else
				await ctx.RespondAsync("This command is only allowed in NSFW channels!");
		}

		[Command("cureninja"), Description("[Images]Gets a random image from cure.ninja.\nNSFW.")]
		public async Task CureNinja(CommandContext ctx, [RemainingText, Description("Search query")]string Query)
		{
			if (ctx.Channel.IsNSFW)
			{
				await ctx.RespondAsync(await Helpers.Pervert.GetCureninjaImageLink(Query));
			}
			else
				await ctx.RespondAsync("This command is only allowed in NSFW channels!");
		}

		[Command("yandere"), Description("[Images]Gets a random image from Yande.re.\nNSFW.")]
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
