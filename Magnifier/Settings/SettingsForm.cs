using System;
using System.Drawing;
using System.Windows.Forms;

namespace Magnifier
{
    public partial class SettingsForm : Form
    {
        private Magnifier parentMagnifier;
        private Color selectedTransparencyKey;

        public SettingsForm(Magnifier magnifier)
        {
            parentMagnifier = magnifier;
            selectedTransparencyKey = parentMagnifier.TransparencyKeyColor; // Initial color
            InitializeComponent();
        }
    }
}
