using System;
using Superpower;

namespace DateTimeTextParser
{
    class Program
    {
        static void ParseAndPrint(string input)
        {
            try
            {
                var dt = DateTimeTextParser.DateTime.Parse(input);
                Console.WriteLine("Input: '{0}', ParsedValue: '{1}'", input, dt.ToString("o"));
            }
            catch (System.Exception ex)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine(ex.ToString());
                Console.ForegroundColor = ConsoleColor.White;
            }
        }

        static void Main(string[] args)
        {
            ParseAndPrint("12:38");
            ParseAndPrint("12:38:10");
            ParseAndPrint("2017-01-01");
            ParseAndPrint("2017-01-01 05:28:10");
            ParseAndPrint("2017-01-01 05:28");
            ParseAndPrint("\"2017-01-01\"");
            ParseAndPrint("\"2017-01-01");
            ParseAndPrint("2017-01-01 05:x8:10");
            ParseAndPrint("2017-01-01T05:28:10");
            ParseAndPrint("2017-01-01T05:28");
        }
    }
}
