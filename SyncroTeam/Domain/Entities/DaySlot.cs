using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SyncroTeam.Domain.Enumerations;

namespace SyncroTeam.Domain.Entities
{
    public class DaySlot
    {
        public DayPeriod Period { get; set; }
        public List<Activity> Activities { get; set; } //List
        
        public bool IsManualOverride { get; set; } // permet de tracer les changements manuels

        public DaySlot(DayPeriod period)
        {
            Period = period;
        }

        public DaySlot() { }
    }
}
