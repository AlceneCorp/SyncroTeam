using SyncroTeam.Domain.Enumerations;
using SyncroTeam.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SyncroTeam.Domain.Core
{
    public class ActivityTemplate
    {
        public string Name { get; set; }

        public List<DayOfWeek> Days { get; set; } = new();

        public List<DayPeriod> ValidPeriods { get; set; } = new() { DayPeriod.Morning, DayPeriod.Afternoon };

        public List<Agent> AuthorizedAgents { get; set; } = new();

        public ActivityTemplate() { }

        public ActivityTemplate(string name, IEnumerable<DayOfWeek> days, IEnumerable<Agent> agents, IEnumerable<DayPeriod> periods = null)
        {
            Name = name;
            Days = new List<DayOfWeek>(days);
            AuthorizedAgents = new List<Agent>(agents);
            ValidPeriods = periods?.ToList() ?? new() { DayPeriod.Morning, DayPeriod.Afternoon };
        }

        public ActivitySetting ToActivitySetting()
        {
            return new ActivitySetting(this.Days, this.AuthorizedAgents);
        }
    }
}
