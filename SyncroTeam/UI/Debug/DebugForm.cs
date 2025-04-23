using SyncroTeam.Domain.Core;
using SyncroTeam.Objects;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SyncroTeam.UI.Debug
{
    public partial class DebugForm : Form
    {
        private DebugInspectorView _inspectorView;

        private Company _company;

        public DebugForm(Company param_Company)
        {
            InitializeComponent();

            this._company = param_Company;
            _inspectorView = new DebugInspectorView();
            _inspectorView.SetObject(this._company);
            _inspectorView.Dock = DockStyle.Fill;
            panel_Debug.Controls.Add(_inspectorView);
        }

        private void DebugForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            var result = MessageBox.Show(
                "Voulez-vous enregistrer les modifications avant de quitter ?",
                "Confirmation de fermeture",
                MessageBoxButtons.YesNoCancel,
                MessageBoxIcon.Question
            );

            switch(result)
            {
                case DialogResult.Yes:
                    //sauvegarde ici
                    XmlManager.SaveObject(_company, "company_data.xml");
                    break;

                case DialogResult.No:
                    //Ne rien faire, la fermeture continue
                    break;

                case DialogResult.Cancel:
                    e.Cancel = true;
                    break;
            }
        }

    }
}
