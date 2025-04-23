using SyncroTeam.Domain.Core;
using SyncroTeam.Domain.Entities;
using SyncroTeam.Domain.Enumerations;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SyncroTeam.Views
{
    public partial class ActivitiesView : UserControl
    {

        private Font titleFont = new Font("Segoe UI", 16, FontStyle.Bold);
        private Font headerFont = new Font("Segoe UI", 10, FontStyle.Bold);
        private Font contentFont = new Font("Segoe UI", 10);
        private Company _company;
        private Weeks _currentWeek;

        public ActivitiesView()
        {
            InitializeComponent();

            this.Dock = DockStyle.Fill;
            this.BackColor = Color.White;

            this.DoubleBuffered = true;
            this.SetStyle(ControlStyles.OptimizedDoubleBuffer, true);
            this.SetStyle(ControlStyles.UserPaint, true);
            this.SetStyle(ControlStyles.AllPaintingInWmPaint, true);
            this.UpdateStyles();
        }

        public void LoadData(Company company, Weeks week)
        {
            _company = company;
            _currentWeek = week;
            this.Invalidate(); // Redessine
        }

        public void LoadWeek(Weeks week)
        {
            _currentWeek = week;
            this.Invalidate(); // Redessine le contrôle
        }


        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            Graphics g = e.Graphics;

            if(_company == null || _currentWeek == null) return;

            var templates = _company.Settings.ActivityTemplates;
            var daysOfWeek = new[] { DayOfWeek.Monday, DayOfWeek.Tuesday, DayOfWeek.Wednesday, DayOfWeek.Thursday, DayOfWeek.Friday };
            var periods = new[] { DayPeriod.Morning, DayPeriod.Afternoon };

            int panelWidth = this.Width;
            int margin = 20;
            int titleOffset = 30;
            int topOffset = 100;
            int rowHeight = 40;
            int headerHeight = 50;

            int totalCols = 1 + daysOfWeek.Length * periods.Length;
            int colWidth = (panelWidth - 2 * margin) / totalCols;

            // Titre
            string title = $"ACTIVITÉS QUOTIDIENNES - Semaine {_currentWeek.Number} : {_currentWeek.Start:dd/MM/yyyy} - {_currentWeek.End:dd/MM/yyyy}";
            g.DrawString(title, new Font("Arial", 16, FontStyle.Bold), Brushes.Black, margin, titleOffset);

            // En-têtes colonnes
            g.FillRectangle(Brushes.LightGray, margin, topOffset, colWidth, headerHeight);
            g.DrawRectangle(Pens.Black, margin, topOffset, colWidth, headerHeight);
            g.DrawString("Activités", new Font("Arial", 10, FontStyle.Bold), Brushes.Black, margin + 5, topOffset + 15);

            List<String> DayOfWeeksString = new() { "Lundi", "Mardi", "Mercredi", "Jeudi", "Vendredi", "Samedi" };

            for(int d = 0; d < daysOfWeek.Length; d++)
            {
                for(int p = 0; p < periods.Length; p++)
                {
                    int colIndex = 1 + d * 2 + p;
                    int x = margin + colIndex * colWidth;
                    string text = $"{DayOfWeeksString[d]}\n{(periods[p] == DayPeriod.Morning ? "Matin" : "Après-Midi")}";
                    g.FillRectangle(Brushes.LightGray, x, topOffset, colWidth, headerHeight);
                    g.DrawRectangle(Pens.Black, x, topOffset, colWidth, headerHeight);
                    g.DrawString(text, new Font("Arial", 8, FontStyle.Bold), Brushes.Black, x + 5, topOffset + 10);
                }
            }

            // Données
            int y = topOffset + headerHeight;
            foreach(var template in templates)
            {
                g.FillRectangle(Brushes.WhiteSmoke, margin, y, colWidth, rowHeight);
                g.DrawRectangle(Pens.Black, margin, y, colWidth, rowHeight);
                g.DrawString(template.Name, new Font("Arial", 10, FontStyle.Bold), Brushes.Black, margin + 5, y + 10);

                for(int d = 0; d < daysOfWeek.Length; d++)
                {
                    for(int p = 0; p < periods.Length; p++)
                    {
                        int colIndex = 1 + d * 2 + p;
                        int x = margin + colIndex * colWidth;
                        var day = _currentWeek.Days.FirstOrDefault(day => day.DayOfWeek == daysOfWeek[d]);
                        if(day == null) continue;

                        DaySlot slot = (periods[p] == DayPeriod.Morning) ? day.MorningPeriod : day.AfternoonPeriod;
                        var activity = slot?.Activities?.FirstOrDefault(a => a.Name == template.Name);

                        g.FillRectangle(Brushes.White, x, y, colWidth, rowHeight);
                        g.DrawRectangle(Pens.Black, x, y, colWidth, rowHeight);

                        if(activity?.AssignedAgent != null)
                        {
                            var agent = activity.AssignedAgent;
                            using var brush = new SolidBrush(agent.Color);
                            g.FillRectangle(brush, x, y, colWidth, rowHeight);
                            g.DrawString(agent.Name, new Font("Arial", 9, FontStyle.Bold), Brushes.White, x + 5, y + 10);
                        }
                        else if(template.AuthorizedAgents.Count == _company.Agents.Count)
                        {
                            g.FillRectangle(Brushes.DimGray, x, y, colWidth, rowHeight);
                            g.DrawString("Tout le monde", new Font("Arial", 8, FontStyle.Bold), Brushes.White, x + 5, y + 10);
                        }
                    }
                }

                y += rowHeight;
            }
        }

    }
}
