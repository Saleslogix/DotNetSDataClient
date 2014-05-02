using System;
using Saleslogix.SData.Client;
using Saleslogix.SData.Client.Framework;

namespace SDataClientApp
{
    public partial class SingleResource : BaseControl
    {
        private SDataResource _resource;

        public SingleResource()
        {
            InitializeComponent();
        }

        public override void Refresh()
        {
            UpdateUrl();
        }

        private void tbSingleResourceKind_TextChanged(object sender, EventArgs e)
        {
            UpdateUrl();
        }

        private void tbSingleResourceSelector_TextChanged(object sender, EventArgs e)
        {
            UpdateUrl();
        }

        private void tbSingleResourceInclude_TextChanged(object sender, EventArgs e)
        {
            UpdateUrl();
        }

        private void UpdateUrl()
        {
            var uri = new SDataUri(Client.Uri);
            if (!string.IsNullOrEmpty(tbSingleResourceKind.Text))
            {
                var selector = tbSingleResourceSelector.Text;
                if (!string.IsNullOrEmpty(selector))
                {
                    selector = SDataUri.FormatConstant(selector);
                }
                uri.AppendPath(new UriPathSegment(tbSingleResourceKind.Text, selector));
            }
            if (!string.IsNullOrEmpty(tbSingleResourceInclude.Text))
            {
                uri.Include = tbSingleResourceInclude.Text;
            }
            tbSingleURL.Text = uri.ToString();
        }

        private void btnSingleRead_Click(object sender, EventArgs e)
        {
            var options = !string.IsNullOrEmpty(tbSingleResourceInclude.Text)
                ? new SDataPayloadOptions {Include = tbSingleResourceInclude.Text}
                : null;
            _resource = Client.Get(tbSingleResourceSelector.Text, tbSingleResourceKind.Text, options);
            UpdateGrid();
            StatusLabel.Text = "Read completed successfully.";
        }

        private void btnSingleCreate_Click(object sender, EventArgs e)
        {
            var options = !string.IsNullOrEmpty(tbSingleResourceInclude.Text)
                ? new SDataPayloadOptions {Include = tbSingleResourceInclude.Text}
                : null;
            _resource = Client.Post(_resource, tbSingleResourceKind.Text, options);
            UpdateGrid();
            StatusLabel.Text = "Create completed successfully.";
        }

        private void btnSingleUpdate_Click(object sender, EventArgs e)
        {
            var options = !string.IsNullOrEmpty(tbSingleResourceInclude.Text)
                ? new SDataPayloadOptions {Include = tbSingleResourceInclude.Text}
                : null;
            _resource = Client.Put(_resource, tbSingleResourceKind.Text, options);
            UpdateGrid();
            StatusLabel.Text = "Update completed successfully.";
        }

        private void btnSingleDelete_Click(object sender, EventArgs e)
        {
            Client.Delete(_resource, tbSingleResourceKind.Text);
            _resource = null;
            UpdateGrid();
            StatusLabel.Text = "Delete completed successfully.";
        }

        private void UpdateGrid()
        {
            var exists = _resource != null;
            btnSingleCreate.Enabled = exists;
            btnSingleUpdate.Enabled = exists;
            btnSingleDelete.Enabled = exists;
            tbSingleResourceSelector.Text = exists ? _resource.Key : null;
            singlePayloadGrid.SelectedObject = _resource;
        }
    }
}