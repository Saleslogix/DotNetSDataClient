namespace SlxJobScheduler
{
    partial class ResultForm
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
            this._resultText = new System.Windows.Forms.TextBox();
            this.SuspendLayout();
            // 
            // _resultText
            // 
            this._resultText.Dock = System.Windows.Forms.DockStyle.Fill;
            this._resultText.Font = new System.Drawing.Font("Courier New", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this._resultText.Location = new System.Drawing.Point(0, 0);
            this._resultText.Multiline = true;
            this._resultText.Name = "_resultText";
            this._resultText.ReadOnly = true;
            this._resultText.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this._resultText.Size = new System.Drawing.Size(384, 461);
            this._resultText.TabIndex = 0;
            this._resultText.WordWrap = false;
            // 
            // ResultForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(384, 461);
            this.Controls.Add(this._resultText);
            this.KeyPreview = true;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "ResultForm";
            this.ShowIcon = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Result";
            this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.ResultForm_KeyDown);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox _resultText;

    }
}