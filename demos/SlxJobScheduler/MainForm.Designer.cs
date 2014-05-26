namespace SlxJobScheduler
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
            System.Windows.Forms.Label label1;
            System.Windows.Forms.Label label2;
            System.Windows.Forms.Label label3;
            System.Windows.Forms.Label label4;
            System.Windows.Forms.Label label6;
            System.Windows.Forms.Panel panel1;
            System.Windows.Forms.Panel panel2;
            System.Windows.Forms.Panel panel3;
            System.Windows.Forms.SplitContainer splitContainer1;
            System.Windows.Forms.SplitContainer splitContainer2;
            System.Windows.Forms.SplitContainer splitContainer4;
            System.Windows.Forms.Label label5;
            System.Windows.Forms.Label label9;
            System.Windows.Forms.SplitContainer splitContainer5;
            System.Windows.Forms.SplitContainer splitContainer3;
            System.Windows.Forms.SplitContainer splitContainer6;
            System.Windows.Forms.Label label7;
            System.Windows.Forms.Label label8;
            this._refreshAllJobsButton = new System.Windows.Forms.Button();
            this._refreshJobButton = new System.Windows.Forms.Button();
            this._triggerJobButton = new System.Windows.Forms.Button();
            this._interuptJobButton = new System.Windows.Forms.Button();
            this._resumeJobButton = new System.Windows.Forms.Button();
            this._pauseJobButton = new System.Windows.Forms.Button();
            this._refreshAllTriggersButton = new System.Windows.Forms.Button();
            this._deleteTriggerButton = new System.Windows.Forms.Button();
            this._updateTriggerButton = new System.Windows.Forms.Button();
            this._resumeTriggerButton = new System.Windows.Forms.Button();
            this._createTriggerButton = new System.Windows.Forms.Button();
            this._pauseTriggerButton = new System.Windows.Forms.Button();
            this._refreshTriggerButton = new System.Windows.Forms.Button();
            this._refreshAllExecutionsButton = new System.Windows.Forms.Button();
            this._refreshExecutionButton = new System.Windows.Forms.Button();
            this._interruptExecutionButton = new System.Windows.Forms.Button();
            this._deleteExecutionButton = new System.Windows.Forms.Button();
            this._resultExecutionButton = new System.Windows.Forms.Button();
            this._jobsGrid = new System.Windows.Forms.DataGridView();
            this.splitContainer7 = new System.Windows.Forms.SplitContainer();
            this._jobParametersGrid = new System.Windows.Forms.DataGridView();
            this._jobStateGrid = new System.Windows.Forms.DataGridView();
            this._triggersGrid = new System.Windows.Forms.DataGridView();
            this._triggerParametersGrid = new System.Windows.Forms.DataGridView();
            this._executionsGrid = new System.Windows.Forms.DataGridView();
            this._executionStateGrid = new System.Windows.Forms.DataGridView();
            this._logText = new System.Windows.Forms.TextBox();
            this._serverUrlText = new System.Windows.Forms.TextBox();
            label1 = new System.Windows.Forms.Label();
            label2 = new System.Windows.Forms.Label();
            label3 = new System.Windows.Forms.Label();
            label4 = new System.Windows.Forms.Label();
            label6 = new System.Windows.Forms.Label();
            panel1 = new System.Windows.Forms.Panel();
            panel2 = new System.Windows.Forms.Panel();
            panel3 = new System.Windows.Forms.Panel();
            splitContainer1 = new System.Windows.Forms.SplitContainer();
            splitContainer2 = new System.Windows.Forms.SplitContainer();
            splitContainer4 = new System.Windows.Forms.SplitContainer();
            label5 = new System.Windows.Forms.Label();
            label9 = new System.Windows.Forms.Label();
            splitContainer5 = new System.Windows.Forms.SplitContainer();
            splitContainer3 = new System.Windows.Forms.SplitContainer();
            splitContainer6 = new System.Windows.Forms.SplitContainer();
            label7 = new System.Windows.Forms.Label();
            label8 = new System.Windows.Forms.Label();
            panel1.SuspendLayout();
            panel2.SuspendLayout();
            panel3.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(splitContainer1)).BeginInit();
            splitContainer1.Panel1.SuspendLayout();
            splitContainer1.Panel2.SuspendLayout();
            splitContainer1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(splitContainer2)).BeginInit();
            splitContainer2.Panel1.SuspendLayout();
            splitContainer2.Panel2.SuspendLayout();
            splitContainer2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(splitContainer4)).BeginInit();
            splitContainer4.Panel1.SuspendLayout();
            splitContainer4.Panel2.SuspendLayout();
            splitContainer4.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this._jobsGrid)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer7)).BeginInit();
            this.splitContainer7.Panel1.SuspendLayout();
            this.splitContainer7.Panel2.SuspendLayout();
            this.splitContainer7.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this._jobParametersGrid)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this._jobStateGrid)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(splitContainer5)).BeginInit();
            splitContainer5.Panel1.SuspendLayout();
            splitContainer5.Panel2.SuspendLayout();
            splitContainer5.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this._triggersGrid)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this._triggerParametersGrid)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(splitContainer3)).BeginInit();
            splitContainer3.Panel1.SuspendLayout();
            splitContainer3.Panel2.SuspendLayout();
            splitContainer3.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(splitContainer6)).BeginInit();
            splitContainer6.Panel1.SuspendLayout();
            splitContainer6.Panel2.SuspendLayout();
            splitContainer6.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this._executionsGrid)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this._executionStateGrid)).BeginInit();
            this.SuspendLayout();
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new System.Drawing.Point(8, 12);
            label1.Name = "label1";
            label1.Size = new System.Drawing.Size(66, 13);
            label1.TabIndex = 0;
            label1.Text = "Server URL:";
            // 
            // label2
            // 
            label2.Dock = System.Windows.Forms.DockStyle.Top;
            label2.Location = new System.Drawing.Point(0, 0);
            label2.Name = "label2";
            label2.Size = new System.Drawing.Size(513, 16);
            label2.TabIndex = 0;
            label2.Text = "Jobs:";
            // 
            // label3
            // 
            label3.Dock = System.Windows.Forms.DockStyle.Top;
            label3.Location = new System.Drawing.Point(0, 0);
            label3.Name = "label3";
            label3.Size = new System.Drawing.Size(616, 16);
            label3.TabIndex = 0;
            label3.Text = "Triggers:";
            // 
            // label4
            // 
            label4.Dock = System.Windows.Forms.DockStyle.Top;
            label4.Location = new System.Drawing.Point(0, 0);
            label4.Name = "label4";
            label4.Size = new System.Drawing.Size(968, 16);
            label4.TabIndex = 0;
            label4.Text = "Log:";
            // 
            // label6
            // 
            label6.Dock = System.Windows.Forms.DockStyle.Top;
            label6.Location = new System.Drawing.Point(0, 0);
            label6.Name = "label6";
            label6.Size = new System.Drawing.Size(264, 16);
            label6.TabIndex = 0;
            label6.Text = "Parameters:";
            // 
            // panel1
            // 
            panel1.Controls.Add(this._refreshAllJobsButton);
            panel1.Controls.Add(this._refreshJobButton);
            panel1.Controls.Add(this._triggerJobButton);
            panel1.Controls.Add(this._interuptJobButton);
            panel1.Controls.Add(this._resumeJobButton);
            panel1.Controls.Add(this._pauseJobButton);
            panel1.Dock = System.Windows.Forms.DockStyle.Right;
            panel1.Location = new System.Drawing.Point(884, 0);
            panel1.Name = "panel1";
            panel1.Size = new System.Drawing.Size(84, 173);
            panel1.TabIndex = 1;
            // 
            // _refreshAllJobsButton
            // 
            this._refreshAllJobsButton.Location = new System.Drawing.Point(4, 4);
            this._refreshAllJobsButton.Name = "_refreshAllJobsButton";
            this._refreshAllJobsButton.Size = new System.Drawing.Size(75, 23);
            this._refreshAllJobsButton.TabIndex = 0;
            this._refreshAllJobsButton.Text = "Refresh All";
            this._refreshAllJobsButton.UseVisualStyleBackColor = true;
            this._refreshAllJobsButton.Click += new System.EventHandler(this._refreshAllJobsButton_Click);
            // 
            // _refreshJobButton
            // 
            this._refreshJobButton.Enabled = false;
            this._refreshJobButton.Location = new System.Drawing.Point(4, 32);
            this._refreshJobButton.Name = "_refreshJobButton";
            this._refreshJobButton.Size = new System.Drawing.Size(75, 23);
            this._refreshJobButton.TabIndex = 1;
            this._refreshJobButton.Text = "Refresh";
            this._refreshJobButton.UseVisualStyleBackColor = true;
            this._refreshJobButton.Click += new System.EventHandler(this._refreshJobButton_Click);
            // 
            // _triggerJobButton
            // 
            this._triggerJobButton.Enabled = false;
            this._triggerJobButton.Location = new System.Drawing.Point(4, 60);
            this._triggerJobButton.Name = "_triggerJobButton";
            this._triggerJobButton.Size = new System.Drawing.Size(75, 23);
            this._triggerJobButton.TabIndex = 2;
            this._triggerJobButton.Text = "Trigger";
            this._triggerJobButton.UseVisualStyleBackColor = true;
            this._triggerJobButton.Click += new System.EventHandler(this._triggerJobButton_Click);
            // 
            // _interuptJobButton
            // 
            this._interuptJobButton.Enabled = false;
            this._interuptJobButton.Location = new System.Drawing.Point(4, 88);
            this._interuptJobButton.Name = "_interuptJobButton";
            this._interuptJobButton.Size = new System.Drawing.Size(75, 23);
            this._interuptJobButton.TabIndex = 3;
            this._interuptJobButton.Text = "Interrupt";
            this._interuptJobButton.UseVisualStyleBackColor = true;
            this._interuptJobButton.Click += new System.EventHandler(this._interuptJobButton_Click);
            // 
            // _resumeJobButton
            // 
            this._resumeJobButton.Enabled = false;
            this._resumeJobButton.Location = new System.Drawing.Point(4, 144);
            this._resumeJobButton.Name = "_resumeJobButton";
            this._resumeJobButton.Size = new System.Drawing.Size(75, 23);
            this._resumeJobButton.TabIndex = 5;
            this._resumeJobButton.Text = "Resume";
            this._resumeJobButton.UseVisualStyleBackColor = true;
            this._resumeJobButton.Click += new System.EventHandler(this._resumeJobButton_Click);
            // 
            // _pauseJobButton
            // 
            this._pauseJobButton.Enabled = false;
            this._pauseJobButton.Location = new System.Drawing.Point(4, 116);
            this._pauseJobButton.Name = "_pauseJobButton";
            this._pauseJobButton.Size = new System.Drawing.Size(75, 23);
            this._pauseJobButton.TabIndex = 4;
            this._pauseJobButton.Text = "Pause";
            this._pauseJobButton.UseVisualStyleBackColor = true;
            this._pauseJobButton.Click += new System.EventHandler(this._pauseJobButton_Click);
            // 
            // panel2
            // 
            panel2.Controls.Add(this._refreshAllTriggersButton);
            panel2.Controls.Add(this._deleteTriggerButton);
            panel2.Controls.Add(this._updateTriggerButton);
            panel2.Controls.Add(this._resumeTriggerButton);
            panel2.Controls.Add(this._createTriggerButton);
            panel2.Controls.Add(this._pauseTriggerButton);
            panel2.Controls.Add(this._refreshTriggerButton);
            panel2.Dock = System.Windows.Forms.DockStyle.Right;
            panel2.Location = new System.Drawing.Point(884, 0);
            panel2.Name = "panel2";
            panel2.Size = new System.Drawing.Size(84, 200);
            panel2.TabIndex = 1;
            // 
            // _refreshAllTriggersButton
            // 
            this._refreshAllTriggersButton.Location = new System.Drawing.Point(4, 4);
            this._refreshAllTriggersButton.Name = "_refreshAllTriggersButton";
            this._refreshAllTriggersButton.Size = new System.Drawing.Size(75, 23);
            this._refreshAllTriggersButton.TabIndex = 0;
            this._refreshAllTriggersButton.Text = "Refresh All";
            this._refreshAllTriggersButton.UseVisualStyleBackColor = true;
            this._refreshAllTriggersButton.Click += new System.EventHandler(this._refreshAllTriggersButton_Click);
            // 
            // _deleteTriggerButton
            // 
            this._deleteTriggerButton.Enabled = false;
            this._deleteTriggerButton.Location = new System.Drawing.Point(4, 172);
            this._deleteTriggerButton.Name = "_deleteTriggerButton";
            this._deleteTriggerButton.Size = new System.Drawing.Size(75, 23);
            this._deleteTriggerButton.TabIndex = 6;
            this._deleteTriggerButton.Text = "Delete";
            this._deleteTriggerButton.UseVisualStyleBackColor = true;
            this._deleteTriggerButton.Click += new System.EventHandler(this._deleteTriggerButton_Click);
            // 
            // _updateTriggerButton
            // 
            this._updateTriggerButton.Enabled = false;
            this._updateTriggerButton.Location = new System.Drawing.Point(4, 144);
            this._updateTriggerButton.Name = "_updateTriggerButton";
            this._updateTriggerButton.Size = new System.Drawing.Size(75, 23);
            this._updateTriggerButton.TabIndex = 5;
            this._updateTriggerButton.Text = "Update";
            this._updateTriggerButton.UseVisualStyleBackColor = true;
            this._updateTriggerButton.Click += new System.EventHandler(this._updateTriggerButton_Click);
            // 
            // _resumeTriggerButton
            // 
            this._resumeTriggerButton.Enabled = false;
            this._resumeTriggerButton.Location = new System.Drawing.Point(4, 88);
            this._resumeTriggerButton.Name = "_resumeTriggerButton";
            this._resumeTriggerButton.Size = new System.Drawing.Size(75, 23);
            this._resumeTriggerButton.TabIndex = 3;
            this._resumeTriggerButton.Text = "Resume";
            this._resumeTriggerButton.UseVisualStyleBackColor = true;
            this._resumeTriggerButton.Click += new System.EventHandler(this._resumeTriggerButton_Click);
            // 
            // _createTriggerButton
            // 
            this._createTriggerButton.Enabled = false;
            this._createTriggerButton.Location = new System.Drawing.Point(4, 116);
            this._createTriggerButton.Name = "_createTriggerButton";
            this._createTriggerButton.Size = new System.Drawing.Size(75, 23);
            this._createTriggerButton.TabIndex = 4;
            this._createTriggerButton.Text = "Create";
            this._createTriggerButton.UseVisualStyleBackColor = true;
            this._createTriggerButton.Click += new System.EventHandler(this._createTriggerButton_Click);
            // 
            // _pauseTriggerButton
            // 
            this._pauseTriggerButton.Enabled = false;
            this._pauseTriggerButton.Location = new System.Drawing.Point(4, 60);
            this._pauseTriggerButton.Name = "_pauseTriggerButton";
            this._pauseTriggerButton.Size = new System.Drawing.Size(75, 23);
            this._pauseTriggerButton.TabIndex = 2;
            this._pauseTriggerButton.Text = "Pause";
            this._pauseTriggerButton.UseVisualStyleBackColor = true;
            this._pauseTriggerButton.Click += new System.EventHandler(this._pauseTriggerButton_Click);
            // 
            // _refreshTriggerButton
            // 
            this._refreshTriggerButton.Enabled = false;
            this._refreshTriggerButton.Location = new System.Drawing.Point(4, 32);
            this._refreshTriggerButton.Name = "_refreshTriggerButton";
            this._refreshTriggerButton.Size = new System.Drawing.Size(75, 23);
            this._refreshTriggerButton.TabIndex = 1;
            this._refreshTriggerButton.Text = "Refresh";
            this._refreshTriggerButton.UseVisualStyleBackColor = true;
            this._refreshTriggerButton.Click += new System.EventHandler(this._refreshTriggerButton_Click);
            // 
            // panel3
            // 
            panel3.Controls.Add(this._refreshAllExecutionsButton);
            panel3.Controls.Add(this._refreshExecutionButton);
            panel3.Controls.Add(this._interruptExecutionButton);
            panel3.Controls.Add(this._deleteExecutionButton);
            panel3.Controls.Add(this._resultExecutionButton);
            panel3.Dock = System.Windows.Forms.DockStyle.Right;
            panel3.Location = new System.Drawing.Point(884, 0);
            panel3.Name = "panel3";
            panel3.Size = new System.Drawing.Size(84, 176);
            panel3.TabIndex = 1;
            // 
            // _refreshAllExecutionsButton
            // 
            this._refreshAllExecutionsButton.Location = new System.Drawing.Point(4, 4);
            this._refreshAllExecutionsButton.Name = "_refreshAllExecutionsButton";
            this._refreshAllExecutionsButton.Size = new System.Drawing.Size(75, 23);
            this._refreshAllExecutionsButton.TabIndex = 0;
            this._refreshAllExecutionsButton.Text = "Refresh All";
            this._refreshAllExecutionsButton.UseVisualStyleBackColor = true;
            this._refreshAllExecutionsButton.Click += new System.EventHandler(this._refreshAllExecutionsButton_Click);
            // 
            // _refreshExecutionButton
            // 
            this._refreshExecutionButton.Enabled = false;
            this._refreshExecutionButton.Location = new System.Drawing.Point(4, 32);
            this._refreshExecutionButton.Name = "_refreshExecutionButton";
            this._refreshExecutionButton.Size = new System.Drawing.Size(75, 23);
            this._refreshExecutionButton.TabIndex = 1;
            this._refreshExecutionButton.Text = "Refresh";
            this._refreshExecutionButton.UseVisualStyleBackColor = true;
            this._refreshExecutionButton.Click += new System.EventHandler(this._refreshExecutionButton_Click);
            // 
            // _interruptExecutionButton
            // 
            this._interruptExecutionButton.Enabled = false;
            this._interruptExecutionButton.Location = new System.Drawing.Point(4, 88);
            this._interruptExecutionButton.Name = "_interruptExecutionButton";
            this._interruptExecutionButton.Size = new System.Drawing.Size(75, 23);
            this._interruptExecutionButton.TabIndex = 3;
            this._interruptExecutionButton.Text = "Interrupt";
            this._interruptExecutionButton.UseVisualStyleBackColor = true;
            this._interruptExecutionButton.Click += new System.EventHandler(this._interruptExecutionButton_Click);
            // 
            // _deleteExecutionButton
            // 
            this._deleteExecutionButton.Enabled = false;
            this._deleteExecutionButton.Location = new System.Drawing.Point(4, 116);
            this._deleteExecutionButton.Name = "_deleteExecutionButton";
            this._deleteExecutionButton.Size = new System.Drawing.Size(75, 23);
            this._deleteExecutionButton.TabIndex = 4;
            this._deleteExecutionButton.Text = "Delete";
            this._deleteExecutionButton.UseVisualStyleBackColor = true;
            this._deleteExecutionButton.Click += new System.EventHandler(this._deleteExecutionButton_Click);
            // 
            // _resultExecutionButton
            // 
            this._resultExecutionButton.Enabled = false;
            this._resultExecutionButton.Location = new System.Drawing.Point(4, 60);
            this._resultExecutionButton.Name = "_resultExecutionButton";
            this._resultExecutionButton.Size = new System.Drawing.Size(75, 23);
            this._resultExecutionButton.TabIndex = 2;
            this._resultExecutionButton.Text = "Result";
            this._resultExecutionButton.UseVisualStyleBackColor = true;
            this._resultExecutionButton.Click += new System.EventHandler(this._resultExecutionButton_Click);
            // 
            // splitContainer1
            // 
            splitContainer1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            splitContainer1.Location = new System.Drawing.Point(8, 36);
            splitContainer1.Name = "splitContainer1";
            splitContainer1.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // splitContainer1.Panel1
            // 
            splitContainer1.Panel1.Controls.Add(splitContainer2);
            // 
            // splitContainer1.Panel2
            // 
            splitContainer1.Panel2.Controls.Add(splitContainer3);
            splitContainer1.Size = new System.Drawing.Size(968, 717);
            splitContainer1.SplitterDistance = 377;
            splitContainer1.TabIndex = 2;
            // 
            // splitContainer2
            // 
            splitContainer2.Dock = System.Windows.Forms.DockStyle.Fill;
            splitContainer2.Location = new System.Drawing.Point(0, 0);
            splitContainer2.Name = "splitContainer2";
            splitContainer2.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // splitContainer2.Panel1
            // 
            splitContainer2.Panel1.Controls.Add(splitContainer4);
            splitContainer2.Panel1.Controls.Add(panel1);
            // 
            // splitContainer2.Panel2
            // 
            splitContainer2.Panel2.Controls.Add(splitContainer5);
            splitContainer2.Panel2.Controls.Add(panel2);
            splitContainer2.Size = new System.Drawing.Size(968, 377);
            splitContainer2.SplitterDistance = 173;
            splitContainer2.TabIndex = 0;
            // 
            // splitContainer4
            // 
            splitContainer4.Dock = System.Windows.Forms.DockStyle.Fill;
            splitContainer4.Location = new System.Drawing.Point(0, 0);
            splitContainer4.Name = "splitContainer4";
            // 
            // splitContainer4.Panel1
            // 
            splitContainer4.Panel1.Controls.Add(this._jobsGrid);
            splitContainer4.Panel1.Controls.Add(label2);
            // 
            // splitContainer4.Panel2
            // 
            splitContainer4.Panel2.Controls.Add(this.splitContainer7);
            splitContainer4.Size = new System.Drawing.Size(884, 173);
            splitContainer4.SplitterDistance = 513;
            splitContainer4.TabIndex = 0;
            // 
            // _jobsGrid
            // 
            this._jobsGrid.AllowUserToAddRows = false;
            this._jobsGrid.AllowUserToDeleteRows = false;
            this._jobsGrid.AllowUserToResizeRows = false;
            this._jobsGrid.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this._jobsGrid.Dock = System.Windows.Forms.DockStyle.Fill;
            this._jobsGrid.Location = new System.Drawing.Point(0, 16);
            this._jobsGrid.Name = "_jobsGrid";
            this._jobsGrid.ReadOnly = true;
            this._jobsGrid.RowHeadersWidth = 20;
            this._jobsGrid.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this._jobsGrid.Size = new System.Drawing.Size(513, 157);
            this._jobsGrid.TabIndex = 1;
            this._jobsGrid.SelectionChanged += new System.EventHandler(this._jobsGrid_SelectionChanged);
            // 
            // splitContainer7
            // 
            this.splitContainer7.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer7.Location = new System.Drawing.Point(0, 0);
            this.splitContainer7.Name = "splitContainer7";
            this.splitContainer7.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // splitContainer7.Panel1
            // 
            this.splitContainer7.Panel1.Controls.Add(this._jobParametersGrid);
            this.splitContainer7.Panel1.Controls.Add(label5);
            // 
            // splitContainer7.Panel2
            // 
            this.splitContainer7.Panel2.Controls.Add(this._jobStateGrid);
            this.splitContainer7.Panel2.Controls.Add(label9);
            this.splitContainer7.Size = new System.Drawing.Size(367, 173);
            this.splitContainer7.SplitterDistance = 94;
            this.splitContainer7.TabIndex = 0;
            // 
            // _jobParametersGrid
            // 
            this._jobParametersGrid.AllowUserToAddRows = false;
            this._jobParametersGrid.AllowUserToDeleteRows = false;
            this._jobParametersGrid.AllowUserToResizeRows = false;
            this._jobParametersGrid.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this._jobParametersGrid.Dock = System.Windows.Forms.DockStyle.Fill;
            this._jobParametersGrid.Location = new System.Drawing.Point(0, 16);
            this._jobParametersGrid.MultiSelect = false;
            this._jobParametersGrid.Name = "_jobParametersGrid";
            this._jobParametersGrid.RowHeadersWidth = 20;
            this._jobParametersGrid.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this._jobParametersGrid.Size = new System.Drawing.Size(367, 78);
            this._jobParametersGrid.TabIndex = 1;
            // 
            // label5
            // 
            label5.Dock = System.Windows.Forms.DockStyle.Top;
            label5.Location = new System.Drawing.Point(0, 0);
            label5.Name = "label5";
            label5.Size = new System.Drawing.Size(367, 16);
            label5.TabIndex = 0;
            label5.Text = "Parameters:";
            // 
            // _jobStateGrid
            // 
            this._jobStateGrid.AllowUserToAddRows = false;
            this._jobStateGrid.AllowUserToDeleteRows = false;
            this._jobStateGrid.AllowUserToResizeRows = false;
            this._jobStateGrid.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this._jobStateGrid.Dock = System.Windows.Forms.DockStyle.Fill;
            this._jobStateGrid.Location = new System.Drawing.Point(0, 16);
            this._jobStateGrid.MultiSelect = false;
            this._jobStateGrid.Name = "_jobStateGrid";
            this._jobStateGrid.ReadOnly = true;
            this._jobStateGrid.RowHeadersWidth = 20;
            this._jobStateGrid.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this._jobStateGrid.Size = new System.Drawing.Size(367, 59);
            this._jobStateGrid.TabIndex = 1;
            // 
            // label9
            // 
            label9.Dock = System.Windows.Forms.DockStyle.Top;
            label9.Location = new System.Drawing.Point(0, 0);
            label9.Name = "label9";
            label9.Size = new System.Drawing.Size(367, 16);
            label9.TabIndex = 0;
            label9.Text = "State:";
            // 
            // splitContainer5
            // 
            splitContainer5.Dock = System.Windows.Forms.DockStyle.Fill;
            splitContainer5.Location = new System.Drawing.Point(0, 0);
            splitContainer5.Name = "splitContainer5";
            // 
            // splitContainer5.Panel1
            // 
            splitContainer5.Panel1.Controls.Add(this._triggersGrid);
            splitContainer5.Panel1.Controls.Add(label3);
            // 
            // splitContainer5.Panel2
            // 
            splitContainer5.Panel2.Controls.Add(this._triggerParametersGrid);
            splitContainer5.Panel2.Controls.Add(label6);
            splitContainer5.Size = new System.Drawing.Size(884, 200);
            splitContainer5.SplitterDistance = 616;
            splitContainer5.TabIndex = 0;
            // 
            // _triggersGrid
            // 
            this._triggersGrid.AllowUserToAddRows = false;
            this._triggersGrid.AllowUserToDeleteRows = false;
            this._triggersGrid.AllowUserToResizeRows = false;
            this._triggersGrid.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this._triggersGrid.Dock = System.Windows.Forms.DockStyle.Fill;
            this._triggersGrid.Location = new System.Drawing.Point(0, 16);
            this._triggersGrid.Name = "_triggersGrid";
            this._triggersGrid.ReadOnly = true;
            this._triggersGrid.RowHeadersWidth = 20;
            this._triggersGrid.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this._triggersGrid.Size = new System.Drawing.Size(616, 184);
            this._triggersGrid.TabIndex = 1;
            this._triggersGrid.SelectionChanged += new System.EventHandler(this._triggersGrid_SelectionChanged);
            // 
            // _triggerParametersGrid
            // 
            this._triggerParametersGrid.AllowUserToAddRows = false;
            this._triggerParametersGrid.AllowUserToDeleteRows = false;
            this._triggerParametersGrid.AllowUserToResizeRows = false;
            this._triggerParametersGrid.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this._triggerParametersGrid.Dock = System.Windows.Forms.DockStyle.Fill;
            this._triggerParametersGrid.Location = new System.Drawing.Point(0, 16);
            this._triggerParametersGrid.MultiSelect = false;
            this._triggerParametersGrid.Name = "_triggerParametersGrid";
            this._triggerParametersGrid.ReadOnly = true;
            this._triggerParametersGrid.RowHeadersWidth = 20;
            this._triggerParametersGrid.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this._triggerParametersGrid.Size = new System.Drawing.Size(264, 184);
            this._triggerParametersGrid.TabIndex = 1;
            // 
            // splitContainer3
            // 
            splitContainer3.Dock = System.Windows.Forms.DockStyle.Fill;
            splitContainer3.Location = new System.Drawing.Point(0, 0);
            splitContainer3.Name = "splitContainer3";
            splitContainer3.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // splitContainer3.Panel1
            // 
            splitContainer3.Panel1.Controls.Add(splitContainer6);
            splitContainer3.Panel1.Controls.Add(panel3);
            // 
            // splitContainer3.Panel2
            // 
            splitContainer3.Panel2.Controls.Add(this._logText);
            splitContainer3.Panel2.Controls.Add(label4);
            splitContainer3.Size = new System.Drawing.Size(968, 336);
            splitContainer3.SplitterDistance = 176;
            splitContainer3.TabIndex = 1;
            // 
            // splitContainer6
            // 
            splitContainer6.Dock = System.Windows.Forms.DockStyle.Fill;
            splitContainer6.Location = new System.Drawing.Point(0, 0);
            splitContainer6.Name = "splitContainer6";
            // 
            // splitContainer6.Panel1
            // 
            splitContainer6.Panel1.Controls.Add(this._executionsGrid);
            splitContainer6.Panel1.Controls.Add(label7);
            // 
            // splitContainer6.Panel2
            // 
            splitContainer6.Panel2.Controls.Add(this._executionStateGrid);
            splitContainer6.Panel2.Controls.Add(label8);
            splitContainer6.Size = new System.Drawing.Size(884, 176);
            splitContainer6.SplitterDistance = 616;
            splitContainer6.TabIndex = 0;
            // 
            // _executionsGrid
            // 
            this._executionsGrid.AllowUserToAddRows = false;
            this._executionsGrid.AllowUserToDeleteRows = false;
            this._executionsGrid.AllowUserToResizeRows = false;
            this._executionsGrid.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this._executionsGrid.Dock = System.Windows.Forms.DockStyle.Fill;
            this._executionsGrid.Location = new System.Drawing.Point(0, 16);
            this._executionsGrid.Name = "_executionsGrid";
            this._executionsGrid.ReadOnly = true;
            this._executionsGrid.RowHeadersWidth = 20;
            this._executionsGrid.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this._executionsGrid.Size = new System.Drawing.Size(616, 160);
            this._executionsGrid.TabIndex = 1;
            this._executionsGrid.SelectionChanged += new System.EventHandler(this._executionsGrid_SelectionChanged);
            // 
            // label7
            // 
            label7.Dock = System.Windows.Forms.DockStyle.Top;
            label7.Location = new System.Drawing.Point(0, 0);
            label7.Name = "label7";
            label7.Size = new System.Drawing.Size(616, 16);
            label7.TabIndex = 0;
            label7.Text = "Executions:";
            // 
            // _executionStateGrid
            // 
            this._executionStateGrid.AllowUserToAddRows = false;
            this._executionStateGrid.AllowUserToDeleteRows = false;
            this._executionStateGrid.AllowUserToResizeRows = false;
            this._executionStateGrid.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this._executionStateGrid.Dock = System.Windows.Forms.DockStyle.Fill;
            this._executionStateGrid.Location = new System.Drawing.Point(0, 16);
            this._executionStateGrid.MultiSelect = false;
            this._executionStateGrid.Name = "_executionStateGrid";
            this._executionStateGrid.ReadOnly = true;
            this._executionStateGrid.RowHeadersWidth = 20;
            this._executionStateGrid.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this._executionStateGrid.Size = new System.Drawing.Size(264, 160);
            this._executionStateGrid.TabIndex = 1;
            // 
            // label8
            // 
            label8.Dock = System.Windows.Forms.DockStyle.Top;
            label8.Location = new System.Drawing.Point(0, 0);
            label8.Name = "label8";
            label8.Size = new System.Drawing.Size(264, 16);
            label8.TabIndex = 0;
            label8.Text = "State:";
            // 
            // _logText
            // 
            this._logText.Dock = System.Windows.Forms.DockStyle.Fill;
            this._logText.Font = new System.Drawing.Font("Courier New", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this._logText.Location = new System.Drawing.Point(0, 16);
            this._logText.Multiline = true;
            this._logText.Name = "_logText";
            this._logText.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this._logText.Size = new System.Drawing.Size(968, 140);
            this._logText.TabIndex = 1;
            this._logText.WordWrap = false;
            // 
            // _serverUrlText
            // 
            this._serverUrlText.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this._serverUrlText.Location = new System.Drawing.Point(80, 8);
            this._serverUrlText.Name = "_serverUrlText";
            this._serverUrlText.Size = new System.Drawing.Size(892, 20);
            this._serverUrlText.TabIndex = 1;
            this._serverUrlText.Leave += new System.EventHandler(this._serverUrlText_Leave);
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(984, 761);
            this.Controls.Add(splitContainer1);
            this.Controls.Add(this._serverUrlText);
            this.Controls.Add(label1);
            this.Name = "MainForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Saleslogix Job Scheduler";
            panel1.ResumeLayout(false);
            panel2.ResumeLayout(false);
            panel3.ResumeLayout(false);
            splitContainer1.Panel1.ResumeLayout(false);
            splitContainer1.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(splitContainer1)).EndInit();
            splitContainer1.ResumeLayout(false);
            splitContainer2.Panel1.ResumeLayout(false);
            splitContainer2.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(splitContainer2)).EndInit();
            splitContainer2.ResumeLayout(false);
            splitContainer4.Panel1.ResumeLayout(false);
            splitContainer4.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(splitContainer4)).EndInit();
            splitContainer4.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this._jobsGrid)).EndInit();
            this.splitContainer7.Panel1.ResumeLayout(false);
            this.splitContainer7.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer7)).EndInit();
            this.splitContainer7.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this._jobParametersGrid)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this._jobStateGrid)).EndInit();
            splitContainer5.Panel1.ResumeLayout(false);
            splitContainer5.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(splitContainer5)).EndInit();
            splitContainer5.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this._triggersGrid)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this._triggerParametersGrid)).EndInit();
            splitContainer3.Panel1.ResumeLayout(false);
            splitContainer3.Panel2.ResumeLayout(false);
            splitContainer3.Panel2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(splitContainer3)).EndInit();
            splitContainer3.ResumeLayout(false);
            splitContainer6.Panel1.ResumeLayout(false);
            splitContainer6.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(splitContainer6)).EndInit();
            splitContainer6.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this._executionsGrid)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this._executionStateGrid)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox _serverUrlText;
        private System.Windows.Forms.DataGridView _jobsGrid;
        private System.Windows.Forms.DataGridView _triggersGrid;
        private System.Windows.Forms.TextBox _logText;
        private System.Windows.Forms.Button _refreshAllTriggersButton;
        private System.Windows.Forms.Button _refreshAllJobsButton;
        private System.Windows.Forms.Button _refreshJobButton;
        private System.Windows.Forms.Button _triggerJobButton;
        private System.Windows.Forms.Button _interuptJobButton;
        private System.Windows.Forms.Button _pauseJobButton;
        private System.Windows.Forms.Button _resumeJobButton;
        private System.Windows.Forms.Button _deleteTriggerButton;
        private System.Windows.Forms.Button _refreshTriggerButton;
        private System.Windows.Forms.Button _pauseTriggerButton;
        private System.Windows.Forms.Button _resumeTriggerButton;
        private System.Windows.Forms.Button _updateTriggerButton;
        private System.Windows.Forms.Button _createTriggerButton;
        private System.Windows.Forms.DataGridView _jobParametersGrid;
        private System.Windows.Forms.DataGridView _triggerParametersGrid;
        private System.Windows.Forms.DataGridView _executionsGrid;
        private System.Windows.Forms.Button _refreshAllExecutionsButton;
        private System.Windows.Forms.Button _refreshExecutionButton;
        private System.Windows.Forms.Button _interruptExecutionButton;
        private System.Windows.Forms.Button _resultExecutionButton;
        private System.Windows.Forms.Button _deleteExecutionButton;
        private System.Windows.Forms.DataGridView _executionStateGrid;
        private System.Windows.Forms.SplitContainer splitContainer7;
        private System.Windows.Forms.DataGridView _jobStateGrid;
    }
}