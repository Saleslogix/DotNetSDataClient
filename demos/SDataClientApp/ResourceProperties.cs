using System;
using System.Collections.Generic;
using System.Linq;
using Saleslogix.SData.Client;
using Saleslogix.SData.Client.Framework;

namespace SDataClientApp
{
    public partial class ResourceProperties : BaseControl
    {
        public ResourceProperties()
        {
            InitializeComponent();
        }

        public override void Refresh()
        {
            UpdateUrl();
        }

        private void btnAddProperty_Click(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(tbResourceProperty.Text))
            {
                lbProperties.Items.Add(tbResourceProperty.Text);
                UpdateUrl();
            }
        }

        private void tbRPResourceKind_TextChanged(object sender, EventArgs e)
        {
            UpdateUrl();
        }

        private void tbRPResourceSelector_TextChanged(object sender, EventArgs e)
        {
            UpdateUrl();
        }

        private void btnClearProperties_Click(object sender, EventArgs e)
        {
            lbProperties.Items.Clear();
            UpdateUrl();
        }

        private void UpdateUrl()
        {
            var uri = new SDataUri(Client.Uri);
            if (!string.IsNullOrEmpty(tbRPResourceKind.Text))
            {
                var selector = tbRPResourceSelector.Text;
                if (!string.IsNullOrEmpty(selector))
                {
                    selector = SDataUri.FormatConstant(selector);
                }
                uri.AppendPath(new UriPathSegment(tbRPResourceKind.Text, selector));
            }
            if (lbProperties.Items.Count > 0)
            {
                uri.AppendPath(lbProperties.Items.Cast<string>().ToArray());
            }
            tbResourcePropertiesURL.Text = uri.ToString();
        }

        private void btnPropertiesRead_Click(object sender, EventArgs e)
        {
            if (cbIsFeed.Checked)
            {
                var parts = new List<string>();
                if (!string.IsNullOrEmpty(tbRPResourceKind.Text))
                {
                    var selector = tbRPResourceSelector.Text;
                    if (!string.IsNullOrEmpty(selector))
                    {
                        selector = SDataUri.FormatConstant(selector);
                    }
                    parts.Add(new UriPathSegment(tbRPResourceKind.Text, selector).Segment);
                }
                if (lbProperties.Items.Count > 0)
                {
                    parts.AddRange(lbProperties.Items.Cast<string>());
                }

                var collection = Client.Execute<SDataCollection<SDataResource>>(
                    new SDataParameters
                        {
                            Path = string.Join("/", parts)
                        }).Content;
                gridRPPayloads.SelectedObject = null;

                rpGridEntries.Rows.Clear();
                rpGridEntries.Columns.Clear();
                if (collection.Count > 0)
                {
                    foreach (var key in collection[0].Keys)
                    {
                        rpGridEntries.Columns.Add(key, key);
                    }
                    foreach (var item in collection)
                    {
                        rpGridEntries.Rows.Add(item.Values.ToArray());
                    }
                }

                rpGridEntries.Refresh();
                rpGridEntries.AutoResizeColumns();
            }
            else
            {
                var resource = Client.Get(tbRPResourceSelector.Text, tbRPResourceKind.Text);
                rpGridEntries.Rows.Clear();
                rpGridEntries.Columns.Clear();
                gridRPPayloads.SelectedObject = resource;
            }
        }
    }
}