using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Drawing;
using System.Windows.Forms;

namespace NEA
{
	public class Territory
	{
		public string name { get; set; }
		public string trigram { get; set; }
		public int id { get; set; }
		public string file { get;  set; }
		public int xpos { get;  set; }
		public int ypos { get;  set; }
		public int lblx { get; set; }
		public int lbly { get; set; }
		public bool land { get;  set; }
		public Nullable<int> nation { get;  set; }
		public bool factory { get;  set; }	// True if factory is land, false if it is naval
		public bool start { get;  set; }
		public int[] adjacent { get; set; }
		public int[][] canal_connect { get;  set; }
		public Nullable<int> currentNation;
		public List<MilitaryUnit> occupying_units = new List<MilitaryUnit>();

		public Bitmap bitmap;

		public bool hasFactory = false;

		// Returns the factory situation of the territory. -1 if no factory, 0 if land factory and 1 if naval factory.
		public int GetFactory()
        {
			if (nation != null)
            {
				if (hasFactory)
                {
					if (factory)
                    {
						return 0;
                    }
					else
                    {
						return 1;
                    }
                }
            }
			return -1;
        }

		// Determines whether the territory is occupied by hostile foreign forces, if it is a home nation
		public bool RestrictedByHostiles()
        {
			if (nation != null)
            {
				foreach (MilitaryUnit unit in occupying_units)
                {
					if (unit.isHostile)
                    {
						return true;
                    }
                }
            }
			return false;
        }
	}
}
