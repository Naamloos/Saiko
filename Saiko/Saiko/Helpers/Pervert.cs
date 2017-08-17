using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

// Partly based off of very old nadeko code but improved over the year(s)
// Don't hate if u masturbate lmao
namespace Saiko.Helpers
{
    public enum RequestHttpMethod
    {
        Get,
        Post
    }

    public class Pervert
    {
        public static Stream GetResponseStreamAsync(string url,
            IEnumerable<KeyValuePair<string, string>> headers = null, RequestHttpMethod method = RequestHttpMethod.Get)
        {
            if (string.IsNullOrWhiteSpace(url))
                throw new ArgumentNullException(nameof(url));
            var httpClient = new HttpClient();
            switch (method)
            {
                case RequestHttpMethod.Get:
                    if (headers != null)
                    {
                        foreach (var header in headers)
                        {
                            httpClient.DefaultRequestHeaders.TryAddWithoutValidation(header.Key, header.Value);
                        }
                    }
                    return httpClient.GetStreamAsync(url).Result;
                case RequestHttpMethod.Post:
                    FormUrlEncodedContent formContent = null;
                    if (headers != null)
                    {
                        formContent = new FormUrlEncodedContent(headers);
                    }
                    var message = httpClient.PostAsync(url, formContent).ConfigureAwait(false);
                    return message.GetAwaiter().GetResult().Content.ReadAsStreamAsync().Result;
                default:
                    throw new NotImplementedException("That type of request is unsupported.");
            }
        }
        public static string GetResponseStringAsync(string url,
            IEnumerable<KeyValuePair<string, string>> headers = null,
            RequestHttpMethod method = RequestHttpMethod.Get)
        {

            using (var streamReader = new StreamReader(GetResponseStreamAsync(url, headers, method)))
            {
                return streamReader.ReadToEndAsync().Result;
            }
        }

        public static async Task<string> GetDanbooruImageLink(params string[] tags)
        {
            var rng = new Random();

            var link = $"http://danbooru.donmai.us/posts?" +
                        $"page={rng.Next(0, 15)}" + (tags.Count() == 0 ? "" : "&tags=");
            foreach (string tag in tags)
            {
                if (!string.IsNullOrWhiteSpace(tag))
                    link += $"{tag.Replace(" ", "_")}+";
            }
            using (var http = new HttpClient())
            {
                var webpage = await http.GetStringAsync(link).ConfigureAwait(false);
                var matches = Regex.Matches(webpage, "data-large-file-url=\"(.*)\"");

                if (matches.Count == 0)
                    return "Nothing found!";
                return $"http://danbooru.donmai.us/" +
                       $"{matches[rng.Next(0, matches.Count)].Groups[1].Value}";
            }
        }

        public static async Task<string> GetGelbooruImageLink(params string[] tags)
        {
            var rng = new Random();
            var link = $"http://gelbooru.com/index.php?page=dapi&s=post&q=index&limit=100&tags=";

            foreach (string tag in tags)
            {
                if (!string.IsNullOrWhiteSpace(tag))
                    link += $"{tag.Replace(" ", "_")}+";
            }

            using (var http = new HttpClient())
            {
                var webpage = await http.GetStringAsync(link).ConfigureAwait(false);
                var matches = Regex.Matches(webpage, "file_url=\"(.*?)\" ");
                if (matches.Count == 0)
                    return "Nothing found!";

                var match = matches[rng.Next(0, matches.Count)];
                return "http:" + matches[rng.Next(0, matches.Count)].Groups[1].Value;
            }
        }

        public static async Task<string> GetR34ImageLink(params string[] tags)
        {
            var rng = new Random();
            var url =
            $"http://rule34.xxx/index.php?page=dapi&s=post&q=index&limit=100&tags=";
            foreach (string tag in tags)
            {
                if (!string.IsNullOrWhiteSpace(tag))
                    url += $"{tag.Replace(" ", "_")}+";
            }
            using (var http = new HttpClient())
            {
                var webpage = await http.GetStringAsync(url).ConfigureAwait(false);
                var matches = Regex.Matches(webpage, "file_url=\"(.*?)\" ");
                if (matches.Count == 0)
                    return "Nothing found!";
                var match = matches[rng.Next(0, matches.Count)];
                return "http:" + matches[rng.Next(0, matches.Count)].Groups[1].Value;
            }
        }

        public static async Task<string> GetCureninjaImageLink(params string[] tags)
        {
            var rng = new Random();
            string url = "https://cure.ninja/booru/api/json?f=a&o=r&s=1&q=";

            foreach (string tag in tags)
            {
                if (!string.IsNullOrWhiteSpace(tag))
                    url += $"{tag.Replace(" ", "_")}+";
            }

            using (var http = new HttpClient())
            {
                var webpage = await http.GetStringAsync(url).ConfigureAwait(false);
                var matches = Regex.Matches(webpage, "\"url\":\"(.*?)\"");
                if (matches.Count == 0)
                    return "Nothing found!";
                var match = matches[rng.Next(0, matches.Count)];
                return matches[rng.Next(0, matches.Count)].Groups[1].Value.Replace("\\/", "/");
            }
        }

        public static async Task<string> GetKonachanImageLink(params string[] tags)
        {
            var rng = new Random();

            var link = $"http://konachan.com/post?" +
                        $"page={rng.Next(0, 5)}&tags=";

            foreach (string tag in tags)
            {
                if (!string.IsNullOrWhiteSpace(tag))
                    link += $"{tag.Replace(" ", "_")}+";
            }

            using (var http = new HttpClient())
            {
                var webpage = await http.GetStringAsync(link).ConfigureAwait(false);
                var matches = Regex.Matches(webpage, "<a class=\"directlink smallimg\" href=\"(.*?)\"");

                if (matches.Count == 0)
                    return "Nothing found!";
                return "http:" + matches[rng.Next(0, matches.Count)].Groups[1].Value;
            }
        }

        public static async Task<string> GetYandereImageLink(params string[] tags)
        {
            var rng = new Random();
            var url =
            $"https://yande.re/post.xml?" +
            $"limit=25" +
            $"&page={rng.Next(0, 15)}" +
            $"&tags=";

            foreach (string tag in tags)
            {
                if (!string.IsNullOrWhiteSpace(tag))
                    url += $"{tag.Replace(" ", "_")}+";
            }

            using (var http = new HttpClient())
            {
                var webpage = await http.GetStringAsync(url).ConfigureAwait(false);
                var matches = Regex.Matches(webpage, "file_url=\"(.*?)\"");
                //var rating = Regex.Matches(webpage, "rating=\"(?<rate>.*?)\"");
                if (matches.Count == 0)
                    return "Nothing found!";
                return matches[rng.Next(0, matches.Count)].Groups[1].Value;
            }
        }

        public static async Task<string> GetSafebooruImageLink(params string[] tags)
        {
            var rng = new Random();
            var url =
            $"http://safebooru.org/index.php?page=dapi&s=post&q=index&limit=100&tags=";

            foreach (string tag in tags)
            {
                if (!string.IsNullOrWhiteSpace(tag))
                    url += $"{tag.Replace(" ", "_")}+";
            }

            using (var http = new HttpClient())
            {
                var webpage = await http.GetStringAsync(url).ConfigureAwait(false);
                var matches = Regex.Matches(webpage, "file_url=\"(.*?)\"");
                if (matches.Count == 0)
                    return "Nothing found!";
                var match = matches[rng.Next(0, matches.Count)];
                return "http:" + matches[rng.Next(0, matches.Count)].Groups[1].Value;
            }
        }
    }
}
