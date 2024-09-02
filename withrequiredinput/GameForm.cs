using System;
using System.Linq;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace NEA
{
    public partial class GameForm : Form
    {
        // Debugging variables
        private int inc = 0;
        public Label lblTesting;

        private bool debugMode;
        public bool invalidJsonSelfDestruct = false;
        private bool paintForResize = false;    // For proper autosizing of controls, painting of the form should be done twice

        private GameState gameState;

        private float sizeMult = 1.0f;   // Multiplier to adjust display to screen resolution.
                                         // Automatically calculated in CalculateMaxScaling()
        private float[] dpi = new float[2]; // Horizontal DPI index 0, vertical DPI index 1
        private Font labelFont;
        private int boardXsize = 1280;  //
        private int boardYsize = 720;   //
        private int minXspacing = 300;
        private int minYspacing = 300;

        private DataGridView nationsOverview = new DataGridView();
        private DataGridView playerOverview = new DataGridView();
        private DataGridView bondsDataGridView = new DataGridView();
        private ListBox rondelListBox = new ListBox();
        private Button confirmButton = new Button();
        private Button skipButton = new Button();
        private Label selectedLabel = new Label();
        private Label currentActionLabel = new Label();
        private Label currentPlayerLabel = new Label();
        private Label currentNationLabel = new Label();

        private List<TerritoryPictureBox> territoryPictureBoxes = new List<TerritoryPictureBox>();

        private int _territoryLastClicked = -1; // Buffer variable for territoryLastClicked (infinite loop prevention)
        public int territoryLastClicked
        {
            get
            {
                return _territoryLastClicked;
            }
            set
            {
                _territoryLastClicked = value;
                OnLastTerritoryChanged();
            }
        }
        public (int, int) bondLastClicked;  // Column (nation) first, then row (value)

        // Double buffer the form for smoother painting
        protected override bool DoubleBuffered
        {
            get
            {
                return true;
            }
        }

        public GameForm(bool debugMode, string in_json, int in_playerCount, bool in_randomBonds, bool in_nineBonds, bool in_investorCard, bool in_taxType)
        {
            this.debugMode = debugMode;

            FormBorderStyle = FormBorderStyle.FixedSingle;
            InitializeComponent();

            gameState = new GameState(debugMode, in_json, in_playerCount, in_randomBonds, in_nineBonds, in_investorCard, in_taxType);
            if (gameState.invalidJsonSelfDestruct)
            {
                invalidJsonSelfDestruct = true;
            }

            CalculateMaxScaling(ref sizeMult);
            this.Location = new Point(0, 0);

            dpi = GetDPIRedundantGraphics();
            labelFont = new Font("Arial", (8));

            DrawTerritoriesToForm();
            CreateRondelListBox();
            CreateNationDataGridView();
            CreatePlayerDataGridView();
            CreateBondsDataGridView();
            CreateConfirmButton();
            CreateLabels();
            CreateSkipButton();

            //testing
            lblTesting = new Label();
            lblTesting.Location = new Point(1300, 800);
            lblTesting.Text = sizeMult.ToString();
            lblTesting.AutoSize = true;
            lblTesting.Parent = this;
            if (debugMode)
            {
                lblTesting.Show();
            }
        }

        // Reopen the main menu when form is closed.
        private void GameForm_FormClosed(object sender, FormClosedEventArgs e)
        {
            this.Owner.Opacity = 1;
        }

        private int GetDataGridViewColumnsWidth(DataGridView dataGridView)
        {
            int actualWidth = dataGridView.RowHeadersWidth;
            foreach (DataGridViewColumn column in dataGridView.Columns)
            {
                actualWidth += column.Width;
            }
            return actualWidth + 3;
        }

        private int GetDataGridViewRowsHeight(DataGridView dataGridView)
        {
            int actualHeight = dataGridView.ColumnHeadersHeight;
            foreach (DataGridViewRow row in dataGridView.Rows)
            {
                actualHeight += row.Height;
            }
            return actualHeight;
        }

        // Make the given DataGridView readonly, and automatically resize it
        private void ReadOnlyAndCustomiseDataGridView(ref DataGridView dataGridView)
        {
            dataGridView.AllowUserToResizeRows = false;
            dataGridView.AllowUserToResizeColumns = false;
            dataGridView.AllowUserToAddRows = false;
            dataGridView.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            dataGridView.RowHeadersWidthSizeMode = DataGridViewRowHeadersWidthSizeMode.AutoSizeToAllHeaders;
            dataGridView.ReadOnly = true;
            dataGridView.EnableHeadersVisualStyles = false;

            int height = GetDataGridViewRowsHeight(dataGridView);
            dataGridView.Size = new Size((int)((minXspacing - 40) * sizeMult), height);

            foreach (DataGridViewColumn column in dataGridView.Columns)
            {
                column.SortMode = DataGridViewColumnSortMode.NotSortable;
                column.AutoSizeMode = DataGridViewAutoSizeColumnMode.ColumnHeader;
            }
        }

        // If there is a horizontal scroll bar for the given DataGridView, add extra height to accomodate it
        // Does not work because the AutoSize of columns does not occur until after the form is painted
        private void AccomodateHScrollBarOffset(ref DataGridView dataGridView)
        {
            int actualWidth = GetDataGridViewColumnsWidth(dataGridView);
            if (actualWidth > dataGridView.Width)
            {
                dataGridView.Height += SystemInformation.HorizontalScrollBarHeight;
            }
        }

        private void FindCorrectWidth(ref DataGridView dataGridView)
        {
            dataGridView.Width = GetDataGridViewColumnsWidth(dataGridView);
        }

        // Create the DataGridView that displays information about each nation
        private void CreateNationDataGridView()
        {
            nationsOverview.ColumnCount = 2;
            nationsOverview.TopLeftHeaderCell.Value = "Nation";

            nationsOverview.Columns[0].HeaderText = "Power";
            nationsOverview.Columns[1].HeaderText = "Treasury";

            for (int i = 0; i < gameState.nations.Count; ++i)
            {
                string[] rowToAdd = { gameState.nations[i].powerPoints.ToString(), gameState.nations[i].treasury.ToString() };
                nationsOverview.Rows.Add(rowToAdd);
                nationsOverview.Rows[i].HeaderCell.Value = gameState.nations[i].name;
            }

            ReadOnlyAndCustomiseDataGridView(ref nationsOverview);
            AccomodateHScrollBarOffset(ref nationsOverview);

            this.Controls.Add(nationsOverview);

            nationsOverview.RowPrePaint += nationsOverview_DataGridViewOnPrePaint;
        }

        // Create the DataGridView that displays information about each player
        private void CreatePlayerDataGridView()
        {
            playerOverview.ColumnCount = 1;
            playerOverview.TopLeftHeaderCell.Value = "Player";

            playerOverview.Columns[0].HeaderText = "Money";

            for (int i = 0; i < gameState.players.Count; ++i)
            {
                string[] rowToAdd = { gameState.players[i].money.ToString() };
                playerOverview.Rows.Add(rowToAdd);
                playerOverview.Rows[i].HeaderCell.Value = $"[{i + 1}] {gameState.players[i].name}";
            }

            ReadOnlyAndCustomiseDataGridView(ref playerOverview);
            AccomodateHScrollBarOffset(ref playerOverview);

            this.Controls.Add(playerOverview);

            if (nationsOverview.Controls.OfType<HScrollBar>().First().Visible)
            {
                nationsOverview.Height += SystemInformation.HorizontalScrollBarHeight;
            }
        }

        private void CreateBondsDataGridView()
        {
            

            bondsDataGridView.TopLeftHeaderCell.Value = "Bonds";

            bondsDataGridView.CellClick += bondsDataGridView_CellClick;

            for (int i = 0; i < 9; ++i)
            {
                bondsDataGridView.Columns.Add(new DataGridViewButtonColumn());
                bondsDataGridView.Columns[i].HeaderText = gameState.bonds[0][i].cost.ToString() + " | " + gameState.bonds[0][i].interest.ToString();
            }

            for (int i = 0; i < gameState.nations.Count; ++i)
            {
                bondsDataGridView.Rows.Add(new DataGridViewRow());
                bondsDataGridView.Rows[i].HeaderCell.Value = gameState.nations[i].name;
                foreach (DataGridViewCell cell in bondsDataGridView.Rows[i].Cells)
                {
                    cell.Value = "";
                }
            }

            ReadOnlyAndCustomiseDataGridView(ref bondsDataGridView);

            this.Controls.Add(bondsDataGridView);
        }

        private void bondsDataGridView_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            bondLastClicked = (e.RowIndex, e.ColumnIndex);
            gameState.BondClicked(bondLastClicked);
            selectedLabel.Text = ($"{gameState.GetNationBond(gameState.nonPlayerBondInteractedWith ?? (-1, -1))} / " +
                $"{gameState.GetNationBond(gameState.playerBondInteractedWith ?? (-1, -1))}");
        }

        // Set the colour scheme for the DataGridView that displays nation information [NOT WORKING]
        private void nationsOverview_DataGridViewOnPrePaint(object sender, DataGridViewRowPrePaintEventArgs e)
        {
            //nationsOverview.Rows[e.RowIndex].HeaderCell.Style.BackColor = nations[e.RowIndex].primaryColour;
            //nationsOverview.Rows[e.RowIndex].DefaultCellStyle.BackColor = nations[e.RowIndex].secondaryColour;
            //nationsOverview.Rows[e.RowIndex].HeaderCell.Style.ForeColor = nations[e.RowIndex].monoContrastColour;
        }

        private void CreateRondelListBox()
        {
            rondelListBox.Location = new Point((int)((boardXsize + 10) * sizeMult), (int)(10 * sizeMult));

            // 0-indexed
            rondelListBox.Items.Add("Investor");
            rondelListBox.Items.Add("Import");
            rondelListBox.Items.Add("Production");
            rondelListBox.Items.Add("Manoeuvre");
            rondelListBox.Items.Add("Taxation");
            rondelListBox.Items.Add("Factory");
            rondelListBox.Items.Add("Production");
            rondelListBox.Items.Add("Manoeuvre");

            //rondelListBox.IntegralHeight = false;

            rondelListBox.Width = 100;

            rondelListBox.MouseClick += IdentifyClickedRondelSlot;

            this.Controls.Add(rondelListBox);
        }

        private void IdentifyClickedRondelSlot(object sender, MouseEventArgs e)
        {
            int clickedRondelIndex = rondelListBox.IndexFromPoint(e.Location);
            if (clickedRondelIndex != ListBox.NoMatches)
            {
                if ((gameState.gameStateTracker == -1) && (gameState.gameStarted == true))
                {
                    gameState.rondelInteractedWith = clickedRondelIndex;
                    selectedLabel.Text = rondelListBox.Items[clickedRondelIndex].ToString();
                }
            }
        }

        private void CreateConfirmButton()
        {
            confirmButton.Location = new Point((int)((boardXsize + minXspacing - 120) * sizeMult), (int)((boardYsize + minYspacing - 90) * sizeMult));
            confirmButton.Size = new Size((int)(86 * sizeMult), (int)(31 * sizeMult));
            confirmButton.Text = "Confirm";
            confirmButton.Click += ConfirmButtonClicked;

            this.Controls.Add(confirmButton);            
        }

        private void CreateSkipButton()
        {
            skipButton.Location = new Point((int)((boardXsize + minXspacing - 246) * sizeMult), (int)((boardYsize + minYspacing - 90) * sizeMult));
            skipButton.Size = new Size((int)(116 * sizeMult), (int)(31 * sizeMult));
            skipButton.Text = "Skip action";
            skipButton.Click += SkipButtonClicked;

            this.Controls.Add(skipButton);
        }

        private void CreateLabels()
        {
            selectedLabel.Location = new Point((int)((boardXsize + 10) * sizeMult), (int)((boardYsize + minYspacing - 130) * sizeMult));
            selectedLabel.AutoSize = true;
            selectedLabel.Text = "Nothing selected";

            this.Controls.Add(selectedLabel);

            currentNationLabel.Location = new Point((int)((boardXsize + 10) * sizeMult), (int)((boardYsize + 50) * sizeMult));
            currentNationLabel.AutoSize = true;
            currentNationLabel.Text = gameState.nations[gameState.currentNation].name;

            this.Controls.Add(currentNationLabel);

            currentPlayerLabel.Location = new Point((int)((boardXsize + 10) * sizeMult), (int)((boardYsize + 90) * sizeMult));
            currentPlayerLabel.AutoSize = true;
            currentPlayerLabel.Text = gameState.players[gameState.currentPlayerAction].name;

            this.Controls.Add(currentPlayerLabel);

            currentActionLabel.Location = new Point((int)((boardXsize + 10) * sizeMult), (int)((boardYsize + 130) * sizeMult));
            currentActionLabel.AutoSize = true;
            currentActionLabel.Text = "Game started - purchase bonds";

            this.Controls.Add(currentActionLabel);
        }

        // Get the DPI for both x and y in situations where graphics have not yet been defined.
        private float[] GetDPIRedundantGraphics()
        {
            Graphics g = CreateGraphics();
            float[] dpi = new float[2];
            dpi[0] = g.DpiX;
            dpi[1] = g.DpiY;
            g.Dispose();
            return dpi;
        }

        // Calculate the scaling that needs to be applied to the map for all side widgets to work
        private void CalculateMaxScaling(ref float sizeMultVal)
        {
            Rectangle screenMaxArea = Screen.FromControl(this).WorkingArea;
            //int titleBarHeight = RectangleToScreen(this.ClientRectangle).Top - this.Top;
            sizeMultVal = Math.Min((float)((float)screenMaxArea.Width / (boardXsize + minXspacing)),
                                (float)((float)screenMaxArea.Height / (boardYsize + minYspacing)));
            this.Height = (int)((boardYsize + minYspacing) * sizeMultVal);
            this.Width = (int)((boardXsize + minXspacing) * sizeMultVal);
        }

        // Draw territories in, ocean territories before land territories.
        // Due to the ExStyle 0x00000020 flag, this is done in reverse to what would be expected
        private void DrawTerritoriesToForm()
        {
            // Draw land territories (last - see above) so they are on top of ocean territories
            for (int i = 0; i < gameState.territories.Count; ++i)
            {
                territoryPictureBoxes.Add(new TerritoryPictureBox(i));
                if (gameState.territories[i].land == true)
                {
                    DrawTerritoryPictureBox(i);
                }
            }
            for (int i = 0; i < gameState.territories.Count; ++i)
            {

                if (gameState.territories[i].land == false)
                {
                    DrawTerritoryPictureBox(i);
                }
            }
            LastTerritoryChanged += UpdateTerritoryLabelsOnClickDebug;
            LastTerritoryChanged += PerformTerritoryClickedAction;
        }

        // Draw a PictureBox and its label
        private void DrawTerritoryPictureBox(int index)
        {
            territoryPictureBoxes[index].Image = (Image)gameState.territories[index].bitmap;

            gameState.territories[index].bitmap.SetResolution(dpi[0], dpi[1]);
            territoryPictureBoxes[index].Location = new Point((int)(gameState.territories[index].xpos * sizeMult), (int)(gameState.territories[index].ypos * sizeMult));
            territoryPictureBoxes[index].Height = (int)(gameState.territories[index].bitmap.Height * sizeMult);
            territoryPictureBoxes[index].Width = (int)(gameState.territories[index].bitmap.Width * sizeMult);

            territoryPictureBoxes[index].Parent = this;
            territoryPictureBoxes[index].BringToFront();

            territoryPictureBoxes[index].lblTerritoryName.BackColor = Color.Transparent;
            territoryPictureBoxes[index].lblTerritoryName.Parent = territoryPictureBoxes[index];
            territoryPictureBoxes[index].lblTerritoryName.Text = gameState.territories[index].trigram.ToUpper();
            territoryPictureBoxes[index].lblTerritoryName.Font = labelFont;

            territoryPictureBoxes[index].Click += BeforeProcessingTerritoryClicked;
            territoryPictureBoxes[index].lblTerritoryName.Click += BeforeProcessingTerritoryClicked;
        }

        private void RelocateTerritoryLabels()
        {
            for (int i = 0; i < gameState.territories.Count; ++i)
            {
                int xcoord = Math.Max((territoryPictureBoxes[i].Width - territoryPictureBoxes[i].lblTerritoryName.Width) / 2, 0);
                int ycoord = Math.Max((territoryPictureBoxes[i].Height - territoryPictureBoxes[i].lblTerritoryName.Height) / 2, 0);
                if (gameState.territories[i].lblx != -1)
                {
                    xcoord = gameState.territories[i].lblx;
                }
                if (gameState.territories[i].lbly != -1)
                {
                    ycoord = gameState.territories[i].lbly;
                }
                Point textLocation = new Point(xcoord, ycoord);
                territoryPictureBoxes[i].lblTerritoryName.Location = textLocation;
            }
        }

        private void RelocateDataGridViews()
        {
            bondsDataGridView.Location = new Point((int)(10 * sizeMult), (int)((boardYsize + 10) * sizeMult));
            nationsOverview.Location = new Point((int)((boardXsize + 10) * sizeMult), (int)(20 * sizeMult) + rondelListBox.Height);
            playerOverview.Location = new Point((int)((boardXsize + 10) * sizeMult), nationsOverview.Height + (int)(30 * sizeMult) + rondelListBox.Height);
        }

        private void ResizeControlsAfterFirstPaint()
        {
            FindCorrectWidth(ref bondsDataGridView);
            rondelListBox.Height = (int)(rondelListBox.ItemHeight * (rondelListBox.Items.Count + 1) * sizeMult);
            RelocateTerritoryLabels();
            RelocateDataGridViews();
        }

        private void GameForm_Paint(object sender, PaintEventArgs e)
        {
            string filename = System.IO.Path.Combine(gameState.jsonFileLocation.Remove(gameState.jsonFileLocation.Length - 5), "background.png");
            Bitmap background = new Bitmap(filename);
            background.SetResolution(dpi[0] / sizeMult, dpi[1] / sizeMult);
            e.Graphics.DrawImage(background, Point.Empty);
            if (!paintForResize)
            {
                paintForResize = true;
                ResizeControlsAfterFirstPaint();
                Invalidate();
            }
        }

        private void GameForm_Click(object sender, EventArgs e)
        {
            IdentifyClickedTerritory(Cursor.Position);
            lblTesting.Text = Cursor.Position.ToString();
        }

        public event EventHandler LastTerritoryChanged;

        protected virtual void OnLastTerritoryChanged()
        {
            // If null, do not invoke
            LastTerritoryChanged?.Invoke(this, EventArgs.Empty);
        }

        private void UpdateTerritoryLabelsOnClickDebug(object sender, EventArgs e)
        {
            lblTesting.Text = territoryLastClicked.ToString();
        }

        private void UpdateCurrentActionLabel()
        {
            currentActionLabel.Text = "";
            if (!gameState.gameStarted)
            {
                currentActionLabel.Text = "Game started - purchase bonds";
            }
            else
            {
                if (gameState.gameEnded)
                {
                    currentActionLabel.Text = "Game ended";
                }
                else
                {
                    string toOutput = "";
                    if (debugMode)
                    {
                        toOutput = $"[{gameState.gameStateTracker}]";
                    }
                    string[] actionLabelTexts = {
                        "Select rondel slot", // -1
                        "Select rondel slot", // 0
                        "Select rondel slot", // 1
                        "Select rondel slot", // 2
                        "Select rondel slot", // 3
                        "Select rondel slot", // 4
                        "Select rondel slot", // 5
                        "Calculating investor action...", // 6
                        "Select bond to purchase", // 7
                        "Select bond to purchase", // 8
                        "Select territory to move to", // 9
                        "Select territory to import to", // 10
                        "Select territory to move to", // 11
                        "Determine conflicts if needed", // 12
                        "Select unit to move", // 13
                        "Select bond to purchase", // 14
                        "Calculating taxation action...", // 15
                        "Select territory to build in", // 16
                        "Select territories to produce", // 17
                        "Finish turn", // 18
                        "Select unit to move", // 19
                        ""  // 20
                        };
                    toOutput += actionLabelTexts[gameState.gameStateTracker + 1];
                    currentActionLabel.Text += toOutput;
                }
            }
        }

        public void BeforeProcessingTerritoryClicked(object sender, EventArgs e)
        {
            IdentifyClickedTerritory(Cursor.Position);
        }

        // On a click, figure out what territory was clicked
        public void IdentifyClickedTerritory(Point mouseLocation)
        {
            Rectangle screen = this.RectangleToScreen(this.ClientRectangle);
            Point screenRelativeMouseLocation = mouseLocation;
            screenRelativeMouseLocation.X = mouseLocation.X - screen.Left;
            screenRelativeMouseLocation.Y = mouseLocation.Y - screen.Top;
            if ((screenRelativeMouseLocation.X > 0) && (screenRelativeMouseLocation.Y > 0) && 
                (screenRelativeMouseLocation.X < boardXsize * sizeMult) && (screenRelativeMouseLocation.Y < boardYsize * sizeMult))
            {
                int territoryClicked = -3;  // -3 if not yet changed, -2 if oceans changed multiple times, -1 if land changed multiple times
                Point relativeMouseLocation = screenRelativeMouseLocation;    // The mouse location relative to the territory currently being tested
                for (int i = 0; i < gameState.territories.Count; ++i)
                {
                    relativeMouseLocation.X = screenRelativeMouseLocation.X - territoryPictureBoxes[i].Location.X;
                    relativeMouseLocation.Y = screenRelativeMouseLocation.Y - territoryPictureBoxes[i].Location.Y;
                    if ((relativeMouseLocation.X < (int)(gameState.territories[i].bitmap.Width * sizeMult)) &&
                        (relativeMouseLocation.Y < (int)(gameState.territories[i].bitmap.Height * sizeMult)) &&
                        (relativeMouseLocation.X >= 0) &&
                        (relativeMouseLocation.Y >= 0))
                    {
                        Color mouseLocationColour = gameState.territories[i].bitmap.GetPixel((int)(relativeMouseLocation.X / sizeMult), (int)(relativeMouseLocation.Y / sizeMult));
                        if (mouseLocationColour.A == 255)
                        {
                            if (territoryClicked == -3)
                            {
                                territoryClicked = gameState.territories[i].id;
                            }
                            else if (gameState.territories[i].land == false)
                            {
                                if (territoryClicked >= 0)
                                {
                                    if (gameState.territories[territoryClicked].land == false)
                                    {
                                        territoryClicked = -2;
                                    }
                                }
                            }
                            else
                            {
                                if (territoryClicked >= 0)
                                {
                                    if (gameState.territories[territoryClicked].land == false)
                                    {
                                        territoryClicked = gameState.territories[i].id;
                                    }
                                    else
                                    {
                                        break;
                                    }
                                }
                                else if (territoryClicked == -2)
                                {
                                    territoryClicked = gameState.territories[i].id;
                                }
                            }
                        }
                    }
                }
                if (territoryClicked <= -1) territoryClicked = -1;
                if (territoryLastClicked != territoryClicked) territoryLastClicked = territoryClicked;
            }
        }

        // Inform the GameState that changes should be made
        // Use the information changes from within the GameState to update graphics displays
        private void ConfirmButtonClicked(object sender, EventArgs e)
        {
            if (gameState.gameEnded)
            {
                DisplayGameEndStats();
            }
            /* CHANGESENACTEDFLAG states
             0: [OK] everything is OK
             1: [ERROR] gameState.EnactChanges failed to meet any of its if statements
             2: [OK] no rondel slot was selected in gameState == -1
             3: [ERROR] rondel slot selected was out of bounds in gameState == -1
             4: [OK] player didn't select a bond in requiredInput == 1
             5: [OK] action not able to be carried out in gameState == 6 (investor) / gameState == 16 (factory)
            */
            int changesEnactedFlag = gameState.EnactChanges(territoryLastClicked);
            if (changesEnactedFlag == 0)
            {
                if (gameState.requiredInput == 0)
                {
                    if (gameState.gameStateTracker == 2)
                    {

                    }
                    if (gameState.gameStateTracker == 16)
                    {
                        // Display that the factory has been built
                    }
                }
                else if (gameState.requiredInput == 1)
                {
                    UpdateBondsDataGridView();
                    UpdatePlayerMoney();
                    UpdateNationTreasury();
                    gameState.BeforeGameStartNextPlayer();
                }
                else if (gameState.requiredInput == 2)
                {
                    gameState.RondelSlotSelected();
                    if (gameState.gameEnded)
                    {
                        confirmButton.Text = "End Game";
                        skipButton.Hide();
                    }
                }
                else if (gameState.gameStateTracker == 18)
                {
                    if (!gameState.investorCard)
                    {
                        gameState.PostTurnInvestorlessBondPurchasing();
                    }
                }

                if (gameState.gameStateTracker == 18)
                {
                    confirmButton.Text = "End turn";
                }
                UpdateLabels();
            }
            else if (changesEnactedFlag == 1)
            {
                selectedLabel.Text = "ERROR: no required state was found";
            }
            else if (changesEnactedFlag == 2)
            {
                selectedLabel.Text = "No rondel slot selected";
            }
            else if (changesEnactedFlag == 3)
            {
                throw new ApplicationException("Game state mismatched");
            }
            else if (changesEnactedFlag == 4)
            {
                selectedLabel.Text = "No valid non-player bond selected";
            }
            else if (changesEnactedFlag == 5)
            {
                selectedLabel.Text = "Not enough money to carry out action";
            }
        }

        private void SkipButtonClicked(object sender, EventArgs e)
        {
            if (!gameState.gameStarted)
            {
                if (!debugMode)
                {
                    DialogResult buttonClicked = MessageBox.Show($"{gameState.players[gameState.currentPlayerAction].name} " +
                        $"(Player {gameState.currentPlayerAction + 1}): " +
                        $"Skip purchasing a bond from {gameState.nations[gameState.currentNation].name}?", "Skip?", MessageBoxButtons.YesNo);
                    if (buttonClicked == DialogResult.Yes)
                    {
                        gameState.BeforeGameStartNextPlayer();
                    }
                }
                else
                {
                    gameState.BeforeGameStartNextPlayer();
                }
            }
            UpdateLabels();
        }

        private void PerformTerritoryClickedAction(object sender, EventArgs e)
        {
            // Import
            if (gameState.gameStateTracker == 9)
            {

            }
            // Manouvre
            else if (gameState.gameStateTracker == 20)
            {

            }
            // Production
            else if (gameState.gameStateTracker == 17)
            {

            }
            // Ignore factory as that only requires one click and a confirm
        }

        private void DisplayGameEndStats()
        {

        }

        private void UpdatePlayerMoney()
        {
            for (int i = 0; i < gameState.players.Count; ++i)
            {
                playerOverview[0, i].Value = gameState.players[i].money;
            }
        }

        private void UpdateNationTreasury()
        {
            for (int i = 0; i < gameState.nations.Count; ++i)
            {
                nationsOverview[1, i].Value = gameState.nations[i].treasury;
            }
        }

        private void UpdateBondsDataGridView()
        {
            for (int i = 0; i < gameState.nations.Count; ++i)
            {
                for (int j = 0; j < gameState.bonds[0].Count; ++j)
                {
                    if (gameState.bonds[i][j].playerID == -1)
                    {
                        bondsDataGridView[j, i].Value = "";
                    }
                    else
                    {
                        if (bondsDataGridView[j, i].Value.ToString() != (gameState.bonds[i][j].playerID + 1).ToString())
                        {
                            bondsDataGridView[j, i].Value = gameState.currentPlayerAction + 1;
                        }
                    }
                }
            }
        }

        private void UpdateLabels()
        {
            currentPlayerLabel.Text = gameState.players[gameState.currentPlayerAction].name;
            currentNationLabel.Text = gameState.nations[gameState.currentNation].name;
            UpdateCurrentActionLabel();
        }
    }
}
