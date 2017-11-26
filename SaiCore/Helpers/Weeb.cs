using DSharpPlus;
using DSharpPlus.Entities;
using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace SaiCore.Helpers
{
    public static class Weeb
    {
        private static async Task<string> GetMalLinkAStringAsync(string link, string MalCredentials)
        {
            var httpClient = new HttpClient();
            var byteArray = Encoding.ASCII.GetBytes(MalCredentials);
            var header = new AuthenticationHeaderValue("Basic", Convert.ToBase64String(byteArray));
            httpClient.DefaultRequestHeaders.Authorization = header;
            var response = await httpClient.GetAsync(link);
            var content = response.Content;
            return await content.ReadAsStringAsync();
        }

        public async static Task<DiscordEmbed> Anime(string search, string credentials)
        {
            XDocument doc = XDocument.Parse(await GetMalLinkAStringAsync($"https://myanimelist.net/api/anime/search.xml?q={search}", credentials));

            var anime = doc.Descendants("anime").First();

            DiscordEmbedBuilder b = new DiscordEmbedBuilder();
            b.WithTitle(anime.Descendants("title").First().Value + ", " + anime.Descendants("english").First().Value)
                .WithDescription(anime.Descendants("synonyms").First().Value)
                .AddField("Score", anime.Descendants("score").First().Value, true)
                .AddField("Episodes", anime.Descendants("episodes").First().Value, true)
                .AddField("Type", anime.Descendants("type").First().Value, true)
                .AddField("Status", anime.Descendants("status").First().Value, true)
                .AddField("Run time", anime.Descendants("start_date").First().Value + " - " + anime.Descendants("end_date").First().Value)
                .AddField("Description", WebUtility.HtmlDecode(anime.Descendants("synopsis").First().Value).Replace("<br />", "\n").Replace("[i]", "").Replace("[/i]", "").Replace("\n\n", "\n").MaxLength(1000) + "...")
                .WithImageUrl(anime.Descendants("image").First().Value)
                .WithColor(new DiscordColor("#ffcff7"));

            return b.Build();
        }

        public async static Task<DiscordEmbed> Manga(string search, string credentials)
        {
            XDocument doc = XDocument.Parse(await GetMalLinkAStringAsync($"https://myanimelist.net/api/manga/search.xml?q={search}", credentials));

            var anime = doc.Descendants("manga").First();

            DiscordEmbedBuilder b = new DiscordEmbedBuilder();
            b.WithTitle(anime.Descendants("title").First().Value + ", " + anime.Descendants("english").First().Value)
                .WithDescription(anime.Descendants("synonyms").First().Value)
                .AddField("Score", anime.Descendants("score").First().Value, true)
                .AddField("C/V", anime.Descendants("chapters").First().Value + "/" + anime.Descendants("volumes").First().Value, true)
                .AddField("Type", anime.Descendants("type").First().Value, true)
                .AddField("Status", anime.Descendants("status").First().Value, true)
                .AddField("Run time", anime.Descendants("start_date").First().Value + " - " + anime.Descendants("end_date").First().Value)
                .AddField("Description", WebUtility.HtmlDecode(anime.Descendants("synopsis").First().Value).Replace("<br />", "\n").Replace("[i]", "").Replace("[/i]", "").Replace("\n\n", "\n").MaxLength(1000) + "...")
                .WithImageUrl(anime.Descendants("image").First().Value)
                .WithColor(new DiscordColor("#ffcff7"));

            return b.Build();
        }

        public static string MaxLength(this string s, int length)
        {
            if (s.Length > length)
                return s.Remove(length);
            return s;
        }
    }
}
