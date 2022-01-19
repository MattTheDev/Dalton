using Dalton.Bot.Models;
using Discord.Commands;
using Discord.WebSocket;
using Microsoft.Extensions.Options;

namespace Dalton.Bot.Services
{
    public class CommandService
    {
        private readonly DiscordSocketClient _discord;
        private readonly Discord.Commands.CommandService _commands;
        private readonly Settings _settings;
        private readonly IServiceProvider _provider;

        public CommandService(
            DiscordSocketClient discord, 
            Discord.Commands.CommandService commands, 
            IOptions<Settings> settings,
            IServiceProvider provider)
        {
            _discord = discord;
            _commands = commands;
            _settings = settings.Value;
            _provider = provider;
        }

        public void Init()
        {
            _discord.MessageReceived += OnMessageReceivedAsync;
        }

        private async Task OnMessageReceivedAsync(SocketMessage s)
        {
            if (
                s is not SocketUserMessage msg ||
                msg.Author.IsBot ||
                msg.Author.IsWebhook)
            {
                return;
            }

            var context = new SocketCommandContext(_discord, msg);
            var prefix = _settings.Prefix;
            var argPos = 0;

            if (msg.HasStringPrefix(prefix, ref argPos) ||
                msg.HasMentionPrefix(_discord.CurrentUser, ref argPos))
            {
                var result = await _commands.ExecuteAsync(context, argPos, _provider);

                if (!result.IsSuccess)
                {
                    await msg.DeleteAsync();
                    await context.Channel.SendMessageAsync($"<:redx:756214731068670014> `{msg.Content}` is not a valid Dalton command.");
                }
            }
        }
    }
}