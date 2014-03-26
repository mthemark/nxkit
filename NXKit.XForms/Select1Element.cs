﻿using System.Linq;
using System.Xml.Linq;

namespace NXKit.XForms
{

    [Element("select1")]
    public class Select1Element :
        SingleNodeUIBindingElement,
        ISupportsUiCommonAttributes
    {

        bool selectedItemNodeCached;
        ItemElement selectedItemNode;

        /// <summary>
        /// Initializes a new instance.
        /// </summary>
        /// <param name="xml"></param>
        public Select1Element(XElement xml)
            : base(xml)
        {

        }

        [Interactive]
        public bool Incremental
        {
            get { return Module.GetAttributeValue(Xml, "incremental") == "true"; }
        }

        [Interactive]
        public Selection Selection
        {
            get { return Module.GetAttributeValue(Xml, "selection") == "open" ? Selection.Open : Selection.Closed; }
        }

        protected override void SetValue(object value)
        {
            // deselect current visual
            if (SelectedItemNode != null &&
                SelectedItemNode.Selectable != null)
                SelectedItemNode.Selectable.Deselect(this);

            // clear selected visual state
            selectedItemNodeCached = true;
            selectedItemNode = null;
            GetState<Select1State>().SelectedNodeId = null;

            // set node value
            if (Binding != null &&
                Binding.ModelItem != null)
                base.SetValue(value);
        }

        /// <summary>
        /// Gets the currently selected item visual.
        /// </summary>
        public ItemElement SelectedItemNode
        {
            get { return GetSelectedItemNode(); }
            set { SetSelectedItemNode(value); }
        }

        /// <summary>
        /// Implements the getter for SelectedItemVisual.
        /// </summary>
        /// <returns></returns>
        ItemElement GetSelectedItemNode()
        {
            if (!selectedItemNodeCached)
            {
                foreach (var itemNode in this.Descendants().OfType<ItemElement>())
                {
                    // find selectable visuals underneath item
                    if (itemNode.Selectable == null)
                        continue;

                    if (itemNode.Selectable.Selected(this))
                    {
                        selectedItemNode = itemNode;
                        break;
                    }
                }

                selectedItemNodeCached = true;
            }

            return selectedItemNode;
        }

        /// <summary>
        /// Implements the setter for SelectedItemVisual.
        /// </summary>
        /// <param name="node"></param>
        void SetSelectedItemNode(ItemElement node)
        {
            // deselect current visual
            if (SelectedItemNode != null &&
                SelectedItemNode.Selectable != null)
                SelectedItemNode.Selectable.Deselect(this);

            if (node != null &&
                node.Selectable != null)
            {
                // pre-cache
                selectedItemNodeCached = true;
                selectedItemNode = node;

                // store selected item
                GetState<Select1State>().SelectedNodeId = node.UniqueId;

                // apply selection
                selectedItemNode.Selectable.Select(this);
            }
        }

        [Interactive]
        public string SelectedItemNodeId
        {
            get { return SelectedItemNode != null ? SelectedItemNode.UniqueId : null; }
            set { SetSelectedItemNodeId(value); }
        }

        /// <summary>
        /// Implements the setter for SelectedItemVisualId.
        /// </summary>
        /// <param name="id"></param>
        void SetSelectedItemNodeId(string id)
        {
            SelectedItemNode = this.Descendants()
                .OfType<ItemElement>()
                .FirstOrDefault(i => i.UniqueId == id);
        }

        public override void Refresh()
        {
            base.Refresh();

            // clear selected item cache
            selectedItemNode = null;
            selectedItemNodeCached = false;

            // if no item is selected, attempt to find one
            var selectedItemId = GetState<Select1State>().SelectedNodeId;
            if (selectedItemId == null)
            {
                // ensure descendant itemsets are refreshed, kind of a hack
                foreach (var itemSet in this.Descendants().OfType<ItemSetElement>())
                    itemSet.Refresh();

                foreach (var item in this.Descendants().OfType<ItemElement>())
                {
                    if (item.Selectable == null)
                        continue;

                    // is this the current selection?
                    if (item.Selectable.Selected(this))
                    {
                        // pre-cache
                        selectedItemNodeCached = true;
                        selectedItemNode = item;

                        // store selected item
                        GetState<Select1State>().SelectedNodeId = item.UniqueId;

                        break;
                    }
                }
            }
        }

    }

}
