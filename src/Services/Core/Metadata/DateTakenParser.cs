using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;

namespace MagicMedia.Metadata;

public class DateTakenParser : IDateTakenParser
{
    private static List<DateTakenParserDefinition> _parsers = new List<DateTakenParserDefinition>
        {
            new DateTakenParserDefinition
            {
                DateFormat = "yyyyMMdd",
                CanUse = (file) => file.StartsWith("IMG-") || file.StartsWith("VID-"),
                ExtractDatePart = (file) => file.Split('-').LastOrDefault(),
            },
            new DateTakenParserDefinition
            {
                CanUse = (file) => Regex.Match(file, @"(\d{8})(_)(\d{6})").Success,
                DateFormat = "yyyyMMdd_HHmmss"
            }
        };

    public DateTime? Parse(string value)
    {
        foreach (DateTakenParserDefinition? parser in _parsers)
        {
            try
            {
                if (parser.CanUse == null || parser.CanUse(value))
                {
                    var dateString = parser.ExtractDatePart != null ?
                        parser.ExtractDatePart(value) : value;

                    DateTime taken;
                    if (DateTime.TryParseExact(
                        dateString,
                        parser.DateFormat,
                        null,
                        DateTimeStyles.None,
                        out taken))
                    {
                        return taken;
                    }
                }
            }
            catch
            {
                //Ignore
            }
        }

        return null;
    }
}


public class DateTakenParserDefinition
{
    public Func<string, bool>? CanUse { get; set; }

    public Func<string, string?>? ExtractDatePart { get; set; }

    public string? DateFormat { get; set; }
}
