using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SyncroTeam.Domain.Entities
{
    [TypeConverter(typeof(ExpandableObjectConverter))]
    public class WeeklyShift
    {
        public String Agent { get; set; }
        public TimeOnly Start { get; set; }
        public TimeOnly End { get; set; }
    }
}
