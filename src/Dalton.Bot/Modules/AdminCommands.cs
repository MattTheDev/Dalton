using Dalton.Bot.Mediator.Requests;
using Dalton.Bot.Models;
using Dalton.Bot.Utilities;
using Discord;
using Discord.Commands;
using MediatR;
using Microsoft.Extensions.Options;

namespace Dalton.Bot.Modules;

public class AdminCommands : ModuleBase
{
    private readonly Settings _settings;
    private readonly IMediator _mediator;

    public AdminCommands(
        IMediator mediator, 
        IOptions<Settings> settings)
    {
        _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        _settings = settings == null ? throw new ArgumentNullException(nameof(settings)) : settings.Value;
    }

    [Command("rename", RunMode = RunMode.Async)]
    [Summary("Mass rename users in the server with non-alphanumeric names.")]
    [RequireUserPermission(GuildPermission.ManageGuild)]
    [RequireBotPermission(GuildPermission.ManageNicknames)]
    public async Task RenameAsync()
    {
        var count = 0;
        foreach (var user in await Context.Guild.GetUsersAsync())
        {
            var nameInUse = user.Nickname ?? user.Username;
            if (!nameInUse.IsAlphaNumeric(_settings.AllowedCharacters.ToCharArray()))
            {
                nameInUse = await _mediator.Send(new GenerateRandomNameRequest());

                // TODO Add a throttle and iterate here.
                await user.ModifyAsync(x => x.Nickname = nameInUse);
                count++;
            }

            continue;
        }

        await ReplyAsync($"A total of {count} names WOULD have been updated.");
    }
}