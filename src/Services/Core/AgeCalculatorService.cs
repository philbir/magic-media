using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NodaTime;

namespace MagicMedia
{
    public class AgeCalculatorService
    {
        public int? CalculateAge(DateTimeOffset? dateTaken, DateTime dateOfBirth)
        {
            if (dateTaken.HasValue)
            {
                var taken = LocalDate.FromDateTime(dateTaken.Value.Date);
                var birth = LocalDate.FromDateTime(dateOfBirth);
                Period period = Period.Between(taken, birth, PeriodUnits.Months);

                return Math.Abs(period.Months);
            }

            return null;
        }
    }
}
