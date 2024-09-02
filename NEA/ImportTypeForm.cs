using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace NEA
{
    public partial class ImportTypeSelectionForm : Form
    {
        public ImportTypeSelectionForm()
        {
            /* DialogResult:
               Army returns 3 (Abort)
               Fleet returns 5 (Ignore) */
            InitializeComponent();
        }
    }
}
