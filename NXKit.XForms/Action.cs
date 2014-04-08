﻿using System;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Xml.Linq;

using NXKit.DOMEvents;

namespace NXKit.XForms
{

    [NXElementInterface("{http://www.w3.org/2002/xforms}action")]
    public class Action :
        IAction
    {

        readonly XElement element;

        /// <summary>
        /// Initializes a new instance.
        /// </summary>
        /// <param name="element"></param>
        public Action(XElement element)
        {
            Contract.Requires<ArgumentNullException>(element != null);

            this.element = element;
        }

        /// <summary>
        /// Gets the associated element.
        /// </summary>
        public XElement Element
        {
            get { return element; }
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
            var actions = element
                .Elements()
                .SelectMany(i => i.Interfaces<IAction>());

            foreach (var action in actions)
                Module.InvokeAction(action);
        }

    }

}
