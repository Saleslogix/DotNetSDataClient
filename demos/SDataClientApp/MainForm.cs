using System;
using System.Windows.Forms;
using Saleslogix.SData.Client;
using SDataClientApp.Properties;

namespace SDataClientApp
{
    public partial class MainForm : Form
    {
        public MainForm()
        {
            InitializeComponent();

            var client = new SDataClient(null);

            foreach (TabPage tab in tabControl1.TabPages)
            {
                ((BaseControl) tab.Controls[0]).Client = client;
            }

            tabControl1_SelectedIndexChanged(null, null);
        }

        protected override void OnFormClosed(FormClosedEventArgs e)
        {
            Settings.Default.Save();
            base.OnFormClosed(e);
        }

        private void tabControl1_SelectedIndexChanged(object sender, EventArgs e)
        {
            ((BaseControl) tabControl1.SelectedTab.Controls[0]).Refresh();
            statusLabel.Text = string.Empty;
        }
    }
}