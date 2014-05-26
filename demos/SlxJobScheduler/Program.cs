using System;
using System.Windows.Forms;

namespace SlxJobScheduler
{
    internal static class Program
    {
        [STAThread]
        private static void Main()
        {
            Application.ThreadException += (sender, e) => MessageBox.Show(e.Exception.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new MainForm());
        }
    }
}