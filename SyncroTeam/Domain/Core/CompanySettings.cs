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

        public List<DateOnly> PublicHolidays { get; set; } = new();

        public List<ShiftTemplate> ShiftTemplates { get; set; } = new()
        {
            new ShiftTemplate(new TimeOnly(8, 0),  new TimeOnly(16, 30), 3),
            new ShiftTemplate(new TimeOnly(9, 0),  new TimeOnly(17, 30), 2),
            new ShiftTemplate(new TimeOnly(9, 30), new TimeOnly(18, 0), 2)
        };

        public List<ActivityTemplate> ActivityTemplates { get; set; } = new();

        public bool EnableSaturdayShift { get; set; } = true;

        public bool IsPublicHoliday(DateOnly date)
        {
            return PublicHolidays.Contains(date);
        }


        public static List<DateOnly> GetFrenchPublicHolidays(int year)
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

        private static DateTime GetEasterDate(int year)
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
            this.PublicHolidays = GetFrenchPublicHolidays(year);
        }
    }
}
