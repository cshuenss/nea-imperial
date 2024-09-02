using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace NEA
{
    public partial class DonateForm : Form
    {
        List<Nation> nations;
        List<Player> players;

        public DonateForm(List<Nation> in_nations, List<Player> in_players)
        {
            InitializeComponent();

            players = in_players;
            nations = in_nations;

            PopulateComboBoxes();
            // Other initiating functions are called by the event being triggered when the combo box values change
            // so there is no need for them to be called here
        }

        private void DonateForm_Paint(object sender, PaintEventArgs e)
        {
            Pen pen = new Pen(nations[comboBoxNation.SelectedIndex].primaryColour, 12);
            pen.EndCap = System.Drawing.Drawing2D.LineCap.ArrowAnchor;
            e.Graphics.DrawLine(pen, 230, 77, 330, 77);
        }

        private void PopulateComboBoxes()
        {
            for (int i = 0; i < nations.Count; i++)
            {
                comboBoxNation.Items.Add(nations[i].name);
            }
            for (int i = 0; i < players.Count; i++)
            {
                comboBoxPlayer.Items.Add($"[{i + 1}] {players[i].name}");
            }
            comboBoxPlayer.SelectedIndex = 0;
            comboBoxNation.SelectedIndex = 0;
        }

        private void PopulateMoneyNumericUpDown()
        {
            numUpDownMoney.Value = 0;
            if ((comboBoxNation.SelectedIndex != -1) && (comboBoxPlayer.SelectedIndex != -1))
            {
                numUpDownMoney.Maximum = players[comboBoxPlayer.SelectedIndex].money;
            }
        }

        private void SetLabelValues()
        {
            if ((comboBoxNation.SelectedIndex != -1) && (comboBoxPlayer.SelectedIndex != -1))
            {
                lblNationMoney.Text = $"Treasury: {nations[comboBoxNation.SelectedIndex].treasury}";
                lblPlayerMoney.Text = $"Money: {players[comboBoxPlayer.SelectedIndex].money}";
            }
        }

        private void comboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            PopulateMoneyNumericUpDown();
            SetLabelValues();
            Refresh();
        }

        private void btnConfirmTransfer_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.OK;
            this.Close();
        }

        public List<int> GetDonation()
        {
            List<int> returnList = new List<int>();
            returnList.Add(comboBoxPlayer.SelectedIndex);
            returnList.Add(comboBoxNation.SelectedIndex);
            returnList.Add((int)numUpDownMoney.Value);
            return returnList;
        }
    }
}
