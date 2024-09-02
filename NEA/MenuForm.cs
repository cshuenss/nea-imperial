using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace NEA
{
    public partial class MenuForm : Form
    {
        bool debugMode = false;

        public MenuForm()
        {
            InitializeComponent();
            lblDebugActive.Hide();
        }

        private void btnMenuExit_Click(object sender, EventArgs e)
        {
            Program.QuitProgram(debugMode);
        }

        private void btnOpenSettings_Click(object sender, EventArgs e)
        {
            SettingsForm settingsForm = new SettingsForm(debugMode) { Owner = this };
            settingsForm.Show();
            this.Opacity = 0;
        }

        // The following (uncommented) code is taken from the thread at https://github.com/dotnet/corefx/issues/10361
        private void btnMenuRules_Click(object sender, EventArgs e)
        {
            // The URL to open - a PDF containing the rules of the (2030 variant of) Imperial
            string rulesURL = "https://www.fgbradleys.com/rules/rules4/Imperial%202030%20-%20rules.pdf";
            // Attempt to open the rules in the default web browser
            try
            {
                Process.Start(rulesURL);
            }
            // Could fail due to status of UseShellExecute, so try OS-specific calls
            catch
            {
                try
                {
                    if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
                    {
                        rulesURL = rulesURL.Replace("&", "^&");
                        Process.Start(new ProcessStartInfo(rulesURL) { UseShellExecute = true });
                    }
                    else if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
                    {
                        Process.Start("xdg-open", rulesURL);
                    }
                    else if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
                    {
                        Process.Start("open", rulesURL);
                    }
                    else
                    {
                        MessageBox.Show("Could not open\nhttps://www.fgbradleys.com/rules/rules4/Imperial%202030%20-%20rules.pdf", "Failed to open rules");
                    }
                }
                catch
                {
                    MessageBox.Show("Could not open\nhttps://www.fgbradleys.com/rules/rules4/Imperial%202030%20-%20rules.pdf", "Failed to open rules");
                }
            }
        }

        private void lblCaption_Click(object sender, EventArgs e)
        {
            debugMode = !debugMode;
            if (debugMode)
            {
                lblDebugActive.Show();
            }
            else
            {
                lblDebugActive.Hide();
            }
        }
    }
}
