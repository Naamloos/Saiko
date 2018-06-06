using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;
using DSharpPlus.Interactivity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SaiCore.Commands
{
	public class Fun : BaseCommandModule
	{
		private Saiko bot { get; set; }
		public Fun(Saiko bot)
		{
			this.bot = bot;
		}

		[Command("playpriority")]
		[Description("[Voice]Play a song right away")]
		[RequireUserPermissions(Permissions.BanMembers)]
		public async Task PlayPriorityAsync(CommandContext ctx, string song)
		{
			var res = await bot._lavalink.ResolveSongAsync(song);
			var hasstream = false;

			res.RemoveAll(x =>
			{
				if (x.Info.IsStream)
				{
					hasstream = true;
					return true;
				}
				return false;
			});

			if (res.Count > 0)
			{
				if (!bot._lavalink.IsPlaying(ctx.Guild.Id))
				{
					bot._lavalink.PlaySong(res[0], ctx.Guild.Id);
					await ctx.RespondAsync($"Started playing: **{res[0].Info.Title}** by _**{res[0].Info.Author}**_{(res.Count > 1 ? $" and added {res.Count - 1} others from this playlist to the queue" : "")}!");
					res.RemoveAt(0);
				}
				else
				{
					await ctx.RespondAsync($"Added **{res[0].Info.Title}**  by _**{res[0].Info.Author}**_ {(res.Count > 1 ? $" and {res.Count - 1} others from this playlist" : "")} to the beginning of the queue!");
				}

				res.AddRange(bot._lavalinkqueue);
				bot._lavalinkqueue = res;

				if (bot._queuechannels.Keys.Contains(ctx.Guild.Id))
					bot._queuechannels[ctx.Guild.Id] = ctx.Channel.Id;
				else
					bot._queuechannels.Add(ctx.Guild.Id, ctx.Channel.Id);
			}
			else
			{
				if (hasstream)
					await ctx.RespondAsync("I don't want to play streams!");
				else
					await ctx.RespondAsync("Couldn't resolve that link!");
			}
		}

		[Command("queue")]
		[Description("[Voice]Shows current queue")]
		public async Task QueueAsync(CommandContext ctx)
		{
			var sngs = bot._lavalinkqueue.Select(x => $"`{x.Info.Title}` [{x.Info.Author}]").ToList();
			var pages = new List<Page>();

			while(sngs.Count() > 0)
			{
				var taken = sngs.Take(10);
				pages.Add(new Page()
				{
					Embed = new DiscordEmbedBuilder().WithDescription(string.Join("\n", taken)).Build()
				});
				sngs.RemoveAll(x => taken.Contains(x));
			}

			await bot._interactivity.SendPaginatedMessage(ctx.Channel, ctx.User, pages);
		}

		[Command("connect")]
		[Description("[Voice]Connect to a voice channel")]
		public async Task ConnectAsync(CommandContext ctx, DiscordChannel chn)
		{
			await ctx.Guild.ConnectWithoutVnext(chn);
			await ctx.RespondAsync("Connected!");
		}

		[Command("connect")]
		[Description("[Voice]Connect to a voice channel")]
		public async Task ConnectAsync(CommandContext ctx)
		{
			if (ctx.Member?.VoiceState?.Channel != null)
			{
				await ctx.Guild.ConnectWithoutVnext(ctx.Member.VoiceState.Channel);
				await ctx.RespondAsync("Connected!");
			}
			else
			{
				await ctx.RespondAsync("You're not connected to voice!");
			}
		}

		[Command("disconnect")]
		[Description("[Voice]Connect to a voice channel")]
		public async Task DisonnectAsync(CommandContext ctx)
		{
			if (ctx.Guild.Members.First(x => x.Id == ctx.Client.CurrentUser.Id)?.VoiceState?.Channel != null)
			{
				await ctx.Guild.DisonnectVoiceWithoutVnext();
				await ctx.RespondAsync("Disconnected!");
			}
			else
			{
				await ctx.RespondAsync("I'm not connected to voice!");
			}
		}

		[Command("play")]
		[Description("[Voice]Play a song")]
		public async Task PlayAsync(CommandContext ctx, string song)
		{
			var res = await bot._lavalink.ResolveSongAsync(song);
			var hasstream = false;

			res.RemoveAll(x => 
			{
				if (x.Info.IsStream) {
					hasstream = true;
					return true;
				}
				return false;
			});

			if (res.Count > 0)
			{
				if (!bot._lavalink.IsPlaying(ctx.Guild.Id))
				{
					bot._lavalink.PlaySong(res[0], ctx.Guild.Id);
					await ctx.RespondAsync($"Started playing: **{res[0].Info.Title}** by _**{res[0].Info.Author}**_{(res.Count > 1 ? $" and added {res.Count - 1} others from this playlist to the queue" : "")}!");
					res.RemoveAt(0);
				}
				else
				{
					await ctx.RespondAsync($"Added **{res[0].Info.Title}**  by _**{res[0].Info.Author}**_ {(res.Count > 1 ? $" and {res.Count - 1} others from this playlist" : "")} to the queue!");
				}

				bot._lavalinkqueue.AddRange(res);

				if (bot._queuechannels.Keys.Contains(ctx.Guild.Id))
					bot._queuechannels[ctx.Guild.Id] = ctx.Channel.Id;
				else
					bot._queuechannels.Add(ctx.Guild.Id, ctx.Channel.Id);
			}
			else
			{
				if(hasstream)
					await ctx.RespondAsync("I don't want to play streams!");
				else
					await ctx.RespondAsync("Couldn't resolve that link!");
			}
		}

		[Command("stop")]
		[Description("[Voice]Stop playing alltogether")]
		[RequireUserPermissions(Permissions.BanMembers)]
		public async Task StopAsync(CommandContext ctx)
		{
			bot._lavalink.StopSong(ctx.Guild.Id);
			bot._lavalinkqueue.Clear();
			await ctx.RespondAsync($"Stopped!");
		}

		[Command("skip")]
		[Description("[Voice]Skips this song")]
		[RequireUserPermissions(Permissions.BanMembers)]
		public async Task SkipAsync(CommandContext ctx)
		{
			if (bot._lavalinkqueue.Count > 0)
			{
				await Task.Delay(1000);
				bot._lavalink.PlaySong(bot._lavalinkqueue[0], ctx.Guild.Id);
				// TODO: make async and notify guild of new song
				await ctx.RespondAsync($"Skipped this song and started **{bot._lavalinkqueue[0].Info.Title}** by _**{bot._lavalinkqueue[0].Info.Author}**_!");
				bot._lavalinkqueue.RemoveAt(0);
			}
			else
			{
				await ctx.RespondAsync($"Skipped this song!");
			}
		}

		[Command("clearqueue")]
		[Description("[Voice]Clears queue but keeps playing")]
		[RequireUserPermissions(Permissions.BanMembers)]
		public async Task ClearQueueAsync(CommandContext ctx)
		{
			bot._lavalinkqueue.Clear();
			await ctx.RespondAsync($"Cleared queue!");
		}

		[Command("shuffle")]
		[Description("[Voice]Shuffles the queue")]
		[RequireUserPermissions(Permissions.BanMembers)]
		public async Task ShuffleAsync(CommandContext ctx)
		{
			if (bot._lavalinkqueue.Count > 1)
				bot._lavalinkqueue.Shuffle();

			await ctx.RespondAsync($"Shuffled whatever is in the queue!");
		}

		[Command("volume")]
		[Description("[Voice]Set song volume")]
		public async Task StopAsync(CommandContext ctx, int volume)
		{
			bot._lavalink.SetVolume(ctx.Guild.Id, volume);
			await ctx.RespondAsync($"Set volume to {volume}!");
		}

		[Command("guess")]
		[Description("[Fun]A fun little game where everyone has to guess the number!")]
		public async Task GuessAsync(CommandContext ctx, int Minimum, int Maximum, TimeSpan Timeout)
		{
			await ctx.RespondAsync($"🎲 Hey! Guess the number between {Minimum} and {Maximum} in " +
				$"{Timeout.ToString()}! Time goes in... NOW!");

			var RND = new Random();
			int Guess = RND.Next(Minimum, Maximum);

			var m = await bot._interactivity.WaitForMessageAsync(x => x.Channel.Id == ctx.Channel.Id && x.Content == Guess.ToString(), Timeout);

			if (m != null)
				await ctx.RespondAsync($"Congratulations! {m.Message.Author.Mention} won the game!! The number was {Guess}!");
			else
				await ctx.RespondAsync($"Aww... Nobody guessed that the number I chose was {Guess}...");
		}

		[Command("fast")]
		[Description("[Fun]the first one to type the word wins!")]
		public async Task FastAsync(CommandContext ctx, int length = 20)
		{
			var word = new char[length];
			for (int i = 0; i < length; i++)
			{
				word[i] = TextConsts.Normal[new Random().Next(0, TextConsts.Normal.Length - 1)];
			}
			var str = string.Join("", word).Replace("\\", "");
			var f = Helpers.RandomBitmap.GenerateWithText(str);
			await ctx.RespondWithFileAsync("img.png", f, "The first to type this wins!");
			var m = await bot._interactivity.WaitForMessageAsync(x => x.ChannelId == ctx.Channel.Id && x.Content == str);
			if (m != null)
				await ctx.RespondAsync($"Congratulations {m.User.Mention}! You won!");
			else
				await ctx.RespondAsync($"Nobody won? How hard is it to type `{str}`?!");
		}

		[Command("tictactoe")]
		[Description("[Fun]play a game of tic tac toe!")]
		public async Task TicTacToeAsync(CommandContext ctx, DiscordMember m)
		{
			if (m.Id == ctx.Member.Id)
			{
				await ctx.RespondAsync("Get a friend!!");
				return;
			}
			#region ask confirmation
			await ctx.RespondAsync($"Hey, {m.Mention}! Want to play Tic Tac Toe? Respond with `ok` if you're in!");
			var confirm = await bot._interactivity.WaitForMessageAsync(x => x.Channel.Id == ctx.Channel.Id && x.Author.Id == m.Id && x.Content.ToLower() == "ok");
			if (confirm != null)
			{

			}
			else
			{
				await ctx.RespondAsync($"{m.Username} didn't want to play.. :c");
				return;
			}
			#endregion
			var t = new Helpers.TicTacToe($"{ctx.Member.Username}#{ctx.Member.Discriminator}", $"{m.Username}#{m.Discriminator}");

			DiscordMember winner = null;
			bool player1turn = true;
			char[] board = new char[9] { '1', '2', '3', '4', '5', '6', '7', '8', '9' };

			var s = t.GetImage();
			var mess = await ctx.RespondWithFileAsync("ttt.png", s, $"{ctx.Member.Mention}, it's your turn! `1-9`.");

			while (winner == null)
			{
				var mm = await bot._interactivity.WaitForMessageAsync(x => board.Contains(x.Content[0])
				&& x.Channel.Id == ctx.Channel.Id
				&& x.Author.Id == (player1turn ? ctx.Member.Id : m.Id)
				&& x.Content[0] != 'x' && x.Content[0] != 'o');
				await mess.DeleteAsync();
				if (mm == null)
				{
					await ctx.RespondAsync("Game timed out!");
					return;
				}

				int index = int.Parse(mm.Message.Content[0].ToString()) - 1;
				board[index] = player1turn ? 'x' : 'o';
				s.Dispose();
				s = t.SetValue(index, player1turn ? Helpers.Players.one : Helpers.Players.two);
				var w = Checktttwinner(player1turn, board);
				mess = await ctx.RespondWithFileAsync("ttt.png", s, $"{(player1turn ? m.Mention : ctx.Member.Mention)}, it's your turn! `1-9`.");
				if (ctx.Channel.PermissionsFor(ctx.Guild.CurrentMember).HasPermission(Permissions.ManageMessages))
					await mm.Message.DeleteAsync();

				if (w == -1)
					break;
				else if (w == 1)
				{
					winner = player1turn ? ctx.Member : m;
					break;
				}
				player1turn = !player1turn;
			}
			if (winner == null)
				await mess.ModifyAsync("aww, it's a draw!");
			else
				await mess.ModifyAsync($"{winner.Mention}, you won!");
		}

		public int Checktttwinner(bool player1turn, char[] arr)
		{
			char c = player1turn ? 'x' : 'o';

			#region Horzontal Winning Condtion
			//Winning Condition For First Row   
			if (arr[0] == c && arr[1] == c && arr[2] == c)
			{
				return 1;
			}
			//Winning Condition For Second Row   
			if (arr[3] == c && arr[4] == c && arr[5] == c)
			{
				return 1;
			}
			//Winning Condition For Third Row   
			if (arr[6] == c && arr[7] == c && arr[8] == c)
			{
				return 1;
			}
			#endregion

			#region vertical Winning Condtion
			//Winning Condition For First Column       
			if (arr[0] == c && arr[3] == c && arr[6] == c)
			{
				return 1;
			}
			//Winning Condition For Second Column  
			if (arr[1] == c && arr[4] == c && arr[7] == c)
			{
				return 1;
			}
			//Winning Condition For Third Column  
			if (arr[2] == c && arr[5] == c && arr[8] == c)
			{
				return 1;
			}
			#endregion

			#region Diagonal Winning Condition
			if (arr[0] == c && arr[4] == c && arr[8] == c)
			{
				return 1;
			}
			if (arr[2] == c && arr[4] == c && arr[6] == c)
			{
				return 1;
			}
			#endregion

			#region Checking For Draw
			else if (arr[0] != '1' && arr[1] != '2' && arr[2] != '3' && arr[3] != '4' && arr[4] != '5' && arr[5] != '6' && arr[6] != '7' && arr[7] != '8' && arr[8] != '9')
			{
				return -1;
			}
			#endregion
			return 0;
		}

	}
}
