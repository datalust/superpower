using Superpower;
using Superpower.Model;
using Superpower.Parsers;
using System;

namespace Sample
{
    public class Program
    {
        public static void Main(string[] args)
        {
            const string input = "(abc (234) 567 defgh) ";
            Console.WriteLine($"Input is: {input}");

            var tok = new SExpressionTokenizer();
            foreach (var token in tok.Tokenize(input))
            {
                Console.WriteLine(token);
            }

            var number = Parse.Token(SExpressionToken.Number)
                              .Apply(t => Numerics.IntegerInt32);

            var number1 = Parse.Token(SExpressionToken.Number)
                               .Apply(Numerics.IntegerInt32);

            var number2 = Parse.Token(SExpressionToken.Number)
                               .Select(t => int.Parse(t.Value));

            var numbers = number.AtLeastOnce();

            var stream = tok.Tokenize("1 23 456");

            var result = numbers(stream);

            if (result.HasValue)
            {
                foreach (var n in result.Value)
                {
                    Console.WriteLine(n);
                }
            }
            else
            {
                Console.WriteLine(result);
            }
        }
    }
}
