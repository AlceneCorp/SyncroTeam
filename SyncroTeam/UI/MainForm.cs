using SyncroTeam.Domain.Core;
using SyncroTeam.Domain.Entities;
using SyncroTeam.Domain.Enumerations;
using SyncroTeam.Objects;
using SyncroTeam.UI.Debug;
using SyncroTeam.Views;
using System.Diagnostics;

namespace SyncroTeam
{
    public partial class MainForm : Form
    {

        private DebugForm _debugForm;

        private Company _company;

        private ScheduleView _scheduleView;
        private ActivitiesView _activitiesView;
        private ScreensView _screensView;

        private Weeks _currentWeek;

        public MainForm()
        {
            InitializeComponent();

            _scheduleView = new ScheduleView();
            _activitiesView = new ActivitiesView();
            _screensView = new ScreensView();





            if(!File.Exists("company_data.xml"))
            {
                this._company = new Company("Peach_Harem", "232323");

                this._company.AddAgent(new Agent("Jordan", Color.Red));
                this._company.AddAgent(new Agent("Lucas", Color.DarkGreen));
                this._company.AddAgent(new Agent("Alexandre", Color.Orange));
                this._company.AddAgent(new Agent("Philippe", Color.Blue));
                this._company.AddAgent(new Agent("Amandine", Color.Violet));
                this._company.AddAgent(new Agent("Thierry", Color.Chocolate));
                this._company.AddAgent(new Agent("Emilien", Color.Pink));

                this._company.InitializeDefaultFrenchHolidays(DateTime.Now.Year);

                this._company.GenerateWeeksForYear(DateTime.Now.Year);





                _company.Settings.ActivityTemplates.Add(new ActivityTemplate
                {
                    Name = "Chat",
                    Days = new List<DayOfWeek> { DayOfWeek.Monday, DayOfWeek.Tuesday, DayOfWeek.Wednesday, DayOfWeek.Thursday, DayOfWeek.Friday },
                    ValidPeriods = new List<DayPeriod> { DayPeriod.Morning, DayPeriod.Afternoon },
                    MaxOccurrencesPerAgentPerWeek = 2,
                    AuthorizedAgents = new List<String>
                    {
                        "Emilien",
                        "Amandine",
                        "Jordan",
                        "Lucas",
                        "Philippe",
                        "Alexandre"
                    }
                });

                _company.Settings.ActivityTemplates.Add(new ActivityTemplate
                {
                    Name = "Tickets nouveaux / LED",
                    Days = new List<DayOfWeek> { DayOfWeek.Monday, DayOfWeek.Tuesday, DayOfWeek.Wednesday, DayOfWeek.Thursday, DayOfWeek.Friday },
                    ValidPeriods = new List<DayPeriod> { DayPeriod.Morning, DayPeriod.Afternoon },
                    AuthorizedAgents = null //Tout le monde
                });

                _company.Settings.ActivityTemplates.Add(new ActivityTemplate
                {
                    Name = "Onboarding",
                    Days = new List<DayOfWeek> { DayOfWeek.Monday, DayOfWeek.Wednesday },
                    ValidPeriods = new List<DayPeriod> { DayPeriod.Afternoon },
                    AuthorizedAgents = new List<String>
                    {
                        "Jordan",
                        "Amandine",
                        "Alexandre",
                        "Emilien"
                    }
                });

                _company.Settings.ActivityTemplates.Add(new ActivityTemplate
                {
                    Name = "Havas / Passerelles",
                    Days = new List<DayOfWeek> { DayOfWeek.Monday, DayOfWeek.Tuesday, DayOfWeek.Wednesday, DayOfWeek.Thursday, DayOfWeek.Friday },
                    ValidPeriods = new List<DayPeriod> { DayPeriod.Morning, DayPeriod.Afternoon },
                    AuthorizedAgents = new List<String>
                    {
                        "Thierry"
                    }
                });

                _company.Settings.ActivityTemplates.Add(new ActivityTemplate
                {
                    Name = "Médisances et véhémence",
                    Days = new List<DayOfWeek> { DayOfWeek.Monday, DayOfWeek.Tuesday, DayOfWeek.Wednesday, DayOfWeek.Thursday, DayOfWeek.Friday },
                    ValidPeriods = new List<DayPeriod> { DayPeriod.Morning, DayPeriod.Afternoon },
                    MaxOccurrencesPerAgentPerWeek = 1,
                    AuthorizedAgents = new List<String>
                    {
                        "Jordan",
                        "Amandine",
                        "Lucas",
                        "Philippe",
                        "Alexandre",
                        "Emilien",
                        "Thierry"
                    }
                });



                _company.AssignRotatingWeeklySchedule();

                foreach(var week in _company.Weeks)
                {
                    _company.GenerateActivitiesFromTemplates(week);
                    _company.AssignActivitiesAutomatically();
                }

                

                XmlManager.SaveObject(this._company, "company_data.xml");
            }

            this._company = XmlManager.LoadObject<Company>("company_data.xml");

            comboBoxWeeks.DataSource = _company.Weeks;
            comboBoxWeeks.DisplayMember = "ToString"; // utilise la méthode ToString() du Week


            _currentWeek = _company.GetWeekForDate(DateOnly.FromDateTime(DateTime.Today));

            _scheduleView.LoadData(_company, _currentWeek);
            _activitiesView.LoadData(_company, _currentWeek);

            if(_currentWeek != null)
            {

                Int32 index = comboBoxWeeks.Items.IndexOf(_currentWeek);

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
                _activitiesView.LoadWeek(selectedWeek);
                _currentWeek = selectedWeek;
            }
        }

        private void debugToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this._debugForm = new DebugForm(this._company);
            this._debugForm.ShowDialog();

            XmlManager.SaveObject(this._company, "company_data.xml");
        }
    }
}
