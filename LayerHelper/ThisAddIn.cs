using System;
using System.Windows.Forms;

namespace LayerHelper
{
    public partial class ThisAddIn
    {
        public void TogglePanel()
        {
            _panelManager.TogglePanel(Application.ActiveWindow);
        }

        private PanelManager _panelManager;
        private ShortcutManager _shortcutManager;

        private void ThisAddIn_Startup(object sender, EventArgs e)
        {
            _panelManager = new PanelManager(this);
            _shortcutManager = new ShortcutManager();

            Application.OnKeystrokeMessageForAddon += _shortcutManager.OnKeystrokeMessageForAddon;
        }

        private void ThisAddIn_Shutdown(object sender, EventArgs e)
        {
            _panelManager.Dispose();
            Application.OnKeystrokeMessageForAddon -= _shortcutManager.OnKeystrokeMessageForAddon;
        }

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InternalStartup()
        {
            SciterSharp.Interop.SciterX.Use3264DLLNaming = true;
            Startup += ThisAddIn_Startup;
            Shutdown += ThisAddIn_Shutdown;
        }

    }
}
