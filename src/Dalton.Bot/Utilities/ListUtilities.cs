namespace Dalton.Bot.Utilities;

public static class ListUtilities
{
    public static IEnumerable<T> Randomize<T>(this IEnumerable<T> source)
    {
        var rnd = new Random();
        return source.OrderBy((_) => rnd.Next());
    }
}