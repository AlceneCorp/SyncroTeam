using SyncroTeam.Domain.Core;
using SyncroTeam.Domain.Entities;
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
    public partial class ScheduleView : UserControl
    {
        private Font titleFont = new Font("Segoe UI", 16, FontStyle.Bold);
        private Font headerFont = new Font("Segoe UI", 10, FontStyle.Bold);
        private Font contentFont = new Font("Segoe UI", 10);
        private Company _company;
        private Weeks _currentWeek;

        public ScheduleView()
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

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            if(_company == null || _currentWeek == null) return;

            Graphics g = e.Graphics;
            g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;

            int panelWidth = this.Width;
            int panelHeight = this.Height;

            int margin = 20;
            int topOffset = 20;
            int titleHeight = 50;
            int rowHeight = 40;

            int tableTop = topOffset + titleHeight + margin;

            int numCols = 3;
            int colWidth = (panelWidth - 2 * margin) / numCols;

            // Titre semaine
            string weekTitle = $"{_currentWeek.Start:dd/MM/yyyy} - {_currentWeek.End:dd/MM/yyyy}";
            g.DrawString($"Horaires Semaine {_currentWeek.Number} : {weekTitle}", titleFont, Brushes.Black, margin, topOffset);

            // En-têtes colonnes
            string[] headers = { "Horaires", "Agents", "Commentaires" };
            for(int col = 0; col < numCols; col++)
            {
                int x = margin + col * colWidth;
                g.FillRectangle(Brushes.LightGray, x, tableTop, colWidth, rowHeight);
                g.DrawRectangle(Pens.Black, x, tableTop, colWidth, rowHeight);
                g.DrawString(headers[col], headerFont, Brushes.Black, x + 10, tableTop + 10);
            }

            // Données agents
            int startY = tableTop + rowHeight;
            foreach(var shift in _currentWeek.WeeklyShifts)
            {
                // Horaires classiques de la semaine (hors samedi)
                if(shift.Start != new TimeOnly(8, 0) || shift.End != new TimeOnly(12, 0))
                {
                    DrawShiftRow(g, shift, margin, colWidth, startY);
                    startY += rowHeight;
                }
            }

            // Ajout ligne spéciale "Samedi"
            var saturdayShift = _currentWeek.WeeklyShifts
                .FirstOrDefault(s => s.Start == new TimeOnly(8, 0) && s.End == new TimeOnly(12, 0));

            if(saturdayShift != null)
            {
                int col = 0;
                int x0 = margin + col++ * colWidth;
                int x1 = margin + col++ * colWidth;
                int x2 = margin + col * colWidth;

                // Horaires
                g.DrawRectangle(Pens.Black, x0, startY, colWidth, rowHeight);
                g.DrawString("Samedi 08h00 - 12h00", contentFont, Brushes.Black, x0 + 10, startY + 10);

                // Agent
                Brush colorBrush = new SolidBrush(_company.GetAgentByName(saturdayShift.Agent).Color);
                g.FillRectangle(colorBrush, x1, startY, colWidth, rowHeight);
                g.DrawRectangle(Pens.Black, x1, startY, colWidth, rowHeight);
                g.DrawString(saturdayShift.Agent, contentFont, Brushes.White, x1 + 10, startY + 10);

                // Commentaire
                g.DrawRectangle(Pens.Black, x2, startY, colWidth, rowHeight);
                g.DrawString("-", contentFont, Brushes.DarkRed, x2 + 10, startY + 10);
            }
        }

        // Utilitaire : Dessin d’un shift normal
        private void DrawShiftRow(Graphics g, WeeklyShift shift, int margin, int colWidth, int y)
        {
            int col = 0;
            int x0 = margin + col++ * colWidth;
            int x1 = margin + col++ * colWidth;
            int x2 = margin + col * colWidth;

            // Colonne Horaires
            string horaire = $"{shift.Start:hh\\:mm} - {shift.End:hh\\:mm}";
            g.DrawRectangle(Pens.Black, x0, y, colWidth, 40);
            g.DrawString(horaire, contentFont, Brushes.Black, x0 + 10, y + 10);

            // Colonne Agent
            Brush colorBrush = new SolidBrush(_company.GetAgentByName(shift.Agent).Color);
            g.FillRectangle(colorBrush, x1, y, colWidth, 40);
            g.DrawRectangle(Pens.Black, x1, y, colWidth, 40);
            g.DrawString(shift.Agent, contentFont, Brushes.White, x1 + 10, y + 10);

            // Colonne Commentaire
            g.DrawRectangle(Pens.Black, x2, y, colWidth, 40);
            g.DrawString(this.GetAgentComment(_company.GetAgentByName(shift.Agent), _currentWeek), contentFont, Brushes.Gray, x2 + 10, y + 10);
        }

        public void LoadWeek(Weeks week)
        {
            _currentWeek = week;
            this.Invalidate(); // Redessine le contrôle
        }

        private string GetAgentComment(Agent agent, Weeks week)
        {
            var comments = new List<string>();

            if(agent.AbsentDays != null)
            {
                var absences = agent.AbsentDays
                    .Where(d => d >= week.Start && d <= week.End)
                    .Select(d => d.ToString("dd/MM"));

                if(absences.Any())
                    comments.Add("En congés le " + string.Join(", ", absences));
            }

            if(agent.HasManualSchedule)
                comments.Add("Horaire manuel");

            if(agent.IsLockedToTask)
                comments.Add("Affectation fixe");

            return comments.Count > 0 ? string.Join(" | ", comments) : "-";
        }
    }
}
