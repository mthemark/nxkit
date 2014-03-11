﻿using System;
using System.Diagnostics.CodeAnalysis;
using System.Diagnostics.Contracts;
using System.IO;
using System.Linq;
using System.Xml;
using System.Xml.Linq;

namespace NXKit
{

    /// <summary>
    /// Hosts an NXKit document. Provides access to the visual tree for a renderer or other processor.
    /// </summary>
    public class NXDocument :
        INXDocument
    {

        /// <summary>
        /// Creates a new default <see cref="NXDocumentConfiguration"/> instance.
        /// </summary>
        /// <returns></returns>
        public static NXDocumentConfiguration CreateDefaultConfiguration()
        {
            return new NXDocumentConfiguration();
        }

        readonly NXDocumentConfiguration configuration;
        readonly XDocument xml;
        readonly IResolver resolver;

        readonly VisualStateCollection visualState;
        int nextElementId;

        StructuralVisual rootVisual;
        Module[] modules;

        [ContractInvariantMethod]
        [SuppressMessage("Microsoft.Performance", "CA1822:MarkMembersAsStatic", Justification = "Required for code contracts.")]
        void ObjectInvariant()
        {
            Contract.Invariant(configuration != null);
            Contract.Invariant(xml != null);
            Contract.Invariant(resolver != null);
            Contract.Invariant(visualState != null);
            Contract.Invariant(nextElementId >= 0);
        }

        /// <summary>
        /// Initializes a new instance.
        /// </summary>
        /// <param name="uri"></param>
        /// <param name="resolver"></param>
        public NXDocument(Uri uri, IResolver resolver)
            : this(resolver.Get(uri), resolver)
        {
            Contract.Requires<ArgumentNullException>(uri != null);
            Contract.Requires<ArgumentNullException>(resolver != null);
        }

        /// <summary>
        /// Initializes a new instance.
        /// </summary>
        /// <param name="document"></param>
        /// <param name="resolver"></param>
        public NXDocument(Stream document, IResolver resolver)
            : this(XDocument.Load(document), resolver)
        {
            Contract.Requires<ArgumentNullException>(document != null);
            Contract.Requires<ArgumentNullException>(resolver != null);
        }

        /// <summary>
        /// Initializes a new instance.
        /// </summary>
        /// <param name="document"></param>
        /// <param name="resolver"></param>
        public NXDocument(XmlReader document, IResolver resolver)
            : this(CreateDefaultConfiguration(), document, resolver)
        {
            Contract.Requires<ArgumentNullException>(document != null);
            Contract.Requires<ArgumentNullException>(resolver != null);
        }

        /// <summary>
        /// Initializes a new instance.
        /// </summary>
        /// <param name="document"></param>
        /// <param name="resolver"></param>
        public NXDocument(XmlDocument document, IResolver resolver)
            : this(CreateDefaultConfiguration(), document, resolver)
        {
            Contract.Requires<ArgumentNullException>(document != null);
            Contract.Requires<ArgumentNullException>(resolver != null);
        }

        /// <summary>
        /// Initializes a new instance.
        /// </summary>
        /// <param name="document"></param>
        /// <param name="resolver"></param>
        public NXDocument(XDocument document, IResolver resolver)
            : this(CreateDefaultConfiguration(), document, resolver)
        {
            Contract.Requires<ArgumentNullException>(document != null);
            Contract.Requires<ArgumentNullException>(resolver != null);
        }

        /// <summary>
        /// Initializes a new instance.
        /// </summary>
        /// <param name="document"></param>
        /// <param name="resolver"></param>
        public NXDocument(string document, IResolver resolver)
            : this(CreateDefaultConfiguration(), document, resolver)
        {
            Contract.Requires<ArgumentNullException>(document != null);
            Contract.Requires<ArgumentNullException>(resolver != null);
        }

        /// <summary>
        /// Initializes a new instance.
        /// </summary>
        /// <param name="configuration"></param>
        /// <param name="document"></param>
        /// <param name="resolver"></param>
        public NXDocument(NXDocumentConfiguration configuration, string document, IResolver resolver)
            : this(configuration, XDocument.Parse(document), resolver)
        {
            Contract.Requires<ArgumentNullException>(configuration != null);
            Contract.Requires<ArgumentNullException>(document != null);
            Contract.Requires<ArgumentNullException>(resolver != null);
        }

        /// <summary>
        /// Initializes a new instance.
        /// </summary>
        /// <param name="configuration"></param>
        /// <param name="uri"></param>
        /// <param name="resolver"></param>
        public NXDocument(NXDocumentConfiguration configuration, Uri uri, IResolver resolver)
            : this(configuration, resolver.Get(uri), resolver)
        {
            Contract.Requires<ArgumentNullException>(uri != null);
            Contract.Requires<ArgumentNullException>(resolver != null);
        }

        /// <summary>
        /// Initializes a new instance.
        /// </summary>
        /// <param name="configuration"></param>
        /// <param name="document"></param>
        /// <param name="resolver"></param>
        public NXDocument(NXDocumentConfiguration configuration, Stream document, IResolver resolver)
            : this(configuration, XDocument.Load(document), resolver)
        {
            Contract.Requires<ArgumentNullException>(configuration != null);
            Contract.Requires<ArgumentNullException>(document != null);
            Contract.Requires<ArgumentNullException>(resolver != null);
        }

        /// <summary>
        /// Initializes a new instance.
        /// </summary>
        /// <param name="configuration"></param>
        /// <param name="document"></param>
        /// <param name="resolver"></param>
        public NXDocument(NXDocumentConfiguration configuration, XmlReader document, IResolver resolver)
            : this(configuration, XDocument.Load(document), resolver)
        {
            Contract.Requires<ArgumentNullException>(configuration != null);
            Contract.Requires<ArgumentNullException>(document != null);
            Contract.Requires<ArgumentNullException>(resolver != null);
        }

        /// <summary>
        /// Initializes a new instance.
        /// </summary>
        /// <param name="configuration"></param>
        /// <param name="document"></param>
        /// <param name="resolver"></param>
        public NXDocument(NXDocumentConfiguration configuration, XmlDocument document, IResolver resolver)
            : this(configuration, new XmlNodeReader(document), resolver)
        {
            Contract.Requires<ArgumentNullException>(configuration != null);
            Contract.Requires<ArgumentNullException>(document != null);
            Contract.Requires<ArgumentNullException>(resolver != null);
        }

        /// <summary>
        /// Initializes a new instance.
        /// </summary>
        /// <param name="configuration"></param>
        /// <param name="document"></param>
        /// <param name="resolver"></param>
        public NXDocument(NXDocumentConfiguration configuration, XDocument document, IResolver resolver)
            : this(configuration, document, resolver, 1, new VisualStateCollection())
        {
            Contract.Requires<ArgumentNullException>(configuration != null);
            Contract.Requires<ArgumentNullException>(document != null);
            Contract.Requires<ArgumentNullException>(resolver != null);
        }

        /// <summary>
        /// Initializes a new instance.
        /// </summary>
        /// <param name="state"></param>
        /// <param name="resolver"></param>
        public NXDocument(NXDocumentState state, IResolver resolver)
            : this(state.Configuration, XDocument.Parse(state.Document), resolver, state.NextElementId, state.VisualState)
        {
            Contract.Requires<ArgumentNullException>(state != null);
            Contract.Requires<ArgumentNullException>(resolver != null);
        }

        /// <summary>
        /// Initializes a new instance.
        /// </summary>
        /// <param name="configuration"></param>
        /// <param name="document"></param>
        /// <param name="resolver"></param>
        /// <param name="nextElementId"></param>
        /// <param name="visualState"></param>
        NXDocument(NXDocumentConfiguration configuration, XDocument document, IResolver resolver, int nextElementId, VisualStateCollection visualState)
        {
            Contract.Requires<ArgumentNullException>(configuration != null);
            Contract.Requires<ArgumentNullException>(document != null);
            Contract.Requires<ArgumentNullException>(resolver != null);
            Contract.Requires<ArgumentOutOfRangeException>(nextElementId >= 0);
            Contract.Requires<ArgumentNullException>(visualState != null);

            this.configuration = configuration;
            this.xml = new XDocument(document);
            this.resolver = resolver;

            this.nextElementId = nextElementId;
            this.visualState = visualState;

            Initialize();
        }

        /// <summary>
        /// Initializes modules
        /// </summary>
        void Initialize()
        {
            // dictionary of types to instances
            var m = configuration.ModuleTypes
                .ToDictionary(i => i, i => (Module)null);

            do
            {
                // instantiate types
                foreach (var i in m.ToList())
                    if (i.Value == null)
                        m[i.Key] = (Module)Activator.CreateInstance(i.Key);

                // add dependency types
                foreach (var i in m.ToList())
                    foreach (var d in i.Value.DependsOn)
                        if (!m.ContainsKey(d))
                            m[d] = null;
            }
            // end when all types are instantiated
            while (m.Any(i => i.Value == null));

            // generate final module list
            modules = m.Values.ToArray();

            // initialize modules
            foreach (var module in modules)
                module.Initialize(this);

            // initiate run
            Invoke();
        }

        /// <summary>
        /// Gets the current engine configuration.
        /// </summary>
        public NXDocumentConfiguration Configuration
        {
            get { return configuration; }
        }

        /// <summary>
        /// Gets a reference to the current <see cref="Xml"/> being handled.
        /// </summary>
        public XDocument Xml
        {
            get { return xml; }
        }

        /// <summary>
        /// Gets a reference to the <see cref="IResolver"/> which is used to save or load external resources.
        /// </summary>
        public IResolver Resolver
        {
            get { return resolver; }
        }

        /// <summary>
        /// Gets the loaded module instance of the specified type.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public T GetModule<T>()
            where T : Module
        {
            return modules.OfType<T>().FirstOrDefault();
        }

        /// <summary>
        /// Gets a reference to the per-<see cref="Visual"/> state collection.
        /// </summary>
        public VisualStateCollection VisualState
        {
            get { return visualState; }
        }

        /// <summary>
        /// Invokes any outstanding actions.
        /// </summary>
        public void Invoke()
        {
            // run each module until no module does anything
            bool run;
            do
            {
                run = false;
                foreach (var module in modules)
                    run |= module.Invoke();
            }
            while (run);

            // raise the added event for visuals that have not yet had it raised
            foreach (var visual in RootVisual.Descendants())
                visual.RaiseAddedEvent();
        }

        /// <summary>
        /// Gets the 'id' attribute for the given Element, or creates it on demand.
        /// </summary>
        /// <param name="element"></param>
        /// <returns></returns>
        public string GetElementId(XElement element)
        {
            var idAttr = element.Attribute("id");
            if (idAttr == null)
            {
                element.SetAttributeValue("id", "_element" + ++nextElementId);
                idAttr = element.Attribute("id");
            }

            return idAttr.Value;
        }

        /// <summary>
        /// Gets a reference to the root <see cref="Visual"/> instance for navigating the visual tree.
        /// </summary>
        public StructuralVisual RootVisual
        {
            get { return rootVisual ?? (rootVisual = CreateRootVisual()); }
        }

        /// <summary>
        /// Creates a new root <see cref="Visual"/> instance for navigating the visual tree.
        /// </summary>
        /// <returns></returns>
        StructuralVisual CreateRootVisual()
        {
            Contract.Ensures(Contract.Result<StructuralVisual>() != null);

            return (StructuralVisual)((INXDocument)this).CreateVisual(null, Xml.Root) ?? new UnknownRootVisual(this, null, Xml.Root);
        }

        /// <summary>
        /// Creates a <see cref="Visual"/> from the loaded <see cref="Module"/>s.
        /// </summary>
        /// <param name="parent"></param>
        /// <param name="element"></param>
        /// <returns></returns>
        Visual CreateVisualFromModules(XElement element)
        {
            Contract.Requires<ArgumentNullException>(element != null);

            return modules.Select(i => i.CreateVisual(element.Name)).FirstOrDefault(i => i != null);
        }

        /// <summary>
        /// Implements IVisualBuilder.CreateVisual.
        /// </summary>
        /// <param name="parent"></param>
        /// <param name="node"></param>
        /// <returns></returns>
        Visual INXDocument.CreateVisual(StructuralVisual parent, XNode node)
        {
            Contract.Requires<ArgumentNullException>(node != null);

            if (node is XText)
            {
                var v = new TextVisual();
                v.Initialize(this, parent, node);
                return v;
            }
            else if (node is XElement)
            {
                // create new instance of visual using extensions
                var visual = CreateVisualFromModules((XElement)node);
                if (visual != null)
                {
                    visual.Initialize(this, parent, node);

                    // give each module a chance to add additional information to the visual
                    foreach (var module2 in modules)
                        module2.AnnotateVisual(visual);

                    return visual;
                }
            }

            return null;
        }

        /// <summary>
        /// Saves the current state of the processor in a serializable format.
        /// </summary>
        /// <returns></returns>
        public NXDocumentState Save()
        {
            return new NXDocumentState()
            {
                Configuration = configuration,
                Document = xml.ToString(SaveOptions.DisableFormatting),
                NextElementId = nextElementId,
                VisualState = visualState,
            };
        }

        /// <summary>
        /// Invoke to begin a form submission.
        /// </summary>
        public void Submit()
        {
            if (ProcessSubmit != null)
                ProcessSubmit(this, EventArgs.Empty);
        }

        /// <summary>
        /// Raised to initiate submission of the form.
        /// </summary>
        public event EventHandler ProcessSubmit;

    }

}