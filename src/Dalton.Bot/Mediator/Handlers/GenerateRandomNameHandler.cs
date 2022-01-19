using Dalton.Bot.Mediator.Requests;
using Dalton.Bot.Utilities;
using MediatR;

namespace Dalton.Bot.Mediator.Handlers;

public class GenerateRandomNameHandler : IRequestHandler<GenerateRandomNameRequest, string>
{
    private readonly Random _random;

    public GenerateRandomNameHandler(Random random)
    {
        _random = random;
    }

    public async Task<string> Handle(GenerateRandomNameRequest request, CancellationToken cancellationToken)
    {
        var adjectives = (await File.ReadAllTextAsync("english-adjectives.txt", cancellationToken)).Split("\n").Randomize().ToList();
        var adjective = adjectives[_random.Next(0, adjectives.Count)];
        var nouns = (await File.ReadAllTextAsync("english-nouns.txt", cancellationToken)).Split("\n").Randomize().ToList();
        var noun = nouns[_random.Next(0, nouns.Count)];
        var number = _random.Next(1, 999);

        return $"{adjective.FirstLetterToUpper()}{noun.FirstLetterToUpper()}{number}";
    }
}