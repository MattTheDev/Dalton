using System.Reflection;
using Dalton.Bot.Models;
using Discord;
using Discord.WebSocket;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Dalton.Bot.Services
{
    public class StartupService
    {
        private readonly DiscordSocketClient _discord;
        private readonly Discord.Commands.CommandService _commands;
        private readonly Settings _settings;
        private readonly IServiceProvider _serviceProvider;
        private readonly ILogger<StartupService> _logger;

        public StartupService(
            DiscordSocketClient discord,
            Discord.Commands.CommandService commands,
            IOptions<Settings> settings,
            IServiceProvider serviceProvider, 
            ILogger<StartupService> logger)
        {
            _settings = settings.Value;
            _discord = discord;
            _commands = commands;
            _serviceProvider = serviceProvider;
            _logger = logger;
        }

        public async Task StartAsync()
        {
            _logger.LogInformation("Starting connection to Discord ...");
            var discordToken = _settings.BotToken;

            if (string.IsNullOrWhiteSpace(discordToken))
            {
                _logger.LogError("Bot token missing for the `Settings.json` file. Please enter the bot token, and restart the service.");

                throw new Exception("Please enter your bot's token into the `Settings.json` file found in the applications root directory.");
            }

            await _discord.LoginAsync(TokenType.Bot, discordToken);
            await _discord.StartAsync();

            await _commands.AddModulesAsync(Assembly.GetEntryAssembly(), _serviceProvider);

            _logger.LogInformation("Connection to Discord Established ...");
        }
    }
}