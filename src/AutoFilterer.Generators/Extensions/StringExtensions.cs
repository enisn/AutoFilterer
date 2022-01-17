namespace AutoFilterer.Generators.Extensions;

public static class StringExtensions
{
    public static string EnsureEndsWith(this string source, string suffix)
    {
        if (source.EndsWith(suffix))
        {
            return source;
        }

        return source + suffix;
    }
}
