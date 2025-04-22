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
        public Activity Activity { get; set; }
        public string AssignedAgent { get; set; } // nom de l'agent affecté
        public bool IsManualOverride { get; set; } // permet de tracer les changements manuels

        public DaySlot(DayPeriod period)
        {
            Period = period;
        }

        public DaySlot() { }
    }
}
