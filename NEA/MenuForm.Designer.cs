
namespace NEA
{
    partial class MenuForm
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
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
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.btnMenuExit = new System.Windows.Forms.Button();
            this.btnOpenSettings = new System.Windows.Forms.Button();
            this.lblTitle = new System.Windows.Forms.Label();
            this.btnMenuRules = new System.Windows.Forms.Button();
            this.lblCaption = new System.Windows.Forms.Label();
            this.lblDebugActive = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // btnMenuExit
            // 
            this.btnMenuExit.Location = new System.Drawing.Point(287, 83);
            this.btnMenuExit.Margin = new System.Windows.Forms.Padding(4, 6, 4, 6);
            this.btnMenuExit.Name = "btnMenuExit";
            this.btnMenuExit.Size = new System.Drawing.Size(129, 46);
            this.btnMenuExit.TabIndex = 0;
            this.btnMenuExit.Text = "Quit";
            this.btnMenuExit.UseVisualStyleBackColor = true;
            this.btnMenuExit.Click += new System.EventHandler(this.btnMenuExit_Click);
            // 
            // btnOpenSettings
            // 
            this.btnOpenSettings.Location = new System.Drawing.Point(150, 83);
            this.btnOpenSettings.Margin = new System.Windows.Forms.Padding(4, 6, 4, 6);
            this.btnOpenSettings.Name = "btnOpenSettings";
            this.btnOpenSettings.Size = new System.Drawing.Size(129, 46);
            this.btnOpenSettings.TabIndex = 1;
            this.btnOpenSettings.Text = "Start";
            this.btnOpenSettings.UseVisualStyleBackColor = true;
            this.btnOpenSettings.Click += new System.EventHandler(this.btnOpenSettings_Click);
            // 
            // lblTitle
            // 
            this.lblTitle.AutoSize = true;
            this.lblTitle.Font = new System.Drawing.Font("Segoe UI Semibold", 12F, ((System.Drawing.FontStyle)((System.Drawing.FontStyle.Bold | System.Drawing.FontStyle.Italic))), System.Drawing.GraphicsUnit.Point);
            this.lblTitle.ForeColor = System.Drawing.Color.Firebrick;
            this.lblTitle.Location = new System.Drawing.Point(153, 9);
            this.lblTitle.Name = "lblTitle";
            this.lblTitle.Size = new System.Drawing.Size(123, 38);
            this.lblTitle.TabIndex = 2;
            this.lblTitle.Text = "Imperial";
            // 
            // btnMenuRules
            // 
            this.btnMenuRules.Location = new System.Drawing.Point(13, 83);
            this.btnMenuRules.Margin = new System.Windows.Forms.Padding(4, 6, 4, 6);
            this.btnMenuRules.Name = "btnMenuRules";
            this.btnMenuRules.Size = new System.Drawing.Size(129, 46);
            this.btnMenuRules.TabIndex = 3;
            this.btnMenuRules.Text = "Rules";
            this.btnMenuRules.UseVisualStyleBackColor = true;
            this.btnMenuRules.Click += new System.EventHandler(this.btnMenuRules_Click);
            // 
            // lblCaption
            // 
            this.lblCaption.AutoSize = true;
            this.lblCaption.Cursor = System.Windows.Forms.Cursors.No;
            this.lblCaption.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Italic, System.Drawing.GraphicsUnit.Point);
            this.lblCaption.ForeColor = System.Drawing.Color.DarkRed;
            this.lblCaption.Location = new System.Drawing.Point(136, 47);
            this.lblCaption.Name = "lblCaption";
            this.lblCaption.Size = new System.Drawing.Size(155, 30);
            this.lblCaption.TabIndex = 4;
            this.lblCaption.Text = "the board game";
            this.lblCaption.Click += new System.EventHandler(this.lblCaption_Click);
            // 
            // lblDebugActive
            // 
            this.lblDebugActive.AutoSize = true;
            this.lblDebugActive.Font = new System.Drawing.Font("Comic Sans MS", 8.142858F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.lblDebugActive.ForeColor = System.Drawing.Color.BlueViolet;
            this.lblDebugActive.Location = new System.Drawing.Point(291, 9);
            this.lblDebugActive.Name = "lblDebugActive";
            this.lblDebugActive.Size = new System.Drawing.Size(125, 28);
            this.lblDebugActive.TabIndex = 5;
            this.lblDebugActive.Text = "Debug mode";
            // 
            // MenuForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(12F, 30F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(428, 141);
            this.Controls.Add(this.lblDebugActive);
            this.Controls.Add(this.lblCaption);
            this.Controls.Add(this.btnMenuRules);
            this.Controls.Add(this.lblTitle);
            this.Controls.Add(this.btnOpenSettings);
            this.Controls.Add(this.btnMenuExit);
            this.Margin = new System.Windows.Forms.Padding(4, 6, 4, 6);
            this.Name = "MenuForm";
            this.Text = "Imperial";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button btnMenuExit;
        private System.Windows.Forms.Button btnOpenSettings;
        private System.Windows.Forms.Label lblTitle;
        private System.Windows.Forms.Button btnMenuRules;
        private System.Windows.Forms.Label lblCaption;
        private System.Windows.Forms.Label lblDebugActive;
    }
}

