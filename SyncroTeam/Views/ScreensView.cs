using SyncroTeam.Domain.Core;
using SyncroTeam.Domain.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Printing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SyncroTeam.Views
{
    public partial class ScreensView : UserControl
    {
        private Font titleFont = new Font("Segoe UI", 16, FontStyle.Bold);
        private Font headerFont = new Font("Segoe UI", 10, FontStyle.Bold);
        private Font contentFont = new Font("Segoe UI", 10);
        private Company _company;
        private Weeks _currentWeek;

        public ScreensView()
        {
            InitializeComponent();

            this.Dock = DockStyle.Fill;
            this.BackColor = Color.White;
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
            if(_company == null || _currentWeek == null) return;

            Graphics g = e.Graphics;
            g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;

            // 📐 Dimensions
            int margin = 20;
            int colCount = 6; // 1 colonne pour les labels + 5 jours
            int rowCount = 4; // 1 ligne pour titres + 3 postes
            int cellWidth = (this.Width - margin * 2) / colCount;
            int cellHeight = (this.Height - 150) / rowCount;
            int startX = margin;
            int startY = 100;

            // 📆 Titre de la semaine
            string title = $"Vérification Écran {_currentWeek.Number} : {_currentWeek.Start:dd/MM/yyyy} - {_currentWeek.End:dd/MM/yyyy}";
            g.DrawString(title, titleFont, Brushes.Black, new PointF(margin, 30));

            // 📋 En-tête colonnes
            string[] jours = { "LUNDI", "MARDI", "MERCREDI", "JEUDI", "VENDREDI" };
            g.FillRectangle(Brushes.Gray, startX, startY, cellWidth, cellHeight);
            g.DrawRectangle(Pens.Black, startX, startY, cellWidth, cellHeight);
            g.DrawString($"{_currentWeek.Start:dd/MM/yyyy} - {_currentWeek.End:dd/MM/yyyy}", headerFont, Brushes.White, startX + 5, startY + 20);

            for(int d = 0; d < jours.Length; d++)
            {
                int x = startX + (d + 1) * cellWidth;
                g.FillRectangle(Brushes.Gainsboro, x, startY, cellWidth, cellHeight);
                g.DrawRectangle(Pens.Black, x, startY, cellWidth, cellHeight);
                g.DrawString(jours[d], headerFont, Brushes.Black, x + 20, startY + 20);
            }

            // 📌 Postes
            string[] postes = { "Console LG4", "Incident Monitoring / Samsung", "Visible Console" };
            for(int i = 0; i < postes.Length; i++)
            {
                int y = startY + (i + 1) * cellHeight;
                g.FillRectangle(Brushes.WhiteSmoke, startX, y, cellWidth, cellHeight);
                g.DrawRectangle(Pens.Black, startX, y, cellWidth, cellHeight);
                g.DrawString(postes[i], headerFont, Brushes.Black, startX + 10, y + 20);
            }

            // 👥 Données agents
            var checks = _company.GenerateMorningCheckAssignments(_currentWeek.Number);
            var daysOfWeek = new[] { DayOfWeek.Monday, DayOfWeek.Tuesday, DayOfWeek.Wednesday, DayOfWeek.Thursday, DayOfWeek.Friday };

            for(int d = 0; d < daysOfWeek.Length; d++)
            {
                var agents = checks[daysOfWeek[d]];
                for(int a = 0; a < 3; a++)
                {
                    int x = startX + (d + 1) * cellWidth;
                    int y = startY + (a + 1) * cellHeight;

                    if(_company.GetAgentByName(agents[a]) is Agent agent)
                    {
                        using var brush = new SolidBrush(agent.Color);
                        g.FillRectangle(brush, x, y, cellWidth, cellHeight);
                        g.DrawRectangle(Pens.Black, x, y, cellWidth, cellHeight);
                        g.DrawString(agent.Name, headerFont, Brushes.White, x + 10, y + 20);
                    }
                }
            }
        }

    }
}
