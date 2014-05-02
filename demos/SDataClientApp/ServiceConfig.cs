using System;
using Saleslogix.SData.Client.Framework;

namespace SDataClientApp
{
    public partial class ServiceConfig : BaseControl
    {
        public ServiceConfig()
        {
            InitializeComponent();
        }

        public override void Refresh()
        {
            FormatUrl();
        }

        private void cbProtocol_SelectedIndexChanged(object sender, EventArgs e)
        {
            FormatUrl();
        }

        private void tbServer_TextChanged(object sender, EventArgs e)
        {
            FormatUrl();
        }

        private void tbApplication_TextChanged(object sender, EventArgs e)
        {
            FormatUrl();
        }

        private void tbContract_TextChanged(object sender, EventArgs e)
        {
            FormatUrl();
        }

        private void tbDataSet_TextChanged(object sender, EventArgs e)
        {
            FormatUrl();
        }

        private void FormatUrl()
        {
            var server = tbServer.Text;
            var pos = server.IndexOf(':');
            var uri = new SDataUri();
            int port;

            if (pos >= 0 && int.TryParse(server.Substring(pos + 1), out port))
            {
                server = server.Substring(0, pos);
                uri.Port = port;
            }

            uri.Scheme = cbProtocol.Text;
            uri.Host = server;
            uri.AppendPath(tbVirtualDirectory.Text, tbApplication.Text, tbContract.Text, tbDataSet.Text);

            tbURL.Text = uri.ToString();
        }

        private void btnInitialize_Click(object sender, EventArgs e)
        {
            var server = tbServer.Text;
            var pos = server.IndexOf(':');
            int port;

            if (pos >= 0 && int.TryParse(server.Substring(pos + 1), out port))
            {
                server = server.Substring(0, pos);
            }
            else
            {
                port = 80;
            }

            var uri = new SDataUri
                {
                    Scheme = cbProtocol.Text,
                    Host = server,
                    Port = port
                };
            uri.AppendPath(tbVirtualDirectory.Text, tbApplication.Text, tbContract.Text, tbDataSet.Text);

            Client.Uri = uri.ToString();
            Client.UserName = tbUserName.Text;
            Client.Password = tbPassword.Text;

            StatusLabel.Text = "SData client initialized.";
        }
    }
}