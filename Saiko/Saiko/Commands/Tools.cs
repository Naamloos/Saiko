using System.Threading.Tasks;
using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using System.Net;
using System.Text.RegularExpressions;

namespace Saiko.Commands
{
    [Group("tools"), Aliases("t"), Description("Just some useful tools you can make use of")]
    public class Tools
    {
        [Command("regex"), Aliases("r"), Description("Regex tester on input strings")]
        public async Task RegexTest(CommandContext ctx, [Description("Regex to use")]string pattern, [RemainingText, Description("Input string")]string input)
        {
            var ms = Regex.Matches(input, pattern);
            string matches = $"Found matches for regex: `{pattern}`";
            foreach (var m in ms)
            {
                matches += $"\n{m.ToString()}";
            }
            await ctx.RespondAsync(matches);
        }

        [Command("webregex"), Aliases("w", "wr"), Description("Regex tester on webpages")]
        public async Task RegexWebTest(CommandContext ctx, [Description("Regex pattern")]string pattern, [Description("Url to grab HTML from")]string url)
        {
            string input = "";
            using (WebClient wc = new WebClient())
            {
                input = await wc.DownloadStringTaskAsync(url);
                wc.Dispose();
            }
            var ms = Regex.Matches(input, pattern);
            string matches = $"Found matches for: `{pattern}` at `{url}`.";
            foreach (var m in ms)
            {
                matches += $"\n{m.ToString()}";
            }
            await ctx.RespondAsync(matches);
        }

    }
}
