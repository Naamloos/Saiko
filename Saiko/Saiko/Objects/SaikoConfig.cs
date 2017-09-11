using Newtonsoft.Json;
using DSharpPlus;
using DSharpPlus.Entities;

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

        [JsonProperty("osukey")]
        public string OsuToken = "Your Osu! API key here";

        [JsonProperty("using-pgsql")]
        public bool UsingPQSQL = true;

        [JsonProperty("database-host")]
        public string DatabaseHost = "localhost";

        [JsonProperty("database-port")]
        public int DatabasePort = 5432;

        [JsonProperty("database-name")]
        public string DatabaseName = "saiko";

        [JsonProperty("database-user")]
        public string DatabaseUser = "username";

        [JsonProperty("database-pass")]
        public string DatabasePassword = "password";
    }
}
