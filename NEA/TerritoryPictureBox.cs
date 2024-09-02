using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace NEA
{
    /* Extend the PictureBox and Label classes to implement transparency functionality */
    public class TransparentLabel : Label
    {
        public int territoryID;

        public TransparentLabel(int in_territoryID)
        {
            SetStyle(ControlStyles.OptimizedDoubleBuffer, false);   // Black boxes or graphical errors appear if OptimizedDoubleBuffer is used
            this.AutoSize = true;
            territoryID = in_territoryID;
        }
    }

    public class TerritoryPictureBox : PictureBox
    {
        public int territoryID;

        public TransparentLabel lblTerritoryName;

        protected override CreateParams CreateParams
        {
            get
            {
                CreateParams cp = base.CreateParams;
                cp.ExStyle |= 0x00000020;   // WS_EX_TRANSPARENT (paints marked objects after sibling objects)
                return cp;
            }
        }

        // Do not paint the backgrounds
        protected override void OnPaintBackground(PaintEventArgs e)
        {
            return;
        }

        public TerritoryPictureBox(int idOfTerritory)
        {
            territoryID = idOfTerritory;
            lblTerritoryName = new TransparentLabel(territoryID)
            {
                AutoSize = true
            };
            SetStyle(ControlStyles.SupportsTransparentBackColor, true);
            SetStyle(ControlStyles.OptimizedDoubleBuffer, false);   // As with the label - black boxes appear if this is set to true
            SizeMode = PictureBoxSizeMode.Zoom;
        }
    }
}
