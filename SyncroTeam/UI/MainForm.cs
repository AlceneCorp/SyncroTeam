using SyncroTeam.Domain.Core;
using SyncroTeam.Domain.Entities;
using SyncroTeam.Domain.Enumerations;
using SyncroTeam.Objects;
using SyncroTeam.Views;

namespace SyncroTeam
{
    public partial class MainForm : Form
    {
        private Company _company;

        private ScheduleView _scheduleView;
        private ActivitiesView _activitiesView;
        private ScreensView _screensView;

        public MainForm()
        {
            InitializeComponent();

            _scheduleView = new ScheduleView();
            _activitiesView = new ActivitiesView();
            _screensView = new ScreensView();



            if(!File.Exists("company_data.xml"))
            {
                this._company = new Company("Peach_Harem", "232323");

                this._company.Settings.InitializeDefaultFrenchHolidays(DateTime.Now.Year);

                this._company.GenerateWeeksForYear(DateTime.Now.Year);



                this._company.AddAgent(new Agent("Jordan", new TimeOnly(), new TimeOnly(), Color.Red));
                this._company.AddAgent(new Agent("Lucas", new TimeOnly(), new TimeOnly(), Color.DarkGreen));
                this._company.AddAgent(new Agent("Alexandre", new TimeOnly(), new TimeOnly(), Color.Orange));
                this._company.AddAgent(new Agent("Philippe", new TimeOnly(), new TimeOnly(), Color.Blue));
                this._company.AddAgent(new Agent("Amandine", new TimeOnly(), new TimeOnly(), Color.Violet));
                this._company.AddAgent(new Agent("Thierry", new TimeOnly(), new TimeOnly(), Color.LightGoldenrodYellow));
                this._company.AddAgent(new Agent("Emilien", new TimeOnly(), new TimeOnly(), Color.Pink));

                _company.Settings.ActivityTemplates.Add(new ActivityTemplate
                {
                    Name = "Chat",
                    Days = new List<DayOfWeek> { DayOfWeek.Monday, DayOfWeek.Tuesday, DayOfWeek.Wednesday, DayOfWeek.Thursday, DayOfWeek.Friday },
                    ValidPeriods = new List<DayPeriod> { DayPeriod.Morning, DayPeriod.Afternoon },
                    AuthorizedAgents = new List<Agent>
                    {
                        _company.GetAgentByName("Emilien"),
                        _company.GetAgentByName("Amandine"),
                        _company.GetAgentByName("Jordan"),
                        _company.GetAgentByName("Lucas"),
                        _company.GetAgentByName("Thierry"),
                        _company.GetAgentByName("Alexandre")
                    }
                });

                _company.Settings.ActivityTemplates.Add(new ActivityTemplate
                {
                    Name = "Tickets nouveaux / LED",
                    Days = new List<DayOfWeek> { DayOfWeek.Monday, DayOfWeek.Tuesday, DayOfWeek.Wednesday, DayOfWeek.Thursday, DayOfWeek.Friday },
                    ValidPeriods = new List<DayPeriod> { DayPeriod.Morning, DayPeriod.Afternoon },
                    AuthorizedAgents = _company.Agents.ToList() // Tous les agents directement
                });

                _company.Settings.ActivityTemplates.Add(new ActivityTemplate
                {
                    Name = "Onboarding",
                    Days = new List<DayOfWeek> { DayOfWeek.Monday, DayOfWeek.Wednesday },
                    ValidPeriods = new List<DayPeriod> { DayPeriod.Afternoon },
                    AuthorizedAgents = new List<Agent>
                    {
                        _company.GetAgentByName("Jordan"),
                        _company.GetAgentByName("Amandine"),
                        _company.GetAgentByName("Alexandre"),
                        _company.GetAgentByName("Emilien")
                    }
                });

                _company.Settings.ActivityTemplates.Add(new ActivityTemplate
                {
                    Name = "Havas / Passerelles",
                    Days = new List<DayOfWeek> { DayOfWeek.Monday, DayOfWeek.Tuesday, DayOfWeek.Wednesday, DayOfWeek.Thursday, DayOfWeek.Friday },
                    ValidPeriods = new List<DayPeriod> { DayPeriod.Morning },
                    AuthorizedAgents = new List<Agent>
                    {
                        _company.GetAgentByName("Thierry")
                    }
                });

                _company.Settings.ActivityTemplates.Add(new ActivityTemplate
                {
                    Name = "Mésidances et véhémence",
                    Days = new List<DayOfWeek> { DayOfWeek.Monday, DayOfWeek.Tuesday, DayOfWeek.Wednesday, DayOfWeek.Thursday, DayOfWeek.Friday },
                    ValidPeriods = new List<DayPeriod> { DayPeriod.Morning, DayPeriod.Afternoon },
                    AuthorizedAgents = new List<Agent>
                    {
                        _company.GetAgentByName("Jordan"),
                        _company.GetAgentByName("Amandine"),
                        _company.GetAgentByName("Lucas"),
                        _company.GetAgentByName("Philippe"),
                        _company.GetAgentByName("Alexandre"),
                        _company.GetAgentByName("Emilien"),
                        _company.GetAgentByName("Thierry")
                    }
                });

                _company.AssignRotatingWeeklySchedule();

                foreach(var week in _company.Weeks)
                {
                    _company.GenerateActivitiesFromTemplates(week);
                }

                _company.AssignActivitiesAutomatically();

                XmlManager.SaveObject(this._company, "company_data.xml");
            }

            this._company = XmlManager.LoadObject<Company>("company_data.xml");

            comboBoxWeeks.DataSource = _company.Weeks;
            comboBoxWeeks.DisplayMember = "ToString"; // utilise la méthode ToString() du Week


            Weeks currentWeek = _company.GetWeekForDate(DateOnly.FromDateTime(DateTime.Today));

            _scheduleView.LoadData(_company, currentWeek);

            if(currentWeek != null)
            {

                Int32 index = comboBoxWeeks.Items.IndexOf(currentWeek);

                if(index >= 0)
                {
                    comboBoxWeeks.SelectedIndex = index;
                    
                }
            }

            // Affichage par défaut
            LoadView(_scheduleView);
        }

        private void LoadView(UserControl control)
        {
            panelContent.Controls.Clear();
            panelContent.Controls.Add(control);
        }

        private void btnHoraires_Click(object sender, EventArgs e)
        {
            LoadView(_scheduleView);
        }

        private void btnActivites_Click(object sender, EventArgs e)
        {
            LoadView(_activitiesView);
        }

        private void btnEcrans_Click(object sender, EventArgs e)
        {
            LoadView(_screensView);
        }

        private void comboBoxWeeks_SelectedIndexChanged(object sender, EventArgs e)
        {
            if(comboBoxWeeks.SelectedItem is Weeks selectedWeek)
            {
                _scheduleView.LoadWeek(selectedWeek);
            }
        }
    }
}
