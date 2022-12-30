using System;
using System.Diagnostics;
using System.IO;
using Microsoft.Office.Interop.Visio;
using Newtonsoft.Json;
using SciterSharp;
using SciterSharp.Interop;
using Path = System.IO.Path;

namespace LayerHelper
{
    /// <summary>
    /// Integrates a winform in Visio.
    /// Creates an anchor window for the given diagram window, and installs the specified form as a child in that panel.
    /// </summary>
    /// 
    public sealed class PanelFrame : SciterHost, IVisEventProc
    {
        private const string AddonWindowMergeId = "83f14b0c-be12-4c5a-a591-bc13237eccb0";
        
        #region fields

        private Window _visioWindow;
        private LayersWindow _layersWindow;

        #endregion

        protected override SciterXDef.LoadResult OnLoadData(SciterXDef.SCN_LOAD_DATA sld)
        {
            if (sld.uri.StartsWith("file:"))
            {
                var dir = AppDomain.CurrentDomain.BaseDirectory;
#if DEBUG
                dir = Path.GetFullPath(dir + @"..\..\");
#endif
                // load resource from SciterArchive
                var path = Path.Combine(dir, "html", sld.uri.Replace("file:", ""));
                var data = File.ReadAllBytes(path);
                SciterX.API.SciterDataReady(_layersWindow.Handle, sld.uri, data, (uint)data.Length);

                return SciterXDef.LoadResult.LOAD_OK;
            }

            return base.OnLoadData(sld);
        }

        /// <summary>
        /// The event is triggered when user closes the panel using "x" button
        /// </summary>
        /// <param name="window">The parent diagram window for which the panel was closed.</param>
        /// 
        public delegate void PanelFrameClosedEventHandler(Window window);
        public event PanelFrameClosedEventHandler PanelFrameClosed;

        /// <summary>
        /// Constructs a new panel frame.
        /// </summary>
        public PanelFrame(LayersWindow layersWindow)
        {
            _layersWindow = layersWindow;
        }

#region methods

        /// <summary>
        /// Destroys the panel frame along with the form.
        /// </summary>
        public void DestroyWindow()
        {
            try
            {
                if (_visioWindow != null)
                {
                    _visioWindow.Close();
                    _visioWindow = null;
                }

                if (_layersWindow != null)
                {
                    _layersWindow.Close();
                    _layersWindow = null;
                }
            }
            // ReSharper disable once EmptyGeneralCatchClause : ignore all errors on exit
            catch
            {
            }
        }

        /// <summary>
        /// Install the panel into given window (actually creates the form and shows it)
        /// </summary>
        /// <param name="visioParentWindow">The parent Visio window where the panel should be installed to.</param>
        /// <returns></returns>
        public Window CreateWindow(Window visioParentWindow, string text)
        {
            Window retVal = null;

            try
            {
                if (visioParentWindow == null)
                    return null;

                if (_layersWindow != null)
                {
                    _visioWindow = visioParentWindow.Windows.Add(
                        text,
                        (int)VisWindowStates.visWSDockedRight | (int)VisWindowStates.visWSAnchorMerged,
                        VisWinTypes.visAnchorBarAddon,
                        0,
                        0,
                        400,
                        400,
                        AddonWindowMergeId,
                        string.Empty,
                        0);

                    _visioWindow.BeforeWindowClosed += OnBeforeWindowClosed;

                    _visioWindow.Visible = false;

                    var parentWindowHandle = (IntPtr)_visioWindow.WindowHandle32;

                    _layersWindow.CreateChildWindow(parentWindowHandle);
                    SetupWindow(_layersWindow);
                    AttachEvh(new EvntHandler(_layersWindow));
                    _layersWindow.Show();

                    _layersWindow.LoadStartPage();

                    _visioWindow.Visible = true;
                    _visioWindow.Activate();
                    
                    retVal = _visioWindow;
                }
            }
            catch (Exception ex)
            {
                Debug.Write(ex.Message);
            }

            return retVal;
        }

#endregion

        object IVisEventProc.VisEventProc(short nEventCode, object pSourceObj, int nEventId, int nEventSeqNum, object pSubjectObj, object vMoreInfo)
        {
            object returnValue = false;

            try
            {
                var subjectWindow = pSubjectObj as Window;
                switch (nEventCode)
                {
                    case ((short)VisEventCodes.visEvtDel + (short)VisEventCodes.visEvtWindow):
                        {
                            OnBeforeWindowClosed(subjectWindow);
                            break;
                        }
                }
            }
            catch (Exception ex)
            {
                Debug.Write(ex.Message);
            }

            return returnValue;
        }

        private void OnBeforeWindowClosed(Window visioWindow)
        {
            if (PanelFrameClosed != null)
                PanelFrameClosed(_visioWindow.ParentWindow);

            DestroyWindow();
        }
    }
}