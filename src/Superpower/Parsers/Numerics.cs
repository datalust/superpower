using Superpower.Model;

namespace Superpower.Parsers
{
    public static class Numerics
    {
        static readonly string[] ExpectedDigit = { "digit" };

        public static CharParser<StringSpan> Integer { get; } = input =>
        {
            var next = input.ConsumeChar();
            if (!next.HasValue || !char.IsDigit(next.Value))
                return CharResult.Empty<StringSpan>(input, ExpectedDigit);

            StringSpan remainder;
            do
            {
                remainder = next.Remainder;
                next = remainder.ConsumeChar();
            } while (next.HasValue && char.IsDigit(next.Value));

            return CharResult.Value(input.Until(remainder), input, remainder);
        };

        public static CharParser<int> IntegerInt32 { get; } = input =>
        {
            var next = input.ConsumeChar();
            if (!next.HasValue || !char.IsDigit(next.Value))
                return CharResult.Empty<int>(input, ExpectedDigit);

            StringSpan remainder;
            var val = 0;
            do
            {
                val = 10 * val + (next.Value - '0');
                remainder = next.Remainder;
                next = remainder.ConsumeChar();
            } while (next.HasValue && char.IsDigit(next.Value));

            return CharResult.Value(val, input, remainder);
        };
    }
}