using System.Windows.Forms;

namespace SlxJobScheduler
{
    public partial class EditForm : Form
    {
        public EditForm()
        {
            InitializeComponent();
        }

        public DialogResult ShowDialog(object target, IWin32Window owner)
        {
            _propertyGrid.SelectedObject = target;
            return ShowDialog(owner);
        }
    }
}