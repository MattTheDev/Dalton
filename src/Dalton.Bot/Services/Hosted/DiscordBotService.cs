using Discord;
using Discord.WebSocket;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Dalton.Bot.Services.Hosted
{
    public class DiscordBotService : IHostedService
    {
        private readonly DiscordSocketClient _discord;
        private readonly CommandService _commandService;
        private readonly StartupService _startupService;
        private readonly GuildInteractionService _guildInteractionService;
        private readonly ILogger<DiscordBotService> _logger;

        public DiscordBotService(
            DiscordSocketClient discord,
            CommandService commandService,
            StartupService startupService, 
            ILogger<DiscordBotService> logger, 
            GuildInteractionService guildInteractionService)
        {
            _discord = discord ?? throw new ArgumentNullException(nameof(discord));
            _commandService = commandService ?? throw new ArgumentNullException(nameof(commandService));
            _startupService = startupService;
            _logger = logger;
            _guildInteractionService = guildInteractionService;
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            await _startupService.StartAsync();

            while (_discord.CurrentUser == null)
            {
                _logger.LogInformation("Discord user connection pending ...");
                await Task.Delay(5000, cancellationToken);
            }

            while (_discord.ConnectionState != ConnectionState.Connected)
            {
                _logger.LogInformation("Discord user connection pending ...");
                await Task.Delay(5000, cancellationToken);
            }

            _logger.LogInformation("Discord user connected: {Username}", _discord.CurrentUser.Username);

            _commandService.Init();
            _guildInteractionService.Init();
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("Bot stopping");

            return Task.CompletedTask;
        }
    }
}