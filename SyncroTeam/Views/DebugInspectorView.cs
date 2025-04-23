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
    public partial class DebugInspectorView : UserControl
    {
        private Object _debugTarget;

        public DebugInspectorView()
        {
            InitializeComponent();
        }

        public void SetObject(Object param_DebugTarget)
        {
            this._debugTarget = param_DebugTarget;
            this.RefreshView();
        }

        private void RefreshView()
        {
            string filter = textBoxSearch.Text;
            if(_debugTarget == null)
            {
                propertyGrid.SelectedObject = null;
            }
            else
            {
                propertyGrid.SelectedObject = new FilteredObjectWrapper(_debugTarget, filter);
            }
        }

        private void textBoxSearch_TextChanged(object sender, EventArgs e)
        {
            this.RefreshView();
        }

        private void btnClear_Click(object sender, EventArgs e)
        {
            textBoxSearch.Text = string.Empty;
        }
    }
}
