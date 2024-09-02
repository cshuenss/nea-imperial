
namespace NEA
{
    partial class ImportTypeSelectionForm
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
            this.btnArmy = new System.Windows.Forms.Button();
            this.btnFleet = new System.Windows.Forms.Button();
            this.lblImportChoice = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // btnArmy
            // 
            this.btnArmy.DialogResult = System.Windows.Forms.DialogResult.Abort;
            this.btnArmy.Location = new System.Drawing.Point(12, 81);
            this.btnArmy.Name = "btnArmy";
            this.btnArmy.Size = new System.Drawing.Size(94, 29);
            this.btnArmy.TabIndex = 0;
            this.btnArmy.Text = "Army";
            this.btnArmy.UseVisualStyleBackColor = true;
            // 
            // btnFleet
            // 
            this.btnFleet.DialogResult = System.Windows.Forms.DialogResult.Ignore;
            this.btnFleet.Location = new System.Drawing.Point(112, 81);
            this.btnFleet.Name = "btnFleet";
            this.btnFleet.Size = new System.Drawing.Size(94, 29);
            this.btnFleet.TabIndex = 1;
            this.btnFleet.Text = "Fleet";
            this.btnFleet.UseVisualStyleBackColor = true;
            // 
            // lblImportChoice
            // 
            this.lblImportChoice.AutoSize = true;
            this.lblImportChoice.Location = new System.Drawing.Point(31, 31);
            this.lblImportChoice.Name = "lblImportChoice";
            this.lblImportChoice.Size = new System.Drawing.Size(150, 20);
            this.lblImportChoice.TabIndex = 2;
            this.lblImportChoice.Text = "Import army or fleet?";
            // 
            // ImportTypeSelectionForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 20F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(215, 119);
            this.Controls.Add(this.lblImportChoice);
            this.Controls.Add(this.btnFleet);
            this.Controls.Add(this.btnArmy);
            this.Name = "ImportTypeSelectionForm";
            this.ShowIcon = false;
            this.Text = "Choose import";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button btnArmy;
        private System.Windows.Forms.Button btnFleet;
        private System.Windows.Forms.Label lblImportChoice;
    }
}