using System;
using System.Windows.Forms;

namespace WoW.Fishing
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            //Utilities.WindowsAPI.ListWindows();

            Application.Run(new frmMain());
        }
    }
}
