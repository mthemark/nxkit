﻿using System;
using System.Diagnostics.Contracts;
using System.Xml.Linq;

using NXKit.DOMEvents;

namespace NXKit.XForms
{

    [NXElementInterface("{http://www.w3.org/2002/xforms}setvalue")]
    public class SetValue :
        IAction
    {

        readonly XElement element;

        /// <summary>
        /// Initializes a new instance.
        /// </summary>
        /// <param name="element"></param>
        public SetValue(XElement element)
        {
            Contract.Requires<ArgumentNullException>(element != null);

            this.element = element;
        }

        /// <summary>
        /// Gets the XForms module.
        /// </summary>
        XFormsModule Module
        {
            get { return element.Host().Module<XFormsModule>(); }
        }

        public void Handle(Event ev)
        {
            Module.InvokeAction(this);
        }

        public void Invoke()
        {
            throw new NotImplementedException();

            //Binding valueBinding = null;

            //if (Binding != null)
            //{
            //    var valueAttr = Module.GetAttributeValue(element, "value");
            //    if (valueAttr != null)
            //        valueBinding = new Binding(this, new EvaluationContext(Binding.Context.Model, Binding.Context.Instance, Binding.Context.ModelItem, 1, 1), valueAttr);
            //}

            //if (Binding == null ||
            //    Binding.ModelItem == null)
            //    return;

            //// default value
            //string newValue = null;

            //// resolve value from 'value' attribute
            //if (valueBinding != null)
            //    newValue = valueBinding.Value;
            //else
            //{
            //    // resolve value from contents
            //    var content = Xml.Value.TrimToNull();
            //    if (content != null)
            //        newValue = content;
            //}

            //// default setting
            //if (newValue == null)
            //    newValue = "";

            //// instruct model to complete deferred update
            //Binding.ModelItem.Value = newValue;
            //Binding.ModelItem.Model.State.RecalculateFlag = true;
            //Binding.ModelItem.Model.State.RevalidateFlag = true;
            //Binding.ModelItem.Model.State.RefreshFlag = true;
        }

    }

}
