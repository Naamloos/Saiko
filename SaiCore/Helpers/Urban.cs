using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace SaiCore.Helpers
{
    public class List
    {
        [JsonProperty("definition")]
        public string Definition { get; set; }

        [JsonProperty("permalink")]
        public string Permalink { get; set; }

        [JsonProperty("thumbs_up")]
        public int ThumbsUp { get; set; }

        [JsonProperty("author")]
        public string Author { get; set; }

        [JsonProperty("word")]
        public string Word { get; set; }

        [JsonProperty("defid")]
        public int Defid { get; set; }

        [JsonProperty("current_vote")]
        public string CurrentVote { get; set; }

        [JsonProperty("example")]
        public string Example { get; set; }

        [JsonProperty("thumbs_down")]
        public int ThumbsDown { get; set; }
    }

    public class Data
    {
        [JsonProperty("tags")]
        public string[] Tags { get; set; }

        [JsonProperty("result_type")]
        public string ResultType { get; set; }

        [JsonProperty("list")]
        public List[] List { get; set; }

        [JsonProperty("sounds")]
        public string[] Sounds { get; set; }
    }

    public class UrbanDict
    {
        public async static Task<KeyValuePair<bool, Data>> GetDataAsync(string query)
        {
            using (var http = new HttpClient())
            {
                var result = await http.GetStringAsync($"http://api.urbandictionary.com/v0/define?term={WebUtility.UrlEncode(query)}");
                var data = JsonConvert.DeserializeObject<Data>(result);

                return new KeyValuePair<bool, Data>(data.ResultType == "no_results" ? false : true, data);
            }
        }
    }
}