using SyncroTeam.Domain.Core;
using SyncroTeam.Domain.Entities;
using SyncroTeam.Domain.Objects;

namespace SyncroTeam
{
    public partial class MainForm : Form
    {
        private Company _company;
        public MainForm()
        {
            InitializeComponent();

            this._company = new Company("Peach_Harem", "232323");
            this._company.GenerateWeeksForYear(DateTime.Now.Year);

            this._company.AddAgent(new Agent("Jordan", new TimeOnly(), new TimeOnly(), Color.Red));
            this._company.AddAgent(new Agent("Lucas", new TimeOnly(), new TimeOnly(), Color.Red));
            this._company.AddAgent(new Agent("Alexandre", new TimeOnly(), new TimeOnly(), Color.Red));
            this._company.AddAgent(new Agent("Philippe", new TimeOnly(), new TimeOnly(), Color.Red));
            this._company.AddAgent(new Agent("Amandine", new TimeOnly(), new TimeOnly(), Color.Red));
            this._company.AddAgent(new Agent("Thierry", new TimeOnly(), new TimeOnly(), Color.Red));
            this._company.AddAgent(new Agent("Emilien", new TimeOnly(), new TimeOnly(), Color.Red));

            XmlManager.SaveObject(this._company, "company_data.xml");
        }
    }
}
