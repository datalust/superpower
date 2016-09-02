namespace Superpower
{
    static class Presentation
    {
        public static string FormatKind<TTokenKind>(TTokenKind kind)
        {
            return kind.ToString().ToLower();
        }
    }
}