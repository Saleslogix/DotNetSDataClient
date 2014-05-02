using System;
using System.Linq;
using System.Windows.Forms;
using Saleslogix.SData.Client;
using Saleslogix.SData.Client.Framework;

namespace SDataClientApp
{
    public partial class ResourceCollection : BaseControl
    {
        private SDataCollection<SDataResource> _collection;

        public ResourceCollection()
        {
            InitializeComponent();
        }

        public override void Refresh()
        {
            UpdateUrl();
        }

        private void tbCollectionResourceKind_TextChanged(object sender, EventArgs e)
        {
            UpdateUrl();
        }

        private void numStartIndex_ValueChanged(object sender, EventArgs e)
        {
            UpdateUrl();
        }

        private void numCount_ValueChanged(object sender, EventArgs e)
        {
            UpdateUrl();
        }

        private void UpdateUrl()
        {
            var uri = new SDataUri(Client.Uri)
                {
                    StartIndex = (int) numStartIndex.Value,
                    Count = (int) numCount.Value
                };
            if (!string.IsNullOrEmpty(tbCollectionResourceKind.Text))
            {
                uri.AppendPath(tbCollectionResourceKind.Text);
            }
            tbCollectionURL.Text = uri.ToString();
        }

        private void btnCollectionRead_Click(object sender, EventArgs e)
        {
            collectionPayloadGrid.SelectedObject = null;
            UpdateCollection();
        }

        private void atomEntryGrid_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (_collection == null)
            {
                return;
            }

            var index = atomEntryGrid.SelectedRows[0].Index;
            collectionPayloadGrid.SelectedObject = _collection[index];
        }

        private void btnFirst_Click(object sender, EventArgs e)
        {
            NavigateCollection("first");
        }

        private void btnPrevious_Click(object sender, EventArgs e)
        {
            NavigateCollection("previous");
        }

        private void btnNext_Click(object sender, EventArgs e)
        {
            NavigateCollection("next");
        }

        private void btnLast_Click(object sender, EventArgs e)
        {
            NavigateCollection("last");
        }

        private void NavigateCollection(string relation)
        {
            var link = _collection.Links.First(item => item.Relation == relation);
            var query = link.Uri.Query;
            var pos1 = query.IndexOf("startIndex=", StringComparison.Ordinal) + 11;
            var pos2 = query.IndexOf("&", pos1, StringComparison.Ordinal);

            if (pos2 < 0)
            {
                pos2 = query.Length;
            }

            numStartIndex.Value = int.Parse(query.Substring(pos1, pos2 - pos1));
            UpdateCollection();
        }

        private void UpdateCollection()
        {
            _collection = Client.Execute<SDataCollection<SDataResource>>(
                new SDataParameters
                    {
                        Path = tbCollectionResourceKind.Text,
                        StartIndex = (int) numStartIndex.Value,
                        Count = (int) numCount.Value
                    }).Content;

            var lookup = _collection.Links.ToLookup(link => link.Relation);
            btnFirst.Enabled = lookup["first"].Any();
            btnPrevious.Enabled = lookup["previous"].Any();
            btnNext.Enabled = lookup["next"].Any();
            btnLast.Enabled = lookup["last"].Any();

            atomEntryGrid.Rows.Clear();
            atomEntryGrid.Columns.Clear();
            if (_collection.Count > 0)
            {
                foreach (var key in _collection[0].Keys)
                {
                    atomEntryGrid.Columns.Add(key, key);
                }
                foreach (var item in _collection)
                {
                    atomEntryGrid.Rows.Add(item.Values.ToArray());
                }
            }

            atomEntryGrid.Refresh();
            atomEntryGrid.AutoResizeColumns();

            if (atomEntryGrid.SelectedRows.Count != 0)
            {
                atomEntryGrid_CellClick(null, null);
            }
        }
    }
}