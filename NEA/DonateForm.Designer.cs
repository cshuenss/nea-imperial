namespace NEA
{
    partial class DonateForm
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
            this.comboBoxNation = new System.Windows.Forms.ComboBox();
            this.lblNation = new System.Windows.Forms.Label();
            this.lblPlayer = new System.Windows.Forms.Label();
            this.comboBoxPlayer = new System.Windows.Forms.ComboBox();
            this.numUpDownMoney = new System.Windows.Forms.NumericUpDown();
            this.lblPlayerMoney = new System.Windows.Forms.Label();
            this.lblNationMoney = new System.Windows.Forms.Label();
            this.btnConfirmTransfer = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.numUpDownMoney)).BeginInit();
            this.SuspendLayout();
            // 
            // comboBoxNation
            // 
            this.comboBoxNation.FormattingEnabled = true;
            this.comboBoxNation.Location = new System.Drawing.Point(336, 58);
            this.comboBoxNation.Name = "comboBoxNation";
            this.comboBoxNation.Size = new System.Drawing.Size(212, 38);
            this.comboBoxNation.TabIndex = 1;
            this.comboBoxNation.SelectedIndexChanged += new System.EventHandler(this.comboBox_SelectedIndexChanged);
            // 
            // lblNation
            // 
            this.lblNation.AutoSize = true;
            this.lblNation.Location = new System.Drawing.Point(472, 16);
            this.lblNation.Name = "lblNation";
            this.lblNation.Size = new System.Drawing.Size(76, 30);
            this.lblNation.TabIndex = 2;
            this.lblNation.Text = "Nation";
            // 
            // lblPlayer
            // 
            this.lblPlayer.AutoSize = true;
            this.lblPlayer.Location = new System.Drawing.Point(12, 16);
            this.lblPlayer.Name = "lblPlayer";
            this.lblPlayer.Size = new System.Drawing.Size(69, 30);
            this.lblPlayer.TabIndex = 3;
            this.lblPlayer.Text = "Player";
            // 
            // comboBoxPlayer
            // 
            this.comboBoxPlayer.FormattingEnabled = true;
            this.comboBoxPlayer.Location = new System.Drawing.Point(12, 58);
            this.comboBoxPlayer.Name = "comboBoxPlayer";
            this.comboBoxPlayer.Size = new System.Drawing.Size(212, 38);
            this.comboBoxPlayer.TabIndex = 0;
            this.comboBoxPlayer.SelectedIndexChanged += new System.EventHandler(this.comboBox_SelectedIndexChanged);
            // 
            // numUpDownMoney
            // 
            this.numUpDownMoney.Location = new System.Drawing.Point(230, 16);
            this.numUpDownMoney.Name = "numUpDownMoney";
            this.numUpDownMoney.Size = new System.Drawing.Size(100, 35);
            this.numUpDownMoney.TabIndex = 6;
            // 
            // lblPlayerMoney
            // 
            this.lblPlayerMoney.AutoSize = true;
            this.lblPlayerMoney.Location = new System.Drawing.Point(12, 112);
            this.lblPlayerMoney.Name = "lblPlayerMoney";
            this.lblPlayerMoney.Size = new System.Drawing.Size(68, 30);
            this.lblPlayerMoney.TabIndex = 4;
            this.lblPlayerMoney.Text = "label3";
            // 
            // lblNationMoney
            // 
            this.lblNationMoney.AutoSize = true;
            this.lblNationMoney.Location = new System.Drawing.Point(336, 112);
            this.lblNationMoney.Name = "lblNationMoney";
            this.lblNationMoney.Size = new System.Drawing.Size(68, 30);
            this.lblNationMoney.TabIndex = 5;
            this.lblNationMoney.Text = "label4";
            // 
            // btnConfirmTransfer
            // 
            this.btnConfirmTransfer.Location = new System.Drawing.Point(417, 149);
            this.btnConfirmTransfer.Name = "btnConfirmTransfer";
            this.btnConfirmTransfer.Size = new System.Drawing.Size(131, 40);
            this.btnConfirmTransfer.TabIndex = 7;
            this.btnConfirmTransfer.Text = "OK";
            this.btnConfirmTransfer.UseVisualStyleBackColor = true;
            this.btnConfirmTransfer.Click += new System.EventHandler(this.btnConfirmTransfer_Click);
            // 
            // DonateForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(12F, 30F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(560, 201);
            this.Controls.Add(this.btnConfirmTransfer);
            this.Controls.Add(this.numUpDownMoney);
            this.Controls.Add(this.lblNationMoney);
            this.Controls.Add(this.lblPlayerMoney);
            this.Controls.Add(this.lblPlayer);
            this.Controls.Add(this.lblNation);
            this.Controls.Add(this.comboBoxNation);
            this.Controls.Add(this.comboBoxPlayer);
            this.Name = "DonateForm";
            this.Text = "Donate money";
            this.Paint += new System.Windows.Forms.PaintEventHandler(this.DonateForm_Paint);
            ((System.ComponentModel.ISupportInitialize)(this.numUpDownMoney)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.ComboBox comboBoxNation;
        private System.Windows.Forms.Label lblNation;
        private System.Windows.Forms.Label lblPlayer;
        private System.Windows.Forms.ComboBox comboBoxPlayer;
        private System.Windows.Forms.NumericUpDown numUpDownMoney;
        private System.Windows.Forms.Label lblPlayerMoney;
        private System.Windows.Forms.Label lblNationMoney;
        private System.Windows.Forms.Button btnConfirmTransfer;
    }
}