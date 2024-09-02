
namespace NEA
{
    partial class SettingsForm
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
            this.btnPlay = new System.Windows.Forms.Button();
            this.btnReturnToMenu = new System.Windows.Forms.Button();
            this.grpBoxRandomBonds = new System.Windows.Forms.GroupBox();
            this.radioSetBonds = new System.Windows.Forms.RadioButton();
            this.radioRandomBonds = new System.Windows.Forms.RadioButton();
            this.grpBoxBondCount = new System.Windows.Forms.GroupBox();
            this.radioEightBonds = new System.Windows.Forms.RadioButton();
            this.radioNineBonds = new System.Windows.Forms.RadioButton();
            this.btnSelectMap = new System.Windows.Forms.Button();
            this.lblPlayerCount = new System.Windows.Forms.Label();
            this.numUpDownPlayerCount = new System.Windows.Forms.NumericUpDown();
            this.lblSelectedFile = new System.Windows.Forms.Label();
            this.lblSettings = new System.Windows.Forms.Label();
            this.playerNamePanel = new System.Windows.Forms.Panel();
            this.lblPlayerNames = new System.Windows.Forms.Label();
            this.grpBoxRandomBonds.SuspendLayout();
            this.grpBoxBondCount.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numUpDownPlayerCount)).BeginInit();
            this.playerNamePanel.SuspendLayout();
            this.SuspendLayout();
            // 
            // btnPlay
            // 
            this.btnPlay.Location = new System.Drawing.Point(635, 472);
            this.btnPlay.Margin = new System.Windows.Forms.Padding(5, 6, 5, 6);
            this.btnPlay.Name = "btnPlay";
            this.btnPlay.Size = new System.Drawing.Size(129, 46);
            this.btnPlay.TabIndex = 0;
            this.btnPlay.Text = "Play";
            this.btnPlay.UseVisualStyleBackColor = true;
            this.btnPlay.Click += new System.EventHandler(this.btnPlay_Click);
            // 
            // btnReturnToMenu
            // 
            this.btnReturnToMenu.Location = new System.Drawing.Point(60, 472);
            this.btnReturnToMenu.Margin = new System.Windows.Forms.Padding(5, 6, 5, 6);
            this.btnReturnToMenu.Name = "btnReturnToMenu";
            this.btnReturnToMenu.Size = new System.Drawing.Size(129, 46);
            this.btnReturnToMenu.TabIndex = 1;
            this.btnReturnToMenu.Text = "Cancel";
            this.btnReturnToMenu.UseVisualStyleBackColor = true;
            this.btnReturnToMenu.Click += new System.EventHandler(this.btnReturnToMenu_Click);
            // 
            // grpBoxRandomBonds
            // 
            this.grpBoxRandomBonds.Controls.Add(this.radioSetBonds);
            this.grpBoxRandomBonds.Controls.Add(this.radioRandomBonds);
            this.grpBoxRandomBonds.Location = new System.Drawing.Point(61, 147);
            this.grpBoxRandomBonds.Name = "grpBoxRandomBonds";
            this.grpBoxRandomBonds.Size = new System.Drawing.Size(350, 111);
            this.grpBoxRandomBonds.TabIndex = 2;
            this.grpBoxRandomBonds.TabStop = false;
            this.grpBoxRandomBonds.Text = "Random or purchased bonds?";
            // 
            // radioSetBonds
            // 
            this.radioSetBonds.AutoSize = true;
            this.radioSetBonds.Location = new System.Drawing.Point(6, 74);
            this.radioSetBonds.Name = "radioSetBonds";
            this.radioSetBonds.Size = new System.Drawing.Size(296, 34);
            this.radioSetBonds.TabIndex = 1;
            this.radioSetBonds.TabStop = true;
            this.radioSetBonds.Text = "Start with purchasing bonds";
            this.radioSetBonds.UseVisualStyleBackColor = true;
            // 
            // radioRandomBonds
            // 
            this.radioRandomBonds.AutoSize = true;
            this.radioRandomBonds.Checked = true;
            this.radioRandomBonds.Location = new System.Drawing.Point(6, 34);
            this.radioRandomBonds.Name = "radioRandomBonds";
            this.radioRandomBonds.Size = new System.Drawing.Size(266, 34);
            this.radioRandomBonds.TabIndex = 0;
            this.radioRandomBonds.TabStop = true;
            this.radioRandomBonds.Text = "Start with random bonds";
            this.radioRandomBonds.UseVisualStyleBackColor = true;
            this.radioRandomBonds.CheckedChanged += new System.EventHandler(this.radioRandomBonds_CheckedChanged);
            // 
            // grpBoxBondCount
            // 
            this.grpBoxBondCount.Controls.Add(this.radioEightBonds);
            this.grpBoxBondCount.Controls.Add(this.radioNineBonds);
            this.grpBoxBondCount.Location = new System.Drawing.Point(61, 264);
            this.grpBoxBondCount.Name = "grpBoxBondCount";
            this.grpBoxBondCount.Size = new System.Drawing.Size(350, 113);
            this.grpBoxBondCount.TabIndex = 3;
            this.grpBoxBondCount.TabStop = false;
            this.grpBoxBondCount.Text = "Extra bond tier in gameplay?";
            // 
            // radioEightBonds
            // 
            this.radioEightBonds.AutoSize = true;
            this.radioEightBonds.Checked = true;
            this.radioEightBonds.Location = new System.Drawing.Point(6, 34);
            this.radioEightBonds.Name = "radioEightBonds";
            this.radioEightBonds.Size = new System.Drawing.Size(201, 34);
            this.radioEightBonds.TabIndex = 3;
            this.radioEightBonds.TabStop = true;
            this.radioEightBonds.Text = "Play with 8 bonds";
            this.radioEightBonds.UseVisualStyleBackColor = true;
            this.radioEightBonds.CheckedChanged += new System.EventHandler(this.radioEightBonds_CheckedChanged);
            // 
            // radioNineBonds
            // 
            this.radioNineBonds.AutoSize = true;
            this.radioNineBonds.Location = new System.Drawing.Point(6, 74);
            this.radioNineBonds.Name = "radioNineBonds";
            this.radioNineBonds.Size = new System.Drawing.Size(201, 34);
            this.radioNineBonds.TabIndex = 2;
            this.radioNineBonds.TabStop = true;
            this.radioNineBonds.Text = "Play with 9 bonds";
            this.radioNineBonds.UseVisualStyleBackColor = true;
            // 
            // btnSelectMap
            // 
            this.btnSelectMap.Location = new System.Drawing.Point(61, 396);
            this.btnSelectMap.Name = "btnSelectMap";
            this.btnSelectMap.Size = new System.Drawing.Size(167, 40);
            this.btnSelectMap.TabIndex = 4;
            this.btnSelectMap.Text = "Select map...";
            this.btnSelectMap.UseVisualStyleBackColor = true;
            this.btnSelectMap.Click += new System.EventHandler(this.btnSelectMap_Click);
            // 
            // lblPlayerCount
            // 
            this.lblPlayerCount.AutoSize = true;
            this.lblPlayerCount.Location = new System.Drawing.Point(100, 81);
            this.lblPlayerCount.Name = "lblPlayerCount";
            this.lblPlayerCount.Size = new System.Drawing.Size(128, 30);
            this.lblPlayerCount.TabIndex = 5;
            this.lblPlayerCount.Text = "Player count";
            // 
            // numUpDownPlayerCount
            // 
            this.numUpDownPlayerCount.Location = new System.Drawing.Point(261, 79);
            this.numUpDownPlayerCount.Name = "numUpDownPlayerCount";
            this.numUpDownPlayerCount.Size = new System.Drawing.Size(150, 35);
            this.numUpDownPlayerCount.TabIndex = 6;
            this.numUpDownPlayerCount.ValueChanged += new System.EventHandler(this.numUpDownPlayerCount_ValueChanged);
            // 
            // lblSelectedFile
            // 
            this.lblSelectedFile.AutoSize = true;
            this.lblSelectedFile.Location = new System.Drawing.Point(234, 401);
            this.lblSelectedFile.Name = "lblSelectedFile";
            this.lblSelectedFile.Size = new System.Drawing.Size(68, 30);
            this.lblSelectedFile.TabIndex = 7;
            this.lblSelectedFile.Text = "label2";
            // 
            // lblSettings
            // 
            this.lblSettings.Font = new System.Drawing.Font("Segoe UI", 14.14286F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this.lblSettings.Location = new System.Drawing.Point(1, -8);
            this.lblSettings.Name = "lblSettings";
            this.lblSettings.Size = new System.Drawing.Size(825, 84);
            this.lblSettings.TabIndex = 9;
            this.lblSettings.Text = "Game Settings";
            this.lblSettings.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // playerNamePanel
            // 
            this.playerNamePanel.AutoScroll = true;
            this.playerNamePanel.BackColor = System.Drawing.Color.WhiteSmoke;
            this.playerNamePanel.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.playerNamePanel.Controls.Add(this.lblPlayerNames);
            this.playerNamePanel.Location = new System.Drawing.Point(440, 79);
            this.playerNamePanel.Name = "playerNamePanel";
            this.playerNamePanel.Size = new System.Drawing.Size(324, 298);
            this.playerNamePanel.TabIndex = 10;
            // 
            // lblPlayerNames
            // 
            this.lblPlayerNames.Location = new System.Drawing.Point(3, 4);
            this.lblPlayerNames.Name = "lblPlayerNames";
            this.lblPlayerNames.Size = new System.Drawing.Size(316, 30);
            this.lblPlayerNames.TabIndex = 0;
            this.lblPlayerNames.Text = "Player names";
            this.lblPlayerNames.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            // 
            // SettingsForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(12F, 30F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(821, 553);
            this.Controls.Add(this.playerNamePanel);
            this.Controls.Add(this.lblSettings);
            this.Controls.Add(this.lblSelectedFile);
            this.Controls.Add(this.numUpDownPlayerCount);
            this.Controls.Add(this.lblPlayerCount);
            this.Controls.Add(this.btnSelectMap);
            this.Controls.Add(this.grpBoxBondCount);
            this.Controls.Add(this.grpBoxRandomBonds);
            this.Controls.Add(this.btnReturnToMenu);
            this.Controls.Add(this.btnPlay);
            this.Margin = new System.Windows.Forms.Padding(5, 6, 5, 6);
            this.Name = "SettingsForm";
            this.Text = "Settings";
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.SettingsFormFormClosed);
            this.grpBoxRandomBonds.ResumeLayout(false);
            this.grpBoxRandomBonds.PerformLayout();
            this.grpBoxBondCount.ResumeLayout(false);
            this.grpBoxBondCount.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numUpDownPlayerCount)).EndInit();
            this.playerNamePanel.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button btnPlay;
        private System.Windows.Forms.Button btnReturnToMenu;
        private System.Windows.Forms.GroupBox grpBoxRandomBonds;
        private System.Windows.Forms.GroupBox grpBoxBondCount;
        private System.Windows.Forms.Button btnSelectMap;
        private System.Windows.Forms.Label lblPlayerCount;
        private System.Windows.Forms.NumericUpDown numUpDownPlayerCount;
        private System.Windows.Forms.Label lblSelectedFile;
        private System.Windows.Forms.Label lblSettings;
        private System.Windows.Forms.Panel playerNamePanel;
        private System.Windows.Forms.RadioButton radioSetBonds;
        private System.Windows.Forms.RadioButton radioRandomBonds;
        private System.Windows.Forms.RadioButton radioEightBonds;
        private System.Windows.Forms.RadioButton radioNineBonds;
        private System.Windows.Forms.Label lblPlayerNames;
    }
}