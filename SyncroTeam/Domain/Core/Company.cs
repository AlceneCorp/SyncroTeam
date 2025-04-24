using SyncroTeam.Domain.Entities;
using SyncroTeam.Domain.Enumerations;
using SyncroTeam.Domain.Settings;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace SyncroTeam.Domain.Core
{
    [TypeConverter(typeof(ExpandableObjectConverter))]
    public class Company
    {
        public CompanySettings Settings { get; set; } = new();

        public List<Weeks> Weeks { get; set; } = new();

        public List<Agent> Agents { get; set; } = new();

        public Company() { }

        public List<DateOnly> PublicHolidays { get; set; } = new();

        private Dictionary<string, int> _globalRotationIndexes = new();

        private Dictionary<string, int> _activityRotations = new();

        public Company(String param_Name, String param_EditorPassword)
        {
            this.Settings.Name = param_Name;
            this.Settings.EditorPassword = param_EditorPassword;
        }


        public void AssignActivitiesAutomatically()
        {
            Dictionary<string, int> globalRotationIndexes = new();

            foreach(var week in this.Weeks)
            {
                Dictionary<string, Dictionary<string, int>> weeklyAssignmentCount = new();

                var allActivities = week.Days
                    .SelectMany(d => new[] { d.MorningPeriod, d.AfternoonPeriod })
                    .Where(slot => slot?.Activities != null)
                    .SelectMany(slot => slot.Activities)
                    .Where(a => a.Settings != null)
                    .GroupBy(a => a.Name);

                foreach(var activityGroup in allActivities)
                {
                    string activityName = activityGroup.Key;
                    var activities = activityGroup.ToList();

                    var template = this.Settings.ActivityTemplates.FirstOrDefault(t => t.Name == activityName);
                    if(template == null)
                        continue;

                    bool isForEveryone = template.AuthorizedAgents == null;

                    List<Agent> eligibleAgents = isForEveryone
                        ? this.Agents
                        : this.Agents.Where(a => template.AuthorizedAgents.Contains(a.Name)).ToList();

                    if(eligibleAgents.Count == 0)
                        continue;

                    if(!weeklyAssignmentCount.ContainsKey(activityName))
                        weeklyAssignmentCount[activityName] = new Dictionary<string, int>();

                    if(!globalRotationIndexes.ContainsKey(activityName))
                        globalRotationIndexes[activityName] = 0;

                    int agentIndex = globalRotationIndexes[activityName];

                    foreach(var activity in activities)
                    {
                        if(isForEveryone)
                        {
                            activity.AssignedAgent = "ALL";
                            continue;
                        }

                        Agent assignedAgent = null;
                        int attempts = 0;

                        while(attempts < eligibleAgents.Count)
                        {
                            var agent = eligibleAgents[agentIndex % eligibleAgents.Count];
                            var agentName = agent.Name;

                            int currentCount = weeklyAssignmentCount[activityName].ContainsKey(agentName)
                                ? weeklyAssignmentCount[activityName][agentName]
                                : 0;

                            if(!template.MaxOccurrencesPerAgentPerWeek.HasValue ||
                                currentCount < template.MaxOccurrencesPerAgentPerWeek.Value)
                            {
                                assignedAgent = agent;
                                break;
                            }

                            agentIndex++;
                            attempts++;
                        }

                        if(assignedAgent != null)
                        {
                            activity.AssignedAgent = assignedAgent.Name;

                            if(!weeklyAssignmentCount[activityName].ContainsKey(assignedAgent.Name))
                                weeklyAssignmentCount[activityName][assignedAgent.Name] = 0;

                            weeklyAssignmentCount[activityName][assignedAgent.Name]++;
                            agentIndex++;
                        }
                        else
                        {
                            activity.AssignedAgent = null;
                        }
                    }

                    globalRotationIndexes[activityName] = agentIndex % eligibleAgents.Count;
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

        public Dictionary<DayOfWeek, List<string>> GenerateMorningCheckAssignments(int weekNumber)
        {
            var result = new Dictionary<DayOfWeek, List<string>>();
            var days = new[] { DayOfWeek.Monday, DayOfWeek.Tuesday, DayOfWeek.Wednesday, DayOfWeek.Thursday, DayOfWeek.Friday };

            // Récupère les agents éligibles pour les shifts de 08h00 à 16h30
            var eligibleAgents = Weeks
                .FirstOrDefault()?.WeeklyShifts
                .Where(ws => ws.Start.Hour == 8 && ws.End.Hour == 16)
                .Select(ws => ws.Agent)
                .Distinct()
                .ToList() ?? new List<string>();

            if(eligibleAgents.Count < 3)
            {
                throw new InvalidOperationException("Pas assez d'agents à 08h00 pour planifier les checks.");
            }

            // Applique un roulement basé sur le numéro de la semaine
            var rotatedAgents = eligibleAgents
                .Skip(weekNumber % eligibleAgents.Count)
                .Concat(eligibleAgents.Take(weekNumber % eligibleAgents.Count))
                .ToList();

            // Prépare les compteurs d’assignation
            var assignmentCount = rotatedAgents.ToDictionary(agent => agent, agent => 0);
            int agentIndex = 0;

            foreach(var day in days)
            {
                var assignedToday = new List<string>();

                for(int i = 0; i < 3; i++)
                {
                    // Trouve l'agent avec le moins d'assignations (et pas encore pris aujourd'hui)
                    var next = rotatedAgents
                        .Where(a => !assignedToday.Contains(a))
                        .OrderBy(a => assignmentCount[a])
                        .ThenBy(a => rotatedAgents.IndexOf(a)) // stabilité du tri
                        .First();

                    assignedToday.Add(next);
                    assignmentCount[next]++;
                }

                result[day] = assignedToday;
            }

            return result;
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
                    // Ignorer le shift du samedi ici (on le traite plus bas)
                    if(shift.AgentCount == 1 && shift.End.Hour == 12)
                        continue;

                    for(int i = 0; i < shift.AgentCount && agentCursor < rotatedAgents.Count; i++, agentCursor++)
                    {
                        var agent = rotatedAgents[agentCursor];

                        week.WeeklyShifts.Add(new WeeklyShift
                        {
                            Agent = agent.Name,
                            Start = shift.Start,
                            End = shift.End
                        });

                        foreach(var day in week.Days)
                        {
                            bool isSaturday = day.DayOfWeek == DayOfWeek.Saturday;
                            if(isSaturday && (!Settings.EnableSaturdayShift || agentCursor != 0))
                                continue;

                            foreach(var slot in new[] { day.MorningPeriod, day.AfternoonPeriod })
                            {
                                if(slot.Activities != null)
                                {
                                    foreach(var activity in slot.Activities)
                                    {
                                        if(activity.AssignedAgent == null)
                                        {
                                            activity.AssignedAgent = agent.Name;
                                            break;
                                        }
                                    }
                                }
                            }
                        }
                    }
                }

                // Traitement spécifique du shift du samedi
                if(Settings.EnableSaturdayShift)
                {
                    var saturdayShift = Settings.ShiftTemplates.FirstOrDefault(s => s.AgentCount == 1 && s.End.Hour == 12);
                    if(saturdayShift != null)
                    {
                        var saturday = week.Days.FirstOrDefault(d => d.DayOfWeek == DayOfWeek.Saturday);
                        if(saturday != null)
                        {
                            var agent = week.WeeklyShifts.FirstOrDefault(w => w.Start == saturdayShift.Start && w.End == saturdayShift.End)?.Agent
                                        ?? rotatedAgents[0].Name;

                            week.WeeklyShifts.Add(new WeeklyShift
                            {
                                Agent = agent,
                                Start = saturdayShift.Start,
                                End = saturdayShift.End
                            });

                            if(saturday.MorningPeriod.Activities != null)
                            {
                                foreach(var activity in saturday.MorningPeriod.Activities)
                                {
                                    if(activity.AssignedAgent == null)
                                    {
                                        activity.AssignedAgent = agent;
                                        break;
                                    }
                                }
                            }

                            // Supprimer les activités de l'après-midi le samedi
                            saturday.AfternoonPeriod?.Activities?.Clear();
                        }
                    }
                }
            }
        }


        private List<Agent> RotateAgents(List<Agent> param_agents, int param_shift)
        {
            int actualShift = param_shift % param_agents.Count;
            return param_agents.Skip(param_agents.Count - actualShift).Concat(param_agents.Take(param_agents.Count - actualShift)).ToList();
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
                foreach(var template in Settings.ActivityTemplates)
                {
                    // On passe les activités non prévues ce jour-là
                    if(!template.Days.Contains(day.DayOfWeek))
                        continue;

                    foreach(var period in template.ValidPeriods)
                    {
                        // Sélectionne le bon slot (matin/après-midi)
                        DaySlot slot = (period == DayPeriod.Morning) ? day.MorningPeriod : day.AfternoonPeriod;

                        if(slot.Activities == null)
                            slot.Activities = new List<SyncroTeam.Domain.Entities.Activity>();

                        // Évite de créer deux fois la même activité par erreur
                        if(slot.Activities.Any(a => a.Name == template.Name))
                            continue;

                        // Création unique de l'activité
                        slot.Activities.Add(new SyncroTeam.Domain.Entities.Activity
                        {
                            Name = template.Name,
                            Settings = new ActivitySetting(template.Days, template.AuthorizedAgents),
                            Day = day.DayOfWeek,
                            Period = period,
                            AssignedAgent = null // on le remplira plus tard
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
                Settings = new ActivitySetting(new List<DayOfWeek>(original.Settings.AllowedDays), new List<String>(original.Settings.AuthorizedAgents))
            };
        }

        public void AddActivityTemplate(string name, IEnumerable<DayOfWeek> days, IEnumerable<String> authorizedAgents)
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

        public bool UpdateActivityTemplate(string name, IEnumerable<DayOfWeek> newDays, IEnumerable<String> newAgents)
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

        public bool IsPublicHoliday(DateOnly date)
        {
            return PublicHolidays.Contains(date);
        }

        private DateOnly GetEasterMonday(int year) => DateOnly.FromDateTime(GetEasterDate(year).AddDays(1));
        private DateOnly GetAscension(int year) => DateOnly.FromDateTime(GetEasterDate(year).AddDays(39));
        private DateOnly GetWhitMonday(int year) => DateOnly.FromDateTime(GetEasterDate(year).AddDays(50));



        public List<DateOnly> GetFrenchPublicHolidays(int year)
        {
            List<DateOnly> holidays = new()
            {
                new DateOnly(year, 1, 1),    // Jour de l'an
                new DateOnly(year, 5, 1),    // Fête du travail
                new DateOnly(year, 5, 8),    // Victoire 1945
                new DateOnly(year, 7, 14),   // Fête nationale
                new DateOnly(year, 8, 15),   // Assomption
                new DateOnly(year, 11, 1),   // Toussaint
                new DateOnly(year, 11, 11),  // Armistice
                new DateOnly(year, 12, 25),  // Noël
            };

            

            // Calculs basés sur la date de Pâques
            DateTime easter = GetEasterDate(year);

            holidays.Add(DateOnly.FromDateTime(easter.AddDays(1)));   // Lundi de Pâques
            holidays.Add(DateOnly.FromDateTime(easter.AddDays(39)));  // Ascension
            holidays.Add(DateOnly.FromDateTime(easter.AddDays(50)));  // Lundi de Pentecôte

            return holidays;
        }

        private DateTime GetEasterDate(int year)
        {
            // Algorithme de Meeus/Jones/Butcher pour Pâques
            int a = year % 19;
            int b = year / 100;
            int c = year % 100;
            int d = b / 4;
            int e = b % 4;
            int f = (b + 8) / 25;
            int g = (b - f + 1) / 3;
            int h = (19 * a + b - d - g + 15) % 30;
            int i = c / 4;
            int k = c % 4;
            int l = (32 + 2 * e + 2 * i - h - k) % 7;
            int m = (a + 11 * h + 22 * l) / 451;
            int month = (h + l - 7 * m + 114) / 31;
            int day = ((h + l - 7 * m + 114) % 31) + 1;

            return new DateTime(year, month, day);
        }

        public void InitializeDefaultFrenchHolidays(int year)
        {
            List<DateOnly> holidays = GetFrenchPublicHolidays(year);

            this.Agents.ForEach(agent =>
            {
                agent.AbsentDays ??= new List<DateOnly>();
                agent.AbsentDays.AddRange(holidays);
            });
        }
    }
}
