using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace analytics_engine
{
    public static class Clock
    {
        private static Func<DateTime> _now;
        static Clock()
        {
            _now = () => DateTime.Now;
        }

        public static DateTime Now => _now();
    }
}
