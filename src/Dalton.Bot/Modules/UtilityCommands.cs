using Dalton.Bot.Mediator.Requests;
using Discord.Commands;
using MediatR;

namespace Dalton.Bot.Modules;

public class UtilityCommands : ModuleBase
{
    private readonly IMediator _mediator;

    public UtilityCommands(IMediator mediator)
    {
        _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
    }

    [Command("ping", RunMode = RunMode.Async)]
    [Summary("Test the response of the bot.")]
    public async Task PingAsync()
    {
        await ReplyAsync("Pong!");
    }

    [Command("generate", RunMode = RunMode.Async)]
    [Summary("This command tests the random name generator, outputting a sample of what a randomized nickname could be.")]
    public async Task GenerateAsync()
    {
        await ReplyAsync(await _mediator.Send(new GenerateRandomNameRequest()));
    }
}