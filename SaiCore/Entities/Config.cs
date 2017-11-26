using System;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json;
using DSharpPlus.Entities;
using System.IO;

namespace SaiCore.Entities
{
    internal class Config
    {
        [JsonProperty("token")]
        internal string Token = "token here";

        [JsonProperty("prefix")]
        internal string Prefix = "=+";

        [JsonProperty("osutoken")]
        internal string OsuToken = "";

        [JsonProperty("color")]
        private string _color = "#ffcff7";

        [JsonProperty("malcredentials")]
        internal string MyAnimeList = "username:password";

        [JsonIgnore]
        internal DiscordColor Color => new DiscordColor(_color);

        internal static Config Load(string path)
        {
            return JsonConvert.DeserializeObject<Config>(File.ReadAllText(path));
        }
    }
}
