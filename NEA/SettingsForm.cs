using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows.Forms;

namespace NEA
{
    public partial class SettingsForm : Form
    {
        private bool debugMode;

        GameForm gameForm;

        int maxPlayersForCurrentMap = 2;
        int playerNamesPanelWidth;

        // Dynamic controls
        List<Label> playerNameLabels = new List<Label>();
        List<TextBox> playerNames = new List<TextBox>();

        // Game settings
        private string jsonFileLocation = "theworldmaptris.json";
        private string previousJsonFileLocation = "theworldmaptris.json";
        private bool randomBondsToStart = true;
        private bool nineBonds = false;
        // Player count can be directly accessed using the Value of the numeric up down control

        // Not all settings were implemented
        private bool investorCard = false;
        private bool taxTypeIsWorld = true;

        public SettingsForm(bool debugMode)
        {
            this.debugMode = debugMode;
            InitializeComponent();
            SetDefaultValues();
        }

        private void AddScrollBarsToPanelIfNecessary()
        {
            if (playerNamePanel.VerticalScroll.Visible)
            {
                playerNamePanel.Width = playerNamesPanelWidth + 27;
            }
            else
            {
                playerNamePanel.Width = playerNamesPanelWidth;
            }
        }

        private void SetDefaultValues()
        {
            GetMaxPlayers();
            playerNamesPanelWidth = playerNamePanel.Width;
            numUpDownPlayerCount.Value = 2;
            lblSelectedFile.Text = jsonFileLocation;
            numUpDownPlayerCount.Minimum = 1;
            numUpDownPlayerCount.Maximum = maxPlayersForCurrentMap;
            numUpDownPlayerCount.Value = Math.Min(maxPlayersForCurrentMap, Math.Max(numUpDownPlayerCount.Value, 1));
        }

        // Regex out the number of nations and ensure that that number is greater than or equal to the number of players
        private bool GetMaxPlayers()
        {
            string nationCountString = File.ReadLines(jsonFileLocation).Skip(1).Take(1).First();
            nationCountString = nationCountString.Remove(nationCountString.Length - 1);
            Regex nationCountIdentifier = new Regex("\"count\":\\s\\d+$");
            Match match = nationCountIdentifier.Match(nationCountString);
            if (!match.Success)
            {
                MessageBox.Show("Could not extract necessary data from file", "Invalid map");
                return false;
            }
            maxPlayersForCurrentMap = Int32.Parse(match.Value.Remove(0, 9));
            return true;
        }

        private void numUpDownPlayerCount_ValueChanged(object sender, EventArgs e)
        {
            while (playerNames.Count < numUpDownPlayerCount.Value)
            {
                playerNameLabels.Add(new Label() { Text = $"Player {playerNames.Count + 1} " });
                playerNameLabels.Last().Location = new Point(10, 46 * playerNameLabels.Count);
                playerNameLabels.Last().AutoSize = true;
                playerNamePanel.Controls.Add(playerNameLabels.Last());

                playerNames.Add(new TextBox());
                playerNames.Last().MaxLength = 10;
                playerNames.Last().Location = new Point(playerNameLabels[0].Right, 46 * playerNames.Count);
                playerNames.Last().Width = playerNamesPanelWidth - playerNameLabels[0].Width - 24;
                playerNamePanel.Controls.Add(playerNames.Last());

                AddScrollBarsToPanelIfNecessary();
            }
            while (playerNames.Count > numUpDownPlayerCount.Value)
            {
                playerNamePanel.Controls.Remove(playerNameLabels.Last());
                playerNamePanel.Controls.Remove(playerNames.Last());
                playerNames.RemoveAt(playerNames.Count - 1);
                playerNameLabels.RemoveAt(playerNameLabels.Count - 1);
            }
        }

        private void btnReturnToMenu_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void SettingsFormFormClosed(object sender, FormClosedEventArgs e)
        {
            if (Application.OpenForms.OfType<GameForm>().Count() == 0)
            {
                this.Owner.Opacity = 1;
            }
        }

        // Toggle values. No event tied to other radio button in group because
        // when selected, this radio button deselects and triggers the event anyway
        private void radioRandomBonds_CheckedChanged(object sender, EventArgs e)
        {
            randomBondsToStart = !randomBondsToStart;
        }

        private void radioEightBonds_CheckedChanged(object sender, EventArgs e)
        {
            nineBonds = !nineBonds;
        }

        private void btnSelectMap_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialogue = new OpenFileDialog();
            if (openFileDialogue.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    openFileDialogue.OpenFile();
                    jsonFileLocation = openFileDialogue.FileName;
                    if (GetMaxPlayers())
                    {
                        lblSelectedFile.Text = openFileDialogue.SafeFileName;
                        previousJsonFileLocation = jsonFileLocation;
                    }
                    else
                    {
                        jsonFileLocation = previousJsonFileLocation;
                    }
                }
                catch
                {
                    MessageBox.Show("Could not open file", "Could not open file");
                }
            }
        }

        // Load the game with the given settings
        private void btnPlay_Click(object sender, EventArgs e)
        {
            if (numUpDownPlayerCount.Value == 1)
            {
                DialogResult = MessageBox.Show("Are you sure you wish to play alone?", "Solo play?", MessageBoxButtons.YesNo);
                if (DialogResult == DialogResult.No)
                {
                    return;
                }
            }

            List<Player> players = new List<Player>();
            for (int i = 0; i < numUpDownPlayerCount.Value; ++i)
            {
                players.Add(new Player(playerNames[i].Text));
                if (players[i].name.Trim().Length == 0)
                {
                    players[i].name = $"Player {i + 1}";
                }
            }

            gameForm = new GameForm(debugMode,
                players,
                jsonFileLocation,
                (int)numUpDownPlayerCount.Value,
                randomBondsToStart,
                nineBonds,
                investorCard,
                taxTypeIsWorld) { Owner = this.Owner };
            if (gameForm.invalidJsonSelfDestruct)
            {
                MessageBox.Show("The map loaded was invalid in some way!");
            }
            else
            {
                gameForm.Show();
                this.Close();
            }
        }
    }
}
