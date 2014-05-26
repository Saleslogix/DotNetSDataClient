using System.Windows.Forms;

namespace SlxJobScheduler
{
    public partial class ResultForm : Form
    {
        public ResultForm()
        {
            InitializeComponent();
        }

        public DialogResult ShowDialog(string result, IWin32Window owner)
        {
            _resultText.Text = result;
            return ShowDialog(owner);
        }

        private void ResultForm_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Escape)
            {
                Close();
                e.Handled = true;
            }
        }
    }
}