﻿using System;
using System.Diagnostics.Contracts;
using System.Xml.Linq;

namespace NXKit.Scripting
{

    /// <summary>
    /// Provides an interface towards executing scripts within the document.
    /// </summary>
    public class DocumentScript :
        IDocumentScript
    {

        readonly XDocument document;

        /// <summary>
        /// Initializes a new instance.
        /// </summary>
        /// <param name="document"></param>
        public DocumentScript(XDocument document)
        {
            Contract.Requires<ArgumentNullException>(document != null);

            this.document = document;
        }

        IScriptDispatcher ScriptDispatcher
        {
            get { return document.Host().Container.GetExportedValue<IScriptDispatcher>(); }
        }

        public object Execute(string type, string code)
        {
            return ScriptDispatcher.Execute(type, code);
        }

    }

}
