using System;
using System.Collections.Generic;
using Superpower;
using Superpower.Model;
using Superpower.Parsers;

namespace DateTimeTextParser
{
    static public class DateTimeTextParser 
    {
        static TextParser<char[]> Repeat(this TextParser<char> parser, int count)
        {
            return input =>
            {
                List<char> result = new List<char>();

                Result<char> next = input.ConsumeChar();
                var beginning = next.Location;

                for (int i = 0; i < count; i++)
                {
                    var parserResult = parser.Invoke(next.Location);
                    if (parserResult.HasValue) 
                    {
                        result.Add(parserResult.Value);
                        next = next.Remainder.ConsumeChar();
                    }
                    else
                        return Result.Empty<char[]>(input);
                }

                return Result.Value(result.ToArray(), beginning, next.Location);
            };
        }


        static TextParser<string> TwoDigits =
            Character.Digit.Repeat(2).Select(chs => new String(chs));

        static TextParser<string> YearOfDate = 
            Character.Digit.Repeat(4).Select(chs => new String(chs));

        static TextParser<string> MonthOfDate = 
            TwoDigits;

        static TextParser<string> DayOfDate = 
            TwoDigits;

        public static TextParser<DateTime> Date = 
            from year in YearOfDate.Select(Int32.Parse)
            from sep1 in Character.EqualTo('-')
            from mon in MonthOfDate.Select(Int32.Parse)
            from sep2 in Character.EqualTo('-')
            from day in DayOfDate.Select(Int32.Parse)
            select new DateTime(year, mon, day);

        static TextParser<int> secondWithSep = 
            from sep in Character.EqualTo(':')
            from second in TwoDigits.Select(Int32.Parse)
            select second;

        public static TextParser<TimeSpan> Time =
            from hour in TwoDigits.Select(Int32.Parse)
            from sep1 in Character.EqualTo(':')
            from minute in TwoDigits.Select(Int32.Parse)
            from second in secondWithSep.OptionalOrDefault()
            select new TimeSpan(hour, minute, second);

        public static TextParser<DateTime> DateTime = 
            from q1 in Character.EqualTo('"').Optional()
            from date in (from date in Date
                          from s in Character.In('T', ' ')
                          from time in Time
                          select date + time).Try()
                          .Or(from time in Time
                              select System.DateTime.Now.Date + time).Try()
                          .Or(Date)
            from q2 in Character.EqualTo('"').Optional().AtEnd()
            where (q1 == null && q2 == null) || (q1 != null && q2 != null)
            select date;
    }
}