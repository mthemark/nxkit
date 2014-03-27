﻿using System;
using System.Xml.Linq;
using NXKit.Util;

namespace NXKit.XForms
{

    [Element("value")]
    public class ValueElement : 
        SingleNodeUIBindingElement, 
        ISelectableNode
    {

        /// <summary>
        /// Initializes a new instance.
        /// </summary>
        /// <param name="element"></param>
        public ValueElement(XElement element)
            : base(element)
        {

        }

        /// <summary>
        /// Gets the text value associated with this value visual.
        /// </summary>
        public string InlineContent
        {
            get { return Xml.Value.TrimToNull(); }
        }

        /// <summary>
        /// Obtains the simple value to be set.
        /// </summary>
        /// <returns></returns>
        private string GetNewValue()
        {
            if (Binding != null &&
                Binding.Value != null)
                return Binding.Value;
            else if (InlineContent != null)
                return InlineContent;
            else
                return "";
        }

        public void Select(SingleNodeUIBindingElement element)
        {
            if (element.Binding == null ||
                element.Binding.ModelItem == null)
                throw new InvalidOperationException();

            if (Binding != null &&
                Binding.Value == null)
                throw new InvalidOperationException();

            element.Binding.ModelItem.Value = GetNewValue();
        }

        public void Deselect(SingleNodeUIBindingElement visual)
        {

        }

        public bool Selected(SingleNodeUIBindingElement visual)
        {
            if (visual.Binding == null ||
                visual.Binding.ModelItem == null ||
                visual.Binding.Value == null)
                return false;

            // our value matches the current value?
            return visual.Binding.Value == GetNewValue();
        }

        public int GetValueHashCode()
        {
            if (Binding != null &&
                Binding.Value == null)
                return 0;

            return GetNewValue().GetHashCode();
        }
    }

}