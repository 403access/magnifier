using System;
using System.Windows.Forms;

namespace Magnifier
{
    public partial class SettingsForm : Form
    {
        private Magnifier parentMagnifier;

        public SettingsForm(Magnifier magnifier)
        {
            parentMagnifier = magnifier;
            InitializeComponent();
        }
    }
}
