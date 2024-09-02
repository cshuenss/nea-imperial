
namespace NEA
{
    partial class CombatForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(CombatForm));
            this.numUpDownDefenseFactories = new System.Windows.Forms.NumericUpDown();
            this.numUpDownDefenseArmies = new System.Windows.Forms.NumericUpDown();
            this.numUpDownDefenseFleets = new System.Windows.Forms.NumericUpDown();
            this.numUpDownAttackFleets = new System.Windows.Forms.NumericUpDown();
            this.numUpDownAttackArmies = new System.Windows.Forms.NumericUpDown();
            this.lblCurrentNation = new System.Windows.Forms.Label();
            this.lblDefenseFleets = new System.Windows.Forms.Label();
            this.lblDefenseFactories = new System.Windows.Forms.Label();
            this.lblDefenseNation = new System.Windows.Forms.Label();
            this.lblAttackArmies = new System.Windows.Forms.Label();
            this.lblAttackFleets = new System.Windows.Forms.Label();
            this.lblDefenseArmies = new System.Windows.Forms.Label();
            this.btnConfirmCombat = new System.Windows.Forms.Button();
            this.btnSkipCombat = new System.Windows.Forms.Button();
            this.lblAttackerTotal = new System.Windows.Forms.Label();
            this.lblDefenseTotal = new System.Windows.Forms.Label();
            this.lblCurrentTerritory = new System.Windows.Forms.Label();
            this.comboBoxDefenseNations = new System.Windows.Forms.ComboBox();
            this.lblAttackIndicatorLabel = new System.Windows.Forms.Label();
            this.lblCombatInfoLabel = new System.Windows.Forms.Label();
            this.btnContinueCombat = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.numUpDownDefenseFactories)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numUpDownDefenseArmies)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numUpDownDefenseFleets)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numUpDownAttackFleets)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numUpDownAttackArmies)).BeginInit();
            this.SuspendLayout();
            // 
            // numUpDownDefenseFactories
            // 
            this.numUpDownDefenseFactories.Location = new System.Drawing.Point(526, 195);
            this.numUpDownDefenseFactories.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.numUpDownDefenseFactories.Name = "numUpDownDefenseFactories";
            this.numUpDownDefenseFactories.Size = new System.Drawing.Size(112, 35);
            this.numUpDownDefenseFactories.TabIndex = 1;
            this.numUpDownDefenseFactories.ValueChanged += new System.EventHandler(this.UpdateTotalUnitsLabels);
            // 
            // numUpDownDefenseArmies
            // 
            this.numUpDownDefenseArmies.Location = new System.Drawing.Point(284, 195);
            this.numUpDownDefenseArmies.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.numUpDownDefenseArmies.Name = "numUpDownDefenseArmies";
            this.numUpDownDefenseArmies.Size = new System.Drawing.Size(112, 35);
            this.numUpDownDefenseArmies.TabIndex = 2;
            this.numUpDownDefenseArmies.ValueChanged += new System.EventHandler(this.UpdateTotalUnitsLabels);
            // 
            // numUpDownDefenseFleets
            // 
            this.numUpDownDefenseFleets.Location = new System.Drawing.Point(405, 195);
            this.numUpDownDefenseFleets.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.numUpDownDefenseFleets.Name = "numUpDownDefenseFleets";
            this.numUpDownDefenseFleets.Size = new System.Drawing.Size(112, 35);
            this.numUpDownDefenseFleets.TabIndex = 3;
            this.numUpDownDefenseFleets.ValueChanged += new System.EventHandler(this.UpdateTotalUnitsLabels);
            // 
            // numUpDownAttackFleets
            // 
            this.numUpDownAttackFleets.Location = new System.Drawing.Point(140, 195);
            this.numUpDownAttackFleets.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.numUpDownAttackFleets.Name = "numUpDownAttackFleets";
            this.numUpDownAttackFleets.Size = new System.Drawing.Size(112, 35);
            this.numUpDownAttackFleets.TabIndex = 4;
            this.numUpDownAttackFleets.ValueChanged += new System.EventHandler(this.UpdateTotalUnitsLabels);
            // 
            // numUpDownAttackArmies
            // 
            this.numUpDownAttackArmies.Location = new System.Drawing.Point(18, 195);
            this.numUpDownAttackArmies.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.numUpDownAttackArmies.Name = "numUpDownAttackArmies";
            this.numUpDownAttackArmies.Size = new System.Drawing.Size(112, 35);
            this.numUpDownAttackArmies.TabIndex = 5;
            this.numUpDownAttackArmies.ValueChanged += new System.EventHandler(this.UpdateTotalUnitsLabels);
            // 
            // lblCurrentNation
            // 
            this.lblCurrentNation.AutoSize = true;
            this.lblCurrentNation.Location = new System.Drawing.Point(14, 112);
            this.lblCurrentNation.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblCurrentNation.Name = "lblCurrentNation";
            this.lblCurrentNation.Size = new System.Drawing.Size(68, 30);
            this.lblCurrentNation.TabIndex = 6;
            this.lblCurrentNation.Text = "label1";
            // 
            // lblDefenseFleets
            // 
            this.lblDefenseFleets.AutoSize = true;
            this.lblDefenseFleets.Location = new System.Drawing.Point(405, 160);
            this.lblDefenseFleets.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblDefenseFleets.Name = "lblDefenseFleets";
            this.lblDefenseFleets.Size = new System.Drawing.Size(66, 30);
            this.lblDefenseFleets.TabIndex = 7;
            this.lblDefenseFleets.Text = "Fleets";
            // 
            // lblDefenseFactories
            // 
            this.lblDefenseFactories.AutoSize = true;
            this.lblDefenseFactories.Location = new System.Drawing.Point(526, 160);
            this.lblDefenseFactories.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblDefenseFactories.Name = "lblDefenseFactories";
            this.lblDefenseFactories.Size = new System.Drawing.Size(94, 30);
            this.lblDefenseFactories.TabIndex = 8;
            this.lblDefenseFactories.Text = "Factories";
            // 
            // lblDefenseNation
            // 
            this.lblDefenseNation.AutoSize = true;
            this.lblDefenseNation.Location = new System.Drawing.Point(284, 63);
            this.lblDefenseNation.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblDefenseNation.Name = "lblDefenseNation";
            this.lblDefenseNation.Size = new System.Drawing.Size(168, 30);
            this.lblDefenseNation.TabIndex = 9;
            this.lblDefenseNation.Text = "Opposing nation";
            // 
            // lblAttackArmies
            // 
            this.lblAttackArmies.AutoSize = true;
            this.lblAttackArmies.Location = new System.Drawing.Point(18, 160);
            this.lblAttackArmies.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblAttackArmies.Name = "lblAttackArmies";
            this.lblAttackArmies.Size = new System.Drawing.Size(77, 30);
            this.lblAttackArmies.TabIndex = 10;
            this.lblAttackArmies.Text = "Armies";
            // 
            // lblAttackFleets
            // 
            this.lblAttackFleets.AutoSize = true;
            this.lblAttackFleets.Location = new System.Drawing.Point(140, 160);
            this.lblAttackFleets.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblAttackFleets.Name = "lblAttackFleets";
            this.lblAttackFleets.Size = new System.Drawing.Size(66, 30);
            this.lblAttackFleets.TabIndex = 11;
            this.lblAttackFleets.Text = "Fleets";
            // 
            // lblDefenseArmies
            // 
            this.lblDefenseArmies.AutoSize = true;
            this.lblDefenseArmies.Location = new System.Drawing.Point(284, 160);
            this.lblDefenseArmies.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblDefenseArmies.Name = "lblDefenseArmies";
            this.lblDefenseArmies.Size = new System.Drawing.Size(77, 30);
            this.lblDefenseArmies.TabIndex = 12;
            this.lblDefenseArmies.Text = "Armies";
            // 
            // btnConfirmCombat
            // 
            this.btnConfirmCombat.Location = new System.Drawing.Point(411, 274);
            this.btnConfirmCombat.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.btnConfirmCombat.Name = "btnConfirmCombat";
            this.btnConfirmCombat.Size = new System.Drawing.Size(141, 44);
            this.btnConfirmCombat.TabIndex = 13;
            this.btnConfirmCombat.Text = "Confirm";
            this.btnConfirmCombat.UseVisualStyleBackColor = true;
            this.btnConfirmCombat.Click += new System.EventHandler(this.btnConfirmCombat_Click);
            // 
            // btnSkipCombat
            // 
            this.btnSkipCombat.Location = new System.Drawing.Point(111, 274);
            this.btnSkipCombat.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.btnSkipCombat.Name = "btnSkipCombat";
            this.btnSkipCombat.Size = new System.Drawing.Size(141, 44);
            this.btnSkipCombat.TabIndex = 14;
            this.btnSkipCombat.Text = "Skip";
            this.btnSkipCombat.UseVisualStyleBackColor = true;
            this.btnSkipCombat.Click += new System.EventHandler(this.btnSkipCombat_Click);
            // 
            // lblAttackerTotal
            // 
            this.lblAttackerTotal.AutoSize = true;
            this.lblAttackerTotal.Location = new System.Drawing.Point(18, 240);
            this.lblAttackerTotal.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblAttackerTotal.Name = "lblAttackerTotal";
            this.lblAttackerTotal.Size = new System.Drawing.Size(79, 30);
            this.lblAttackerTotal.TabIndex = 15;
            this.lblAttackerTotal.Text = "Total: 0";
            // 
            // lblDefenseTotal
            // 
            this.lblDefenseTotal.AutoSize = true;
            this.lblDefenseTotal.Location = new System.Drawing.Point(284, 240);
            this.lblDefenseTotal.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblDefenseTotal.Name = "lblDefenseTotal";
            this.lblDefenseTotal.Size = new System.Drawing.Size(79, 30);
            this.lblDefenseTotal.TabIndex = 16;
            this.lblDefenseTotal.Text = "Total: 0";
            // 
            // lblCurrentTerritory
            // 
            this.lblCurrentTerritory.AutoSize = true;
            this.lblCurrentTerritory.Location = new System.Drawing.Point(14, 9);
            this.lblCurrentTerritory.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblCurrentTerritory.Name = "lblCurrentTerritory";
            this.lblCurrentTerritory.Size = new System.Drawing.Size(68, 30);
            this.lblCurrentTerritory.TabIndex = 17;
            this.lblCurrentTerritory.Text = "label1";
            // 
            // comboBoxDefenseNations
            // 
            this.comboBoxDefenseNations.FormattingEnabled = true;
            this.comboBoxDefenseNations.Location = new System.Drawing.Point(284, 112);
            this.comboBoxDefenseNations.Name = "comboBoxDefenseNations";
            this.comboBoxDefenseNations.Size = new System.Drawing.Size(252, 38);
            this.comboBoxDefenseNations.TabIndex = 18;
            this.comboBoxDefenseNations.SelectedIndexChanged += new System.EventHandler(this.comboBoxDefenseNations_SelectedIndexChanged);
            // 
            // lblAttackIndicatorLabel
            // 
            this.lblAttackIndicatorLabel.AutoSize = true;
            this.lblAttackIndicatorLabel.Location = new System.Drawing.Point(14, 63);
            this.lblAttackIndicatorLabel.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblAttackIndicatorLabel.Name = "lblAttackIndicatorLabel";
            this.lblAttackIndicatorLabel.Size = new System.Drawing.Size(166, 30);
            this.lblAttackIndicatorLabel.TabIndex = 19;
            this.lblAttackIndicatorLabel.Text = "Attacking nation";
            // 
            // lblCombatInfoLabel
            // 
            this.lblCombatInfoLabel.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Italic, System.Drawing.GraphicsUnit.Point);
            this.lblCombatInfoLabel.ForeColor = System.Drawing.SystemColors.ControlDarkDark;
            this.lblCombatInfoLabel.Location = new System.Drawing.Point(14, 322);
            this.lblCombatInfoLabel.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblCombatInfoLabel.Name = "lblCombatInfoLabel";
            this.lblCombatInfoLabel.Size = new System.Drawing.Size(626, 150);
            this.lblCombatInfoLabel.TabIndex = 20;
            this.lblCombatInfoLabel.Text = resources.GetString("lblCombatInfoLabel.Text");
            this.lblCombatInfoLabel.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            // 
            // btnContinueCombat
            // 
            this.btnContinueCombat.Location = new System.Drawing.Point(261, 274);
            this.btnContinueCombat.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.btnContinueCombat.Name = "btnContinueCombat";
            this.btnContinueCombat.Size = new System.Drawing.Size(141, 44);
            this.btnContinueCombat.TabIndex = 21;
            this.btnContinueCombat.Text = "Continue";
            this.btnContinueCombat.UseVisualStyleBackColor = true;
            this.btnContinueCombat.Click += new System.EventHandler(this.btnContinueCombat_Click);
            // 
            // CombatForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(12F, 30F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(657, 486);
            this.Controls.Add(this.btnContinueCombat);
            this.Controls.Add(this.lblCombatInfoLabel);
            this.Controls.Add(this.lblAttackIndicatorLabel);
            this.Controls.Add(this.comboBoxDefenseNations);
            this.Controls.Add(this.lblCurrentTerritory);
            this.Controls.Add(this.lblDefenseTotal);
            this.Controls.Add(this.lblAttackerTotal);
            this.Controls.Add(this.btnSkipCombat);
            this.Controls.Add(this.btnConfirmCombat);
            this.Controls.Add(this.lblDefenseArmies);
            this.Controls.Add(this.lblAttackFleets);
            this.Controls.Add(this.lblAttackArmies);
            this.Controls.Add(this.lblDefenseNation);
            this.Controls.Add(this.lblDefenseFactories);
            this.Controls.Add(this.lblDefenseFleets);
            this.Controls.Add(this.lblCurrentNation);
            this.Controls.Add(this.numUpDownAttackArmies);
            this.Controls.Add(this.numUpDownAttackFleets);
            this.Controls.Add(this.numUpDownDefenseFleets);
            this.Controls.Add(this.numUpDownDefenseArmies);
            this.Controls.Add(this.numUpDownDefenseFactories);
            this.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.Name = "CombatForm";
            this.Text = "Resolve Combat";
            ((System.ComponentModel.ISupportInitialize)(this.numUpDownDefenseFactories)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numUpDownDefenseArmies)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numUpDownDefenseFleets)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numUpDownAttackFleets)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numUpDownAttackArmies)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.DomainUpDown txtUpDownDefenseSelect;
        private System.Windows.Forms.NumericUpDown numUpDownDefenseFactories;
        private System.Windows.Forms.NumericUpDown numUpDownDefenseArmies;
        private System.Windows.Forms.NumericUpDown numUpDownDefenseFleets;
        private System.Windows.Forms.NumericUpDown numUpDownAttackFleets;
        private System.Windows.Forms.NumericUpDown numUpDownAttackArmies;
        private System.Windows.Forms.Label lblCurrentNation;
        private System.Windows.Forms.Label lblDefenseFleets;
        private System.Windows.Forms.Label lblDefenseFactories;
        private System.Windows.Forms.Label lblDefenseNation;
        private System.Windows.Forms.Label lblAttackArmies;
        private System.Windows.Forms.Label lblAttackFleets;
        private System.Windows.Forms.Label lblDefenseArmies;
        private System.Windows.Forms.Button btnConfirmCombat;
        private System.Windows.Forms.Button btnSkipCombat;
        private System.Windows.Forms.Label lblAttackerTotal;
        private System.Windows.Forms.Label lblDefenseTotal;
        private System.Windows.Forms.Label lblCurrentTerritory;
        private System.Windows.Forms.ComboBox comboBoxDefenseNations;
        private System.Windows.Forms.Label lblAttackIndicatorLabel;
        private System.Windows.Forms.Label lblCombatInfoLabel;
        private System.Windows.Forms.Button btnContinueCombat;
    }
}