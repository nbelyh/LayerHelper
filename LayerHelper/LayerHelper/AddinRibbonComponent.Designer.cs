namespace LayerHelper
{
    partial class AddinRibbonComponent : Microsoft.Office.Tools.Ribbon.RibbonBase
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        public AddinRibbonComponent()
            : base(Globals.Factory.GetRibbonFactory())
        {
            InitializeComponent();
        }

        /// <summary> 
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Component Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.Tab1 = this.Factory.CreateRibbonTab();
            this.Group1 = this.Factory.CreateRibbonGroup();
            this.TogglePanel = this.Factory.CreateRibbonButton();
            this.Tab1.SuspendLayout();
            this.Group1.SuspendLayout();
            this.SuspendLayout();
            // 
            // Tab1
            // 
            this.Tab1.ControlId.ControlIdType = Microsoft.Office.Tools.Ribbon.RibbonControlIdType.Office;
            this.Tab1.ControlId.OfficeId = "TabHome";
            this.Tab1.Groups.Add(this.Group1);
            this.Tab1.Label = "TabHome";
            this.Tab1.Name = "Tab1";
            // 
            // Group1
            // 
            this.Group1.Items.Add(this.TogglePanel);
            this.Group1.Label = "Layers";
            this.Group1.Name = "Group1";
            // 
            // TogglePanel
            // 
            this.TogglePanel.ControlSize = Microsoft.Office.Core.RibbonControlSize.RibbonControlSizeLarge;
            this.TogglePanel.Image = global::LayerHelper.Properties.Resources.TogglePanel;
            this.TogglePanel.Label = "Layer Panel";
            this.TogglePanel.Name = "TogglePanel";
            this.TogglePanel.ShowImage = true;
            this.TogglePanel.Click += new Microsoft.Office.Tools.Ribbon.RibbonControlEventHandler(this.buttonToggle_Click);
            // 
            // AddinRibbonComponent
            // 
            this.Name = "AddinRibbonComponent";
            this.RibbonType = "Microsoft.Visio.Drawing";
            this.Tabs.Add(this.Tab1);
            this.Tab1.ResumeLayout(false);
            this.Tab1.PerformLayout();
            this.Group1.ResumeLayout(false);
            this.Group1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        internal Microsoft.Office.Tools.Ribbon.RibbonTab Tab1;
        internal Microsoft.Office.Tools.Ribbon.RibbonGroup Group1;
        internal Microsoft.Office.Tools.Ribbon.RibbonButton TogglePanel;
    }

    partial class ThisRibbonCollection
    {
        internal AddinRibbonComponent Ribbon
        {
            get { return this.GetRibbon<AddinRibbonComponent>(); }
        }
    }
}
