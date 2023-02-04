using System;

namespace MagicMedia.Store;

public class DateSearch
{
    public int Yaer { get; set; }
    public int Month { get; set; }
    public int Day { get; set; }


    public static DateSearch? Create(DateTimeOffset? date)
    {
        if (date.HasValue)
        {
            return new DateSearch
            {
                Yaer = date.Value.Year,
                Month = date.Value.Month,
                Day = date.Value.Day
            };
        }

        return null;
    }
}
