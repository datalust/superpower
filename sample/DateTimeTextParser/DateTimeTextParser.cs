using System;
using Superpower;
using Superpower.Parsers;

namespace DateTimeTextParser
{
    static public class DateTimeTextParser 
    {
        static TextParser<string> TwoDigits =
            from d1 in Character.Digit
            from d2 in Character.Digit
            select new string(new char[] {d1, d2});

        static TextParser<string> YearOfDate = 
            from y1 in Character.Digit
            from y2 in Character.Digit
            from y3 in Character.Digit
            from y4 in Character.Digit
            select new string(new char[] {y1, y2, y3, y4});
        static TextParser<string> MonthOfDate = 
            TwoDigits;
        static TextParser<string> DayOfDate = 
            TwoDigits;

        public static TextParser<DateTime> Date = 
            from year in YearOfDate.Select(_ => Int32.Parse(_))
            from sep1 in Character.EqualTo('-')
            from mon in MonthOfDate.Select(_ => Int32.Parse(_))
            from sep2 in Character.EqualTo('-')
            from day in DayOfDate.Select(_ => Int32.Parse(_))
            select new DateTime(year, mon, day);

        static TextParser<int> secondWithSep = 
            from sep in Character.EqualTo(':')
            from second in TwoDigits.Select(_ => Int32.Parse(_))
            select second;

        public static TextParser<TimeSpan> Time =
            from hour in TwoDigits.Select(_ => Int32.Parse(_))
            from sep1 in Character.EqualTo(':')
            from minute in TwoDigits.Select(_ => Int32.Parse(_))
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