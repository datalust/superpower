namespace Superpower.Model
{
    public static class Result
    {
        public static Result<T> Empty<T>(StringSpan remainder)
        {
            return new Result<T>(remainder);
        }

        public static Result<T> Value<T>(T value, StringSpan location, StringSpan remainder)
        {
            return new Result<T>(value, location, remainder);
        }
    }
}