﻿using System;
using System.Diagnostics.Contracts;
using System.Xml.Linq;

using NXKit.DOMEvents;

namespace NXKit.XForms
{

    [Interface("{http://www.w3.org/2002/xforms}send")]
    public class Send :
        ElementExtension,
        IAction
    {

        /// <summary>
        /// Initializes a new instance.
        /// </summary>
        /// <param name="element"></param>
        public Send(XElement element)
            : base(element)
        {
            Contract.Requires<ArgumentNullException>(element != null);
        }

        public void Handle(Event ev)
        {
            Invoke();
        }

        public void Invoke()
        {
            throw new NotImplementedException();

            //var submissionAttr = Module.GetAttributeValue(Xml, "submission");
            //if (submissionAttr != null)
            //{
            //    var submissionVisual = ResolveId(submissionAttr);
            //    if (submissionVisual != null)
            //        submissionVisual.Interface<INXEventTarget>().DispatchEvent(Events.Submit);
            //}
        }

    }

}
