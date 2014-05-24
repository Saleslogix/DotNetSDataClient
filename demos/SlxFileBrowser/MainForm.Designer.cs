namespace SlxFileBrowser
{
    partial class MainForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.Windows.Forms.ContextMenuStrip contextMenuStrip;
            System.Windows.Forms.SplitContainer splitContainer1;
            System.Windows.Forms.SplitContainer splitContainer2;
            System.Windows.Forms.Panel panel1;
            System.Windows.Forms.Label label1;
            this._openButton = new System.Windows.Forms.ToolStripMenuItem();
            this._expandAllButton = new System.Windows.Forms.ToolStripMenuItem();
            this._refreshButton = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem1 = new System.Windows.Forms.ToolStripSeparator();
            this._cutButton = new System.Windows.Forms.ToolStripMenuItem();
            this._pasteButton = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem2 = new System.Windows.Forms.ToolStripSeparator();
            this._newFolderButton = new System.Windows.Forms.ToolStripMenuItem();
            this._importButton = new System.Windows.Forms.ToolStripMenuItem();
            this._importFolderButton = new System.Windows.Forms.ToolStripMenuItem();
            this._importFileButton = new System.Windows.Forms.ToolStripMenuItem();
            this._exportButton = new System.Windows.Forms.ToolStripMenuItem();
            this._replaceButton = new System.Windows.Forms.ToolStripMenuItem();
            this._deleteButton = new System.Windows.Forms.ToolStripMenuItem();
            this._treeView = new System.Windows.Forms.TreeView();
            this._imageList = new System.Windows.Forms.ImageList(this.components);
            this._propGrid = new System.Windows.Forms.PropertyGrid();
            this._saveButton = new System.Windows.Forms.Button();
            this._logText = new System.Windows.Forms.TextBox();
            this._formModeCheck = new System.Windows.Forms.CheckBox();
            this._serverUrlText = new System.Windows.Forms.TextBox();
            contextMenuStrip = new System.Windows.Forms.ContextMenuStrip(this.components);
            splitContainer1 = new System.Windows.Forms.SplitContainer();
            splitContainer2 = new System.Windows.Forms.SplitContainer();
            panel1 = new System.Windows.Forms.Panel();
            label1 = new System.Windows.Forms.Label();
            contextMenuStrip.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(splitContainer1)).BeginInit();
            splitContainer1.Panel1.SuspendLayout();
            splitContainer1.Panel2.SuspendLayout();
            splitContainer1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(splitContainer2)).BeginInit();
            splitContainer2.Panel1.SuspendLayout();
            splitContainer2.Panel2.SuspendLayout();
            splitContainer2.SuspendLayout();
            panel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // contextMenuStrip
            // 
            contextMenuStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this._openButton,
            this._expandAllButton,
            this._refreshButton,
            this.toolStripMenuItem1,
            this._cutButton,
            this._pasteButton,
            this.toolStripMenuItem2,
            this._newFolderButton,
            this._importButton,
            this._exportButton,
            this._replaceButton,
            this._deleteButton});
            contextMenuStrip.Name = "_menu";
            contextMenuStrip.Size = new System.Drawing.Size(135, 236);
            // 
            // _openButton
            // 
            this._openButton.Name = "_openButton";
            this._openButton.Size = new System.Drawing.Size(134, 22);
            this._openButton.Text = "Open";
            this._openButton.Click += new System.EventHandler(this._openButton_Click);
            // 
            // _expandAllButton
            // 
            this._expandAllButton.Name = "_expandAllButton";
            this._expandAllButton.Size = new System.Drawing.Size(134, 22);
            this._expandAllButton.Text = "Expand All";
            this._expandAllButton.Click += new System.EventHandler(this._expandAllButton_Click);
            // 
            // _refreshButton
            // 
            this._refreshButton.Name = "_refreshButton";
            this._refreshButton.Size = new System.Drawing.Size(134, 22);
            this._refreshButton.Text = "Refresh";
            this._refreshButton.Click += new System.EventHandler(this._refreshButton_Click);
            // 
            // toolStripMenuItem1
            // 
            this.toolStripMenuItem1.Name = "toolStripMenuItem1";
            this.toolStripMenuItem1.Size = new System.Drawing.Size(131, 6);
            // 
            // _cutButton
            // 
            this._cutButton.Name = "_cutButton";
            this._cutButton.Size = new System.Drawing.Size(134, 22);
            this._cutButton.Text = "Cut";
            this._cutButton.Click += new System.EventHandler(this._cutButton_Click);
            // 
            // _pasteButton
            // 
            this._pasteButton.Name = "_pasteButton";
            this._pasteButton.Size = new System.Drawing.Size(134, 22);
            this._pasteButton.Text = "Paste";
            this._pasteButton.Click += new System.EventHandler(this._pasteButton_Click);
            // 
            // toolStripMenuItem2
            // 
            this.toolStripMenuItem2.Name = "toolStripMenuItem2";
            this.toolStripMenuItem2.Size = new System.Drawing.Size(131, 6);
            // 
            // _newFolderButton
            // 
            this._newFolderButton.Name = "_newFolderButton";
            this._newFolderButton.Size = new System.Drawing.Size(134, 22);
            this._newFolderButton.Text = "New Folder";
            this._newFolderButton.Click += new System.EventHandler(this._newFolderButton_Click);
            // 
            // _importButton
            // 
            this._importButton.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this._importFolderButton,
            this._importFileButton});
            this._importButton.Name = "_importButton";
            this._importButton.Size = new System.Drawing.Size(134, 22);
            this._importButton.Text = "Import";
            // 
            // _importFolderButton
            // 
            this._importFolderButton.Name = "_importFolderButton";
            this._importFolderButton.Size = new System.Drawing.Size(107, 22);
            this._importFolderButton.Text = "Folder";
            this._importFolderButton.Click += new System.EventHandler(this._importFolderButton_Click);
            // 
            // _importFileButton
            // 
            this._importFileButton.Name = "_importFileButton";
            this._importFileButton.Size = new System.Drawing.Size(107, 22);
            this._importFileButton.Text = "File";
            this._importFileButton.Click += new System.EventHandler(this._importFileButton_Click);
            // 
            // _exportButton
            // 
            this._exportButton.Name = "_exportButton";
            this._exportButton.Size = new System.Drawing.Size(134, 22);
            this._exportButton.Text = "Export";
            this._exportButton.Click += new System.EventHandler(this._exportButton_Click);
            // 
            // _replaceButton
            // 
            this._replaceButton.Name = "_replaceButton";
            this._replaceButton.Size = new System.Drawing.Size(134, 22);
            this._replaceButton.Text = "Replace";
            this._replaceButton.Click += new System.EventHandler(this._replaceButton_Click);
            // 
            // _deleteButton
            // 
            this._deleteButton.Name = "_deleteButton";
            this._deleteButton.Size = new System.Drawing.Size(134, 22);
            this._deleteButton.Text = "Delete";
            this._deleteButton.Click += new System.EventHandler(this._deleteButton_Click);
            // 
            // splitContainer1
            // 
            splitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
            splitContainer1.Location = new System.Drawing.Point(0, 28);
            splitContainer1.Name = "splitContainer1";
            splitContainer1.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // splitContainer1.Panel1
            // 
            splitContainer1.Panel1.Controls.Add(splitContainer2);
            // 
            // splitContainer1.Panel2
            // 
            splitContainer1.Panel2.Controls.Add(this._logText);
            splitContainer1.Size = new System.Drawing.Size(784, 533);
            splitContainer1.SplitterDistance = 436;
            splitContainer1.TabIndex = 1;
            // 
            // splitContainer2
            // 
            splitContainer2.Dock = System.Windows.Forms.DockStyle.Fill;
            splitContainer2.Location = new System.Drawing.Point(0, 0);
            splitContainer2.Name = "splitContainer2";
            // 
            // splitContainer2.Panel1
            // 
            splitContainer2.Panel1.Controls.Add(this._treeView);
            // 
            // splitContainer2.Panel2
            // 
            splitContainer2.Panel2.Controls.Add(this._propGrid);
            splitContainer2.Panel2.Controls.Add(this._saveButton);
            splitContainer2.Size = new System.Drawing.Size(784, 436);
            splitContainer2.SplitterDistance = 383;
            splitContainer2.TabIndex = 0;
            // 
            // _treeView
            // 
            this._treeView.AllowDrop = true;
            this._treeView.ContextMenuStrip = contextMenuStrip;
            this._treeView.Dock = System.Windows.Forms.DockStyle.Fill;
            this._treeView.HideSelection = false;
            this._treeView.ImageIndex = 0;
            this._treeView.ImageList = this._imageList;
            this._treeView.ItemHeight = 18;
            this._treeView.LabelEdit = true;
            this._treeView.Location = new System.Drawing.Point(0, 0);
            this._treeView.Name = "_treeView";
            this._treeView.SelectedImageIndex = 0;
            this._treeView.Size = new System.Drawing.Size(383, 436);
            this._treeView.TabIndex = 0;
            this._treeView.AfterLabelEdit += new System.Windows.Forms.NodeLabelEditEventHandler(this._treeView_AfterLabelEdit);
            this._treeView.BeforeExpand += new System.Windows.Forms.TreeViewCancelEventHandler(this._treeView_BeforeExpand);
            this._treeView.AfterSelect += new System.Windows.Forms.TreeViewEventHandler(this._treeView_AfterSelect);
            this._treeView.NodeMouseDoubleClick += new System.Windows.Forms.TreeNodeMouseClickEventHandler(this._treeView_NodeMouseDoubleClick);
            this._treeView.DragDrop += new System.Windows.Forms.DragEventHandler(this._treeView_DragDrop);
            this._treeView.DragOver += new System.Windows.Forms.DragEventHandler(this._treeView_DragOver);
            this._treeView.KeyUp += new System.Windows.Forms.KeyEventHandler(this._treeView_KeyUp);
            this._treeView.MouseUp += new System.Windows.Forms.MouseEventHandler(this._treeView_MouseUp);
            // 
            // _imageList
            // 
            this._imageList.ColorDepth = System.Windows.Forms.ColorDepth.Depth32Bit;
            this._imageList.ImageSize = new System.Drawing.Size(16, 16);
            this._imageList.TransparentColor = System.Drawing.Color.Transparent;
            // 
            // _propGrid
            // 
            this._propGrid.Dock = System.Windows.Forms.DockStyle.Fill;
            this._propGrid.HelpVisible = false;
            this._propGrid.Location = new System.Drawing.Point(0, 0);
            this._propGrid.Name = "_propGrid";
            this._propGrid.Size = new System.Drawing.Size(397, 413);
            this._propGrid.TabIndex = 0;
            this._propGrid.ToolbarVisible = false;
            // 
            // _saveButton
            // 
            this._saveButton.Dock = System.Windows.Forms.DockStyle.Bottom;
            this._saveButton.Location = new System.Drawing.Point(0, 413);
            this._saveButton.Name = "_saveButton";
            this._saveButton.Size = new System.Drawing.Size(397, 23);
            this._saveButton.TabIndex = 1;
            this._saveButton.Text = "Save";
            this._saveButton.UseVisualStyleBackColor = true;
            this._saveButton.Click += new System.EventHandler(this._saveButton_Click);
            // 
            // _logText
            // 
            this._logText.Dock = System.Windows.Forms.DockStyle.Fill;
            this._logText.Font = new System.Drawing.Font("Courier New", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this._logText.Location = new System.Drawing.Point(0, 0);
            this._logText.Multiline = true;
            this._logText.Name = "_logText";
            this._logText.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this._logText.Size = new System.Drawing.Size(784, 93);
            this._logText.TabIndex = 0;
            this._logText.WordWrap = false;
            this._logText.KeyUp += new System.Windows.Forms.KeyEventHandler(this._logBox_KeyUp);
            // 
            // panel1
            // 
            panel1.Controls.Add(this._formModeCheck);
            panel1.Controls.Add(label1);
            panel1.Controls.Add(this._serverUrlText);
            panel1.Dock = System.Windows.Forms.DockStyle.Top;
            panel1.Location = new System.Drawing.Point(0, 0);
            panel1.Name = "panel1";
            panel1.Size = new System.Drawing.Size(784, 28);
            panel1.TabIndex = 0;
            // 
            // _formModeCheck
            // 
            this._formModeCheck.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this._formModeCheck.AutoSize = true;
            this._formModeCheck.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
            this._formModeCheck.Location = new System.Drawing.Point(692, 6);
            this._formModeCheck.Name = "_formModeCheck";
            this._formModeCheck.Size = new System.Drawing.Size(82, 17);
            this._formModeCheck.TabIndex = 7;
            this._formModeCheck.Text = "Form Mode:";
            this._formModeCheck.UseVisualStyleBackColor = true;
            this._formModeCheck.CheckedChanged += new System.EventHandler(this._formModeCheck_CheckedChanged);
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new System.Drawing.Point(8, 8);
            label1.Name = "label1";
            label1.Size = new System.Drawing.Size(66, 13);
            label1.TabIndex = 0;
            label1.Text = "Server URL:";
            // 
            // _serverUrlText
            // 
            this._serverUrlText.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this._serverUrlText.Location = new System.Drawing.Point(76, 4);
            this._serverUrlText.Name = "_serverUrlText";
            this._serverUrlText.Size = new System.Drawing.Size(604, 20);
            this._serverUrlText.TabIndex = 1;
            this._serverUrlText.Leave += new System.EventHandler(this._serverUrlText_Leave);
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(784, 561);
            this.Controls.Add(splitContainer1);
            this.Controls.Add(panel1);
            this.Name = "MainForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Saleslogix File Browser";
            contextMenuStrip.ResumeLayout(false);
            splitContainer1.Panel1.ResumeLayout(false);
            splitContainer1.Panel2.ResumeLayout(false);
            splitContainer1.Panel2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(splitContainer1)).EndInit();
            splitContainer1.ResumeLayout(false);
            splitContainer2.Panel1.ResumeLayout(false);
            splitContainer2.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(splitContainer2)).EndInit();
            splitContainer2.ResumeLayout(false);
            panel1.ResumeLayout(false);
            panel1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TreeView _treeView;
        private System.Windows.Forms.PropertyGrid _propGrid;
        private System.Windows.Forms.ToolStripMenuItem _openButton;
        private System.Windows.Forms.ToolStripMenuItem _deleteButton;
        private System.Windows.Forms.Button _saveButton;
        private System.Windows.Forms.TextBox _logText;
        private System.Windows.Forms.ToolStripMenuItem _newFolderButton;
        private System.Windows.Forms.ToolStripMenuItem _exportButton;
        private System.Windows.Forms.ToolStripMenuItem _refreshButton;
        private System.Windows.Forms.ToolStripMenuItem _importButton;
        private System.Windows.Forms.ToolStripMenuItem _importFolderButton;
        private System.Windows.Forms.ToolStripMenuItem _importFileButton;
        private System.Windows.Forms.ToolStripSeparator toolStripMenuItem1;
        private System.Windows.Forms.ToolStripMenuItem _cutButton;
        private System.Windows.Forms.ToolStripMenuItem _pasteButton;
        private System.Windows.Forms.ToolStripSeparator toolStripMenuItem2;
        private System.Windows.Forms.ToolStripMenuItem _replaceButton;
        private System.Windows.Forms.ImageList _imageList;
        private System.Windows.Forms.ToolStripMenuItem _expandAllButton;
        private System.Windows.Forms.TextBox _serverUrlText;
        private System.Windows.Forms.CheckBox _formModeCheck;
    }
}