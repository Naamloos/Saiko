using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;
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

		[Command("connect")]
		[Description("[voice]Connect to a voice channel")]
		public async Task VoiceAsync(CommandContext ctx, DiscordChannel chn)
		{
			await ctx.Guild.ConnectWithoutVnext(chn);
			await ctx.RespondAsync("Connected!");
		}

		[Command("play")]
		[Description("[voice]Play a song")]
		public async Task PlayAsync(CommandContext ctx, string song)
		{
			var s = await bot._lavalink.PlaySong(ctx.Guild.Id, song);
			await ctx.RespondAsync($"Playing! **{s.Title}**");
		}

		[Command("play")]
		[Description("[voice]Stop a song")]
		public async Task PlayAsync(CommandContext ctx)
		{
			await bot._lavalink.StopSong(ctx.Guild.Id);
			await ctx.RespondAsync($"Stopped!**");
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
