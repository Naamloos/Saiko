using DSharpPlus;
using DSharpPlus.Commands;
using DSharpPlus.Commands.Processors.SlashCommands;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.IO;
using System.Threading.Tasks;

namespace Saiko
{
    class Program
    {
        static DiscordClient client;

        static async Task Main()
        {
            if (!File.Exists("token.txt"))
            {
                Console.WriteLine("No token file found! File created (token.txt), please fill out!");
                File.Create("token.txt");
                Console.ReadKey();
                return;
            }

            var token = "";
            using (StreamReader sw = new StreamReader(File.OpenRead("token.txt")))
            {
                token = sw.ReadToEnd();
            }

            client = new DiscordClient(new DiscordConfiguration()
            {
                Token = File.ReadAllText("token.txt").Trim(),
                Intents = DiscordIntents.AllUnprivileged | DiscordIntents.MessageContents
            });

            var collection = new ServiceCollection()
                .AddSingleton(client);

            var commands = client.UseCommands(new CommandsConfiguration()
            {
                ServiceProvider = collection.BuildServiceProvider()
            });

            commands.AddCommands(typeof(Program).Assembly);

            await commands.AddProcessorAsync(new SlashCommandProcessor());

            try
            {
                await client.ConnectAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
            await Task.Delay(-1);
        }
    }
}
