using Superpower.Model;

namespace Superpower.Parsers
{
    public static class Numerics
    {
        public static readonly CharParser<StringSpan> Integer = input =>
        {
            var next = input.NextChar();
            if (!next.HasValue || !char.IsDigit(next.Value))
                return CharResult.Empty<StringSpan>(input);

            StringSpan remainder;
            do
            {
                remainder = next.Remainder;
                next = remainder.NextChar();
            } while (next.HasValue && char.IsDigit(next.Value));

            return CharResult.Value(input.Until(remainder), input, remainder);
        };

        public static readonly CharParser<int> IntegerInt32 = input =>
        {
            var next = input.NextChar();
            if (!next.HasValue || !char.IsDigit(next.Value))
                return CharResult.Empty<int>(input);

            StringSpan remainder;
            var val = 0;
            do
            {
                val = 10 * val + (next.Value - '0');
                remainder = next.Remainder;
                next = remainder.NextChar();
            } while (next.HasValue && char.IsDigit(next.Value));

            return CharResult.Value(val, input, remainder);
        };
    }
}