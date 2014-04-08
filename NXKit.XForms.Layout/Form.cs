﻿using System;
using System.Diagnostics.Contracts;
using System.Xml.Linq;

namespace NXKit.XForms.Layout
{

    [NXElementInterface("{http://schemas.nxkit.org/nxkit/2014/xforms-layout}form")]
    public class Form
    {

        /// <summary>
        /// Initializes a new instance.
        /// </summary>
        /// <param name="element"></param>
        public Form(XElement element)
        {
            Contract.Requires<ArgumentNullException>(element != null);
        }

    }

}
