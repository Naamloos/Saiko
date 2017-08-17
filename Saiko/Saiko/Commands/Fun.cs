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
    [Group("fun"), Aliases("f"), Description("Commands mainly meant for fun, no \"real\" use.")]
    public class Fun
    {
        [Command("waifu"), Aliases("w"), Description("Rates your waifu using RNGs and seeds for guaranteed same results!")]
        public async Task WaifuAsync(CommandContext ctx, [RemainingText, Description("Name of your Waifu")]string Waifu)
        {
            // Your waifu isn't best girl. It's all randomly generated!!1
            Random RNG = new Random(Waifu.GetHashCode());
            await ctx.RespondAsync($"Hmmm.. I gotta say your waifu is worth a {RNG.Next(0, 10)} out of 10!");
        }

        [Command("guess"), Aliases("g"), Description("A fun little game where everyone has to guess the number!")]
        public async Task GuessAsync(CommandContext ctx, int Minimum, int Maximum, TimeSpan Timeout)
        {
            await ctx.RespondAsync($"🎲 Hey! Guess the number between {Minimum} and {Maximum} in " +
                $"{Timeout.ToString()}! Time goes in... NOW!");

            var RND = new Random();
            int Guess = RND.Next(Minimum, Maximum);

            var m = await ctx.Client.GetInteractivityModule().WaitForMessageAsync(x => x.Channel.Id == ctx.Channel.Id && x.Content == Guess.ToString(), Timeout);

            if (m != null)
                await ctx.RespondAsync($"Congratulations! {m.Author.Mention} won the game!! The number was {Guess}!");
            else
                await ctx.RespondAsync($"Aww... Nobody guessed that the number I chose was {Guess}...");
        }

        [Command("8ball"), Aliases("8", "8b"), Description("Ask the magic 8ball!")]
        public async Task EightBallAsync(CommandContext ctx, [RemainingText] string Question)
        {
            #region Lazy, lazy, lazy, copied from OldSaiko
            string[] answers = new string[]
                {
                            "Signs point to yes",
                            "Yes",
                            "Reply hazy, try again",
                            "Without a doubt",
                            "My sources say no",
                            "As I see it, yes",
                            "You may rely on it",
                            "Concentrate and ask again",
                            "Outlook not so good",
                            "It is decidedly so",
                            "Better not tell you now",
                            "Very doubtful",
                            "Yes - definitely",
                            "It is certain",
                            "Cannot predict now",
                            "Most likely",
                            "Ask again later",
                            "My reply is no",
                            "Outlook good",
                            "Don't count on it",
                };
            #endregion

            Random RND = new Random();

            var b = new DiscordEmbedBuilder();
            b.WithTitle("The magic 8ball has decided!")
                .WithDescription(answers[RND.Next(0, answers.Count() - 1)])
                .WithThumbnailUrl("https://www.magic-emoji.com/emoji/images/402_emoji_iphone_billiards.png") // TODO: replace this with github-hosted image
                .WithColor(Program.SaikoBot.Color);

            await ctx.RespondAsync("", embed: b.Build());
        }
    }
}
