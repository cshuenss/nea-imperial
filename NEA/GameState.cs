using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Windows.Forms;

namespace NEA
{
    public class GameState
    {
        // Debug variables
        private bool debugMode;
        public bool invalidJsonSelfDestruct = false;

        // Game settings
        public string jsonFileLocation;
        public int playerCount;
        public int startingMoney;
        public bool randomBondsToStart;
        public bool nineBonds;
        public bool investorCard;
        public bool taxTypeIsWorld;
        public int[] taxThresholds;

        // Game information about the main parties involved
        public List<Nation> nations = new List<Nation>();
        public List<Territory> territories = new List<Territory>();
        public List<Player> players = new List<Player>();
        public List<List<NationBond>> bonds = new List<List<NationBond>>(); // Nation then bond

        /*  GAME STATES
            -WHEN GameStarted == false:
         (Selecting bonds)
            -WHEN GameStarted == true:
        -1: Start of player turn (R = 2)
         0: Investor (Confirm)
         1: Import (Confirm)
         2: Manoeuvre (Confirm)
         3: Production (Confirm)
         4: Factory (Confirm)
         5: Taxation (Confirm)
        INVESTOR:
         6: Paying out investor (Automated)
         7: Investor card holder selecting bonds (R = 1)
         8: Swiss Bank holder selecting bonds (R = 1)
        14: Player selecting bonds at the end of a turn in game without investor card (R = 1)
        IMPORT:
        10: Selecting territories to place units in (R = 0)
        MANOEUVRE:
        20: Deciding combat after movement of fleets ()
        12: Deciding combat after movement of armies ()
        13: Moving armies (no unit currently selected) (R = 0)
        19: Moving fleets (no unit currently selected) (R = 0)
        11: Unit selected (R = 0)
         9: Fleet selected (R = 0)
        PRODUCTION:
        17: Selecting territories to produce (R = 0)
        FACTORY:
        16: Selecting territory to place factory (R = 0)
        TAXATION:
        15: Automated calculation (Automated)
        MISC:
        18: Turn end (Confirm)
        */

        /*  REQUIRED INPUT
         0: Territory (No confirmation needed)
         1: Bond (Confirmation needed)
         2: Rondel (Confirmation needed)
        */

        /*  CURRENT NATION tracked by nationID, -1 means any nation  */

        // Intra-turn data
        public Nullable<(int, int)> nonPlayerBondInteractedWith = null;
        public Nullable<(int, int)> playerBondInteractedWith = null;
        public int territoryInteractedWith = -1;
        public Nullable<int> rondelInteractedWith = null;
        public MilitaryUnit selectedUnit = new MilitaryUnit();
        public int selectedUnitFrom = -1;
        public List<int> ferriedThisMovement = new List<int>();
        public int intermediateUnitLocation = -1;
        private List<int> producedThisTurn = new List<int>();
        public bool swissBankInvesting = false;
        private bool firstPlayerInvestedYet = false;

        // Inter-turn data
        public int gameStateTracker = -1;
        public bool gameStarted = false;   // Whether turns have started (so is false when selecting bonds)
        public int gameResult = -1;
        public bool gameEnded = false;
        public int currentPlayerTurn = 0;
        public int currentPlayerAction = 0;
        public int currentNation = 0;

        // Counters
        public int turnCounter = 0;
        public int generalCounter = 0;

        // Returns the bond at the given index in the bonds 2D array
        public NationBond GetNationBond((int, int) index)
        {
            try {
                return bonds[index.Item1][index.Item2];
            }
            catch (ArgumentOutOfRangeException)
            {
                return new NationBond();
            }
        }

        public GameState(bool debugMode,
            List<Player> in_players,
            string in_json, 
            int in_playerCount,
            bool in_randomBonds,
            bool in_nineBonds,
            bool in_investorCard,
            bool in_taxTypeIsWorld)
        {
            this.debugMode = debugMode;

            // Populate settings data
            players = in_players;
            jsonFileLocation = in_json;
            playerCount = in_playerCount;
            randomBondsToStart = in_randomBonds;
            nineBonds = in_nineBonds;
            investorCard = in_investorCard;
            taxTypeIsWorld = in_taxTypeIsWorld;

            // Parse the JSON, if unsuccessful then return to settings menu
            try
            {
                ParseJSON(jsonFileLocation);
            }
            catch (JsonException)
            {
                invalidJsonSelfDestruct = true;
            }
            catch (ArgumentNullException)
            {
                invalidJsonSelfDestruct = true;
            }
            catch (ApplicationException)
            {
                invalidJsonSelfDestruct = true;
            }
            // Populate the bonds
            for (int i = 0; i < nations.Count; ++i)
            {
                bonds.Add(new List<NationBond>());
                bonds[i].Add(new NationBond(2, 1, nations[i].id, nations[i].name));
                bonds[i].Add(new NationBond(4, 2, nations[i].id, nations[i].name));
                bonds[i].Add(new NationBond(6, 3, nations[i].id, nations[i].name));
                bonds[i].Add(new NationBond(9, 4, nations[i].id, nations[i].name));
                bonds[i].Add(new NationBond(12, 5, nations[i].id, nations[i].name));
                bonds[i].Add(new NationBond(16, 6, nations[i].id, nations[i].name));
                bonds[i].Add(new NationBond(20, 7, nations[i].id, nations[i].name));
                bonds[i].Add(new NationBond(25, 8, nations[i].id, nations[i].name));
                if (nineBonds)
                {
                    bonds[i].Add(new NationBond(30, 9, nations[i].id, nations[i].name));
                }
            }
            if (randomBondsToStart)
            {
                DistributeRandomBonds();
                StartNextTurn(0);
            }
        }

        // Calculate n! (if n is 20 or less)
        private long Factorial(int n)
        {
            if (n > 20) return -1;
            long ans = 1;
            for (int i = 2; i <= n; ++i)
            {
                ans *= i;
            }
            return ans;
        }

        // Calculate the permutation-th permutation of an array of items from 1 to items, of length items
        private int[] getPermutationForRandomBonds(int items, long permutation, bool oneIndexed = false)
        {
            int[] permutationArray = new int[items];
            List<int> digits = new List<int>();
            int[] factoradic = new int[items];
            for (int i = 0; i < items; i++)
            {
                digits.Add(i + 1);
            }
            // Calculate factoradic
            for (int i = 1; i <= items; ++i)
            {
                factoradic[items - i] = (int)(permutation % i);
                permutation /= i;
            }
            // Calculate final array
            for (int i = 0; i < items; ++i)
            {
                permutationArray[i] = digits[factoradic[i]];
                if (!oneIndexed)
                {
                    permutationArray[i]--;
                }
                digits.RemoveAt(factoradic[i]);
            }
            return permutationArray;
        }

        // Deserialise the map data JSON.
        private void ParseJSON(string file)
        {
            // If any error is raised, don't attempt to further load the map - instead return to menu
            try
            {
                string mapData = System.IO.File.ReadAllText(file);
                using (JsonDocument mapJson = JsonDocument.Parse(mapData))
                {
                    // Deal out starting money
                    if (randomBondsToStart)
                    {
                        JsonElement randomBondValues = mapJson.RootElement.GetProperty("randombonds");
                        startingMoney = randomBondValues[Math.Max(0, playerCount - 2)].GetInt32();
                    }
                    else
                    {
                        JsonElement choiceBondValues = mapJson.RootElement.GetProperty("choicebonds");
                        startingMoney = choiceBondValues[Math.Max(0, playerCount - 2)].GetInt32();
                    }
                    foreach (Player player in players)
                    {
                        player.money = startingMoney;
                    }

                    // Map data known to be split into nations and territories, handle separately.
                    JsonElement taxThresholdsJson = mapJson.RootElement.GetProperty("taxthresholds");
                    JsonElement bondPermutation = mapJson.RootElement.GetProperty("randombondpermutation");
                    JsonElement nationsJson = mapJson.RootElement.GetProperty("nations");
                    JsonElement territoriesJson = mapJson.RootElement.GetProperty("territories");
                    foreach (JsonElement nationJson in nationsJson.EnumerateArray())
                    {
                        nations.Add(JsonSerializer.Deserialize<Nation>(nationJson.ToString()));
                        nations.Last().SetColoursWhenGiven();
                        // Lower case strings are safe null states
                        nations.Last().name = Char.ToUpper(nations.Last().name[0]).ToString() + nations.Last().name.Remove(0, 1);
                        nations.Last().maxLandUnits = nations.Last().landUnits;
                        nations.Last().maxSeaUnits = nations.Last().seaUnits;
                    }
                    if (nations.Count != mapJson.RootElement.GetProperty("count").GetInt32())
                    {
                        throw new ApplicationException("Invalid map data: Nation count misaligned");
                    }
                    // Too many nations for the permutation code
                    if (nations.Count > 20)
                    {
                        throw new ApplicationException("Too many nations!");
                    }
                    foreach (JsonElement territoryJson in territoriesJson.EnumerateArray())
                    {
                        Territory currentTerritory = JsonSerializer.Deserialize<Territory>(territoryJson.ToString());
                        currentTerritory = AddTerritoryBitmap(currentTerritory, jsonFileLocation);
                        if (currentTerritory.start == true)
                        {
                            currentTerritory.hasFactory = true;
                        }
                        territories.Add(currentTerritory);
                    }

                    // Process remaining header data
                    int headerCounter = 0;
                    foreach (int secondaryBondData in getPermutationForRandomBonds(nations.Count, bondPermutation.GetInt64()))
                    {
                        nations[headerCounter].secondaryBonds = secondaryBondData;
                        headerCounter++;
                    }
                    headerCounter = 0;
                    taxThresholds = new int[taxThresholdsJson.GetArrayLength()];
                    foreach (JsonElement taxThresholdJson in taxThresholdsJson.EnumerateArray())
                    {
                        taxThresholds[headerCounter] = taxThresholdJson.GetInt32();
                        headerCounter++;
                    }
                }
            }
            catch
            {
                invalidJsonSelfDestruct = true;
            }
        }

        // Add a bitmap to the territory in preparation for the PictureBox of the territory
        private Territory AddTerritoryBitmap(Territory territory, string jsonFileName)
        {
            string fileName = System.IO.Path.Combine(jsonFileName.Remove(jsonFileName.Length - 5), territory.file);
            territory.bitmap = new Bitmap(fileName);
            return territory;
        }

        // Gives each player random bonds. Not all permutations will be possible for higher nation counts
        // To fix the range of possible distributions, implement a RNG for longs
        private void DistributeRandomBonds()
        {
            Random random = new Random();
            // Get a random order for bonds distribution
            int nationsRemaining = nations.Count;
            int playerMult = 0;
            int[] order = getPermutationForRandomBonds(nations.Count,
                random.Next(0, nations.Count < 12 ? (int)Factorial(nations.Count) : Int32.MaxValue));
            while (nationsRemaining >= playerCount)
            {
                for (int i = 0; i < playerCount; ++i)
                {
                    int primaryNation = order[i + playerCount * playerMult];
                    int secondaryNation = nations[order[i + playerCount * playerMult]].secondaryBonds;
                    bonds[primaryNation][3].playerID = i;
                    bonds[secondaryNation][0].playerID = i;
                    nations[primaryNation].treasury += bonds[primaryNation][3].cost;
                    nations[secondaryNation].treasury += bonds[secondaryNation][0].cost;
                    players[i].money -= (bonds[primaryNation][3].cost + bonds[secondaryNation][0].cost);
                    nationsRemaining--;
                }
                playerMult++;
            }
        }

        // Reset intra-turn variables
        public void ChangePlayerToAct(int playerToAct)
        {
            currentPlayerAction = playerToAct;
            if (playerToAct == -1)
            {
                currentPlayerAction = ((turnCounter - 1) % playerCount);
            }
            currentPlayerTurn = currentPlayerAction;
            nonPlayerBondInteractedWith = null;
            playerBondInteractedWith = null;
            territoryInteractedWith = -1;
            rondelInteractedWith = null;
            ferriedThisMovement = new List<int>();
            producedThisTurn = new List<int>();
            swissBankInvesting = false;
            firstPlayerInvestedYet = false;
        }

        // Set the game state based on the rondel slot selected
        public int RondelSlotSelected()
        {
            int potentialGameState = rondelInteractedWith ?? -1;
            if (potentialGameState == -1)
            {
                return 2;
            }
            // Swiss bank holders can force the investor action...
            if ((potentialGameState <= nations[currentNation].lastRondelSlot) && (nations[currentNation].lastRondelSlot != -1))
            {
                potentialGameState += 8;
                // if it is passed over...
                if (potentialGameState > 8)
                {
                    List<int> swissBankHolders = GetSwissBankHolders();
                    string swissBankString = "";
                    for (int i = 0; i < swissBankHolders.Count; ++i)
                    {
                        swissBankString += $"[{i}] {players[i]}\n";
                    }
                    // and there is sufficient money to pay out money...
                    int interestToPay = 0;
                    foreach (NationBond bond in bonds[currentNation])
                    {
                        if (bond.playerID != -1)
                        {
                            interestToPay += bond.interest;
                        }
                    }
                    if (interestToPay <= nations[currentNation].treasury)
                    {
                        // and if they want to...
                        if (swissBankString != "")
                        {
                            DialogResult result = MessageBox.Show(
                                $"Any of:\n{swissBankString}Force {nations[currentNation].name} to take Investor action?",
                                "Force investor", MessageBoxButtons.YesNo);
                            if (result == DialogResult.Yes)
                            {
                                potentialGameState = 8;
                                MessageBox.Show("Investor action forced by swiss bank holder", "Forced investor");
                            }
                        }
                    }
                }
            }
            if (nations[currentNation].lastRondelSlot != -1)
            {
                int moneyToPay = Math.Max(potentialGameState - nations[currentNation].lastRondelSlot - 3, 0);
                // Advancing too far in one turn
                if (moneyToPay > 3)
                {
                    return 9;
                }
                // Not enough money to pay spaces advanced
                if (players[nations[currentNation].controllingPlayer].money < moneyToPay)
                {
                    return 10;
                }
                players[nations[currentNation].controllingPlayer].money -= moneyToPay;
            }

            gameStateTracker = potentialGameState % 8;
            if (gameStateTracker == 6) gameStateTracker = 2;
            if (gameStateTracker == 7) gameStateTracker = 3;
            nations[currentNation].lastRondelSlot = rondelInteractedWith ?? -1;

            switch (gameStateTracker)
            {
                case 0: // Investor
                    if (!AutomatedInvestor())
                    {
                        return 5;
                    }
                    return 0;
                case 1: // Import
                    gameStateTracker = 10;
                    return 0;
                case 2: // Production
                    gameStateTracker = 17;
                    return 0;
                case 3: // Manoeuvre
                    gameStateTracker = 19;
                    return 0;
                case 4: // Taxation
                    AutomatedTaxation();
                    return 0;
                case 5: // Factory
                    gameStateTracker = 16;
                    return 0;
                default:
                    gameStateTracker = -1;
                    return 3;
            }
        }

        // Set the bond clicked variable to the last clicked bond
        public void BondClicked((int, int) bondIndex)
        {
            if (GetNationBond(bondIndex).playerID == -1)
            {
                nonPlayerBondInteractedWith = bondIndex;
            }
            else if (GetNationBond(bondIndex).playerID == currentPlayerAction)
            {
                playerBondInteractedWith = bondIndex;
            }
        }

        private List<int> GetSwissBankHolders()
        {
            bool[] playerTracker = new bool[playerCount];
            for (int i = 0; i < nations.Count; i++)
            {
                if (nations[i].controllingPlayer != -1)
                {
                    playerTracker[nations[i].controllingPlayer] = true;
                }
            }
            List<int> swissBanks = new List<int>();
            for (int i = 0; i < playerCount; ++i) {
                if (playerTracker[i] == false)
                {
                    swissBanks.Add(i);
                }
            }
            return swissBanks;
        }

        // Get an array of the amount each player has invested in a nation
        public int[] GetPlayerInvestedValues(int nationID)
        {
            int[] playerInvestedValues = new int[players.Count];
            for (int i = 0; i < bonds[nationID].Count; ++i)
            {
                if (bonds[nationID][i].playerID != -1)
                {
                    playerInvestedValues[bonds[nationID][i].playerID] += bonds[nationID][i].cost;
                }
            }
            return playerInvestedValues;
        }

        // Get the amount a specific player has invested in a nation
        public int GetPlayerInvestedValues(int nationID, int playerID)
        {
            return GetPlayerInvestedValues(nationID)[playerID];
        }

        // Check if a player has surpassed the current player in amount invested in a particular nation, and if so update nation ownership
        private void UpdateNationOwnership(int nationID)
        {
            int[] playerInvestedValues = GetPlayerInvestedValues(nationID);
            int maximumInvested = 0;
            int investedBy = -1;
            for (int i = 0; i < playerInvestedValues.Length; ++i)
            {
                if (playerInvestedValues[i] > maximumInvested)
                {
                    maximumInvested = playerInvestedValues[i];
                    investedBy = i;
                }
                else if (playerInvestedValues[i] == maximumInvested)
                {
                    investedBy = -1;
                }
            }
            if (investedBy != -1)
            {
                nations[nationID].controllingPlayer = investedBy;
            }
        }

        // Update all nations' ownership
        private void UpdateNationOwnership()
        {
            for (int i = 0; i < nations.Count; ++i)
            {
                UpdateNationOwnership(i);
            }
        }

        // Set all movement statuses of units to having not moved, in preparation for a Manoeuvre action
        private void ResetAllUnitMovements()
        {
            for (int i = 0; i < territories.Count; ++i)
            {
                for (int j = 0; j < territories[i].occupying_units.Count; ++j)
                {
                    territories[i].occupying_units[j].hasMoved = false;
                    territories[i].occupying_units[j].hasFerried = false;
                }
            }
        }

        // Move onto the next nation to have its turn
        public void StartNextTurn(int preferredNation)
        {
            UpdateNationOwnership();
            gameStarted = true;
            gameStateTracker = -1;
            generalCounter = 0;

            ResetAllUnitMovements();
            selectedUnit = new MilitaryUnit();
            selectedUnitFrom = -1;
            intermediateUnitLocation = -1;

            if (!gameStarted)
            {
                bool gameSoftlockedByIncompetence = true;
                for (int i = 0; i < nations.Count; ++i)
                {
                    if ((i + preferredNation) % nations.Count != i + preferredNation)
                    {
                        turnCounter++;
                    }
                    if ((nations[(i + preferredNation) % nations.Count].controllingPlayer != -1) && (nations[(i + preferredNation) % nations.Count].controllingPlayer != null))
                    {
                        currentNation = (i + preferredNation) % nations.Count;
                        ChangePlayerToAct(nations[currentNation].controllingPlayer);
                        gameSoftlockedByIncompetence = false;
                        break;
                    }
                }
                if (gameSoftlockedByIncompetence)
                {
                    gameEnded = true;
                }
            }
            else
            {
                if (preferredNation == 0)
                {
                    turnCounter++;
                }
                currentNation = preferredNation % nations.Count;
                ChangePlayerToAct(nations[currentNation].controllingPlayer);
                if (nations[currentNation].controllingPlayer == -1)
                {
                    gameStateTracker = 14;
                }
            }
        }

        // When an action is confirmed, enact that action
        public int EnactChanges(int argument = -1)
        {
            int returnVal = 0;
            // Before game starts or gameStateTracker == 14 (after end of turn), players purchasing bonds
            if ((gameStarted == false) || (gameStateTracker == 14))
            {
                returnVal = PlayerPurchasesBonds();
            }
            // Start of turn, selecting rondel
            else if (gameStateTracker == -1)
            {
                returnVal = RondelSlotSelected();
            }
            // After factory
            else if (gameStateTracker == 16)
            {
                int stateToReturn = BuildFactory(argument);
                if (stateToReturn == 0)
                {
                    gameStateTracker = 18;
                }
                returnVal = stateToReturn;
            }
            // After import
            else if (gameStateTracker == 10)
            {
                gameStateTracker = 18;
            }
            // After production
            else if (gameStateTracker == 17)
            {
                gameStateTracker = 18;
            }
            // After finishing ship movements in Manoeuvre, start fleet combat
            else if (gameStateTracker == 19)
            {
                gameStateTracker = 20;
            }
            // After finishing fleet combat, start moving armies
            else if (gameStateTracker == 20)
            {
                gameStateTracker = 13;
            }
            // After finishing army movements in manoeuvre, start deciding army combat
            else if (gameStateTracker == 13)
            {
                gameStateTracker = 12;
            }
            // After finishing army combats, end the turn
            else if (gameStateTracker == 12)
            {
                gameStateTracker = 18;
            }
            // End the turn
            else if (gameStateTracker == 18)
            {
                if (investorCard)
                {
                    StartNextTurn((currentNation + 1) % nations.Count);
                }
            }
            else
            {
                returnVal = 1;
            }

            nonPlayerBondInteractedWith = null;
            playerBondInteractedWith = null;
            territoryInteractedWith = -1;
            return returnVal;
        }

        // Done separately from EnactChanges so changes can be reflected across the display
        public void ClearRondelInteractedWithTracker()
        {
            rondelInteractedWith = null;
        }

        // Go to the next player at the start of the game, if purchasing bonds before game start 
        public void BeforeGameStartNextPlayer()
        {
            if (currentPlayerAction < playerCount - 1)
            {
                ChangePlayerToAct(currentPlayerAction + 1);
            }
            else
            {
                if (currentNation < nations.Count - 1)
                {
                    currentNation++;
                    ChangePlayerToAct(0);
                }
                else
                {
                    StartNextTurn(0);
                }
            }
        }

        public void AfterGameStartNextPlayer()
        {
            if (!swissBankInvesting)
            {
                currentPlayerAction++;
                currentPlayerAction %= playerCount;
            }
            if (currentPlayerAction == currentPlayerTurn)
            {
                swissBankInvesting = true;
            }
            if (swissBankInvesting) {
                do
                {
                    currentPlayerAction++;
                    currentPlayerAction %= playerCount;
                    if (GetSwissBankHolders().Contains(currentPlayerAction))
                    {
                        return;
                    }
                } while (currentPlayerAction != currentPlayerTurn);
                StartNextTurn((currentNation + 1) % nations.Count);
            }
        }

        // A single player purchases or exchanges a bond
        private int PlayerPurchasesBonds()
        {
            (int, int) nonPlayerBondIndex = nonPlayerBondInteractedWith ?? (-1, -1);
            (int, int) playerBondIndex = playerBondInteractedWith ?? (-1, -1);
            NationBond nonPlayerBond = GetNationBond(nonPlayerBondIndex);
            NationBond playerBond = GetNationBond(playerBondIndex);
            // There must be a bond selected that the player does not currently own for this to be successful
            if ((nonPlayerBondInteractedWith != null) && ((nonPlayerBond.nationID == currentNation) || (swissBankInvesting)))
            {
                // Exchanging bonds
                if ((playerBondInteractedWith != null) && (playerBond.nationID == nonPlayerBond.nationID))
                {
                    if (playerBond.cost < nonPlayerBond.cost)
                    {
                        if (players[currentPlayerAction].money - nonPlayerBond.cost + playerBond.cost >= 0)
                        {
                            players[currentPlayerAction].money -= nonPlayerBond.cost - playerBond.cost;
                            nations[nonPlayerBond.nationID].treasury += nonPlayerBond.cost - playerBond.cost;
                            nonPlayerBond.playerID = currentPlayerAction;
                            playerBond.playerID = -1;
                            return 0;
                        }
                    }
                }
                // Purchasing a new bond
                else if (players[currentPlayerAction].money - nonPlayerBond.cost >= 0)
                {
                    players[currentPlayerAction].money -= nonPlayerBond.cost;
                    nations[nonPlayerBond.nationID].treasury += nonPlayerBond.cost;
                    nonPlayerBond.playerID = currentPlayerAction;
                    return 0;
                }
            }
            return 4;
        }

        // At the end of the turn, if there is no investor card then start the post-turn bond purchasing procedure
        public void PostTurnInvestorlessBondPurchasing()
        {
            gameStateTracker = 14;
        }

        // Donate money from a player own pocket to a nation's treasury
        public void DonateMoney()
        {
            DonateForm donateForm = new DonateForm(nations, players);
            if (donateForm.ShowDialog() == DialogResult.OK)
            {
                List<int> donateResult = donateForm.GetDonation();
                players[donateResult[0]].money -= donateResult[2];
                nations[donateResult[1]].treasury += donateResult[2];
            }
            donateForm.Dispose();
        }

        // Conduct actions when territory was clicked. Could be Import, Manoeuvre or Production (not Factory as that requires Confirm button)
        public void TerritoryWasClicked()
        {
            if (territoryInteractedWith != -1)
            {
                // Import
                if (gameStateTracker == 10)
                {
                    if (generalCounter < 3)
                    {
                        if (nations[currentNation].treasury >= 1)
                        {
                            if ((territories[territoryInteractedWith].nation == currentNation) &&
                                (!territories[territoryInteractedWith].RestrictedByHostiles()))
                            {
                                // If the territory has a shipyard then either unit may be imported there
                                if (!territories[territoryInteractedWith].factory)
                                {
                                    ImportTypeSelectionForm importSelector = new ImportTypeSelectionForm();
                                    DialogResult selectedImport = importSelector.ShowDialog();
                                    if (selectedImport != DialogResult.Cancel)
                                    {
                                        generalCounter++;
                                        // Army
                                        if (selectedImport == DialogResult.Abort)
                                        {
                                            PlaceUnitInTerritory(territoryInteractedWith, new MilitaryUnit(false, currentNation, false), false);
                                        }
                                        // Fleet
                                        else if (selectedImport == DialogResult.Ignore)
                                        {
                                            PlaceUnitInTerritory(territoryInteractedWith, new MilitaryUnit(true, currentNation, false), false);
                                        }
                                        nations[currentNation].treasury--;
                                    }
                                    else
                                    {
                                        MessageBox.Show("Unit not imported");
                                    }
                                    importSelector.Dispose();
                                }
                                else
                                {
                                    generalCounter++;
                                    PlaceUnitInTerritory(territoryInteractedWith, new MilitaryUnit(false, currentNation, false), false);
                                    nations[currentNation].treasury--;
                                }
                            }
                            else
                            {
                                MessageBox.Show("Territory is not home territory, or is restricted by hostiles", "Cannot import here");
                            }
                        }
                        else
                        {
                            MessageBox.Show("Not enough money in nation treasury to import units", "Insufficient funds");
                        }
                    }
                    else
                    {
                        MessageBox.Show("Can only import up to three units per import", "Import limit exceeded");
                    }
                }
                // Manoeuvre - select boat to move
                else if (gameStateTracker == 19)
                {
                    for (int i = 0; i < territories[territoryInteractedWith].occupying_units.Count; ++i)
                    {
                        if (territories[territoryInteractedWith].occupying_units[i].nationID == currentNation)
                        {
                            if ((territories[territoryInteractedWith].occupying_units[i].isBoat) &&
                            (!territories[territoryInteractedWith].occupying_units[i].hasMoved))
                            {
                                selectedUnit = territories[territoryInteractedWith].occupying_units[i];
                                selectedUnitFrom = territoryInteractedWith;
                                territories[territoryInteractedWith].occupying_units.RemoveAt(i);
                                gameStateTracker = 9;
                                break;
                            }
                        }
                    }
                }
                // Manoeuvre - select place to move boat to
                else if (gameStateTracker == 9)
                {
                    if (!territories[territoryInteractedWith].land)
                    {
                        if (territories[selectedUnitFrom].adjacent.Contains(territoryInteractedWith))
                        {
                            territories[territoryInteractedWith].occupying_units.Add(selectedUnit);
                            territories[territoryInteractedWith].occupying_units.Last().hasMoved = true;
                            FlagTerritoriesAfterMovement(selectedUnitFrom, territoryInteractedWith);
                        }
                        else
                        {
                            bool isCanalMovement = false;
                            for (int i = 0; i < territories[selectedUnitFrom].canal_connect.Length; ++i)
                            {
                                if (territories[selectedUnitFrom].canal_connect[i].Length > 0)
                                {
                                    if (territories[selectedUnitFrom].canal_connect[i][0] == territoryInteractedWith)
                                    {
                                        // Moving through canal, needs confirmation from player controlling canal
                                        isCanalMovement = true;
                                        bool canalAccess = true;
                                        if (territories[territories[selectedUnitFrom].canal_connect[i][1]].currentNation != null)
                                        {
                                            if (territories[territories[selectedUnitFrom].canal_connect[i][1]].currentNation != currentNation)
                                            {
                                                string part1 = $"{players[nations[territories[territories[selectedUnitFrom].canal_connect[i][1]].currentNation ?? -1].controllingPlayer].name} ";
                                                string part2 = $"(Player {nations[territories[territories[selectedUnitFrom].canal_connect[i][1]].currentNation ?? -1].controllingPlayer + 1}): ";
                                                string part3 = $"Allow ship from {nations[currentNation].name} to move from {territories[selectedUnitFrom].name} ";
                                                string part4 = $"to {territories[territoryInteractedWith].name}?";
                                                DialogResult buttonClicked = MessageBox.Show(part1 + part2 + part3 + part4, "Allow access?", MessageBoxButtons.YesNo);
                                                if (buttonClicked == DialogResult.No)
                                                {
                                                    canalAccess = false;
                                                }
                                            }
                                        }
                                        if (canalAccess)
                                        {
                                            territories[territoryInteractedWith].occupying_units.Add(selectedUnit);
                                            territories[territoryInteractedWith].occupying_units.Last().hasMoved = true;
                                            FlagTerritoriesAfterMovement(selectedUnitFrom, territoryInteractedWith);
                                        }
                                        else
                                        {
                                            territories[selectedUnitFrom].occupying_units.Add(selectedUnit);
                                        }
                                        break;
                                    }
                                }
                            }
                            if (!isCanalMovement)
                            {
                                territories[selectedUnitFrom].occupying_units.Add(selectedUnit);
                            }
                        }
                    }
                    else
                    {
                        territories[selectedUnitFrom].occupying_units.Add(selectedUnit);
                    }
                    selectedUnit = new MilitaryUnit();
                    selectedUnitFrom = -1;
                    gameStateTracker = 19;
                }
                // Manoeuvre - select army to move
                else if (gameStateTracker == 13)
                {
                    for (int i = 0; i < territories[territoryInteractedWith].occupying_units.Count; ++i)
                    {
                        if (territories[territoryInteractedWith].occupying_units[i].nationID == currentNation)
                        {
                            if ((!territories[territoryInteractedWith].occupying_units[i].isBoat) &&
                                ((!territories[territoryInteractedWith].occupying_units[i].hasMoved) ||
                                (territories[territoryInteractedWith].nation == currentNation)))
                            {
                                selectedUnit = territories[territoryInteractedWith].occupying_units[i];
                                selectedUnitFrom = territoryInteractedWith;
                                intermediateUnitLocation = selectedUnitFrom;
                                territories[territoryInteractedWith].occupying_units.RemoveAt(i);
                                gameStateTracker = 11;
                                break;
                            }
                        }
                    }
                }
                // Manoeuvre - select place to move army to
                else if (gameStateTracker == 11)
                {
                    bool movementSuccessful = false;
                    if (territories[intermediateUnitLocation].adjacent.Contains(territoryInteractedWith))
                    {
                        if (territories[territoryInteractedWith].land) {
                            movementSuccessful = true;
                            
                            if ((territories[intermediateUnitLocation].nation != currentNation) ||
                                (territories[territoryInteractedWith].nation != currentNation) ||
                                (territories[territoryInteractedWith].RestrictedByHostiles()) ||
                                (territories[intermediateUnitLocation].RestrictedByHostiles()))
                            {
                                if (selectedUnit.hasMoved)
                                {
                                    movementSuccessful = false;
                                }
                                else
                                {
                                    selectedUnit.hasMoved = true;
                                }
                            }
                            if (movementSuccessful)
                            {
                                territories[territoryInteractedWith].occupying_units.Add(selectedUnit);
                                ferriedThisMovement = new List<int>();
                                selectedUnit = new MilitaryUnit();
                                FlagTerritoriesAfterMovement(selectedUnitFrom, territoryInteractedWith);
                                selectedUnitFrom = -1;
                                gameStateTracker = 13;
                            }
                        }
                        // Convoys
                        else
                        {
                            bool canMove = false;
                            int mover = -1;
                            for (int i = 0; i < territories[territoryInteractedWith].occupying_units.Count; ++i)
                            {
                                if (territories[territoryInteractedWith].occupying_units[i].nationID == currentNation)
                                {
                                    if (territories[territoryInteractedWith].occupying_units[i].hasFerried == false)
                                    {
                                        if (territories[territoryInteractedWith].occupying_units[i].isBoat)
                                        {
                                            canMove = true;
                                            mover = i;
                                            break;
                                        }
                                    }
                                }
                            }
                            if (canMove)
                            {
                                movementSuccessful = true;
                                territories[territoryInteractedWith].occupying_units[mover].hasFerried = true;
                                ferriedThisMovement.Add(territoryInteractedWith);
                                intermediateUnitLocation = territoryInteractedWith;
                            }
                        }
                    }
                    // If entering foreign nation's home territories, decide if unit is hostile or not
                    if (movementSuccessful)
                    {
                        if ((territories[territoryInteractedWith].nation != null) && (territories[territoryInteractedWith].nation != currentNation))
                        {
                            int factoriesOfHomeNation = 0;
                            for (int i = 0; i < nations[territories[territoryInteractedWith].nation ?? -1].homes.Count(); ++i)
                            {
                                if (territories[nations[territories[territoryInteractedWith].nation ?? -1].homes[i]].GetFactory() != -1)
                                {
                                    factoriesOfHomeNation++;
                                }
                            }
                            if (factoriesOfHomeNation > 1)
                            {
                                DialogResult isHostile = MessageBox.Show($"Set unit as hostile to " +
                                    $"{nations[territories[territoryInteractedWith].nation ?? -1].name}?",
                                    "Unit hostility", MessageBoxButtons.YesNo);
                                if (isHostile == DialogResult.Yes)
                                {
                                    territories[territoryInteractedWith].occupying_units.Last().isHostile = true;
                                }
                                else
                                {
                                    territories[territoryInteractedWith].occupying_units.Last().isHostile = false;
                                }
                            }
                            else
                            {
                                territories[territoryInteractedWith].occupying_units.Last().isHostile = false;
                            }
                        }
                        else
                        {
                            territories[territoryInteractedWith].occupying_units.Last().isHostile = false;
                        }
                    }
                    // Reset things if the movement was unsuccessful
                    else
                    {
                        territories[selectedUnitFrom].occupying_units.Add(selectedUnit);
                        for (int i = 0; i < ferriedThisMovement.Count; ++i)
                        {
                            for (int j = 0; j < territories[ferriedThisMovement[i]].occupying_units.Count; ++j)
                            {
                                if (territories[ferriedThisMovement[i]].occupying_units[j].hasFerried)
                                {
                                    territories[ferriedThisMovement[i]].occupying_units[j].hasFerried = false;
                                    break;
                                }
                            }
                        }
                        ferriedThisMovement = new List<int>();
                        gameStateTracker = 13;
                    }
                }
                // Production
                else if (gameStateTracker == 17)
                {
                    if (territories[territoryInteractedWith].nation == currentNation)
                    {
                        if (!producedThisTurn.Contains(territoryInteractedWith))
                        {
                            if (territories[territoryInteractedWith].GetFactory() != -1)
                            {
                                producedThisTurn.Add(territoryInteractedWith);
                                PlaceUnitInTerritory(territoryInteractedWith, new MilitaryUnit());
                            }
                            else
                            {
                                MessageBox.Show("Factory must be present to produce", "Territory cannot produce");
                            }
                        }
                        else
                        {
                            MessageBox.Show("This territory has already produced this turn", "Already produced");
                        }
                    }
                    else
                    {
                        MessageBox.Show("Can only produce in a home territory", "Territory cannot produce");
                    }
                }
            }
            // Ignore factory as that only requires one click and a confirm
        }

        // PlaceUnitInTerritory and RemoveUnitFromTerritory should only be used when there is a net gain or loss in unit quantity
        // This is because they also increment and decrement the total units counters for each nation

        // Places a unit in the specified territory. Type 0 is land, type 1 is sea. Automatic is true for Production.
        private void PlaceUnitInTerritory(int territoryID, MilitaryUnit unit, bool automatic = true)
        {
            if (automatic)
            {
                int nationID = territories[territoryID].nation ?? -1;
                if (nationID != -1)
                {
                    if (territories[territoryID].factory)
                    {
                        if (nations[nationID].landUnits != 0)
                        {
                            territories[territoryID].occupying_units.Add(new MilitaryUnit(false, nationID, false));
                            nations[nationID].landUnits--;
                        }
                        else
                        {
                            MessageBox.Show("Maximum number of land units already deployed!", "Troop limit reached");
                        }
                    }
                    else
                    {
                        if (nations[nationID].seaUnits != 0)
                        {
                            territories[territoryID].occupying_units.Add(new MilitaryUnit(true, nationID, false));
                            nations[nationID].seaUnits--;
                        }
                        else
                        {
                            MessageBox.Show("Maximum number of sea units already deployed!", "Troop limit reached");
                        }
                    }
                }
                else
                {
                    throw new ApplicationException("Attempted to automatically create unit in non-home territory");
                }
            }
            else
            {
                if (!unit.isBoat)
                {
                    if (nations[unit.nationID].landUnits == 0)
                    {
                        MessageBox.Show("Maximum number of land units already deployed!", "Troop limit reached");
                        return;
                    }
                    else
                    {
                        nations[unit.nationID].landUnits--;
                    }
                }
                else
                {
                    if (nations[unit.nationID].seaUnits == 0)
                    {
                        MessageBox.Show("Maximum number of sea units already deployed!", "Troop limit reached");
                        return;
                    }
                    else
                    {
                        nations[unit.nationID].landUnits--;
                    }
                }
                territories[territoryID].occupying_units.Add(unit);
            }
        }

        // Removes a unit from a territory. Returns false if no such unit was found in the territory specified.
        private bool RemoveUnitFromTerritory(int territoryID, MilitaryUnit unit)
        {
            if (territories[territoryID].occupying_units.Remove(unit))
            {
                if (unit.isBoat) {
                    nations[unit.nationID].seaUnits++;
                }
                else
                {
                    nations[unit.nationID].landUnits++;
                }
                return true;
            }
            return false;
        }

        // Removes a unit from a territory by index.
        private void RemoveUnitFromTerritory(int territoryID, int unitIndex)
        {
            if (territories[territoryID].occupying_units[unitIndex].isBoat)
            {
                nations[territories[territoryID].occupying_units[unitIndex].nationID].seaUnits++;
            }
            else
            {
                nations[territories[territoryID].occupying_units[unitIndex].nationID].landUnits++;
            }
            territories[territoryID].occupying_units.RemoveAt(unitIndex);
        }

        // Resolve combat in one territory
        public bool ResolveCombat(int territoryToResolve, int nationInvading, int recursionCounter = 0)
        {
            if (recursionCounter >= 1000)
            {
                MessageBox.Show("Too many separate conflicts resolved in a row\nHow and why have you done this?!?", "Stack overflow");
            }
            else
            {
                // Armies 0, fleets 1, factories 2
                int[,] ofEachType = new int[nations.Count, 3];
                bool otherNationsPresent = false;
                // Collect information about each unit in the territory, including factories
                foreach (MilitaryUnit unit in territories[territoryToResolve].occupying_units)
                {
                    if (unit.nationID != nationInvading)
                    {
                        otherNationsPresent = true;
                    }
                    if (unit.isBoat)
                    {
                        ofEachType[unit.nationID, 1]++;
                    }
                    else
                    {
                        ofEachType[unit.nationID, 0]++;
                    }
                }
                if ((territories[territoryToResolve].GetFactory() != -1) && (territories[territoryToResolve].nation != currentNation))
                {
                    ofEachType[territories[territoryToResolve].nation ?? -1, 2]++;
                    otherNationsPresent = true;
                }
                // If there are no units in the territory belonging to the nation currently taking its turn, then skip this combat resolution
                if ((ofEachType[nationInvading, 0] == 0) && (ofEachType[nationInvading, 1] == 0))
                {
                    return false;
                }
                // If there are no units to fight in the territory, then skip this combat
                if (!otherNationsPresent)
                {
                    return false;
                }

                // Figure out whether the factory, if present, is the last factory left for that nation
                int factoriesOfHomeNation = 0;
                for (int i = 0; i < nations[nationInvading].homes.Count(); ++i)
                {
                    if (territories[nations[nationInvading].homes[i]].GetFactory() != -1)
                    {
                        factoriesOfHomeNation++;
                    }
                }
                CombatForm combatForm = new CombatForm(territories[territoryToResolve], nations, nationInvading, factoriesOfHomeNation);
                // Using the close button will simply reopen the combat form
                while (combatForm.ShowDialog() == DialogResult.Cancel)
                {
                }
                List<int> combatResult = combatForm.GetResultsOfCombat();
                combatForm.Dispose();

                // Iterate backwards so indexing doesn't become skewed after removing elements
                for (int i = territories[territoryToResolve].occupying_units.Count - 1; i >= 0; i--)
                {
                    if (territories[territoryToResolve].occupying_units[i].nationID == nationInvading)
                    {
                        // Land unit of attacking nation
                        if ((!territories[territoryToResolve].occupying_units[i].isBoat) && (combatResult[1] != 0))
                        {
                            combatResult[1]--;
                            RemoveUnitFromTerritory(territoryToResolve, i);
                        }
                        else if ((territories[territoryToResolve].occupying_units[i].isBoat) && (combatResult[2] != 0))
                        {
                            combatResult[2]--;
                            RemoveUnitFromTerritory(territoryToResolve, i);
                        }
                    }
                    else if (territories[territoryToResolve].occupying_units[i].nationID == combatResult[0])
                    {
                        if ((!territories[territoryToResolve].occupying_units[i].isBoat) && (combatResult[3] != 0))
                        {
                            combatResult[3]--;
                            RemoveUnitFromTerritory(territoryToResolve, i);
                        }
                        else if ((territories[territoryToResolve].occupying_units[i].isBoat) && (combatResult[4] != 0))
                        {
                            combatResult[4]--;
                            RemoveUnitFromTerritory(territoryToResolve, i);
                        }
                    }
                }
                if (combatResult[5] == 1)
                {
                    territories[territoryToResolve].hasFactory = false;
                }
                if (combatResult[6] == 0)
                {
                    ResolveCombat(territoryToResolve, nationInvading, recursionCounter + 1);
                }
                if (recursionCounter == 0)
                {
                    FlagTerritory(territoryToResolve);
                }
            }
            return true;
        }

        // After a movement, check if the involved territories should be flagged
        private void FlagTerritoriesAfterMovement(int source, int dest)
        {
            FlagTerritory(source);
            FlagTerritory(dest);
        }

        // Check flag status in a single territory
        private void FlagTerritory(int territoryID)
        {
            if (territories[territoryID].nation == null)
            {
                if (territories[territoryID].occupying_units.Count != 0)
                {
                    int nationFlagging = territories[territoryID].occupying_units[0].nationID;
                    for (int i = 0; i < territories[territoryID].occupying_units.Count; ++i)
                    {
                        if (territories[territoryID].occupying_units[i].nationID != nationFlagging)
                        {
                            return;
                        }
                    }
                    if (nations[nationFlagging].flaggedTerritories.Count < 15)
                    {
                        if (territories[territoryID].currentNation != null) {
                            nations[territories[territoryID].currentNation ?? -1].flaggedTerritories.Remove(territoryID);
                        }
                        nations[nationFlagging].flaggedTerritories.Add(territoryID);
                        territories[territoryID].currentNation = nationFlagging;
                    }
                }
            }
        }

        // Conduct the first stage of the Investor action automatically
        private bool AutomatedInvestor()
        {
            gameStateTracker = 6;
            int[] projectedPlayerPayout = new int[players.Count];
            // The maximum amount for a successful Investor
            int totalPayable = nations[currentNation].treasury + players[nations[currentNation].controllingPlayer].money;
            // Sum the total amount payable by subtracting from the above
            foreach (NationBond bond in bonds[currentNation])
            {
                if (bond.playerID != -1)
                {
                    projectedPlayerPayout[bond.playerID] += bond.interest;
                    if (bond.playerID != nations[currentNation].controllingPlayer)
                    {
                        totalPayable -= bond.interest;
                    }
                }
            }
            if (totalPayable < 0)
            {
                return false;
            }
            for (int i = 0; i < players.Count; ++i)
            {
                if (nations[currentNation].treasury != 0)
                {
                    int amountToPay = Math.Min(nations[currentNation].treasury, projectedPlayerPayout[i]);
                    players[i].money += amountToPay;
                    nations[currentNation].treasury -= amountToPay;
                }
                else
                {
                    players[i].money += projectedPlayerPayout[i];
                    players[nations[currentNation].controllingPlayer].money -= projectedPlayerPayout[i];
                }
            }
            gameStateTracker = 18;
            return true;
        }

        // Produce a unit in the specified territory
        public void ProductionProduceSelected(int territoryID)
        {
            if (territories[territoryID].GetFactory() != -1)
            {
                PlaceUnitInTerritory(territoryID, new MilitaryUnit(false, -1, false));
            }
        }

        // Produce a unit in all territories for a nation
        public void ProductionProduceAll()
        {
            foreach (int territoryID in nations[currentNation].homes)
            {
                ProductionProduceSelected(territoryID);
            }
            gameStateTracker = 18;
        }

        // Add a factory to the specified territory
        private int BuildFactory(int territoryClicked)
        {
            if (territoryClicked != -1)
            {
                Territory currentTerritory = territories[territoryClicked];
                if (currentTerritory.nation == currentNation)
                {
                    if (!currentTerritory.RestrictedByHostiles())
                    {
                        if (nations[currentNation].treasury >= 5)
                        {
                            if (currentTerritory.GetFactory() == -1)
                            {
                                territories[territoryClicked].hasFactory = true;
                                nations[currentNation].treasury -= 5;
                                return 0;
                            }
                            return 6;
                        }
                        return 5;
                    }
                    return 7;
                }
            }
            return 8;
        }

        // Get the projected initial money gain and power gain from Taxation
        public List<int> GetTaxationPowerIncrease(int nationToProject)
        {
            List<int> returnList = new List<int>();
            // Taxation money
            int taxationCollected = 0;
            taxationCollected += nations[nationToProject].flaggedTerritories.Count;
            foreach (int homeTerritoryIndex in nations[nationToProject].homes)
            {
                if (territories[homeTerritoryIndex].GetFactory() != -1)
                {
                    taxationCollected += 2;
                }
            }
            returnList.Add(taxationCollected);

            // Power
            returnList.Add(0);
            for (int i = taxThresholds.Length - 1; i >= 0; --i)
            {
                if (taxationCollected >= taxThresholds[i])
                {
                    returnList[1]++;
                }
            }
            return returnList;
        }

        // Automatically calculate the Taxation action
        private void AutomatedTaxation()
        {
            gameStateTracker = 15;
            List<int> taxationValues = GetTaxationPowerIncrease(currentNation);
            nations[currentNation].treasury += taxationValues[0];

            nations[currentNation].treasury -= Math.Min(nations[currentNation].treasury,
                nations[currentNation].maxLandUnits + nations[currentNation].maxSeaUnits - nations[currentNation].landUnits - nations[currentNation].seaUnits);

            nations[currentNation].powerPoints += taxationValues[1];
            if (taxTypeIsWorld)
            {
                int bonusAmount = Math.Min(nations[currentNation].treasury, taxationValues[1] / 2);
                nations[currentNation].treasury -= bonusAmount;
                players[nations[currentNation].controllingPlayer].money += bonusAmount;
            }

            if (nations[currentNation].powerPoints >= 25)
            {
                gameEnded = true;
            }
            gameStateTracker = 18;
        }

        // No tiebreak score implemented
        public int CalculatePlayerScore(int playerID)
        {
            int scoreInt = players[playerID].money;
            for (int i = 0; i < nations.Count; ++i)
            {
                int totalInterest = 0;
                foreach (NationBond bond in bonds[i])
                {
                    if (bond.playerID == playerID)
                    {
                        totalInterest += bond.interest;
                    }
                }
                scoreInt += totalInterest * ((nations[i].powerPoints - 1) / 5 + 1);
            }
            return scoreInt;
        }

        public void Debug1Function()
        {
            string selectedUnitInfo = $"nation: {(selectedUnit.nationID != -1 ? nations[selectedUnit.nationID].name : "DEFAULT")}\n" +
                $"type: {(selectedUnit.isBoat ? "fleet" : "army")}\n" +
                $"has moved: {selectedUnit.hasMoved}\n" +
                $"starting territory: {(selectedUnitFrom != -1 ? territories[selectedUnitFrom].name : "none")}\n" +
                $"previous territory: {(intermediateUnitLocation != -1 ? territories[intermediateUnitLocation].name : "none")}\n" +
                $"gamestate: {gameStateTracker}\n";
            MessageBox.Show(selectedUnitInfo);
        }
    }
}
