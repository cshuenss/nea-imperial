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
        private bool debugMode;
        public bool invalidJsonSelfDestruct = false;

        public string jsonFileLocation;
        public int playerCount;
        public int startingMoney;
        public bool randomBondsToStart;
        private bool nineBonds;
        public bool investorCard;
        private bool taxTypeIsWorld;

        public int[] taxThresholds;

        public List<Nation> nations = new List<Nation>();
        public List<Territory> territories = new List<Territory>();
        public List<Player> players = new List<Player>();
        public List<List<NationBond>> bonds = new List<List<NationBond>>();

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
        12: Deciding combat after movement ()
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
        20: [UNUSED]
        */

        /*  REQUIRED INPUT
         0: Territory (No confirmation needed)
         1: Bond (Confirmation needed)
         2: Rondel (Confirmation needed)
        */

        /*  CURRENT NATION tracked by nationID, -1 means any nation  */

        public Nullable<(int, int)> nonPlayerBondInteractedWith = null;
        public Nullable<(int, int)> playerBondInteractedWith = null;
        public Nullable<int> territoryInteractedWith = null;
        public Nullable<int> rondelInteractedWith = null;

        public int gameStateTracker = -1;
        public int requiredInput = 1;
        public bool gameStarted = false;   // Whether turns have started (so is false when selecting bonds)
        public int gameResult = -1;
        public bool gameEnded = false;
        public int currentPlayerAction = 0;
        private int currentPlayerTurn = 0;
        public int currentNation = 0;

        public int turnCounter = 0;

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
            string in_json, 
            int in_playerCount,
            bool in_randomBonds,
            bool in_nineBonds,
            bool in_investorCard,
            bool in_taxTypeIsWorld)
        {
            this.debugMode = debugMode;

            jsonFileLocation = in_json;
            playerCount = in_playerCount;
            randomBondsToStart = in_randomBonds;
            nineBonds = in_nineBonds;
            investorCard = in_investorCard;
            taxTypeIsWorld = in_taxTypeIsWorld;

            for (int i = 0; i < playerCount; ++i)
            {
                players.Add(new Player());
                players[i].name = i.ToString();
            }

            try
            {
                ParseJSON(jsonFileLocation);
            }
            catch (JsonException e)
            {
                invalidJsonSelfDestruct = true;
            }
            catch (ArgumentNullException e)
            {
                invalidJsonSelfDestruct = true;
            }

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
            }
        }

        // Deserialise the map data JSON.
        private void ParseJSON(string file)
        {
            string mapData = System.IO.File.ReadAllText(file);
            using (JsonDocument mapJson = JsonDocument.Parse(mapData))
            {
                // Deal out starting money
                if (investorCard)
                {
                    JsonElement investorValues = mapJson.RootElement.GetProperty("withinvestor");
                    startingMoney = investorValues[playerCount - 2].GetInt32();
                }
                else
                {
                    JsonElement investorlessValues = mapJson.RootElement.GetProperty("withoutinvestor");
                    startingMoney = investorlessValues[playerCount - 2].GetInt32();
                }
                foreach (Player player in players)
                {
                    player.money = startingMoney;
                }

                // Map data known to be split into nations and territories, handle separately.
                JsonElement secondaryBonds = mapJson.RootElement.GetProperty("secondarybonds");
                JsonElement taxThresholdsJson = mapJson.RootElement.GetProperty("taxthresholds");
                JsonElement nationsJson = mapJson.RootElement.GetProperty("nations");
                JsonElement territoriesJson = mapJson.RootElement.GetProperty("territories");
                foreach (JsonElement nationJson in nationsJson.EnumerateArray())
                {
                    nations.Add(JsonSerializer.Deserialize<Nation>(nationJson.ToString()));
                    nations.Last().SetColoursWhenGiven();
                }
                if (nations.Count != mapJson.RootElement.GetProperty("count").GetInt32())
                {
                    throw new ApplicationException("Invalid map data: Nation count misaligned");
                }
                foreach (JsonElement territoryJson in territoriesJson.EnumerateArray())
                {
                    Territory currentTerritory = JsonSerializer.Deserialize<Territory>(territoryJson.ToString());
                    currentTerritory.creator = this;
                    currentTerritory = AddTerritoryBitmap(currentTerritory, jsonFileLocation);
                    if (currentTerritory.start == true)
                    {
                        currentTerritory.hasFactory = true;
                    }
                    territories.Add(currentTerritory);
                }

                // Process remaining header data
                int headerCounter = 0;
                foreach (JsonElement secondaryBondData in secondaryBonds.EnumerateArray())
                {
                    nations[headerCounter].secondaryBonds = secondaryBondData.GetInt32();
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

        // Add a bitmap to the territory in preparation for the PictureBox of the territory
        private Territory AddTerritoryBitmap(Territory territory, string jsonFileName)
        {
            string fileName = System.IO.Path.Combine(jsonFileName.Remove(jsonFileName.Length - 5), territory.file);
            territory.bitmap = new Bitmap(fileName);
            return territory;
        }

        private void DistributeRandomBonds()
        {
            throw new NotImplementedException();
        }

        public void ChangePlayerToAct(int playerToAct)
        {
            currentPlayerAction = playerToAct;
            nonPlayerBondInteractedWith = null;
            playerBondInteractedWith = null;
            territoryInteractedWith = null;
            rondelInteractedWith = null;
        }

        public int RondelSlotSelected()
        {
            gameStateTracker = rondelInteractedWith ?? -1;
            if (gameStateTracker == -1)
            {
                return 2;
            }
            if (gameStateTracker == 6) gameStateTracker = 2;
            if (gameStateTracker == 7) gameStateTracker = 3;

            switch (gameStateTracker)
            {
                case 0: // Investor
                    requiredInput = -1;
                    if (!AutomatedInvestor())
                    {
                        return 5;
                    }
                    return 0;
                case 1: // Import
                    requiredInput = 0;
                    return 0;
                case 2: // Production
                    requiredInput = 0;
                    return 0;
                case 3: // Manoeuvre
                    requiredInput = 0;
                    return 0;
                case 4: // Taxation
                    requiredInput = -1;
                    AutomatedTaxation();
                    return 0;
                case 5: // Factory
                    requiredInput = 0;
                    return 0;
                default:
                    return 3;
            }
        }

        public void BondClicked((int, int) bondIndex)
        {
            if (requiredInput == 1)
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
        }

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

        public int GetPlayerInvestedValues(int nationID, int playerID)
        {
            return GetPlayerInvestedValues(nationID)[playerID];
        }

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

        private void UpdateNationOwnership()
        {
            for (int i = 0; i < nations.Count; ++i)
            {
                UpdateNationOwnership(i);
            }
        } 

        private void StartNextTurn(int preferredNation)
        {
            UpdateNationOwnership();
            gameStarted = true;
            gameStateTracker = -1;
            requiredInput = 2;
            for (int i = 0; i < nations.Count; ++i)
            {
                if (nations[i + preferredNation].controllingPlayer != -1)
                {
                    currentNation = i + preferredNation;
                }
            }
        }

        public int EnactChanges(int argument = -1)
        {
            if ((requiredInput == 1) && (gameStarted == false)) {
                return PlayerPurchasesBonds();
            }
            else if ((requiredInput == 2) && (gameStateTracker == -1)) {
                return RondelSlotSelected();
            }
            else if ((requiredInput == 0) && (gameStateTracker == 16))
            {
                return BuildFactory(argument);
            }
            else if (gameStateTracker == 18)
            {
                return 0;
            }
            return 1;
        }

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

        private int PlayerPurchasesBonds()
        {
            (int, int) nonPlayerBondIndex = nonPlayerBondInteractedWith ?? (-1, -1);
            (int, int) playerBondIndex = playerBondInteractedWith ?? (-1, -1);
            NationBond nonPlayerBond = GetNationBond(nonPlayerBondIndex);
            NationBond playerBond = GetNationBond(playerBondIndex);
            if ((nonPlayerBondInteractedWith != null) && (nonPlayerBond.nationID == currentNation))
            {
                if ((playerBondInteractedWith != null) && (playerBond.nationID == currentNation))
                {
                    if (playerBond.cost < nonPlayerBond.cost)
                    {
                        if (players[currentPlayerAction].money - nonPlayerBond.cost + playerBond.cost >= 0)
                        {
                            players[currentPlayerAction].money -= nonPlayerBond.cost - playerBond.cost;
                            nations[currentNation].treasury += nonPlayerBond.cost - playerBond.cost;
                            nonPlayerBond.playerID = currentPlayerAction;
                            playerBond.playerID = -1;
                            return 0;
                        }
                    }
                }
                else if (players[currentPlayerAction].money - nonPlayerBond.cost >= 0)
                {
                    players[currentPlayerAction].money -= nonPlayerBond.cost;
                    nations[currentNation].treasury += nonPlayerBond.cost;
                    nonPlayerBond.playerID = currentPlayerAction;
                    return 0;
                }
            }
            return 4;
        }

        public void PostTurnInvestorlessBondPurchasing()
        {
            gameStateTracker = 14;
            requiredInput = 1;
        }

        private bool AutomatedInvestor()
        {
            gameStateTracker = 6;
            int[] projectedPlayerPayout = new int[players.Count];
            int totalPayable = nations[currentNation].treasury + players[nations[currentNation].controllingPlayer].money;
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
            return true;
        }

        private int BuildFactory(int territoryClicked)
        {
            Territory currentTerritory = territories[territoryClicked];
            if (currentTerritory.nation == currentNation)
            {
                if (currentTerritory.currentNation == null)
                {
                    if (nations[currentNation].treasury >= 5)
                    {
                        if (currentTerritory.GetFactory() == -1)
                        {
                            territories[territoryClicked].hasFactory = true;
                            nations[currentNation].treasury -= 5;
                            return 0;
                        }
                    }
                }
            }
            return 5;
        }

        private void AutomatedTaxation()
        {
            gameStateTracker = 15;
            int taxationCollected = 0;
            taxationCollected += nations[currentNation].flaggedTerritories.Count();
            foreach (int homeTerritoryIndex in nations[currentNation].homes)
            {
                if (territories[homeTerritoryIndex].GetFactory() != -1)
                {
                    taxationCollected += 2;
                }
            }
            nations[currentNation].treasury += taxationCollected;

            nations[currentNation].treasury -= Math.Min(nations[currentNation].treasury, nations[currentNation].landUnits + nations[currentNation].seaUnits);

            for (int i = taxThresholds.Length - 1; i >= 0; --i)
            {
                if (taxationCollected >= taxThresholds[i])
                {
                    nations[currentNation].powerPoints += (i + 1);
                    if (taxTypeIsWorld)
                    {
                        int bonusAmount = Math.Min(nations[currentNation].treasury, i / 2);
                        nations[currentNation].treasury -= bonusAmount;
                        players[nations[currentNation].controllingPlayer].money += bonusAmount;
                    }
                }
            }

            if (nations[currentNation].powerPoints >= 25)
            {
                gameEnded = true;
            }

            gameStateTracker = 18;
        }
    }
}
