using SyncroTeam.Domain.Enumerations;
using SyncroTeam.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace SyncroTeam.Domain.Core
{
    public class ActivityTemplate
    {
        public string Name { get; set; }

        public List<DayOfWeek> Days { get; set; } = new();

        [XmlArray("ValidPeriods")]
        [XmlArrayItem("DayPeriod")]
        public List<DayPeriod> ValidPeriods { get; set; } = new() { DayPeriod.Morning, DayPeriod.Afternoon };

        public List<String> AuthorizedAgents { get; set; } = new();

        public ActivityTemplate() { }

        public ActivityTemplate(string name, IEnumerable<DayOfWeek> days, IEnumerable<String> agents, IEnumerable<DayPeriod> periods = null)
        {
            Name = name;
            Days = new List<DayOfWeek>(days);
            AuthorizedAgents = new List<String>(agents);
            ValidPeriods = periods?.ToList() ?? new() { DayPeriod.Morning, DayPeriod.Afternoon };
        }

        public ActivitySetting ToActivitySetting()
        {
            return new ActivitySetting(this.Days, this.AuthorizedAgents);
        }
    }
}
