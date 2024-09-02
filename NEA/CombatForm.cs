using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace NEA
{
    public partial class CombatForm : Form
    {
        // 0 to continue, 1 to stop
        int continueFlag = 0;

        int nation;
        int homeFactories;
        Territory territory;
        List<Nation> nations;
        List<int> comboBoxIndexes = new List<int>();

        public CombatForm(Territory in_territory, List<Nation> in_nations, int in_nation, int in_factories)
        {
            nation = in_nation;
            territory = in_territory;
            nations = in_nations;
            homeFactories = in_factories;
            InitializeComponent();

            // Update territory label
            SetTerritoryLabel();
            // Update attacker nation label
            lblCurrentNation.Text = nations[nation].name.ToString();
            // Populate defender nation combo box
            SetDefenderNations();
            // Give max amounts in type labels and set max values for value selectors
            numUpDownDefenseFactories.Maximum = 1;  // There will only ever be one factory at most
            SetAttackerUnitCount();
            SetDefenderUnitCount();
        }

        // Gets the nationID from the comboBox index for selecting defending nations
        private int NationID(int comboBoxID)
        {
            return comboBoxIndexes[comboBoxID];
        }

        // Update the territory label that indicates the territory in which combat is currently being resolved
        private void SetTerritoryLabel()
        {
            string trigramIndicator  = "";
            if ((territory.trigram != null) && (territory.trigram != "")) {
                trigramIndicator = $"({territory.trigram})";
            }
            lblCurrentTerritory.Text = $"{territory.name} {trigramIndicator}";
        }

        private void SetDefenderNations()
        {
            foreach (MilitaryUnit unit in territory.occupying_units)
            {
                comboBoxIndexes.Add(unit.nationID);
            }
            if (territory.GetFactory() != -1)
            {
                comboBoxIndexes.Add(territory.nation ?? -1);
            }
            comboBoxIndexes.Sort();
            int previous = -1;
            for (int i = comboBoxIndexes.Count - 1; i >= 0; i--)
            {
                if (comboBoxIndexes[i] == previous)
                {
                    comboBoxIndexes.RemoveAt(i);
                }
                else if (comboBoxIndexes[i] == nation)
                {
                    comboBoxIndexes.RemoveAt(i);
                }
                else if (comboBoxIndexes[i] == -1)
                {
                    comboBoxIndexes.RemoveAt(i);
                }
                else
                {
                    previous = comboBoxIndexes[i];
                }
            }
            for (int i = 0; i < comboBoxIndexes.Count; ++i)
            {
                comboBoxDefenseNations.Items.Add(nations[comboBoxIndexes[i]].name);
            }
            if (comboBoxDefenseNations.Items.Count == 0)
            {
                ResetDefenseCounts();
                numUpDownAttackArmies.Value = 0;
                numUpDownAttackFleets.Value = 0;
                continueFlag = 1;
                this.DialogResult = DialogResult.OK;
                this.Close();
            }
            else
            {
                comboBoxDefenseNations.SelectedIndex = 0;
            }
        }

        private void SetAttackerUnitCount()
        {
            int land = 0, sea = 0;
            for (int i = 0; i < territory.occupying_units.Count; ++i)
            {
                if (territory.occupying_units[i].nationID == nation)
                {
                    if (territory.occupying_units[i].isBoat)
                    {
                        sea++;
                    }
                    else
                    {
                        land++;
                    }
                }
            }
            lblAttackArmies.Text = $"Armies ({land})";
            lblAttackFleets.Text = $"Fleets ({sea})";
            numUpDownAttackArmies.Maximum = land;
            numUpDownAttackFleets.Maximum = sea;
        }

        private void SetDefenderUnitCount()
        {
            int land = 0, sea = 0;
            int selectedNationIndex = NationID(comboBoxDefenseNations.SelectedIndex);
            for (int i = 0; i < territory.occupying_units.Count; ++i)
            {
                if (territory.occupying_units[i].nationID == selectedNationIndex)
                {
                    if (territory.occupying_units[i].isBoat)
                    {
                        sea++;
                    }
                    else
                    {
                        land++;
                    }
                }
            }
            lblDefenseArmies.Text = $"Armies ({land})";
            lblDefenseFleets.Text = $"Fleets ({sea})";
            numUpDownDefenseArmies.Maximum = land;
            numUpDownDefenseFleets.Maximum = sea;
            // Deal with factories
            if ((territory.nation == selectedNationIndex) && (territory.GetFactory() != -1))
            {
                if (homeFactories > 1)
                {
                    numUpDownDefenseFactories.Enabled = true;
                    lblDefenseFactories.Text = "Factories (1)";
                }
            }
            else
            {
                numUpDownDefenseFactories.Enabled = false;
                numUpDownDefenseFactories.Value = 0;
                lblDefenseFactories.Text = "Factories (0)";
            }
        }

        // When the defending nation is changed, set the numeric up down control values back to 0
        private void ResetDefenseCounts()
        {
            numUpDownDefenseArmies.Value = 0;
            numUpDownDefenseFactories.Value = 0;
            numUpDownDefenseFleets.Value = 0;
        }

        // Update the total units committed labels
        private void UpdateTotalUnitsLabels(object sender, EventArgs e)
        {
            lblAttackerTotal.Text = $"Total: {numUpDownAttackFleets.Value + numUpDownAttackArmies.Value}";
            lblDefenseTotal.Text = $"Total: {numUpDownDefenseArmies.Value + numUpDownDefenseFleets.Value + 3 * numUpDownDefenseFactories.Value}";
        }

        private void comboBoxDefenseNations_SelectedIndexChanged(object sender, EventArgs e)
        {
            ResetDefenseCounts();
            SetDefenderUnitCount();
            // lblAttackerTotal and lblDefenseTotal automatically change when one of the NumericalUpDown controls changes value
        }

        private void btnSkipCombat_Click(object sender, EventArgs e)
        {
            // Set everything to zero before closing
            ResetDefenseCounts();
            numUpDownAttackArmies.Value = 0;
            numUpDownAttackFleets.Value = 0;
            continueFlag = 1;
            this.DialogResult = DialogResult.OK;
            this.Close();
        }

        private void btnContinueCombat_Click(object sender, EventArgs e)
        {
            continueFlag = 0;
            EndSpecificCombat();
        }

        private void btnConfirmCombat_Click(object sender, EventArgs e)
        {
            continueFlag = 1;
            EndSpecificCombat();
        }

        // End the combat between this pair of nations, whether combat continues afterwards or not in this territory is up for debate
        private void EndSpecificCombat()
        {
            if ((lblAttackerTotal.Text == lblDefenseTotal.Text) && (lblDefenseTotal.Text != "Total: 0"))
            {
                // Factory can only be destroyed if it is unprotected
                // 0 if no opposing units, 1 if non-home opposing units, 2 if home opposing units
                int homeUnitsDefending = 0;
                bool foreignUnitsPresent = false;
                List<string> nonHomeNonTurnNations = new List<string>();
                for (int i = 0; i < territory.occupying_units.Count; ++i)
                {
                    if (territory.occupying_units[i].nationID == territory.nation)
                    {
                        homeUnitsDefending++;
                    }
                    else if (territory.occupying_units[i].nationID != nation)
                    {
                        foreignUnitsPresent = true;
                        if (!nonHomeNonTurnNations.Contains(nations[territory.occupying_units[i].nationID].name)) {
                            nonHomeNonTurnNations.Add(nations[territory.occupying_units[i].nationID].name);
                        }
                    }
                }
                // Only when factory is selected
                if (numUpDownDefenseFactories.Value != 0)
                {
                    // Only run when all other troops are not simultaneously engaged
                    if (homeUnitsDefending != 0)
                    {
                        if (numUpDownDefenseFleets.Value + numUpDownDefenseArmies.Value != homeUnitsDefending)
                        {
                            MessageBox.Show("Cannot destroy factory while troops of the factory's nation defend it!", "Factory defended");
                            return;
                        }
                    }
                    else if (foreignUnitsPresent == true)
                    {
                        string nonHomeNonTurnNationString = "";
                        foreach (string nationName in nonHomeNonTurnNations)
                        {
                            nonHomeNonTurnNationString += nationName + "\n";
                        }
                        DialogResult factoryDefended = MessageBox.Show($"Troops from one of:\n{nonHomeNonTurnNationString}Defend the factory?",
                            "Defend factory?", MessageBoxButtons.YesNo);
                        if (factoryDefended == DialogResult.Yes)
                        {
                            MessageBox.Show("Factory was defended by other nation. Initiate combat with their troops first" +
                                " and destroy the factory afterwards, if desired", "Factory defended");
                            return;
                        }
                    }
                }
                this.DialogResult = DialogResult.OK;
                this.Close();
            }
            else
            {
                MessageBox.Show("Total troop count on both sides must be equal and nonzero!", "Unequal combat");
            }
        }

        // Returns the results of the combat after the form has been closed through the use of one of the buttons
        public List<int> GetResultsOfCombat()
        {
            List<int> results = new List<int>();
            int selectedNationIndex = NationID(comboBoxDefenseNations.SelectedIndex);
            results.Add(selectedNationIndex);
            results.Add((int)numUpDownAttackArmies.Value);
            results.Add((int)numUpDownAttackFleets.Value);
            results.Add((int)numUpDownDefenseArmies.Value);
            results.Add((int)numUpDownDefenseFleets.Value);
            results.Add((int)numUpDownDefenseFactories.Value);
            results.Add(continueFlag);
            return results;
        }
    }
}
