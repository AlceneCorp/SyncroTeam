﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SyncroTeam.Domain.Entities
{
    [TypeConverter(typeof(ExpandableObjectConverter))]
    public class Weeks
    {

        public DateOnly Start {  get; set; }

        public DateOnly End { get; set; }

        public Int32 Number {  get; set; }

        public List<Days> Days { get; set; } = new();

        public Weeks() { }

        public List<WeeklyShift> WeeklyShifts { get; set; } = new();

        public Weeks(DateOnly param_Start, DateOnly param_End, Int32 param_Number)
        {
            Start = param_Start;
            End = param_End;
            Number = param_Number;

            this.InitializeWeekDays();
        }

        /// <summary>
        /// Initialise automatiquement les jours de la semaine : du lundi au samedi
        /// </summary>
        private void InitializeWeekDays()
        {
            this.Days.Clear();
            for(int i = 0; i < 6; i++) // Lundi (0) à Samedi (5)
            {
                var dayOfWeek = (DayOfWeek)(((int)DayOfWeek.Monday + i) % 7);
                Days.Add(new Days(dayOfWeek));
            }
        }

        public override string ToString() => $"{Start:dd/MM/yyyy} - {End:dd/MM/yyyy}";

        
        
    }
}
