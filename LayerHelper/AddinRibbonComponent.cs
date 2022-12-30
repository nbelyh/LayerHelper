using System;
using System.Collections.Generic;
using Microsoft.Office.Tools.Ribbon;

namespace LayerHelper
{
    public partial class AddinRibbonComponent
    {
        private void buttonToggle_Click(object sender, RibbonControlEventArgs e)
        {
            Globals.ThisAddIn.TogglePanel();
        }
    }
}
