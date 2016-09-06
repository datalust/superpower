using Superpower;
using System;

namespace IntCalc
{
    public class Program
    {
        public static int Main(string[] args)
        {
            var cmdline = string.Join(" ", args);
            if (string.IsNullOrWhiteSpace(cmdline))
            {
                Console.Error.WriteLine("Usage: intcalc.exe <expression>");
                return 1;
            }

            try
            {
                var tok = new ArithmeticExpressionTokenizer();
                var tokens = tok.Tokenize(cmdline);
                var expr = ArithmeticExpressionParser.Lambda.Parse(tokens);
                var compiled = expr.Compile();
                var result = compiled();
                Console.WriteLine(result);
                return 0;
            }
            catch (ParseException ex)
            {
                Console.Error.WriteLine(ex.Message);
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine(ex);
            }
            return 1;
        }
    }
}
