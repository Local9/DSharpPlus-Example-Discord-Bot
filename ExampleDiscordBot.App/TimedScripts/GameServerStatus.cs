using DSharpPlus;
using DSharpPlus.Entities;
using ExampleDiscordBot.App.Entities;
using Newtonsoft.Json;
using System.Timers;
using Timer = System.Timers.Timer;

namespace ExampleDiscordBot.App.TimedScripts
{
    public class GameServerStatus
    {
        static Timer _timer;
        static DiscordClient _discordClient;
        static List<Server> servers;
        static int serverIndex = 0;

        public GameServerStatus(DiscordClient client)
        {
            _timer = new Timer();
            _timer.Elapsed += new ElapsedEventHandler(Timer_Elapsed);
            _timer.Interval = (1000 * 30);
            _timer.Enabled = true;

            _discordClient = client;

            servers = Program.Configuration.Servers;

            Console.WriteLine($"[SERVER STATUS] Monitoring: {servers.Count}");
        }

        private async void Timer_Elapsed(object? sender, ElapsedEventArgs e)
        {
            try
            {
                string serverInformation = string.Empty;

                Server server = servers[serverIndex];

                if (serverIndex == servers.Count - 1)
                    serverIndex = 0;
                else
                    serverIndex++;

                DiscordActivity activity = new();
                activity.Name = "Server Offline";
                activity.ActivityType = ActivityType.ListeningTo;
                try
                {
                    serverInformation = await Utils.HttpTools.GetUrlResultAsync($"http://{server}/info.json");
                }
                catch (Exception ex)
                {
                    await _discordClient.UpdateStatusAsync(activity);
                    return;
                }

                string players = await Utils.HttpTools.GetUrlResultAsync($"http://{server}/players.json");

                List<CitizenFxPlayer> lst = JsonConvert.DeserializeObject<List<CitizenFxPlayer>>(players) ?? new();
                CitizenFxInfo info = JsonConvert.DeserializeObject<CitizenFxInfo>(serverInformation);
                activity.Name = $"{lst?.Count}/{info.Variables["sv_maxClients"]} players on {server.Label}";

                await _discordClient.UpdateStatusAsync(activity);

            }
            catch (Exception ex)
            {
                await Program.SendErrorMessage($"CRITICAL EXCEPTION [GameServerStatus]\n{ex.Message}\n{ex.StackTrace}");
            }
        }
    }
}
