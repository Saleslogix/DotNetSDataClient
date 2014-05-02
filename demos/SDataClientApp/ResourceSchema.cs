using System;
using System.IO;
using System.Windows.Forms;
using Saleslogix.SData.Client;
using Saleslogix.SData.Client.Framework;
using Saleslogix.SData.Client.Metadata;

namespace SDataClientApp
{
    public partial class ResourceSchema : BaseControl
    {
        private SDataSchema _schema;

        public ResourceSchema()
        {
            InitializeComponent();
        }

        public override void Refresh()
        {
            UpdateUrl();
        }

        private void tbSchemaResourceKind_TextChanged(object sender, EventArgs e)
        {
            UpdateUrl();
        }

        private void UpdateUrl()
        {
            var uri = new SDataUri(Client.Uri);
            if (!string.IsNullOrEmpty(tbSchemaResourceKind.Text))
            {
                uri.AppendPath(tbSchemaResourceKind.Text);
            }
            uri.AppendPath("$schema");
            tbSchemaURL.Text = uri.ToString();
        }

        private void btnSchemaRead_Click(object sender, EventArgs e)
        {
            var path = tbSchemaResourceKind.Text;
            if (!string.IsNullOrEmpty(path))
            {
                path += "/";
            }
            _schema = Client.Execute<SDataSchema>(
                new SDataParameters
                    {
                        Path = path + "$schema"
                    }).Content;
            if (_schema != null)
            {
                MessageBox.Show("Read schema completed successfully.");
                btnSchemaSave.Enabled = true;
                btnSchemaSave.Visible = true;
                lbSchemaFileName.Visible = true;
                tbSchemaFileName.Visible = true;
            }
        }

        private void btnSchemaSave_Click(object sender, EventArgs e)
        {
            using (var stream = new FileStream(tbSchemaFileName.Text, FileMode.Create))
            {
                _schema.Write(stream);
            }

            MessageBox.Show("Schema saved successfully.");
        }
    }
}