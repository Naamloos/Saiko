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
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace Saiko.Commands
{
    [Group("text"), Aliases("tx", "txt"), Description("Commands for text manipulation")]
    public class Text
    {
        [Command("big"), Aliases("b"), Description("Converts text to emojis")]
        public async Task BigAsync(CommandContext ctx, [Description("Text input"), RemainingText]string Input)
        {
            Regex r = new Regex("[^A-Z^a-z]");
            var fw = r.Replace(Input, "");
            var fw2 = fw.Select(x => $":regional_indicator_{char.ToLower(x)}:");
            fw = string.Join(" ", fw2);
            await ctx.RespondAsync(fw);
        }

        [Command("fullwidth"), Aliases("f", "fw"), Description("Converts text to fullwidth")]
        public async Task FullWidthAsync(CommandContext ctx, [Description("Text input"), RemainingText]string Input)
        {
            var fw = Input.Select(x => TextConsts.Normal.Contains(x) ? x : ' ');
            await ctx.RespondAsync(string.Join("", fw.Select(x => TextConsts.FullWidth[TextConsts.Normal.IndexOf(x)])));
        }
    }

    public class TextConsts
    {
        public const string Normal = "0123456789abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ!#$%&()*+,-./:;<=>?@[\\]^_`{|}~ ";
        public const string FullWidth = "０１２３４５６７８９ａｂｃｄｅｆｇｈｉｊｋｌｍｎｏｐｑｒｓｔｕｖｗｘｙｚＡＢＣＤＥＦＧＨＩＪＫＬＭＮＯＰＱＲＳＴＵＶＷＸＹＺ！＃＄％＆（）＊＋、ー。／：；〈＝〉？＠［\\］＾＿‘｛｜｝～ ";
    }
}
