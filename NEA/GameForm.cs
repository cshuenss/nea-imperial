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
    /* settings form
     layout 
     end of game */
    public partial class GameForm : Form
    {
        // Debugging variables
        private bool debugMode;
        private int inc = 0;
        private bool territoryAdjacency = false;
        private Button debugButton1;
        private Button debugButtonEndGame;
        private Button debugButtonTerritories;
        public Label testingLabel1;

        // Status variables
        public bool invalidJsonSelfDestruct = false;
        private bool paintForResize = true;    // For proper autosizing of controls, painting of the form should be done twice on start
        private bool nineBonds;

        // The current game state
        private GameState gameState;

        // Display factors
        private float sizeMult = 1.0f;   // Multiplier to adjust display to screen resolution.
                                         // Automatically calculated in CalculateMaxScaling()
        private float[] dpi = new float[2]; // Horizontal DPI index 0, vertical DPI index 1
        private Font controlFont;
        private Font labelFont;
        private int boardXsize = 1280;  //
        private int boardYsize = 720;   //
        private int minXspacing = 300;
        private int minYspacing = 300;

        // Controls drawn to the screen
        private DataGridView nationsOverview = new DataGridView();
        private DataGridView playerOverview = new DataGridView();
        private DataGridView bondsDataGridView = new DataGridView();
        private ListBox rondelListBox = new ListBox();
        private Button confirmButton = new Button();
        private Button skipButton = new Button();
        private Button donateButton = new Button();
        private Label selectedLabel;
        private Label currentActionLabel;
        private Label currentPlayerLabel;
        private Label currentNationLabel;
        private Label lblCurrentlySelected;
        private Label lblCurrentPlayer;
        private Label lblCurrentNation;
        private Label lblCurrentAction;
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
                // Raise territory changed event
                OnLastTerritoryChanged();
            }
        }
        public (int, int) bondLastClicked;  // Column (nation) first, then row (value)

        public event EventHandler LastTerritoryChanged;

        // Double buffer the form for smoother painting
        protected override bool DoubleBuffered
        {
            get
            {
                return true;
            }
        }

        public GameForm(bool debugMode,
            List<Player> in_players,
            string in_json,
            int in_playerCount,
            bool in_randomBonds,
            bool in_nineBonds,
            bool in_investorCard,
            bool in_taxType)
        {
            this.debugMode = debugMode;
            nineBonds = in_nineBonds;

            // Disable resizing
            FormBorderStyle = FormBorderStyle.FixedSingle;
            InitializeComponent();

            gameState = new GameState(debugMode,
                in_players,
                in_json,
                in_playerCount,
                in_randomBonds,
                in_nineBonds,
                in_investorCard,
                in_taxType);
            if (gameState.invalidJsonSelfDestruct)
            {
                invalidJsonSelfDestruct = true;
            }

            if (!invalidJsonSelfDestruct)
            {
                CalculateMaxScaling(ref sizeMult);
                this.Location = new Point(0, 0);

                dpi = GetDPIRedundantGraphics();
                controlFont = new Font("Arial", 8);
                labelFont = new Font("Arial", 12);

                CreateControls();
                if (debugMode) CreateDebugControls();

                // If the game has already started
                if (gameState.gameStarted)
                {
                    currentActionLabel.Text = "Game started - select rondel slot";
                }
            }
        }

        // Draw everything to the form
        private void CreateControls()
        {
            DrawTerritoriesToForm();
            CreateRondelListBox();
            CreateNationDataGridView();
            CreatePlayerDataGridView();
            CreateBondsDataGridView();
            CreateConfirmButton();
            CreateLabels();
            CreateSkipButton();
            CreateDonateButton();
            UpdateNationDataGridView();
        }

        // Create debugging controls.
        private void CreateDebugControls()
        {
            // Testing label
            testingLabel1  
                = new Label();
            testingLabel1.Location = new Point((int)(boardXsize * sizeMult + 1), (int)(boardYsize * sizeMult - 50));
            testingLabel1.Text = sizeMult.ToString();
            testingLabel1.AutoSize = true;
            testingLabel1.Parent = this;
            testingLabel1.Show();

            // Debug buttons
            debugButton1 = new Button();
            debugButton1.Location = new Point((int)((boardXsize + 1) * sizeMult), (int)((boardYsize + 1) * sizeMult));
            debugButton1.Size = new Size((int)(20 * sizeMult), (int)(20 * sizeMult));
            debugButton1.Click += Debug1ButtonClicked;

            this.Controls.Add(debugButton1);

            debugButtonEndGame = new Button();
            debugButtonEndGame.Location = new Point(debugButton1.Right + 10, debugButton1.Top);
            debugButtonEndGame.Size = new Size((int)(20 * sizeMult), (int)(20 * sizeMult));
            debugButtonEndGame.Click += DebugButtonEndGameClicked;

            this.Controls.Add(debugButtonEndGame);

            debugButtonTerritories = new Button();
            debugButtonTerritories.Location = new Point(debugButtonEndGame.Right + 10, debugButtonEndGame.Top);
            debugButtonTerritories.Size = new Size((int)(20 * sizeMult), (int)(20 * sizeMult));
            debugButtonTerritories.BackColor = Color.Tomato;
            debugButtonTerritories.Click += DebugButtonTerritoriesClicked;

            this.Controls.Add(debugButtonTerritories);
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
            sizeMultVal = Math.Min((float)((float)screenMaxArea.Width / (boardXsize + minXspacing)),
                                (float)((float)screenMaxArea.Height / (boardYsize + minYspacing)));
            this.Height = (int)((boardYsize + minYspacing) * sizeMultVal);
            this.Width = (int)((boardXsize + minXspacing) * sizeMultVal);
        }

        // Get an approximation for the width for the given DataGridView
        // Returns a slightly larger value than expected to prevent the scroll bar from appearing
        private int GetDataGridViewColumnsWidth(DataGridView dataGridView)
        {
            int actualWidth = dataGridView.RowHeadersWidth;
            foreach (DataGridViewColumn column in dataGridView.Columns)
            {
                actualWidth += column.Width;
            }
            return actualWidth + 3;
        }

        // Get the height of the given DataGridView
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

            // Make each column read-only
            foreach (DataGridViewColumn column in dataGridView.Columns)
            {
                column.SortMode = DataGridViewColumnSortMode.NotSortable;
                column.AutoSizeMode = DataGridViewAutoSizeColumnMode.ColumnHeader;
            }
        }

        // If there is a horizontal scroll bar for the given DataGridView, add extra height to accomodate it
        // Does not work the first time round because the AutoSize of columns does not occur until after the form is painted
        private void AccomodateHScrollBarOffset(ref DataGridView dataGridView)
        {
            int actualWidth = GetDataGridViewColumnsWidth(dataGridView);
            if (actualWidth > dataGridView.Width)
            {
                dataGridView.Height += SystemInformation.HorizontalScrollBarHeight;
            }
        }

        // Return the width of the DataGridView
        private void FindCorrectWidth(ref DataGridView dataGridView)
        {
            dataGridView.Width = GetDataGridViewColumnsWidth(dataGridView);
        }

        // Create the DataGridView that displays information about each nation
        private void CreateNationDataGridView()
        {
            nationsOverview.ColumnCount = 4;
            nationsOverview.TopLeftHeaderCell.Value = "Nation";

            nationsOverview.Columns[0].HeaderText = "Power";
            nationsOverview.Columns[1].HeaderText = "Treasury";
            nationsOverview.Columns[2].HeaderText = "Rondel";
            nationsOverview.Columns[3].HeaderText = "Tax";

            // Populate
            for (int i = 0; i < gameState.nations.Count; ++i)
            {
                string[] rowToAdd = { gameState.nations[i].powerPoints.ToString(), gameState.nations[i].treasury.ToString(), "", "0" };
                nationsOverview.Rows.Add(rowToAdd);
                nationsOverview.Rows[i].HeaderCell.Value = gameState.nations[i].name;
            }

            ReadOnlyAndCustomiseDataGridView(ref nationsOverview);

            this.Controls.Add(nationsOverview);
        }

        // Create the DataGridView that displays information about each player
        private void CreatePlayerDataGridView()
        {
            playerOverview.ColumnCount = 1;
            playerOverview.TopLeftHeaderCell.Value = "Player";

            playerOverview.Columns[0].HeaderText = "Money";

            // Populate
            for (int i = 0; i < gameState.players.Count; ++i)
            {
                string[] rowToAdd = { gameState.players[i].money.ToString() };
                playerOverview.Rows.Add(rowToAdd);
                playerOverview.Rows[i].HeaderCell.Value = $"[{i + 1}] {gameState.players[i].name}";
            }

            ReadOnlyAndCustomiseDataGridView(ref playerOverview);

            this.Controls.Add(playerOverview);

            if (nationsOverview.Controls.OfType<HScrollBar>().First().Visible)
            {
                nationsOverview.Height += SystemInformation.HorizontalScrollBarHeight;
            }
        }

        // Create the DataGridView that shows information about the current status of all bonds
        private void CreateBondsDataGridView()
        {
            bondsDataGridView.TopLeftHeaderCell.Value = "Bonds";

            bondsDataGridView.CellClick += bondsDataGridView_CellClick;

            int bondCount = 8;
            if (nineBonds) bondCount = 9;

            // Populate
            for (int i = 0; i < bondCount; ++i)
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

            UpdateBondsDataGridView();
        }

        // Add indicator to Nations DataGridView to show owning player
        private void UpdateNationDataGridView()
        {
            for (int i = 0; i < gameState.nations.Count; ++i)
            {
                nationsOverview.Rows[i].HeaderCell.Value = $"[{gameState.nations[i].controllingPlayer + 1}] {gameState.nations[i].name}";
                // 0 is Power, 1 is Treasury, 2 is Previous Rondel Slot, 3 is Projected Tax Income
                nationsOverview.Rows[i].Cells[3].Value = gameState.GetTaxationPowerIncrease(i)[1];
                nationsOverview.Rows[i].Cells[0].Value = gameState.nations[i].powerPoints;
            }
        }

        // Create the rondel in list form
        private void CreateRondelListBox()
        {
            rondelListBox.Location = new Point((int)((boardXsize + 10) * sizeMult), (int)(10 * sizeMult));

            // 0-indexed
            rondelListBox.Items.Add("[1] Investor");
            rondelListBox.Items.Add("[2] Import");
            rondelListBox.Items.Add("[3] Production");
            rondelListBox.Items.Add("[4] Manoeuvre");
            rondelListBox.Items.Add("[5] Taxation");
            rondelListBox.Items.Add("[6] Factory");
            rondelListBox.Items.Add("[7] Production");
            rondelListBox.Items.Add("[8] Manoeuvre");

            rondelListBox.Width = (int)(120 * sizeMult);

            rondelListBox.MouseClick += IdentifyClickedRondelSlot;

            this.Controls.Add(rondelListBox);
        }

        // Creating buttons and information labels to populate the form with
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

        private void CreateDonateButton()
        {
            donateButton.Size = new Size((int)(80 * sizeMult), (int)(31 * sizeMult));
            donateButton.Text = "Donate";
            donateButton.Click += DonateButtonClicked;

            this.Controls.Add(donateButton);
        }

        private void CreateLabels()
        {
            lblCurrentNation = new Label()
            {
                Location = new Point((int)((boardXsize - 250) * sizeMult), (int)((boardYsize + 15) * sizeMult)),
                Font = labelFont,
                AutoSize = true,
                Text = "Current nation:"
            };
            this.Controls.Add(lblCurrentNation);

            currentNationLabel = new Label()
            {
                Location = new Point(lblCurrentNation.Right, (int)((boardYsize + 15) * sizeMult)),
                Font = labelFont,
                AutoSize = true,
                Text = gameState.nations[gameState.currentNation].name
            };
            this.Controls.Add(currentNationLabel);

            lblCurrentPlayer = new Label() {
                Location = new Point((int)((boardXsize - 250) * sizeMult), lblCurrentNation.Bottom + (int)(15 * sizeMult)),
                Font = labelFont,
                AutoSize = true,
                Text = "Current player:"
            };
            this.Controls.Add(lblCurrentPlayer);

            currentPlayerLabel = new Label()
            {
                Location = new Point(lblCurrentPlayer.Right, lblCurrentNation.Bottom + (int)(15 * sizeMult)),
                Font = labelFont,
                AutoSize = true,
                Text = playerOverview.Rows[gameState.currentPlayerAction].HeaderCell.Value.ToString()
            };
            this.Controls.Add(currentPlayerLabel);

            lblCurrentAction = new Label()
            {
                Location = new Point((int)((boardXsize - 250) * sizeMult), lblCurrentPlayer.Bottom + (int)(15 * sizeMult)),
                Font = labelFont,
                AutoSize = true,
                Text = "Current action:"
            };
            this.Controls.Add(lblCurrentAction);

            currentActionLabel = new Label()
            {
                Location = new Point(lblCurrentAction.Right, lblCurrentPlayer.Bottom + (int)(15 * sizeMult)),
                Font = labelFont,
                AutoSize = true,
                Text = "Game started - purchase bonds"
            };
            this.Controls.Add(currentActionLabel);

            lblCurrentlySelected = new Label()
            {
                Location = new Point((int)((boardXsize - 250) * sizeMult), lblCurrentAction.Bottom + (int)(15 * sizeMult)),
                Font = labelFont,
                AutoSize = true,
                Text = "Currently selected:"
            };
            this.Controls.Add(lblCurrentlySelected);

            selectedLabel = new Label()
            {
                Location = new Point(lblCurrentlySelected.Right, lblCurrentAction.Bottom + (int)(15 * sizeMult)),
                Font = labelFont,
                AutoSize = true,
                Text = "Nothing selected"
            };
            this.Controls.Add(selectedLabel);
        }

        // Draw territories in, ocean territories before land territories.
        // Due to the ExStyle 0x00000020 flag, this is done in reverse to what would be expected
        private void DrawTerritoriesToForm()
        {
            // Draw land territories first - see above - so they are on top of ocean territories
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
            if (debugMode)
            {
                LastTerritoryChanged += UpdateTerritoryLabelsOnClickDebug;
            }
            LastTerritoryChanged += UpdateSelectedLabelTerritoryClicked;
            LastTerritoryChanged += PerformTerritoryClickedAction;
        }

        // Draw a territory's PictureBox and its label
        private void DrawTerritoryPictureBox(int index)
        {
            territoryPictureBoxes[index].Image = (Image)gameState.territories[index].bitmap;

            // Set the positioning
            gameState.territories[index].bitmap.SetResolution(dpi[0], dpi[1]);
            territoryPictureBoxes[index].Location = new Point((int)(gameState.territories[index].xpos * sizeMult), (int)(gameState.territories[index].ypos * sizeMult));
            territoryPictureBoxes[index].Height = (int)(gameState.territories[index].bitmap.Height * sizeMult);
            territoryPictureBoxes[index].Width = (int)(gameState.territories[index].bitmap.Width * sizeMult);

            territoryPictureBoxes[index].Parent = this;
            territoryPictureBoxes[index].BringToFront();

            // Set the style
            territoryPictureBoxes[index].lblTerritoryName.BackColor = Color.Transparent;
            territoryPictureBoxes[index].lblTerritoryName.Parent = territoryPictureBoxes[index];
            territoryPictureBoxes[index].lblTerritoryName.Text = gameState.territories[index].trigram.ToUpper();
            territoryPictureBoxes[index].lblTerritoryName.Font = controlFont;

            // Interactivity
            territoryPictureBoxes[index].Paint += TerritoryPictureBoxPaintUnitsAndFactories;
            territoryPictureBoxes[index].Click += BeforeProcessingTerritoryClicked;
            territoryPictureBoxes[index].lblTerritoryName.Click += BeforeProcessingTerritoryClicked;
            // Both the double click functions feed into the same procedure, but the sender is different for each
            territoryPictureBoxes[index].DoubleClick += TerritoryDisplayInfoEvent;
            territoryPictureBoxes[index].lblTerritoryName.DoubleClick += TerritoryDisplayInfoEvent;
        }

        // Centre the labels on each PictureBox, unless otherwise stated in save data
        private void RelocateTerritoryLabels()
        {
            for (int i = 0; i < gameState.territories.Count; ++i)
            {
                int xcoord = Math.Max((territoryPictureBoxes[i].Width - territoryPictureBoxes[i].lblTerritoryName.Width) / 2, 0);
                int ycoord = Math.Max((territoryPictureBoxes[i].Height - territoryPictureBoxes[i].lblTerritoryName.Height) / 2, 0);
                if (gameState.territories[i].lblx != -1)
                {
                    xcoord = (int)(gameState.territories[i].lblx * sizeMult);
                }
                if (gameState.territories[i].lbly != -1)
                {
                    ycoord = (int)(gameState.territories[i].lbly * sizeMult);
                }
                Point textLocation = new Point(xcoord, ycoord);
                territoryPictureBoxes[i].lblTerritoryName.Location = textLocation;
            }
        }

        // Set up positions of DataGridViews in relation to each other, with respect to their corresponding sizes
        private void RelocateDataGridViews()
        {
            bondsDataGridView.Location = new Point((int)(10 * sizeMult), (int)((boardYsize + 10) * sizeMult));
            nationsOverview.Location = new Point((int)((boardXsize + 10) * sizeMult), (int)(20 * sizeMult) + rondelListBox.Height);
            playerOverview.Location = new Point((int)((boardXsize + 10) * sizeMult), nationsOverview.Height + (int)(30 * sizeMult) + rondelListBox.Height);
        }

        // After everything has been painted the first time, sizes of controls are known so size-related positions and resizings can be made
        private void ResizeControlsAfterFirstPaint()
        {
            FindCorrectWidth(ref bondsDataGridView);
            rondelListBox.Height = (int)(rondelListBox.ItemHeight * (rondelListBox.Items.Count + 1));
            AccomodateHScrollBarOffset(ref nationsOverview);
            AccomodateHScrollBarOffset(ref playerOverview);
            RelocateTerritoryLabels();
            RelocateDataGridViews();
            donateButton.Location = new Point((int)((boardXsize + minXspacing - 110) * sizeMult), (int)(rondelListBox.Height - (19 * sizeMult)));
        }

        // Draw factories and units onto the territory PictureBoxes
        private void TerritoryPictureBoxPaintUnitsAndFactories(object sender, PaintEventArgs e) {
            if (sender is TerritoryPictureBox territoryPB)
            {
                // Factories
                if (gameState.territories[territoryPB.territoryID].nation != null)
                {
                    // Create a thin black border to help distinguish the rectangle from the background
                    e.Graphics.DrawRectangle(new Pen(Color.Black, 2.0f),
                            (int)(territoryPB.lblTerritoryName.Left + territoryPB.lblTerritoryName.Width / 2 - 11 * sizeMult),
                            (int)(territoryPB.lblTerritoryName.Top - 21 * sizeMult),
                            22 * sizeMult, 22 * sizeMult);
                    // If there is no factory, then draw a hollow square
                    if (gameState.territories[territoryPB.territoryID].GetFactory() == -1)
                    {
                        if (gameState.territories[territoryPB.territoryID].factory == true)
                        {
                            e.Graphics.DrawRectangle(new Pen(Color.Blue, 4.0f),
                                (int)(territoryPB.lblTerritoryName.Left + territoryPB.lblTerritoryName.Width / 2 - 9 * sizeMult),
                                (int)(territoryPB.lblTerritoryName.Top - 19 * sizeMult),
                                18 * sizeMult, 18 * sizeMult);
                        }
                        else
                        {
                            e.Graphics.DrawRectangle(new Pen(Color.Red, 4.0f),
                                (int)(territoryPB.lblTerritoryName.Left + territoryPB.lblTerritoryName.Width / 2 - 9 * sizeMult),
                                (int)(territoryPB.lblTerritoryName.Top - 19 * sizeMult),
                                18 * sizeMult, 18 * sizeMult);
                        }
                    }
                    // If there is a factory, then draw a solid square
                    else
                    {
                        if (gameState.territories[territoryPB.territoryID].factory == true)
                        {
                            e.Graphics.FillRectangle(new SolidBrush(Color.Blue),
                                (int)(territoryPB.lblTerritoryName.Left + territoryPB.lblTerritoryName.Width / 2 - 10 * sizeMult),
                                (int)(territoryPB.lblTerritoryName.Top - 20 * sizeMult),
                                20 * sizeMult, 20 * sizeMult);
                        }
                        else
                        {
                            e.Graphics.FillRectangle(new SolidBrush(Color.Red),
                                (int)(territoryPB.lblTerritoryName.Left + territoryPB.lblTerritoryName.Width / 2 - 10 * sizeMult),
                                (int)(territoryPB.lblTerritoryName.Top - 20 * sizeMult),
                                20 * sizeMult, 20 * sizeMult);
                        }
                    }
                }
                // Units
                int[] unitsFromEach = new int[gameState.nations.Count];
                bool[] hasHostiles = new bool[gameState.nations.Count];
                int total = 0;
                // Get the number of units from each nation
                foreach (MilitaryUnit unit in gameState.territories[territoryPB.territoryID].occupying_units)
                {
                    unitsFromEach[unit.nationID]++;
                    if (unitsFromEach[unit.nationID] == 1)
                    {
                        total++;
                        if ((unit.isHostile) && (gameState.territories[territoryPB.territoryID].nation != null))
                        {
                            hasHostiles[unit.nationID] = true;
                        }
                    }
                }
                int xCoord = (territoryPB.lblTerritoryName.Left + territoryPB.lblTerritoryName.Right) / 2 - (int)(11 * sizeMult * total);
                Point unitsDisplayPoint = new Point(xCoord, territoryPB.lblTerritoryName.Bottom + 2);

                // If there are units present, then draw a circle for each with a number indicating unit count
                if (total != 0)
                {
                    for (int i = 0; i < gameState.nations.Count; ++i)
                    {
                        if (unitsFromEach[i] != 0)
                        {
                            // Get the rectangle that represents where to draw the image
                            Rectangle unitsDisplayLocation = new Rectangle(unitsDisplayPoint, new Size((int)(20 * sizeMult), (int)(20 * sizeMult)));
                            // Colour in the circle for this nation
                            e.Graphics.FillEllipse(new SolidBrush(gameState.nations[i].secondaryColour), unitsDisplayLocation);
                            // Give it a border
                            if ((hasHostiles[i]) && (gameState.territories[territoryPB.territoryID].nation != i))
                            {
                                e.Graphics.DrawEllipse(new Pen(Color.Red, 2), unitsDisplayLocation);
                            }
                            else
                            {
                                e.Graphics.DrawEllipse(new Pen(Color.Black, 2), unitsDisplayLocation);
                            }
                            // Align the text location
                            unitsDisplayPoint.X += (int)(4 * sizeMult);
                            unitsDisplayPoint.Y += (int)(3 * sizeMult);
                            // Then draw the text
                            e.Graphics.DrawString(unitsFromEach[i].ToString(), controlFont, new SolidBrush(Color.Black), unitsDisplayPoint);
                            // Align for the next nation's circle
                            unitsDisplayPoint.Y -= (int)(3 * sizeMult);
                            unitsDisplayPoint.X += (int)(17 * sizeMult);
                        }
                    }
                }
            }
        }

        // On paint, also paint the background
        private void GameForm_Paint(object sender, PaintEventArgs e)
        {
            string filename = System.IO.Path.Combine(gameState.jsonFileLocation.Remove(gameState.jsonFileLocation.Length - 5), "background.png");
            Bitmap background = new Bitmap(filename);
            background.SetResolution(dpi[0] / sizeMult, dpi[1] / sizeMult);
            e.Graphics.DrawImage(background, Point.Empty);
            for (int i = 0; i < gameState.territories.Count; ++i)
            {
                if (gameState.territories[i].land == true)
                {
                    territoryPictureBoxes[i].Invalidate();
                }
            }
            for (int i = 0; i < gameState.territories.Count; ++i)
            {
                if (gameState.territories[i].land == false)
                {
                    territoryPictureBoxes[i].Invalidate();
                }
            }
            // If this is the first paint, then repaint everything after figuring out the correct sizes for everything
            if (paintForResize)
            {
                paintForResize = false;
                ResizeControlsAfterFirstPaint();
                Invalidate();
            }
        }

        // When a bond is clicked, set the last clicked bond to that bond
        private void bondsDataGridView_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            bondLastClicked = (e.RowIndex, e.ColumnIndex);
            gameState.BondClicked(bondLastClicked);
            selectedLabel.Text = ($"{gameState.GetNationBond(gameState.nonPlayerBondInteractedWith ?? (-1, -1))} / " +
                $"{gameState.GetNationBond(gameState.playerBondInteractedWith ?? (-1, -1))}");
        }

        // Identify the selected rondel slot
        private void IdentifyClickedRondelSlot(object sender, MouseEventArgs e)
        {
            int clickedRondelIndex = rondelListBox.IndexFromPoint(e.Location);
            if (clickedRondelIndex != ListBox.NoMatches)
            {
                // Only if the rondel is supposed to be interacted with at this point
                if ((gameState.gameStateTracker == -1) && (gameState.gameStarted == true))
                {
                    gameState.rondelInteractedWith = clickedRondelIndex;
                    selectedLabel.Text = rondelListBox.Items[clickedRondelIndex].ToString();
                }
            }
        }

        // If the form itself registers the click, then still treat it as if a territory PictureBox was clicked
        private void GameForm_Click(object sender, EventArgs e)
        {
            IdentifyClickedTerritory(Cursor.Position);
            if (debugMode)
            {
                testingLabel1.Text = Cursor.Position.ToString();
            }
        }

        // Figure out the territory that was clicked
        public void BeforeProcessingTerritoryClicked(object sender, EventArgs e)
        {
            IdentifyClickedTerritory(Cursor.Position);
        }

        // On a click, figure out what territory was clicked
        public int IdentifyClickedTerritory(Point mouseLocation, bool setTerritoryClicked = true)
        {
            Rectangle screen = this.RectangleToScreen(this.ClientRectangle);
            Point screenRelativeMouseLocation = mouseLocation;
            // Get the position of the click relative to the form
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
                        // Only confirm it's a click if the colour there is completely solid, otherwise the click was on the edge of the territory and ignore it
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
                // Return the final result of the algorithm
                if (setTerritoryClicked)
                {
                    if (territoryClicked <= -1) territoryClicked = -1;
                    if (territoryLastClicked != territoryClicked) gameState.territoryInteractedWith = territoryClicked;
                    territoryLastClicked = gameState.territoryInteractedWith;
                }
                return territoryClicked;
            }
            return -1;
        }

        // When a territory is double clicked, show info unless if combat can be resolved there
        private void TerritoryDisplayInfoEvent(object sender, EventArgs e)
        {
            int territoryClicked = IdentifyClickedTerritory(Cursor.Position, false);
            if (territoryClicked > -1)
            {
                if (((gameState.gameStateTracker == 20) && (gameState.territories[territoryClicked].land == false)) ||
                    ((gameState.gameStateTracker == 12) && (gameState.territories[territoryClicked].land == true)))
                {
                    // There should only be one CombatForm open at any given time
                    if (Application.OpenForms.OfType<CombatForm>().Count() == 0)
                    {
                        if (!gameState.ResolveCombat(territoryClicked, gameState.currentNation))
                        {
                            DisplayTerritoryInfo(territoryClicked);
                        }
                        else
                        {
                            UpdateTerritoryOccupierIndicator(territoryClicked);
                        }
                    }
                    else
                    {
                        DisplayTerritoryInfo(territoryClicked);
                    }
                }
                else
                {
                    DisplayTerritoryInfo(territoryClicked);
                }
            }
        }

        // Show information about a territory
        private void DisplayTerritoryInfo(int territoryID)
        {
            // Line 1 (territory name and land/sea status)
            string line1;
            if (gameState.territories[territoryID].land)
            {
                line1 = "[LAND] ";
            }
            else
            {
                line1 = "[SEA] ";
            }
            line1 += gameState.territories[territoryID].name;
            if (!String.IsNullOrEmpty(gameState.territories[territoryID].trigram))
            {
                line1 += $" ({gameState.territories[territoryID].trigram})";
            }
            // Line 2 (controlling nation or home nation)
            string line2 = "";
            string nationName = "no nation";
            string controllerName = "no nation";
            if (gameState.territories[territoryID].nation != null)
            {
                nationName = gameState.nations[gameState.territories[territoryID].nation ?? -1].name;
            }
            else if (gameState.territories[territoryID].currentNation != null)
            {
                controllerName = gameState.nations[gameState.territories[territoryID].currentNation ?? -1].name;
            }
            if (nationName != "no nation")
            {
                line2 += $"Home territory of {nationName}";
            }
            else if (controllerName != "no nation")
            {
                line2 += $"Currently occupied by {controllerName}";
            }
            // Line 2.1 (current factory status if is home nation)
            string line2_1 = "\n";
            if (nationName != "no nation")
            {
                int factoryState = gameState.territories[territoryID].GetFactory();
                if (factoryState == 0)
                {
                    line2_1 += "Land factory present";
                }
                else if (factoryState == 1)
                {
                    line2_1 += "Sea factory present";
                }
                else if (gameState.territories[territoryID].factory)
                {
                    line2_1 += "Potential for land factory";
                }
                else
                {
                    line2_1 += "Potential for sea factory";
                }
                line2 += line2_1;
            }
            // Line 3 (current units)
            string line3 = "Occupying units:\n";
            foreach (MilitaryUnit unit in gameState.territories[territoryID].occupying_units)
            {
                string hostile = "";
                string landBoatString = "(Army)";
                if (unit.isBoat)
                {
                    landBoatString = "(Fleet)";
                }
                if (unit.isHostile)
                {
                    hostile = "[HOSTILE]";
                }
                line3 += $"{hostile} {gameState.nations[unit.nationID].name} {landBoatString}\n";
            }
            if (gameState.territories[territoryID].occupying_units.Count == 0)
            {
                line3 += "(none)";
            }
            // Display
            MessageBox.Show($"{line2}\n{line3}", line1);
        }

        // Invoke the territory changed event
        protected virtual void OnLastTerritoryChanged()
        {
            // If null, do not invoke
            LastTerritoryChanged?.Invoke(this, EventArgs.Empty);
        }

        // Update the selected label when a territory is clicked
        private void UpdateSelectedLabelTerritoryClicked(object sender, EventArgs e)
        {
            if (territoryLastClicked != -1)
            {
                selectedLabel.Text = $"{gameState.territories[territoryLastClicked].name} ({gameState.territories[territoryLastClicked].trigram})";
            }
        }

        // Debug, show the last clicked territory
        private void UpdateTerritoryLabelsOnClickDebug(object sender, EventArgs e)
        {
            testingLabel1.Text = territoryLastClicked.ToString();
        }

        // Update the label that indicates what is to be done currently
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
                        "Determine land based conflicts", // 12
                        "Select army to move", // 13
                        "Select bond to purchase", // 14
                        "Calculating taxation action...", // 15
                        "Select territory to build in", // 16
                        "Select territories to produce", // 17
                        "Finish turn", // 18
                        "Select fleet to move", // 19
                        "Determine sea based conflicts"  // 20
                        };
                    toOutput += actionLabelTexts[gameState.gameStateTracker + 1];
                    currentActionLabel.Text += toOutput;
                }
            }
        }

        // Inform the GameState that changes should be made
        // Use the information changes from within the GameState to update graphics displays
        private void ConfirmButtonClicked(object sender, EventArgs e)
        {
            int changesEnactedFlag = gameState.EnactChanges(territoryLastClicked);
            if (changesEnactedFlag == 0)
            {
                // Game has not yet started
                if (gameState.gameStarted == false)
                {
                    UpdateBondsDataGridView();
                    UpdatePlayerMoney();
                    UpdateNationTreasury();
                    gameState.BeforeGameStartNextPlayer();
                }
                // Bond purchasing after a turn
                else if (gameState.gameStateTracker == 14)
                {
                    gameState.AfterGameStartNextPlayer();
                    UpdateBondsDataGridView();
                    UpdatePlayerMoney();
                    UpdateNationTreasury();
                }
                // Purchasing bonds during gameplay
                else if ((gameState.gameStateTracker == 7) || (gameState.gameStateTracker == 8) || (gameState.gameStateTracker == 14))
                {
                    UpdateBondsDataGridView();
                    UpdatePlayerMoney();
                    UpdateNationTreasury();
                }
                // After all ships have been moved in Manoeuvre - state changed in EnactChanges
                else if (gameState.gameStateTracker == 20)
                {
                    ResolveAllCombat(gameState.currentNation, false);
                }
                // After all units have been moved in Manoeuvre - state changed in EnactChanges
                else if (gameState.gameStateTracker == 12)
                {
                    ResolveAllCombat(gameState.currentNation, true);
                }
                // Turn was ended after pressing confirm button, do post turn end things
                else if (gameState.gameStateTracker == 18)
                {
                    if (!gameState.investorCard)
                    {
                        gameState.PostTurnInvestorlessBondPurchasing();
                    }
                    UpdateBondsDataGridView();
                    UpdatePlayerMoney();
                    UpdateNationTreasury();
                }

                // If the game has ended (game ends after Taxation action exclusively)
                if (gameState.gameEnded)
                {
                    DisplayGameEndStats();
                    this.Close();
                    this.Dispose();
                    return;
                }
                // Production
                if (gameState.gameStateTracker == 17)
                {
                    skipButton.Text = "Produce all";
                }
                else
                {
                    skipButton.Text = "Skip action";
                }
                // Turn ended
                if (gameState.gameStateTracker == 18)
                {
                    confirmButton.Text = "End turn";
                }
                else
                {
                    confirmButton.Text = "Confirm";
                }
                // At the start of each turn
                if (gameState.gameStateTracker == -1)
                {
                    UpdateNationDataGridView();
                }
                if (territoryLastClicked != -1)
                {
                    RefreshAllAdjacentLandTerritories(territoryLastClicked);
                }
                // Update the nation rondel display
                if (gameState.rondelInteractedWith != null)
                {
                    nationsOverview.Rows[gameState.currentNation].Cells[2].Value = rondelListBox.Items[gameState.rondelInteractedWith ?? -1];
                }
                gameState.ClearRondelInteractedWithTracker();
                UpdateLabels();
                UpdatePlayerMoney();
            }
            // Error states
            /* CHANGESENACTEDFLAG states
             0: [OK] everything is OK
             1: [ERROR] gameState.EnactChanges failed to meet any of its if statements
             2: [OK] no rondel slot was selected in gameState == -1
             3: [ERROR] rondel slot selected was out of bounds in gameState == -1
             4: [OK] player didn't select a bond when purchasing bonds
             5: [OK] action not able to be carried out in gameState == 6 (investor) / gameState == 16 (factory)
             6: [OK] territory already contains factory in gameState == 16 (factory)
             7: [OK] territory restricted by hostile units in gameState == 16 (factory)
             8: [OK] territory ineligible for factory selected in gameState == 16 (factory)
             9: [OK] player advanced too far on selecting rondel
            10: [OK] player has not enough money to advance that far on rondel
            */
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
            else if (changesEnactedFlag == 6)
            {
                selectedLabel.Text = "Factory already present in territory";
            }
            else if (changesEnactedFlag == 7)
            {
                selectedLabel.Text = "Territory restricted by hostile forces";
            }
            else if (changesEnactedFlag == 8)
            {
                selectedLabel.Text = "Cannot build factory in selected territory";
            }
            else if (changesEnactedFlag == 9)
            {
                selectedLabel.Text = "Must advance 1 to 6 spaces per turn";
            }
            else if (changesEnactedFlag == 10)
            {
                selectedLabel.Text = "Not enough money to advance on rondel";
            }
        }

        // Skip things where possible
        private void SkipButtonClicked(object sender, EventArgs e)
        {
            if (!gameState.gameStarted)
            {
                SkipToNextTurn();
            }
            // Production - instead of skip, the button is 'Produce All'
            else if (gameState.gameStateTracker == 17)
            {
                gameState.ProductionProduceAll();
                skipButton.Text = "Skip action";
            }
            // Factory
            else if ((gameState.gameStateTracker == 16) && (!gameState.investorCard)) {
                gameState.gameStateTracker = 14;
            }
            else if (gameState.gameStateTracker == 16)
            {
                gameState.gameStateTracker = 18;
                confirmButton.Text = "End turn";
            }
            // After turn, purchasing bonds
            else if (gameState.gameStateTracker == 14)
            {
                gameState.AfterGameStartNextPlayer();
            }

            // If it is a new turn
            if (gameState.gameStateTracker == -1)
            {
                UpdateNationDataGridView();
            }
            UpdateLabels();
        }

        // A player wants to donate some of their money to a nation
        private void DonateButtonClicked(object sender, EventArgs e)
        {
            gameState.DonateMoney();
            UpdatePlayerMoney();
            UpdateNationTreasury();
        }

        // Move to the next player's turn
        private void SkipToNextTurn()
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
            UpdateLabels();
            Refresh();
        }

        // For every territory of one type (land or sea), resolve combat
        private void ResolveAllCombat(int nationInvading, bool resolveLand)
        {
            while (Application.OpenForms.OfType<CombatForm>().Count() != 0)
            {
                Application.OpenForms.OfType<CombatForm>().First().Dispose();
            }
            foreach (Territory territory in gameState.territories)
            {
                // If the two are equal, then we are currently resolving conflict in that type of territory
                if (!(resolveLand^territory.land))
                {
                    gameState.ResolveCombat(territory.id, nationInvading);
                    UpdateTerritoryOccupierIndicator(territory.id);
                }
            }
        }

        // Change territory labels dependent on which territory was clicked
        private void PerformTerritoryClickedAction(object sender, EventArgs e)
        {
            // If not debugging territory adjacencies, then act as usual
            if (!territoryAdjacency)
            {
                gameState.TerritoryWasClicked();
                if (territoryLastClicked != -1)
                {
                    UpdateTerritoryOccupierIndicator(territoryLastClicked);
                    RefreshAllAdjacentLandTerritories(territoryLastClicked);
                }
            }
            // This will cause unexpected behaviour if the game continues to be played
            else
            {
                DebugShowTerritoryAdjacency();
            }
        }

        // Repaints all adjacent land territories, but no others, to prevent flickering when a territory is clicked
        // while simultaneously providing accurate information
        private void RefreshAllAdjacentLandTerritories(int territoryAtCentre)
        {
            territoryPictureBoxes[territoryAtCentre].Refresh();
            if (!gameState.territories[territoryAtCentre].land)
            {
                for (int i = 0; i < gameState.territories[territoryAtCentre].adjacent.Length; ++i)
                {
                    if (gameState.territories[gameState.territories[territoryAtCentre].adjacent[i]].land == true)
                    {
                        territoryPictureBoxes[gameState.territories[territoryAtCentre].adjacent[i]].Refresh();
                    }
                }
            }
        }

        // Updates the colour of the label showing the territory name so the colour matches the controlling nation of the territory
        private void UpdateTerritoryOccupierIndicator(int territoryToChange)
        {
            if ((gameState.territories[territoryToChange].currentNation != null) && (gameState.territories[territoryToChange].currentNation != -1))
            {
                territoryPictureBoxes[territoryToChange].lblTerritoryName.BackColor = gameState.nations[gameState.territories[territoryToChange].currentNation ?? -1].primaryColour;
                territoryPictureBoxes[territoryToChange].lblTerritoryName.ForeColor = gameState.nations[gameState.territories[territoryToChange].currentNation ?? -1].monoContrastColour;
            }
        }

        private void DisplayGameEndStats()
        {
            List<Tuple<int, int> > scores = new List<Tuple<int, int> >();
            for (int i = 0; i < gameState.playerCount; ++i)
            {
                scores.Add(new Tuple<int, int>(gameState.CalculatePlayerScore(i), i));
            }
            scores.Sort();
            string results = "Game ended\n";
            for (int i = scores.Count - 1; i >= 0; --i)
            {
                results += $"[{scores[i].Item2 + 1}] {gameState.players[scores[i].Item2].name}: {scores[i].Item1}\n";
            }
            MessageBox.Show(results, "Game ended");
            this.Close();
        }

        // Update relevant displayed information
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
                            bondsDataGridView[j, i].Value = (gameState.bonds[i][j].playerID + 1).ToString();
                        }
                    }
                }
            }
        }

        // Update the labels according to the current game state
        private void UpdateLabels()
        {
            currentPlayerLabel.Text = playerOverview.Rows[gameState.currentPlayerAction].HeaderCell.Value.ToString();
            currentNationLabel.Text = gameState.nations[gameState.currentNation].name;
            if (gameState.swissBankInvesting)
            {
                currentNationLabel.Text = "Any";
            }
            selectedLabel.Text = ($"{gameState.GetNationBond(gameState.nonPlayerBondInteractedWith ?? (-1, -1))} / " +
                $"{gameState.GetNationBond(gameState.playerBondInteractedWith ?? (-1, -1))}");
            UpdateCurrentActionLabel();
        }

        // Reopen the main menu when form is closed
        private void GameForm_FormClosed(object sender, FormClosedEventArgs e)
        {
            this.Owner.Opacity = 1;
        }

        private void DebugShowTerritoryAdjacency()
        {
            if (territoryLastClicked != -1)
            {
                for (int i = 0; i < gameState.territories.Count; ++i)
                {
                    if (gameState.territories[territoryLastClicked].adjacent.Contains(i))
                    {
                        territoryPictureBoxes[i].lblTerritoryName.BackColor = Color.Chartreuse;
                    }
                    else
                    {
                        territoryPictureBoxes[i].lblTerritoryName.BackColor = Color.Transparent;
                    }
                }
            }
        }

        private void Debug1ButtonClicked(object sender, EventArgs e)
        {
            gameState.Debug1Function();
        }

        private void DebugButtonEndGameClicked(object sender, EventArgs e)
        {
            gameState.gameEnded = true;
        }

        private void DebugButtonTerritoriesClicked(object sender, EventArgs e)
        {
            if (MessageBox.Show("Debugging, irreversible: Unexpected behaviour ahead. Continue?", "Debug", MessageBoxButtons.YesNo) == DialogResult.Yes)
            {
                territoryAdjacency = true;
            }
        }
    }
}
