using System;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Windows.Forms;
using Saleslogix.SData.Client;
using Saleslogix.SData.Client.Framework;
using SlxFileBrowser.FileSystem;
using SlxFileBrowser.Properties;

namespace SlxFileBrowser
{
    public partial class MainForm : Form
    {
        private readonly ICredentials _credentials = new PromptCredentials();
        private TreeNode _copyNode;
        private IFileSystemInfo _copyInfo;

        public MainForm()
        {
            InitializeComponent();

            SDataRequest.Trace.Switch.Level = SourceLevels.All;
            _imageList.Images.Add(ShellUtils.GetDriveIcon());
            _imageList.Images.Add(ShellUtils.GetFolderIcon());
            _serverUrlText.Text = Settings.Default.ServerUrl;
            _formModeCheck.Checked = Settings.Default.FormMode;
            Initialize();
        }

        private void _serverUrlText_Leave(object sender, EventArgs e)
        {
            Settings.Default.ServerUrl = _serverUrlText.Text;
            Initialize();
        }

        private void _formModeCheck_CheckedChanged(object sender, EventArgs e)
        {
            Settings.Default.FormMode = _formModeCheck.Checked;
            Initialize();
        }

        private void Initialize()
        {
            var client = new SDataClient(_serverUrlText.Text)
                {
                    Credentials = _credentials,
                    NamingScheme = NamingScheme.CamelCase
                };
            SDataRequest.Trace.Listeners.Clear();
            SDataRequest.Trace.Listeners.Add(new TextBoxTraceListener(_logText, client.Uri.Length) {Filter = new EventTypeFilter(SourceLevels.All)});
            Settings.Default.Save();
            _treeView.Nodes.Clear();
            AddDriveNode(_treeView.Nodes, new SDataDriveInfo(client, _formModeCheck.Checked));
        }

        private void _treeView_BeforeExpand(object sender, TreeViewCancelEventArgs e)
        {
            if (e.Node.Nodes.Count == 1 && e.Node.Nodes[0].Tag == null)
            {
                var dir = (IDirectoryInfo) e.Node.Tag;
                e.Node.Nodes.Clear();

                foreach (var subDir in dir.GetDirectories())
                {
                    AddDirectoryNode(e.Node.Nodes, subDir, true);
                }

                foreach (var file in dir.GetFiles())
                {
                    AddFileNode(e.Node.Nodes, file);
                }
            }
        }

        private void _treeView_AfterSelect(object sender, TreeViewEventArgs e)
        {
            var obj = e.Node.Tag;
            var holder = obj as IResourceHolder;
            var isDir = obj is IDirectoryInfo;
            var isFile = obj is IFileInfo;

            _openButton.Enabled = isFile;
            _replaceButton.Enabled = isFile;
            _expandAllButton.Enabled = isDir;
            _newFolderButton.Enabled = isDir;
            _importButton.Enabled = isDir;
            _pasteButton.Enabled = isDir && (_copyInfo != null);
            _saveButton.Enabled = (holder != null);

            if (holder != null)
            {
                obj = holder.Resource;
            }

            _propGrid.SelectedObject = obj;
        }

        private void _treeView_MouseUp(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                _treeView.SelectedNode = _treeView.GetNodeAt(e.X, e.Y);
            }
        }

        private void _treeView_NodeMouseDoubleClick(object sender, TreeNodeMouseClickEventArgs e)
        {
            OpenFile();
        }

        private void _treeView_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyData == Keys.Delete)
            {
                DeleteInfo();
                e.Handled = true;
            }
            else if (e.KeyData == Keys.F2)
            {
                _treeView.SelectedNode.BeginEdit();
            }
        }

        private void _treeView_AfterLabelEdit(object sender, NodeLabelEditEventArgs e)
        {
            var info = e.Node.Tag as IFileSystemInfo;

            if (e.Label != null && info != null)
            {
                var destinationName = Path.Combine(Path.GetDirectoryName(info.FullName), e.Label);
                info.MoveTo(destinationName);
                RefreshPropertyGrid();
            }
        }

        private void _treeView_DragOver(object sender, DragEventArgs e)
        {
            var node = _treeView.GetNodeAt(_treeView.PointToClient(new Point(e.X, e.Y)));
            if (node == null)
            {
                e.Effect = DragDropEffects.None;
                return;
            }

            var dir = node.Tag as IDirectoryInfo;
            if (dir == null || !e.Data.GetDataPresent(DataFormats.FileDrop, false))
            {
                e.Effect = DragDropEffects.None;
                return;
            }

            e.Effect = DragDropEffects.Link;
            _treeView.SelectedNode = node;
        }

        private void _treeView_DragDrop(object sender, DragEventArgs e)
        {
            var node = _treeView.SelectedNode;
            if (node == null)
            {
                return;
            }

            var dir = node.Tag as IDirectoryInfo;
            if (dir == null || !e.Data.GetDataPresent(DataFormats.FileDrop, false))
            {
                return;
            }

            var fileNames = (string[]) e.Data.GetData(DataFormats.FileDrop, false);
            node.Expand();

            foreach (var fileName in fileNames)
            {
                ImportFile(node, dir, fileName);
            }
        }

        private void _openButton_Click(object sender, EventArgs e)
        {
            OpenFile();
        }

        private void _expandAllButton_Click(object sender, EventArgs e)
        {
            _treeView.SelectedNode.ExpandAll();
        }

        private void _refreshButton_Click(object sender, EventArgs e)
        {
            var node = _treeView.SelectedNode;
            var info = node.Tag as IFileSystemInfo;

            if (info == null)
            {
                return;
            }

            info.Refresh();

            if (info != info.DriveInfo.RootDirectory)
            {
                node.Text = info.Name;
            }

            if (info is IDirectoryInfo && node.IsExpanded)
            {
                node.Collapse();
                node.Nodes.Clear();
                node.Nodes.Add(new TreeNode());
                node.Expand();
            }

            RefreshPropertyGrid();
        }

        private void _cutButton_Click(object sender, EventArgs e)
        {
            _copyNode = _treeView.SelectedNode;
            _copyInfo = _copyNode.Tag as IFileSystemInfo;
        }

        private void _pasteButton_Click(object sender, EventArgs e)
        {
            var node = _treeView.SelectedNode;
            var dir = node.Tag as IDirectoryInfo;

            if (dir == null || _copyInfo == null)
            {
                return;
            }

            node.Expand();
            _copyInfo.MoveTo(Path.Combine(dir.FullName, _copyInfo.Name));
            _copyNode.Parent.Nodes.Remove(_copyNode);
            node.Nodes.Add(_copyNode);
            _treeView.SelectedNode = _copyNode;
        }

        private void _newFolderButton_Click(object sender, EventArgs e)
        {
            var node = _treeView.SelectedNode;
            var dir = node.Tag as IDirectoryInfo;

            if (dir == null)
            {
                return;
            }

            string name = null;

            if (InputBox(this, "New Folder", "Folder name:", ref name) == DialogResult.OK)
            {
                node.Expand();
                var newDir = dir.CreateSubdirectory(name);
                _treeView.SelectedNode = AddDirectoryNode(node.Nodes, newDir, false);
            }
        }

        private void _importFolderButton_Click(object sender, EventArgs e)
        {
            var node = _treeView.SelectedNode;
            var dir = node.Tag as IDirectoryInfo;

            if (dir == null)
            {
                return;
            }

            using (var dialog = new FolderBrowserDialog {SelectedPath = Environment.CurrentDirectory})
            {
                if (dialog.ShowDialog(this) == DialogResult.OK)
                {
                    node.Expand();
                    var newDir = CopyDirectory(new DiskDirectoryInfo(new DiskDriveInfo(new DriveInfo(Path.GetPathRoot(dialog.SelectedPath))), new DirectoryInfo(dialog.SelectedPath)), dir);
                    _treeView.SelectedNode = AddDirectoryNode(node.Nodes, newDir, true);
                }
            }
        }

        private void _importFileButton_Click(object sender, EventArgs e)
        {
            var node = _treeView.SelectedNode;
            var dir = node.Tag as IDirectoryInfo;

            if (dir == null)
            {
                return;
            }

            using (var dialog = new OpenFileDialog {Multiselect = true})
            {
                if (dialog.ShowDialog(this) == DialogResult.OK)
                {
                    node.Expand();

                    foreach (var fileName in dialog.FileNames)
                    {
                        ImportFile(node, dir, fileName);
                    }
                }
            }
        }

        private void _exportButton_Click(object sender, EventArgs e)
        {
            var node = _treeView.SelectedNode;
            var dir = node.Tag as IDirectoryInfo;

            if (dir != null)
            {
                ExportDirectory(dir);
            }
            else
            {
                var file = node.Tag as IFileInfo;

                if (file != null)
                {
                    ExportFile(file);
                }
            }
        }

        private void _replaceButton_Click(object sender, EventArgs e)
        {
            var file = _treeView.SelectedNode.Tag as IFileInfo;

            if (file == null)
            {
                return;
            }

            using (var dialog = new OpenFileDialog {FileName = file.Name, Filter = string.Format("{0}|*{0}", file.Extension)})
            {
                if (dialog.ShowDialog(this) == DialogResult.OK)
                {
                    using (var input = File.OpenRead(dialog.FileName))
                    using (var output = file.Open(FileMode.OpenOrCreate, FileAccess.Write))
                    {
                        input.CopyTo(output);
                    }
                }
            }

            RefreshPropertyGrid();
        }

        private void _deleteButton_Click(object sender, EventArgs e)
        {
            DeleteInfo();
        }

        private void _saveButton_Click(object sender, EventArgs e)
        {
            var holder = _treeView.SelectedNode.Tag as IResourceHolder;
            if (holder != null)
            {
                holder.Save(null);
            }
        }

        private void _logBox_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.Control && e.KeyCode == Keys.A)
            {
                _logText.SelectAll();
                e.Handled = true;
            }
        }

        private void AddDriveNode(TreeNodeCollection nodes, IDriveInfo drive)
        {
            nodes.Add(
                new TreeNode(drive.Name.TrimEnd('\\'))
                    {
                        Tag = drive.RootDirectory,
                        ImageIndex = 0,
                        SelectedImageIndex = 0,
                        Nodes = {new TreeNode()}
                    });
        }

        private TreeNode AddDirectoryNode(TreeNodeCollection nodes, IDirectoryInfo dir, bool expandable)
        {
            var node = new TreeNode(dir.Name)
                {
                    Tag = dir,
                    ImageIndex = 1,
                    SelectedImageIndex = 1
                };

            if (expandable)
            {
                node.Nodes.Add(new TreeNode());
            }

            nodes.Add(node);
            return node;
        }

        private TreeNode AddFileNode(TreeNodeCollection nodes, IFileInfo file)
        {
            var extension = Path.GetExtension(file.Name);

            if (!_imageList.Images.ContainsKey(extension))
            {
                _imageList.Images.Add(extension, ShellUtils.GetFileIcon(extension));
            }

            var node = new TreeNode(file.Name)
                {
                    Tag = file,
                    ImageKey = extension,
                    SelectedImageKey = extension
                };
            nodes.Add(node);
            return node;
        }

        private void OpenFile()
        {
            var file = _treeView.SelectedNode.Tag as IFileInfo;

            if (file == null)
            {
                return;
            }

            var tempFile = Path.Combine(Path.GetTempPath(), file.Name);

            using (var input = file.Open(FileMode.Open, FileAccess.Read))
            using (var output = File.OpenWrite(tempFile))
            {
                input.CopyTo(output);
            }

            Process.Start(tempFile);
        }

        private void ImportFile(TreeNode node, IDirectoryInfo dir, string fileName)
        {
            var name = Path.GetFileName(fileName);
            var file = dir.DriveInfo.GetFileInfo(Path.Combine(dir.FullName, name));

            using (var input = File.OpenRead(fileName))
            using (var output = file.Open(FileMode.OpenOrCreate, FileAccess.Write))
            {
                input.CopyTo(output);
            }

            _treeView.SelectedNode = AddFileNode(node.Nodes, file);
        }

        private void ExportDirectory(IDirectoryInfo dir)
        {
            using (var dialog = new FolderBrowserDialog {SelectedPath = Environment.CurrentDirectory})
            {
                if (dialog.ShowDialog(this) == DialogResult.OK)
                {
                    CopyDirectory(dir, new DiskDirectoryInfo(new DiskDriveInfo(new DriveInfo(Path.GetPathRoot(dialog.SelectedPath))), new DirectoryInfo(dialog.SelectedPath)));
                }
            }
        }

        private void ExportFile(IFileInfo file)
        {
            using (var dialog = new SaveFileDialog {FileName = file.Name, Filter = string.Format("{0}|*{0}", file.Extension)})
            {
                if (dialog.ShowDialog(this) == DialogResult.OK)
                {
                    using (var input = file.Open(FileMode.Open, FileAccess.Read))
                    using (var output = File.OpenWrite(dialog.FileName))
                    {
                        input.CopyTo(output);
                    }
                }
            }
        }

        private void DeleteInfo()
        {
            var node = _treeView.SelectedNode;
            var info = node.Tag as IFileSystemInfo;

            if (info == null)
            {
                return;
            }

            if (MessageBox.Show(this, "Are you sure?", "Confirmation", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                info.Delete();
                node.Remove();
            }
        }

        private static IDirectoryInfo CopyDirectory(IDirectoryInfo source, IDirectoryInfo destination)
        {
            var targetDir = destination.GetDirectories().FirstOrDefault(dir => string.Equals(dir.Name, source.Name, StringComparison.OrdinalIgnoreCase))
                            ?? destination.CreateSubdirectory(source.Name);

            foreach (var sourceFile in source.GetFiles())
            {
                var name = sourceFile.Name;
                var targetFile = targetDir.GetFiles().FirstOrDefault(item => string.Equals(item.Name, name, StringComparison.OrdinalIgnoreCase))
                                 ?? destination.DriveInfo.GetFileInfo(Path.Combine(targetDir.FullName, name));

                using (var input = sourceFile.Open(FileMode.Open, FileAccess.Read))
                using (var output = targetFile.Open(FileMode.OpenOrCreate, FileAccess.Write))
                {
                    input.CopyTo(output);
                }
            }

            foreach (var subSource in source.GetDirectories())
            {
                CopyDirectory(subSource, targetDir);
            }

            return targetDir;
        }

        private void RefreshPropertyGrid()
        {
            var obj = _treeView.SelectedNode.Tag;
            var holder = obj as IResourceHolder;
            if (holder != null)
            {
                obj = holder.Resource;
            }
            _propGrid.SelectedObject = obj;
        }

        private static DialogResult InputBox(IWin32Window owner, string title, string promptText, ref string value)
        {
            var label = new Label
                {
                    Text = promptText,
                    AutoSize = true,
                    Location = new Point(12, 20)
                };
            var textBox = new TextBox
                {
                    Text = value,
                    Bounds = new Rectangle(12, 40, 372, 20),
                    Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right
                };
            var okButton = new Button
                {
                    Text = "OK",
                    DialogResult = DialogResult.OK,
                    Location = new Point(228, 72),
                    Anchor = AnchorStyles.Bottom | AnchorStyles.Right
                };
            var cancelButton = new Button
                {
                    Text = "Cancel",
                    DialogResult = DialogResult.Cancel,
                    Location = new Point(308, 72),
                    Anchor = AnchorStyles.Bottom | AnchorStyles.Right
                };

            using (var form = new Form
                {
                    Text = title,
                    ClientSize = new Size(396, 107),
                    FormBorderStyle = FormBorderStyle.FixedDialog,
                    StartPosition = FormStartPosition.CenterScreen,
                    MinimizeBox = false,
                    MaximizeBox = false,
                    AcceptButton = okButton,
                    CancelButton = cancelButton,
                    Controls = {label, textBox, okButton, cancelButton}
                })
            {
                form.ClientSize = new Size(Math.Max(300, label.Right + 10), form.ClientSize.Height);

                var dialogResult = form.ShowDialog(owner);
                value = textBox.Text;
                return dialogResult;
            }
        }

        #region Nested type: TextBoxTraceListener

        private class TextBoxTraceListener : TraceListener
        {
            private readonly TextBox _box;
            private readonly int _urlOffset;
            private int _lastId = 1;

            public TextBoxTraceListener(TextBox box, int urlOffset)
            {
                _box = box;
                _urlOffset = urlOffset;
            }

            public override void TraceData(TraceEventCache eventCache, string source, TraceEventType eventType, int id, object data)
            {
                var message = string.Empty;

                if (id == 0)
                {
                    var request = (WebRequest) data;
                    if (_lastId == 0)
                    {
                        message = Environment.NewLine;
                    }
                    message += string.Format("{0:hh\\:mm\\:ss} {1,6} {2}",
                        DateTime.Now.TimeOfDay,
                        request.Method,
                        request.RequestUri.ToString().Substring(_urlOffset));
                }
                else
                {
                    var response = data as HttpWebResponse;
                    if (response != null)
                    {
                        message = string.Format(" -> ({0}) {1}",
                            (int) response.StatusCode,
                            response.StatusCode);
                    }
                    message += Environment.NewLine;
                }

                _box.Invoke(new Action(() => _box.AppendText(message)));
                _lastId = id;
            }

            public override void Write(string message)
            {
                throw new NotSupportedException();
            }

            public override void WriteLine(string message)
            {
                throw new NotSupportedException();
            }
        }

        #endregion

        #region Nested type: PromptCredentials

        private class PromptCredentials : ICredentials
        {
            private readonly CredentialCache _cache = new CredentialCache();

            public NetworkCredential GetCredential(Uri uri, string authType)
            {
                if (authType != "basic")
                {
                    return null;
                }
                var credential = _cache.GetCredential(uri, authType);
                if (credential == null)
                {
                    using (var prompt = new UserCredentialsDialog
                        {
                            Target = new SDataUri(uri) {Path = null, Query = null}.ToString(),
                            User = Settings.Default.UserName
                        })
                    {
                        if (prompt.ShowDialog() == DialogResult.Cancel)
                        {
                            throw new OperationCanceledException();
                        }
                        credential = new NetworkCredential(prompt.User, prompt.Password);
                        _cache.Add(uri, authType, credential);
                        Settings.Default.UserName = prompt.User;
                    }
                }
                return credential;
            }
        }

        #endregion
    }
}