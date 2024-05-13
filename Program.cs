using Discord;
using Discord.WebSocket;
using dotenv.net;
using dotenv.net.Utilities;

namespace JimBroBot;

static class Program
{
    static DiscordSocketClient _client;

    static async Task<int> Main()
    {
        DotEnv.Load(options: new DotEnvOptions(probeForEnv: true, ignoreExceptions: false));

        if (!EnvReader.TryGetStringValue("JIM_BRO_BOT_TOKEN", out var botToken))
        {
            Console.WriteLine("Could not read JIM_BRO_BOT_TOKEN from .env");
            return 1;
        }

        _client = new DiscordSocketClient();
        _client.Log += Log;

        await _client.LoginAsync(TokenType.Bot, botToken);
        await _client.StartAsync();

        await Task.Delay(-1);

        return 0;
    }

    static Task Log(LogMessage message)
    {
       Console.WriteLine(message.ToString());
       return Task.CompletedTask;
    }
}