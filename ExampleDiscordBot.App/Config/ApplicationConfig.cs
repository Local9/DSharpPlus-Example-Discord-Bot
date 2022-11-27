using ExampleDiscordBot.App.Entities;
using Newtonsoft.Json;

namespace ExampleDiscordBot.App.Config
{
    internal class ApplicationConfig
    {
        static Configuration _configuration;

        public static async Task<Configuration> GetConfig()
        {
            string json = string.Empty;
            using (FileStream fs = File.OpenRead("config.json"))
            using (StreamReader sr = new StreamReader(fs))
                json = await sr.ReadToEndAsync();

            if (string.IsNullOrEmpty(json))
                throw new Exception("Unable to load config.json");

            _configuration = JsonConvert.DeserializeObject<Configuration>(json);

            return _configuration;
        }

        public static List<Server> Servers => _configuration.Servers;
    }
}
