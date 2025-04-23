using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SyncroTeam.Domain.Enumerations;

namespace SyncroTeam.Domain.Entities
{
    [TypeConverter(typeof(ExpandableObjectConverter))]
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
