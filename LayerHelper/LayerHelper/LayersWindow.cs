using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Newtonsoft.Json;
using Visio = Microsoft.Office.Interop.Visio;
using SciterSharp;

namespace LayerHelper
{
    public class Item
    {
        public string name { get; set; }
        public short index { get; set; }
        public bool visible { get; set; }
        public bool selected { get; set; }
        public int count { get; set; }
    }

    public class EvntHandler : SciterEventHandler
    {
        private LayersWindow _layersWindow;

        public EvntHandler(LayersWindow layersWindow)
        {
            _layersWindow = layersWindow;
        }
        
        protected override bool OnScriptCall(SciterElement se, string name, SciterValue[] args, out SciterValue result)
        {
            switch (name)
            {
                case "SetLayerVisible":
                    OnVisibleClicked(GetItem(args));
                    break;

                case "SetLayerSelected":
                    OnSelectedClicked(GetItem(args));
                    break;

                case "RemoveLayer":
                    RemoveLayer(GetItem(args));
                    break;

                case "AddLayer":
                    AddLayer(GetString(args));
                    break;
            }

            return base.OnScriptCall(se, name, args, out result);
        }

        private static string GetString(SciterValue[] args)
        {
            return args[0].Get("New Layer");
        }

        private static Item GetItem(SciterValue[] args)
        {
            return JsonConvert.DeserializeObject<Item>(args[0].ToJSONString());
        }

        private void AddLayer(string name)
        {
            var page = _layersWindow.VisioWindow.PageAsObj;
            var layer = page.Layers.Add(name);

            foreach (Visio.Shape shape in _layersWindow.VisioWindow.Selection)
                layer.Add(shape, 0);

            _layersWindow.UpdatePanel();
        }

        private void RemoveLayer(Item item)
        {
            var page = _layersWindow.VisioWindow.PageAsObj;

            var layer = page.Layers[item.name];

            layer.Delete(0);

            _layersWindow.UpdatePanel();
        }

        private void OnSelectedClicked(Item item)
        {
            var page = _layersWindow.VisioWindow.PageAsObj;

            var layer = page.Layers[item.name];

            foreach (Visio.Shape shape in _layersWindow.VisioWindow.Selection)
            {
                if (item.selected)
                    layer.Add(shape, 0);
                else
                    layer.Remove(shape, 0);
            }
        }

        private void OnVisibleClicked(Item item)
        {
            var page = _layersWindow.VisioWindow.PageAsObj;
            var layer = page.Layers[item.name];
            layer.CellsC[(short) Visio.tagVisCellIndices.visLayerVisible].ResultFromInt[0] = item.visible ? -1 : 0;
        }
    }

    public class LayersWindow : SciterWindow
    {
        public readonly Visio.Window VisioWindow;

        public LayersWindow(Visio.Window visioWindow)
        {
            VisioWindow = visioWindow;
            VisioWindow.SelectionChanged += VisioWindowOnSelectionChanged;
        }

        private void VisioWindowOnSelectionChanged(Visio.Window window)
        {
            UpdatePanel();
        }

        public static IEnumerable<short> EmptyList = new List<short>();

        public void UpdatePanel()
        {
            var page = VisioWindow.PageAsObj;

            var layerMap = page.Layers.Cast<Visio.Layer>()
                .ToDictionary(layer => layer.Index, layer => new Item
                {
                    name = layer.Name,
                    index = layer.Index,
                    visible = layer.CellsC[(short)Visio.tagVisCellIndices.visLayerVisible].ResultInt[0, 0] != 0,
                    count = page.CreateSelection(Visio.VisSelectionTypes.visSelTypeByLayer, Visio.VisSelectMode.visSelModeSkipSuper, layer).Count
                });

            foreach (Visio.Shape shape in VisioWindow.Selection)
            {
                for (short n = 1; n <= shape.LayerCount; ++n)
                {
                    layerMap[shape.Layer[n].Index].selected = true;
                }
            }
            
            var itemsJson = JsonConvert.SerializeObject(layerMap.Values);

            CallFunction("Data.setItems", SciterValue.FromJSONString(itemsJson));
        }

        public void LoadStartPage()
        {
            var templatePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "html", "index.html");
            LoadPage(templatePath);
            UpdatePanel();
        }
    }
}