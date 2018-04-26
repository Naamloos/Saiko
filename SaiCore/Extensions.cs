using DSharpPlus.CommandsNext;
using DSharpPlus.Entities;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace SaiCore
{
    public static class Extensions
    {
        public async static Task<string> GrabImageURL(CommandContext ctx, string image)
        {
            if (Regex.IsMatch(image, "^https?:\\/\\/.+\\.(png|jpg|jpeg|gif)$"))
            {
                return Regex.Match(image, "^https?:\\/\\/.+\\.(png|jpg|jpeg|gif)$").Value;
            }
            else if (image.Where(x => x != '^').Count() == 0 && image.Contains('^'))
            {
                var msgs = await ctx.Channel.GetMessagesAsync(100);
                foreach (DiscordMessage m in msgs)
                {
                    if (m.Attachments.Count > 0)
                    {
                        if (m.Attachments[0].Url.IsImageUrl())
                        {
                            return m.Attachments[0].Url;
                        }
                    }
                    else if (Regex.IsMatch(m.Content, "https?:\\/\\/.+\\.(png|jpg|jpeg|gif)"))
                    {
                        return Regex.Match(m.Content, "https?:\\/\\/.+\\.(png|jpg|jpeg|gif)").Value;
                    }
                }
            }
            else
            {
                if (ctx.Message.Attachments.Count > 0)
                {
                    if (ctx.Message.Attachments[0].Url.IsImageUrl())
                    {
                        return ctx.Message.Attachments[0].Url;
                    }
                }
            }
            return "";
        }

        public static bool IsImageUrl(this string URL)
        {
            var req = (HttpWebRequest)HttpWebRequest.Create(URL);
            req.Method = "HEAD";
            using (var resp = req.GetResponse())
            {
                return resp.ContentType.ToLower(CultureInfo.InvariantCulture)
                           .StartsWith("image/");
            }
        }

        public static bool EndsWith(this string i, params string[] matches)
        {
            bool ends = true;
            foreach (string m in matches)
            {
                if (!i.EndsWith(m))
                    ends = false;
            }
            return ends;
        }

        public static bool StartsWith(this string i, params string[] matches)
        {
            bool starts = true;
            foreach (string m in matches)
            {
                if (!i.StartsWith(m))
                    starts = false;
            }
            return starts;
        }

        public static bool EqualsAny(this string i, params string[] matches)
        {
            foreach (var m in matches)
            {
                if (i == m)
                    return true;
            }
            return false;
        }

        public static bool EqualsAll(this string i, params string[] matches)
        {
            foreach (var m in matches)
            {
                if (i != m)
                    return false;
            }
            return true;
        }

        public static string TryRemove(this string input, int index)
        {
            if (input.Length > index)
                return input.Remove(index);
            return input;
        }
    }
}