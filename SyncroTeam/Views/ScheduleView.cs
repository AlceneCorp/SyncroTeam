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

            // Données agents de la semaine sélectionnée
            int startY = tableTop + rowHeight;
            foreach(var shift in _currentWeek.WeeklyShifts)
            {
                int col = 0;

                // Colonne Horaires
                string horaire = $"{shift.Start.ToString()} - {shift.End.ToString()}";
                int x0 = margin + col++ * colWidth;
                g.DrawRectangle(Pens.Black, x0, startY, colWidth, rowHeight);
                g.DrawString(horaire, contentFont, Brushes.Black, x0 + 10, startY + 10);

                // Colonne Agent
                int x1 = margin + col++ * colWidth;
                Brush colorBrush = new SolidBrush(this._company.GetAgentByName(shift.Agent).Color);
                g.FillRectangle(colorBrush, x1, startY, colWidth, rowHeight);
                g.DrawRectangle(Pens.Black, x1, startY, colWidth, rowHeight);
                g.DrawString(this._company.GetAgentByName(shift.Agent).Name, contentFont, Brushes.White, x1 + 10, startY + 10);


                // Colonne Commentaire
                int x2 = margin + col * colWidth;
                g.DrawRectangle(Pens.Black, x2, startY, colWidth, rowHeight);
                g.DrawString(this.GetAgentComment(this._company.GetAgentByName(shift.Agent), _currentWeek), contentFont, Brushes.Gray, x2 + 10, startY + 10);

                startY += rowHeight;
            }
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
