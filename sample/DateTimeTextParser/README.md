# Superpower sample / DateTimeTextParser

This example should shows how to build a simple text parser with Superpower.
It uses a simple and well known requirement: parsing date and time values 
according to ISO-8601 format.

## The requirement

In a simple, custom query language a user enter multiple conditions where a
property is compared to a static value. The property identifier the
comparison operator and the static value had to be separated with spaces.
There are text properties and datetime properties to query.

The static text values had to be entered enclosed with `'"'` (double qoutes),
datetime static values can be entered as date, time or date and time. For
better usability not only text can be enclosed with `'"'`, also the date time
static values can be surrounded with double qoutes. When only time is entered
it should be considered as todays time. When only date is entered midnight is
the default time value.

For example:
- `2017-01-01 12:10`
- `"2017-01-01 12:10"`
- `12:10`
- `2017-01-01`

## The code

The `DateTimeTextParser` class contains the `DateTime` Superpower `TextParser`, which 
covers all the requirements to parse a static datetime value.

```cs
public static TextParser<DateTime> DateTime = 
      from q1 in Character.EqualTo('"').Optional()
//    ^- First we looking for a optional double quote

      from date in (from date in Date
                    from s in Character.In('T', ' ')
                    from time in Time
                    select date + time).Try()
//                  ^- here we're looking for date and time values

                    .Or(from time in Time
                        select System.DateTime.Now.Date + time).Try()
//                  ^- when the first parser failed check for time only
//                     and add it to the current date

                    .Or(Date)
//                  ^- else it must be a date value  

      from q2 in Character.EqualTo('"').Optional().AtEnd()
//    ^- then we check for a optional double quote at the end of the input

      where (q1 == null && q2 == null) || (q1 != null && q2 != null)
//    ^- now we're checking if both quote are either set or not set

      select date;
```