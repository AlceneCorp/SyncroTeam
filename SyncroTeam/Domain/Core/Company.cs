using SyncroTeam.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SyncroTeam.Domain.Core
{
    public class Company
    {
        public CompanySettings Settings { get; set; } = new();

        public List<Weeks> Weeks { get; set; } = new();

        public List<Agent> Agents { get; set; } = new();

        public Company() { }

        public Company(String param_Name, String param_EditorPassword)
        {
            this.Settings.Name = param_Name;
            this.Settings.EditorPassword = param_EditorPassword;
        }

        public void AssignActivitiesAutomatically()
        {
            foreach(var week in this.Weeks)
            {
                foreach(var day in week.Days)
                {
                    foreach(var slot in new[] { day.MorningPeriod, day.AfternoonPeriod })
                    {
                        if(slot.Activity == null)
                        {
                            Debug.WriteLine("Aucune activité définie pour ce créneau. Ignoré.");
                            continue;
                        }

                        var setting = slot.Activity.Settings;

                        if(!setting.IsDayAllowed(day.DayOfWeek))
                        {
                            Debug.WriteLine($"L'activité '{slot.Activity.Name}' n'est pas autorisée le {day.DayOfWeek}.");
                            continue;
                        }

                        var eligibleAgent = this.Agents.FirstOrDefault(agent => setting.IsAgentAuthorized(agent.Name));

                        if(eligibleAgent != null)
                        {
                            slot.AssignedAgent = eligibleAgent.Name;
                            Debug.WriteLine($"Agent '{eligibleAgent.Name}' affecté à l'activité '{slot.Activity.Name}' ({slot.Period}) du {day.DayOfWeek}.");
                        }
                        else
                        {
                            Debug.WriteLine($"Aucun agent autorisé disponible pour l'activité '{slot.Activity.Name}' ({slot.Period}) le {day.DayOfWeek}.");
                        }
                    }
                }
            }
        }

        public void AddAgent(Agent param_Agent)
        {
            this.Agents.Add(param_Agent);
        }

        public void AddAgents(List<Agent> param_Agents)
        {
            this.Agents.AddRange(param_Agents);
        }

        public Agent GetAgentByName(string param_Name)
        {
            if(string.IsNullOrWhiteSpace(param_Name))
            {
                throw new ArgumentException("Le nom de l'agent ne peut pas être vide ou nul.");
            }

            var agent = this.Agents?.FirstOrDefault(a => a.Name.Equals(param_Name, StringComparison.OrdinalIgnoreCase));

            if(agent == null)
            {
                throw new InvalidOperationException($"Aucun agent trouvé avec le nom : '{param_Name}'");
            }

            return agent;
        }

        public void GenerateWeeksForYear(Int32 param_Year)
        {
            DateOnly startDate = new DateOnly(param_Year, 1, 1);

            while(startDate.DayOfWeek != DayOfWeek.Monday)
            {
                startDate = startDate.AddDays(-1);
            }

            DateOnly endDate = new DateOnly(param_Year, 12, 31);

            Int32 weekNumber = 1;

            while(startDate <= endDate)
            {
                DateOnly weekEnd = startDate.AddDays(6);

                SyncroTeam.Domain.Entities.Weeks week = new SyncroTeam.Domain.Entities.Weeks(startDate, endDate, weekNumber);

                this.Weeks.Add(week);

                weekNumber++;

                startDate = startDate.AddDays(7);
            }
        }

        public Weeks GetWeekForDate(DateOnly param_Date)
        {
            return this.Weeks.FirstOrDefault(w => param_Date >= w.Start && param_Date <= w.End);
        }
    }
}
