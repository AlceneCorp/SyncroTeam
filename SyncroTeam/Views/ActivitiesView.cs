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
            var dayOfWeekFr = new[] { "Lundi", "Mardi", "Mercredi", "Jeudi", "Vendredi" };

            int panelWidth = this.Width;
            int margin = 20;
            int titleOffset = 30;
            int topOffset = 100;
            int rowHeight = 40;
            int headerHeight = 60;

            int totalCols = 1 + daysOfWeek.Length * 2;
            int firstColWidth = 300; // première colonne plus large
            int remainingWidth = panelWidth - 2 * margin - firstColWidth;
            int colWidth = remainingWidth / (totalCols - 1);

            string weekTitle = $"{_currentWeek.Start:dd/MM/yyyy} - {_currentWeek.End:dd/MM/yyyy}";
            g.DrawString($"Activités Quotidiennes {_currentWeek.Number} : {weekTitle}", titleFont, Brushes.Black, margin, 20);


            // Colonne "Activités"
            g.FillRectangle(Brushes.LightGray, margin, topOffset, firstColWidth, headerHeight);
            g.DrawRectangle(Pens.Black, margin, topOffset, firstColWidth, headerHeight);
            g.DrawString("Activités", new Font("Arial", 10, FontStyle.Bold), Brushes.Black, margin + 5, topOffset + headerHeight / 3);

            // Ligne 1 : Jours fusionnés sur 2 colonnes
            for(int d = 0; d < daysOfWeek.Length; d++)
            {
                int xDay = margin + firstColWidth + d * 2 * colWidth;
                int widthSpan = 2 * colWidth;
                g.FillRectangle(Brushes.LightGray, xDay, topOffset, widthSpan, headerHeight / 2);
                g.DrawRectangle(Pens.Black, xDay, topOffset, widthSpan, headerHeight / 2);
                g.DrawString(dayOfWeekFr[d], new Font("Arial", 12, FontStyle.Bold), Brushes.Black, xDay + 10, topOffset + 5);
            }

            // Ligne 2 : Périodes
            for(int d = 0; d < daysOfWeek.Length; d++)
            {
                for(int p = 0; p < periods.Length; p++)
                {
                    int colIndex = d * 2 + p;
                    int x = margin + firstColWidth + colIndex * colWidth;
                    string label = (periods[p] == DayPeriod.Morning) ? "Matin" : "Après-Midi";

                    g.FillRectangle(Brushes.LightGray, x, topOffset + headerHeight / 2, colWidth, headerHeight / 2);
                    g.DrawRectangle(Pens.Black, x, topOffset + headerHeight / 2, colWidth, headerHeight / 2);
                    g.DrawString(label, new Font("Arial", 8), Brushes.Black, x + 10, topOffset + headerHeight / 2 + 5);
                }
            }

            // Données
            int y = topOffset + headerHeight;
            foreach(var template in templates)
            {
                // Colonne nom activité
                g.FillRectangle(Brushes.WhiteSmoke, margin, y, firstColWidth, rowHeight);
                g.DrawRectangle(Pens.Black, margin, y, firstColWidth, rowHeight);
                g.DrawString(template.Name, new Font("Arial", 10, FontStyle.Bold), Brushes.Black, margin + 5, y + 10);

                // Colonnes jour/période
                for(int d = 0; d < daysOfWeek.Length; d++)
                {
                    for(int p = 0; p < periods.Length; p++)
                    {
                        int colIndex = d * 2 + p;
                        int x = margin + firstColWidth + colIndex * colWidth;
                        var day = _currentWeek.Days.FirstOrDefault(day => day.DayOfWeek == daysOfWeek[d]);
                        if(day == null) continue;

                        DaySlot slot = (periods[p] == DayPeriod.Morning) ? day.MorningPeriod : day.AfternoonPeriod;
                        var activity = slot?.Activities?.FirstOrDefault(a => a.Name == template.Name);

                        g.FillRectangle(Brushes.White, x, y, colWidth, rowHeight);
                        g.DrawRectangle(Pens.Black, x, y, colWidth, rowHeight);

                        if(activity?.AssignedAgent != null)
                        {
                            if(activity.AssignedAgent == "ALL")
                            {
                                g.FillRectangle(Brushes.Black, x, y, colWidth, rowHeight);
                                g.DrawString("Tout le monde", new Font("Arial", 8, FontStyle.Bold), Brushes.White, x + 5, y + 10);
                            }
                            else
                            {
                                var agent = activity.AssignedAgent;
                                var foundAgent = this._company.GetAgentByName(agent);
                                using var brush = new SolidBrush(foundAgent.Color);
                                g.FillRectangle(brush, x, y, colWidth, rowHeight);
                                g.DrawString(foundAgent.Name, new Font("Arial", 9, FontStyle.Bold), Brushes.White, x + 5, y + 10);
                            }
                        }
                    }
                }

                y += rowHeight;
            }
        }
    }
}
