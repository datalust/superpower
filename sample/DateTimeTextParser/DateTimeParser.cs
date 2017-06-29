using System;
using Superpower;
using Superpower.Parsers;

namespace DateTimeParser
{
    static public class DateTimeParser 
    {
        private static TextParser<string> _2DigitParser =
            from d1 in Character.Digit
            from d2 in Character.Digit
            select new string(new char[] {d1, d2});

        private static TextParser<string> YearOfDate = 
            from y1 in Character.Digit
            from y2 in Character.Digit
            from y3 in Character.Digit
            from y4 in Character.Digit
            select new string(new char[] {y1, y2, y3, y4});
        private static TextParser<string> MonthOfDate = 
            _2DigitParser;
        private static TextParser<string> DayOfDate = 
            _2DigitParser;

        public static TextParser<DateTime> Date = 
            from year in YearOfDate.Select(_ => Int32.Parse(_))
            from sep1 in Character.EqualTo('-')
            from mon in MonthOfDate.Select(_ => Int32.Parse(_))
            from sep2 in Character.EqualTo('-')
            from day in DayOfDate.Select(_ => Int32.Parse(_))
            select new DateTime(year, mon, day);

        private static TextParser<int> secondWithSep = 
            from sep in Character.EqualTo(':')
            from second in _2DigitParser.Select(_ => Int32.Parse(_))
            select second;

        public static TextParser<TimeSpan> Time =
            from hour in _2DigitParser.Select(_ => Int32.Parse(_))
            from sep1 in Character.EqualTo(':')
            from minute in _2DigitParser.Select(_ => Int32.Parse(_))
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