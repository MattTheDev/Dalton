using Dalton.Bot.Mediator.Requests;
using Dalton.Bot.Models;
using Dalton.Bot.Utilities;
using Discord;
using Discord.WebSocket;
using MediatR;
using Microsoft.Extensions.Options;

namespace Dalton.Bot.Services
{
    public class GuildInteractionService
    {
        private readonly DiscordSocketClient _discord;
        private readonly IMediator _mediator;
        private readonly Settings _settings;

        public GuildInteractionService(
            DiscordSocketClient discord,
            IMediator mediator, 
            IOptions<Settings> settings)
        {
            _discord = discord ?? throw new ArgumentNullException(nameof(discord));
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
            _settings = settings == null ? throw new ArgumentNullException(nameof(settings)) : settings.Value;
        }

        public void Init()
        {
            _discord.UserJoined += UserJoinedGuild;
            _discord.GuildMemberUpdated += GuildMemberUpdated;
        }

        private async Task GuildMemberUpdated(
            Cacheable<SocketGuildUser, ulong> arg1, 
            SocketGuildUser arg2)
        {
            await ValidateUser(arg2);
        }

        private async Task UserJoinedGuild(IGuildUser arg)
        {
            await ValidateUser(arg);
        }

        private async Task ValidateUser(IGuildUser guildUser)
        {
            // Validate the bot user has the permission to modify a nickname. If not, return and skip.
            var botUser = await guildUser.Guild.GetUserAsync(_discord.CurrentUser.Id);
            if (!botUser.GuildPermissions.ManageNicknames)
            {
                return;
            }

            // Validate if username/nickname needs to be modified.
            var nameToCheck = guildUser.Nickname ?? guildUser.Username;
            var isAlphaNumeric = nameToCheck.IsAlphaNumeric(_settings.AllowedCharacters.ToCharArray());

            // If not, let's change it.
            if (!isAlphaNumeric)
            {
                // Get a random name for them..
                var newNickname = await _mediator.Send(new GenerateRandomNameRequest());
                // .. and set it!
                await guildUser.ModifyAsync(x => x.Nickname = newNickname);
            }
        }
    }
}