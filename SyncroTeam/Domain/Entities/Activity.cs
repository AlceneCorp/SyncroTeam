using SyncroTeam.Domain.Enumerations;
using SyncroTeam.Domain.Settings;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SyncroTeam.Domain.Entities
{
    public class Activity
    {
        public string Name { get; set; }

        public ActivitySetting Settings { get; set; } = new();

        public String AssignedAgent { get; set; } // nom de l'agent affecté

        public DayOfWeek? Day { get; set; }

        public DayPeriod? Period { get; set; }

        public Activity() { }

        public Activity(string name, IEnumerable<DayOfWeek> allowedDays, IEnumerable<String> authorizedAgents)
        {
            Name = name;
            Settings = new ActivitySetting(allowedDays, authorizedAgents);
        }

        

        public override string ToString()
        {
            return Name;
        }
    }
}
