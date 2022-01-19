using Dalton.Bot.Mediator.Requests;
using Dalton.Bot.Utilities;
using Discord;
using Discord.WebSocket;
using MediatR;

namespace Dalton.Bot.Services
{
    public class GuildInteractionService
    {
        private readonly DiscordSocketClient _discord;
        private readonly IMediator _mediator;

        public GuildInteractionService(
            DiscordSocketClient discord,
            IMediator mediator)
        {
            _discord = discord;
            _mediator = mediator;
        }

        public void Init()
        {
            _discord.UserJoined += UserJoinedGuild;
            _discord.GuildMemberUpdated += GuildMemberUpdated;
        }
        private async Task GuildMemberUpdated(Cacheable<SocketGuildUser, ulong> arg1, SocketGuildUser arg2)
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
            var isAlphaNumeric = nameToCheck.IsAlphaNumeric();

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