using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;

namespace NEA
{
    public class NationBond
    {
        public int cost { get; private set; }
        public int interest { get; private set; }
        public int nationID { get; private set; }
        public string nationName = "";
        public int playerID = -1;

        public NationBond(int cost_in, int interest_in, int nation_id, string nation_name)
        {
            cost = cost_in;
            interest = interest_in;
            nationID = nation_id;
            nationName = nation_name;
        }

        public NationBond()
        {
            cost = -1;
            interest = -1;
            nationID = -1;
        }

        public override string ToString()
        {
            if (cost == -1)
            {
                return "none";
            }
            return $"{cost.ToString()}|{interest.ToString()} ({nationName})";
        }
    }

    public class MilitaryUnit
    {
        public int nationID;
        public bool isBoat;
        public bool isHostile;
        public bool hasMoved;
        public bool hasFerried;

        public MilitaryUnit()
        {
            nationID = -1;
            isBoat = false;
            isHostile = false;
            hasMoved = false;
            hasFerried = false;
        }

        public MilitaryUnit(bool isaBoat, int nation_id, bool in_isHostile)
        {
            nationID = nation_id;
            isBoat = isaBoat;
            isHostile = in_isHostile;
            hasMoved = false;
            hasFerried = false;
        }
    }

    public class Nation
    {
        public int controllingPlayer = -1;
        public int treasury = 0;
        public int powerPoints = 0;
        public int secondaryBonds;
        public int maxLandUnits;
        public int maxSeaUnits;
        public int lastRondelSlot = -1;

        public List<int> flaggedTerritories = new List<int>();
        public string name { get; set; }
        public int id { get; set; }
        public int[] homes { get; set; }
        public int landUnits { get; set; }
        public int seaUnits { get; set; }
        public string primaryhex { private get; set; }
        public string secondaryhex { private get; set; }
        public string monocontrasthex { private get; set; }

        public Color primaryColour;
        public Color secondaryColour;
        public Color monoContrastColour;

        public void SetColoursWhenGiven()
        {
            primaryColour = Color.FromArgb(255, Color.FromArgb(Convert.ToInt32(primaryhex, 16)));
            secondaryColour = Color.FromArgb(255, Color.FromArgb(Convert.ToInt32(secondaryhex, 16)));
            monoContrastColour = Color.FromArgb(255, Color.FromArgb(Convert.ToInt32(monocontrasthex, 16)));
        }
    }
}
