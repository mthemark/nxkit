﻿using System;
using System.ComponentModel.Composition;
using System.Xml.Linq;

using NXKit.Composition;

namespace NXKit.XForms
{

    [Extension("{http://www.w3.org/2002/xforms}select")]
    [PartMetadata(ScopeCatalog.ScopeMetadataKey, Scope.Object)]
    public class Select :
        ElementExtension
    {

        /// <summary>
        /// Initializes a new instance.
        /// </summary>
        /// <param name="element"></param>
        public Select(XElement element)
            : base(element)
        {
            if (element == null)
                throw new ArgumentNullException(nameof(element));
        }

    }

}
