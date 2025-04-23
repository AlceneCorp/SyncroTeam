using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SyncroTeam.Domain.Core
{
    [TypeConverter(typeof(ExpandableObjectConverter))]
    public class ShiftTemplate
    {
        public TimeOnly Start { get; set; }
        public TimeOnly End { get; set; }
        public int AgentCount { get; set; }

        public ShiftTemplate() { }

        public ShiftTemplate(TimeOnly start, TimeOnly end, int agentCount)
        {
            Start = start;
            End = end;
            AgentCount = agentCount;
        }
    }
}
