using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace NEA
{
    static class Program
    {
        /// <summary>
        ///  The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.SetHighDpiMode(HighDpiMode.SystemAware);
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new MenuForm());
        }

        public static void QuitProgram(bool debugMode)
        {
            if (!debugMode)
            {
                DialogResult result = MessageBox.Show("Are you sure you want to quit the program?", "Quit?", MessageBoxButtons.YesNo);
                if (result == DialogResult.Yes)
                {
                    if (System.Windows.Forms.Application.MessageLoop)
                    {
                        System.Windows.Forms.Application.Exit();
                    }
                    else
                    {
                        System.Environment.Exit(1);
                    }
                }
            }
            else
            {
                System.Windows.Forms.Application.Exit();
            }
        }
    }
}
