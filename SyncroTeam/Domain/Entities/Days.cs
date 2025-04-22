using SyncroTeam.Domain.Enumerations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SyncroTeam.Domain.Entities
{
    public class Days
    {
        public DayOfWeek DayOfWeek { get; set; }

        public DaySlot MorningPeriod { get; set; } = new DaySlot(DayPeriod.Morning);
        public DaySlot AfternoonPeriod { get; set; } = new DaySlot(DayPeriod.Afternoon);

        public Days() { }

        public Days(DayOfWeek param_Day) 
        { 
            this.DayOfWeek = param_Day;
        }
    }
}
