using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SyncroTeam.Domain.Core
{
    public class CompanySettings
    {
        public String Name { get; set; }

        public String EditorPassword { get; set; }

        

        public List<ShiftTemplate> ShiftTemplates { get; set; } = new()
        {
            new ShiftTemplate(new TimeOnly(8, 0),  new TimeOnly(16, 30), 3),
            new ShiftTemplate(new TimeOnly(9, 0),  new TimeOnly(17, 30), 2),
            new ShiftTemplate(new TimeOnly(9, 30), new TimeOnly(18, 0), 2)
        };

        public List<ActivityTemplate> ActivityTemplates { get; set; } = new();

        public bool EnableSaturdayShift { get; set; } = true;

        
    }
}
