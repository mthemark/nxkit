﻿using System;
using System.Diagnostics.Contracts;

namespace NXKit.XForms
{

    /// <summary>
    /// Provides the attributes for the bind element.
    /// </summary>
    public class ModelItemPropertyAttributes :
        AttributeAccessor
    {

        /// <summary>
        /// Initializes a new instance.
        /// </summary>
        /// <param name="element"></param>
        public ModelItemPropertyAttributes(NXElement element)
            : base(element)
        {
            Contract.Requires<ArgumentNullException>(element != null);
        }

        public string Type
        {
            get { return GetAttribute("type"); }
        }

        public string Calculate
        {
            get { return GetAttribute("calculate"); }
        }

        public string ReadOnly
        {
            get { return GetAttribute("readonly"); }
        }

        public string Required
        {
            get { return GetAttribute("required"); }
        }

        public string Relevant
        {
            get { return GetAttribute("relevant"); }
        }

        public string Constraint
        {
            get { return GetAttribute("constraint"); }
        }

        public string P3PType
        {
            get { return GetAttribute("p3ptype"); }
        }

    }

}