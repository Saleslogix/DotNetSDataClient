namespace SDataClientApp
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
            this.statusLabel = new System.Windows.Forms.ToolStripStatusLabel();
            this.statusStrip = new System.Windows.Forms.StatusStrip();
            this.tabResourceProperties = new System.Windows.Forms.TabPage();
            this.resourceProperties1 = new SDataClientApp.ResourceProperties();
            this.tabSchema = new System.Windows.Forms.TabPage();
            this.resourceSchema1 = new SDataClientApp.ResourceSchema();
            this.tabTemplate = new System.Windows.Forms.TabPage();
            this.resourceTemplate1 = new SDataClientApp.ResourceTemplate();
            this.tabSingle = new System.Windows.Forms.TabPage();
            this.singleResource1 = new SDataClientApp.SingleResource();
            this.tabCollection = new System.Windows.Forms.TabPage();
            this.resourceCollection1 = new SDataClientApp.ResourceCollection();
            this.tabService = new System.Windows.Forms.TabPage();
            this.serviceConfig1 = new SDataClientApp.ServiceConfig();
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.statusStrip.SuspendLayout();
            this.tabResourceProperties.SuspendLayout();
            this.tabSchema.SuspendLayout();
            this.tabTemplate.SuspendLayout();
            this.tabSingle.SuspendLayout();
            this.tabCollection.SuspendLayout();
            this.tabService.SuspendLayout();
            this.tabControl1.SuspendLayout();
            this.SuspendLayout();
            // 
            // statusLabel
            // 
            this.statusLabel.Name = "statusLabel";
            this.statusLabel.Size = new System.Drawing.Size(0, 17);
            // 
            // statusStrip
            // 
            this.statusStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.statusLabel});
            this.statusStrip.Location = new System.Drawing.Point(0, 539);
            this.statusStrip.Name = "statusStrip";
            this.statusStrip.Size = new System.Drawing.Size(784, 22);
            this.statusStrip.TabIndex = 1;
            this.statusStrip.Text = "statusStrip";
            // 
            // tabResourceProperties
            // 
            this.tabResourceProperties.Controls.Add(this.resourceProperties1);
            this.tabResourceProperties.Location = new System.Drawing.Point(4, 22);
            this.tabResourceProperties.Name = "tabResourceProperties";
            this.tabResourceProperties.Padding = new System.Windows.Forms.Padding(3);
            this.tabResourceProperties.Size = new System.Drawing.Size(776, 513);
            this.tabResourceProperties.TabIndex = 6;
            this.tabResourceProperties.Text = "Resource Properties";
            this.tabResourceProperties.UseVisualStyleBackColor = true;
            // 
            // resourceProperties1
            // 
            this.resourceProperties1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.resourceProperties1.Location = new System.Drawing.Point(3, 3);
            this.resourceProperties1.Name = "resourceProperties1";
            this.resourceProperties1.Size = new System.Drawing.Size(770, 507);
            this.resourceProperties1.StatusLabel = this.statusLabel;
            this.resourceProperties1.TabIndex = 0;
            // 
            // tabSchema
            // 
            this.tabSchema.Controls.Add(this.resourceSchema1);
            this.tabSchema.Location = new System.Drawing.Point(4, 22);
            this.tabSchema.Name = "tabSchema";
            this.tabSchema.Padding = new System.Windows.Forms.Padding(3);
            this.tabSchema.Size = new System.Drawing.Size(776, 513);
            this.tabSchema.TabIndex = 4;
            this.tabSchema.Text = "Resource Schema";
            this.tabSchema.UseVisualStyleBackColor = true;
            // 
            // resourceSchema1
            // 
            this.resourceSchema1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.resourceSchema1.Location = new System.Drawing.Point(3, 3);
            this.resourceSchema1.Name = "resourceSchema1";
            this.resourceSchema1.Size = new System.Drawing.Size(770, 507);
            this.resourceSchema1.StatusLabel = this.statusLabel;
            this.resourceSchema1.TabIndex = 0;
            // 
            // tabTemplate
            // 
            this.tabTemplate.Controls.Add(this.resourceTemplate1);
            this.tabTemplate.Location = new System.Drawing.Point(4, 22);
            this.tabTemplate.Name = "tabTemplate";
            this.tabTemplate.Padding = new System.Windows.Forms.Padding(3);
            this.tabTemplate.Size = new System.Drawing.Size(776, 513);
            this.tabTemplate.TabIndex = 3;
            this.tabTemplate.Text = "Resource Template";
            this.tabTemplate.UseVisualStyleBackColor = true;
            // 
            // resourceTemplate1
            // 
            this.resourceTemplate1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.resourceTemplate1.Location = new System.Drawing.Point(3, 3);
            this.resourceTemplate1.Name = "resourceTemplate1";
            this.resourceTemplate1.Size = new System.Drawing.Size(770, 507);
            this.resourceTemplate1.StatusLabel = this.statusLabel;
            this.resourceTemplate1.TabIndex = 0;
            // 
            // tabSingle
            // 
            this.tabSingle.Controls.Add(this.singleResource1);
            this.tabSingle.Location = new System.Drawing.Point(4, 22);
            this.tabSingle.Name = "tabSingle";
            this.tabSingle.Padding = new System.Windows.Forms.Padding(3);
            this.tabSingle.Size = new System.Drawing.Size(776, 513);
            this.tabSingle.TabIndex = 2;
            this.tabSingle.Text = "Single Resource";
            this.tabSingle.UseVisualStyleBackColor = true;
            // 
            // singleResource1
            // 
            this.singleResource1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.singleResource1.Location = new System.Drawing.Point(3, 3);
            this.singleResource1.Name = "singleResource1";
            this.singleResource1.Size = new System.Drawing.Size(770, 507);
            this.singleResource1.StatusLabel = this.statusLabel;
            this.singleResource1.TabIndex = 0;
            // 
            // tabCollection
            // 
            this.tabCollection.Controls.Add(this.resourceCollection1);
            this.tabCollection.Location = new System.Drawing.Point(4, 22);
            this.tabCollection.Name = "tabCollection";
            this.tabCollection.Padding = new System.Windows.Forms.Padding(3);
            this.tabCollection.Size = new System.Drawing.Size(776, 513);
            this.tabCollection.TabIndex = 1;
            this.tabCollection.Text = "Resource Collection";
            this.tabCollection.UseVisualStyleBackColor = true;
            // 
            // resourceCollection1
            // 
            this.resourceCollection1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.resourceCollection1.Location = new System.Drawing.Point(3, 3);
            this.resourceCollection1.Name = "resourceCollection1";
            this.resourceCollection1.Size = new System.Drawing.Size(770, 507);
            this.resourceCollection1.StatusLabel = this.statusLabel;
            this.resourceCollection1.TabIndex = 0;
            // 
            // tabService
            // 
            this.tabService.Controls.Add(this.serviceConfig1);
            this.tabService.Location = new System.Drawing.Point(4, 22);
            this.tabService.Name = "tabService";
            this.tabService.Padding = new System.Windows.Forms.Padding(3);
            this.tabService.Size = new System.Drawing.Size(776, 513);
            this.tabService.TabIndex = 0;
            this.tabService.Text = "Service Config";
            this.tabService.UseVisualStyleBackColor = true;
            // 
            // serviceConfig1
            // 
            this.serviceConfig1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.serviceConfig1.Location = new System.Drawing.Point(3, 3);
            this.serviceConfig1.Name = "serviceConfig1";
            this.serviceConfig1.Size = new System.Drawing.Size(770, 507);
            this.serviceConfig1.StatusLabel = this.statusLabel;
            this.serviceConfig1.TabIndex = 0;
            // 
            // tabControl1
            // 
            this.tabControl1.Controls.Add(this.tabService);
            this.tabControl1.Controls.Add(this.tabCollection);
            this.tabControl1.Controls.Add(this.tabSingle);
            this.tabControl1.Controls.Add(this.tabTemplate);
            this.tabControl1.Controls.Add(this.tabSchema);
            this.tabControl1.Controls.Add(this.tabResourceProperties);
            this.tabControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tabControl1.Location = new System.Drawing.Point(0, 0);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(784, 539);
            this.tabControl1.TabIndex = 0;
            this.tabControl1.SelectedIndexChanged += new System.EventHandler(this.tabControl1_SelectedIndexChanged);
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(784, 561);
            this.Controls.Add(this.tabControl1);
            this.Controls.Add(this.statusStrip);
            this.Name = "MainForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "SData Client App";
            this.statusStrip.ResumeLayout(false);
            this.statusStrip.PerformLayout();
            this.tabResourceProperties.ResumeLayout(false);
            this.tabSchema.ResumeLayout(false);
            this.tabTemplate.ResumeLayout(false);
            this.tabSingle.ResumeLayout(false);
            this.tabCollection.ResumeLayout(false);
            this.tabService.ResumeLayout(false);
            this.tabControl1.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ToolStripStatusLabel statusLabel;
        private System.Windows.Forms.StatusStrip statusStrip;
        private System.Windows.Forms.TabPage tabResourceProperties;
        private System.Windows.Forms.TabPage tabSchema;
        private System.Windows.Forms.TabPage tabTemplate;
        private System.Windows.Forms.TabPage tabSingle;
        private System.Windows.Forms.TabPage tabCollection;
        private System.Windows.Forms.TabPage tabService;
        private System.Windows.Forms.TabControl tabControl1;
        private ResourceProperties resourceProperties1;
        private ResourceSchema resourceSchema1;
        private ResourceTemplate resourceTemplate1;
        private SingleResource singleResource1;
        private ResourceCollection resourceCollection1;
        private ServiceConfig serviceConfig1;
    }
}
