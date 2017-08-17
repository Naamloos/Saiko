using Newtonsoft.Json;
using DSharpPlus;

namespace Saiko
{
    public class SaikoConfig
    {
        [JsonProperty("token")]
        public string Token = "your token here";

        [JsonProperty("prefix")]
        public string Prefix = "";

        [JsonProperty("color")]
        private string _color = "#ffcff7";
        [JsonIgnore]
        public DiscordColor Color => new DiscordColor(_color);

        [JsonProperty("status")]
        public string Status = "❤ Hello! ;) ❤";
    }
}
