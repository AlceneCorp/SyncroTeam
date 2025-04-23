using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SyncroTeam.Domain.Enumerations
{
    [TypeConverter(typeof(ExpandableObjectConverter))]
    public enum DayPeriod
    {
        Morning,
        Afternoon
    }
}
