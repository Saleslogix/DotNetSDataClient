using System.ComponentModel;
using System.Windows.Forms;
using Saleslogix.SData.Client;

namespace SDataClientApp
{
    public class BaseControl : UserControl
    {
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public ISDataClient Client { get; set; }

        public ToolStripItem StatusLabel { get; set; }

        public new virtual void Refresh()
        {
        }
    }
}