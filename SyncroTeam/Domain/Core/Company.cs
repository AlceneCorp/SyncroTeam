using SyncroTeam.Domain.Entities;
using SyncroTeam.Domain.Enumerations;
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
                    var currentDate = week.Start.AddDays((int)day.DayOfWeek - (int)DayOfWeek.Monday);
                    if(Settings.IsPublicHoliday(currentDate))
                    {
                        // Skip assignment for this holiday
                        continue;
                    }

                    HashSet<string> alreadyAssignedToday = new();

                    foreach(var slot in new[] { day.MorningPeriod, day.AfternoonPeriod })
                    {
                        if(slot.Activities == null || slot.Activities.Count == 0)
                            continue;

                        foreach(var activity in slot.Activities)
                        {
                            if(activity.AssignedAgent != null && slot.IsManualOverride)
                                continue;

                            foreach(var agent in this.Agents)
                            {
                                if(activity.Settings.IsAgentAuthorized(agent.Name) && !alreadyAssignedToday.Contains(agent.Name))
                                {
                                    activity.AssignedAgent = agent;
                                    alreadyAssignedToday.Add(agent.Name);
                                    break;
                                }
                            }
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

                if(weekEnd > endDate)
                    weekEnd = endDate;

                var week = new SyncroTeam.Domain.Entities.Weeks(startDate, weekEnd, weekNumber);

                this.Weeks.Add(week);

                weekNumber++;
                startDate = startDate.AddDays(7);
            }
        }

        public Weeks GetWeekForDate(DateOnly param_Date)
        {
            return this.Weeks.FirstOrDefault(w => param_Date >= w.Start && param_Date <= w.End);
        }

        public void AssignRotatingWeeklySchedule()
        {
            for(int weekIndex = 0; weekIndex < Weeks.Count; weekIndex++)
            {
                var week = Weeks[weekIndex];
                var rotatedAgents = RotateAgents(Agents, weekIndex);
                int agentCursor = 0;

                foreach(var shift in Settings.ShiftTemplates)
                {
                    for(int i = 0; i < shift.AgentCount && agentCursor < rotatedAgents.Count; i++, agentCursor++)
                    {
                        var agent = rotatedAgents[agentCursor];

                        week.WeeklyShifts.Add(new WeeklyShift
                        {
                            Agent = agent,
                            Start = shift.Start,
                            End = shift.End
                        });

                        foreach(var day in week.Days)
                        {
                            bool isSaturday = day.DayOfWeek == DayOfWeek.Saturday;
                            if(isSaturday && (!Settings.EnableSaturdayShift || agentCursor != 0))
                                continue;

                            // Affectation à toutes les activités du créneau
                            foreach(var slot in new[] { day.MorningPeriod, day.AfternoonPeriod })
                            {
                                if(slot.Activities != null)
                                {
                                    foreach(var activity in slot.Activities)
                                    {
                                        if(activity.AssignedAgent == null)
                                        {
                                            activity.AssignedAgent = agent;
                                            break;
                                        }
                                    }
                                }
                            }
                        }
                    }
                }

                var saturday = week.Days.FirstOrDefault(d => d.DayOfWeek == DayOfWeek.Saturday);
                if(Settings.EnableSaturdayShift && saturday != null)
                {
                    if(saturday.MorningPeriod.Activities != null)
                    {
                        foreach(var activity in saturday.MorningPeriod.Activities)
                        {
                            if(activity.AssignedAgent == null)
                            {
                                activity.AssignedAgent = rotatedAgents[0];
                                break;
                            }
                        }
                    }

                    // Aucune activité prévue pour l'après-midi le samedi
                    if(saturday.AfternoonPeriod?.Activities != null)
                        saturday.AfternoonPeriod.Activities.Clear();
                }
            }
        }

        private List<Agent> RotateAgents(List<Agent> param_agents, int param_shift)
        {
            int actualShift = param_shift % param_agents.Count;
            return param_agents.Skip(actualShift).Concat(param_agents.Take(actualShift)).ToList();
        }

        public Weeks GetWeekByString(String param_weekText)
        {
            return Weeks.FirstOrDefault(w => w.ToString() == param_weekText);
        }

        public Weeks GetWeekByNumber(int number) => this.Weeks.FirstOrDefault(w => w.Number == number);


        public void GenerateActivitiesFromTemplates(Weeks week)
        {
            foreach(var day in week.Days)
            {
                var currentDate = week.Start.AddDays((int)day.DayOfWeek - (int)DayOfWeek.Monday);
                if(Settings.IsPublicHoliday(currentDate))
                {
                    // Skip generation for this holiday
                    continue;
                }

                foreach(var template in Settings.ActivityTemplates)
                {
                    if(!template.Days.Contains(day.DayOfWeek))
                        continue;

                    foreach(var period in template.ValidPeriods)
                    {
                        DaySlot slot = (period == DayPeriod.Morning) ? day.MorningPeriod : day.AfternoonPeriod;

                        slot.Activities ??= new List<SyncroTeam.Domain.Entities.Activity>();

                        slot.Activities.Add(new SyncroTeam.Domain.Entities.Activity
                        {
                            Name = template.Name,
                            Settings = template.ToActivitySetting(),
                            Day = day.DayOfWeek,
                            Period = period
                        });
                    }
                }
            }
        }

        private SyncroTeam.Domain.Entities.Activity CloneActivity(SyncroTeam.Domain.Entities.Activity original)
        {
            return new SyncroTeam.Domain.Entities.Activity
            {
                Name = original.Name,
                Settings = new ActivitySetting(new List<DayOfWeek>(original.Settings.AllowedDays), new List<Agent>(original.Settings.AuthorizedAgents))
            };
        }

        public void AddActivityTemplate(string name, IEnumerable<DayOfWeek> days, IEnumerable<Agent> authorizedAgents)
        {
            if(string.IsNullOrWhiteSpace(name))
                throw new ArgumentException("Le nom de l'activité ne peut pas être vide.");

            if(days == null || !days.Any())
                throw new ArgumentException("L'activité doit être assignée à au moins un jour.");

            if(authorizedAgents == null)
                throw new ArgumentException("La liste d'agents autorisés ne peut pas être nulle.");

            var exists = Settings.ActivityTemplates.Any(a =>
                a.Name.Equals(name, StringComparison.OrdinalIgnoreCase) &&
                a.Days.SequenceEqual(days));

            if(exists)
            {
                Debug.WriteLine($"L'activité '{name}' avec les jours spécifiés existe déjà.");
                return;
            }

            Settings.ActivityTemplates.Add(new ActivityTemplate(name, days, authorizedAgents));
        }

        public bool RemoveActivityTemplate(string name)
        {
            var toRemove = Settings.ActivityTemplates
                .FirstOrDefault(a => a.Name.Equals(name, StringComparison.OrdinalIgnoreCase));

            if(toRemove != null)
            {
                Settings.ActivityTemplates.Remove(toRemove);
                return true;
            }

            return false;
        }

        public bool UpdateActivityTemplate(string name, IEnumerable<DayOfWeek> newDays, IEnumerable<Agent> newAgents)
        {
            var existing = Settings.ActivityTemplates
                .FirstOrDefault(a => a.Name.Equals(name, StringComparison.OrdinalIgnoreCase));

            if(existing != null)
            {
                existing.Days = newDays?.ToList() ?? new();
                existing.AuthorizedAgents = newAgents?.ToList() ?? new();
                return true;
            }

            return false;
        }
    }
}
