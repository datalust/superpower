namespace Superpower.Model
{
    public static class CharResult
    {
        public static CharResult<T> Empty<T>(StringSpan remainder)
        {
            return new CharResult<T>(remainder, null);
        }

        public static CharResult<T> Empty<T>(StringSpan remainder, string[] expectations)
        {
            return new CharResult<T>(remainder, expectations);
        }

        public static CharResult<T> Value<T>(T value, StringSpan location, StringSpan remainder)
        {
            return new CharResult<T>(value, location, remainder);
        }

        public static CharResult<U> CastEmpty<T, U>(CharResult<T> result)
        {
            return new CharResult<U>(result.Remainder, result.Expectations);
        }

        public static CharResult<T> CombineEmpty<T>(CharResult<T> first, CharResult<T> second)
        {
            if (first.Remainder != second.Remainder)
            {
                if (first.Remainder.Position.Absolute > second.Remainder.Position.Absolute)
                    return first;

                if (second.IsPartial(second.Location))
                    return second;

                return first;
            }

            var expectations = first.Expectations;
            if (expectations == null)
                expectations = second.Expectations;
            else if (second.Expectations != null)
            {
                expectations = new string[first.Expectations.Length + second.Expectations.Length];
                var i = 0;
                for (; i < first.Expectations.Length; ++i)
                    expectations[i] = first.Expectations[i];
                for (var j = 0; j < second.Expectations.Length; ++i, ++j)
                    expectations[i] = second.Expectations[j];
            }

            return new CharResult<T>(first.Remainder, expectations);
        }
    }
}
