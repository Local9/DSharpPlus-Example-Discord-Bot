namespace ExampleDiscordBot.App.Utils
{
    internal class HttpTools
    {
        public static async Task<string> GetUrlResultAsync(string url)
        {
            using (var client = new HttpClient())
            {
                return await client.GetStringAsync(url);
            }
        }
    }
}
