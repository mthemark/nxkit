﻿using System;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Xml.Linq;
using System.Xml.XPath;
using System.Xml.Xsl;

using NXKit.XPath;

namespace NXKit.XForms
{

    /// <summary>
    /// Provides a <see cref="XsltContext"/> for XForms visual operations.
    /// </summary>
    public class XFormsXsltContext :
        XsltContext
    {

        readonly XNode node;
        readonly EvaluationContext evaluationContext;
        readonly IXsltContextFunctionProvider functionProvider;

        /// <summary>
        /// Initializes a new instance.
        /// </summary>
        /// <param name="node"></param>
        /// <param name="evaluationContext"></param>
        internal XFormsXsltContext(
            XNode node,
            EvaluationContext evaluationContext)
        {
            Contract.Requires<ArgumentNullException>(node != null);
            Contract.Requires<ArgumentNullException>(evaluationContext != null);

            this.node = node;
            this.evaluationContext = evaluationContext;
            this.functionProvider = node.Host().Container.GetExportedValue<IXsltContextFunctionProvider>();
        }

        /// <summary>
        /// Gets the <see cref="Node"/> associated with the XSLT operation.
        /// </summary>
        public XNode Node
        {
            get { return node; }
        }

        /// <summary>
        /// Gets the <see cref="EvaluationContext"/> associated with the XSLT operation.
        /// </summary>
        public EvaluationContext EvaluationContext
        {
            get { return evaluationContext; }
        }

        public override bool Whitespace
        {
            get { return true; }
        }

        public override bool PreserveWhitespace(XPathNavigator node)
        {
            return true;
        }

        public override int CompareDocument(string baseUri, string nextbaseUri)
        {
            return 0;
        }

        public override string LookupNamespace(string prefix)
        {
            Contract.Requires<ArgumentNullException>(prefix != null);

            var element = node as XElement;
            if (element == null)
                element = node.Parent as XElement;
            if (element == null)
                throw new NullReferenceException();

            return prefix != "" ? element.GetNamespaceOfPrefix(prefix).NamespaceName : element.GetDefaultNamespace().NamespaceName;
        }

        public override string LookupPrefix(string namespaceName)
        {
            Contract.Requires<ArgumentNullException>(namespaceName != null);

            var element = node as XElement;
            if (element == null)
                element = node.Parent as XElement;
            if (element == null)
                throw new NullReferenceException();

            return element.GetPrefixOfNamespace(namespaceName);
        }

        public override IXsltContextFunction ResolveFunction(string prefix, string localName, XPathResultType[] argTypes)
        {
            var name = (prefix != "" ? LookupNamespace(prefix) : XNamespace.None) + localName;
            if (name == null)
                throw new XPathException("Unable to resolve function name.");

            return functionProvider.GetFunctions()
                .SelectMany(i => i.Metadata.ExpandedName
                    .Select((j, k) => new
                    {
                        Name = i.Metadata.ExpandedName[k],
                        IsPrefixRequired = i.Metadata.IsPrefixRequired[k],
                        Item = i,
                    }))
                .Where(i => ResolveFunctionPredicate(
                    XName.Get(i.Name), 
                    i.IsPrefixRequired, 
                    name))
                .Select(i => i.Item.Value)
                .FirstOrDefault();
        }

        /// <summary>
        /// Test whether the given candidate function data matches with the requested name.
        /// </summary>
        /// <param name="name"></param>
        /// <param name="isPrefixRequired"></param>
        /// <param name="requested"></param>
        /// <returns></returns>
        bool ResolveFunctionPredicate(XName name, bool isPrefixRequired, XName requested)
        {
            if (requested.LocalName != name.LocalName)
                return false;

            if (requested.NamespaceName == "")
                if (!isPrefixRequired)
                    return true;

            if (requested.NamespaceName == name.NamespaceName)
                return true;

            return false;
        }

        public override IXsltContextVariable ResolveVariable(string prefix, string name)
        {
            throw new NotImplementedException();
        }

    }

}
