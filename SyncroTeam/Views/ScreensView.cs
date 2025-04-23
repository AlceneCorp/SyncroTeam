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
    public partial class ScreensView : UserControl
    {
        public ScreensView()
        {
            InitializeComponent();

            this.Dock = DockStyle.Fill;
            this.BackColor = Color.White;
        }
    }
}
