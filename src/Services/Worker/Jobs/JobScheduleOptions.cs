using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MagicMedia.Jobs
{
    public class JobScheduleOptions
    {
        public string Name { get; set; }

        public bool Enabled { get; set; }

        public TimeSpan? Interval { get; set; }

        public string? Cron { get; set; }
    }
}
