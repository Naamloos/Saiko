using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Saiko
{
    class Program
    {
        // public static, so IF I need anything from it, I can reach it from anywhere
        public static SaikoBot SaikoBot;
        static void Main(string[] args) => MainAsync(args).GetAwaiter().GetResult();

        static async Task MainAsync(string[] args)
        {
            #region Config check
            if (!File.Exists("SaikoConfig.json"))
            {
                // whoops!
                var json = JsonConvert.SerializeObject(new SaikoConfig());
                File.WriteAllText("SaikoConfig.json", json, new UTF8Encoding(false));
                Console.WriteLine("Hey! You have not set your configuration yet! Please set all values in SaikoConfig.json.");
                Console.ReadKey();
                return;
            }
            var cfg = JsonConvert.DeserializeObject<SaikoConfig>(File.ReadAllText("SaikoConfig.json"));
            #endregion

            // Let's make Saiko initialize
            SaikoBot = new SaikoBot(cfg);

            // And connect!
            await SaikoBot.ConnectAsync();
            // Endless delay to keep bot up and running :^)
            await Task.Delay(-1);
        }
    }
}
