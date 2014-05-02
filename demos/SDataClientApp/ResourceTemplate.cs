using System;
using System.Windows.Forms;
using Saleslogix.SData.Client;
using Saleslogix.SData.Client.Framework;

namespace SDataClientApp
{
    public partial class ResourceTemplate : BaseControl
    {
        public ResourceTemplate()
        {
            InitializeComponent();
        }

        public override void Refresh()
        {
            UpdateUrl();
        }

        private void tbTemplateResourceKind_TextChanged(object sender, EventArgs e)
        {
            UpdateUrl();
        }

        private void UpdateUrl()
        {
            var uri = new SDataUri(Client.Uri);
            if (!string.IsNullOrEmpty(tbTemplateResourceKind.Text))
            {
                uri.AppendPath(tbTemplateResourceKind.Text);
            }
            uri.AppendPath("$template");
            tbTemplateURL.Text = uri.ToString();
        }

        private void btnTemplateRead_Click(object sender, EventArgs e)
        {
            var path = tbTemplateResourceKind.Text;
            if (!string.IsNullOrEmpty(path))
            {
                path += "/";
            }
            var template = Client.Get(null, path + "$template");
            templatePayloadGrid.SelectedObject = template;
            if (template == null)
            {
                MessageBox.Show("$template not supported");
            }
        }
    }
}