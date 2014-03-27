﻿using System;
using System.Xml.Linq;
using NXKit.DOMEvents;

namespace NXKit.XForms
{

    [Element("range")]
    public class Range : 
        SingleNodeUIBindingElement
    {

        /// <summary>
        /// Initializes a new instance.
        /// </summary>
        /// <param name="element"></param>
        public Range(XElement element)
            : base(element)
        {

        }

        protected override void OnAdded(NXObjectEventArgs args)
        {
            base.OnAdded(args);

            this.Interface<IEventTarget>().AddEventHandler("xforms-invalid", false, i =>
            {
                Console.WriteLine(i);
            });

            this.Interface<IEventTarget>().AddEventHandler("xforms-valid", false, i =>
            {
                Console.WriteLine(i);
            });
        }

        [Interactive]
        public string Start
        {
            get { return Module.GetAttributeValue(Xml, "start"); }
        }

        [Interactive]
        public string End
        {
            get { return Module.GetAttributeValue(Xml, "end"); }
        }

        [Interactive]
        public string Step
        {
            get { return Module.GetAttributeValue(Xml, "step"); }
        }

    }

}