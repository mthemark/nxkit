﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using System.Xml.XPath;

using NXKit.DOMEvents;
using NXKit.Util;

namespace NXKit.XForms
{

    [Element("model")]
    public class ModelElement :
        XFormsElement,
        IEvaluationContextScope,
        IEventDefaultActionHandler
    {

        ModelElementState state;

        /// <summary>
        /// Initializes a new instance.
        /// </summary>
        /// <param name="xml"></param>
        public ModelElement(XElement xml)
            : base(xml)
        {

        }

        /// <summary>
        /// Gets a reference to the model visual's state.
        /// </summary>
        public ModelElementState State
        {
            get { return state ?? (state = GetState<ModelElementState>()); }
        }

        /// <summary>
        /// Provides the model default evaluation context.
        /// </summary>
        public EvaluationContext Context
        {
            get { return DefaultEvaluationContext; }
        }

        /// <summary>
        /// Gets the set of <see cref="InstanceElement"/>s.
        /// </summary>
        public IEnumerable<InstanceElement> Instances
        {
            get { return Elements().OfType<InstanceElement>(); }
        }

        /// <summary>
        /// Gets a new <see cref="EvaluationContext"/> that should be the default used for the model when no
        /// instance is known.
        /// </summary>
        public EvaluationContext DefaultEvaluationContext
        {
            get
            {
                if (State == null)
                    return null;

                var defaultInstance = Instances.FirstOrDefault();
                if (defaultInstance == null)
                    return null;

                if (defaultInstance.State.Document == null)
                    return null;

                return new EvaluationContext(this, defaultInstance, new ModelItem(Module, defaultInstance.State.Document.Root), 1, 1);
            }
        }

        /// <summary>
        /// Gets all of the bound nodes for this model.
        /// </summary>
        /// <returns></returns>
        IEnumerable<IUIBindingNode> GetUIBindingNodes()
        {
            // all available ui bindings
            return Document.Root
                .Descendants(true)
                .OfType<NXElement>()
                .Select(i => i.Interface<IUIBindingNode>())
                .Where(i => i != null);
        }

        /// <summary>
        /// Gets all of the nodes that should be refreshed for this model.
        /// </summary>
        /// <returns></returns>
        IEnumerable<IUIRefreshable> GetUIRefreshableNodes()
        {
            // all available ui bindings
            return Document.Root
                .Descendants(true)
                .OfType<NXElement>()
                .Select(i => i.Interface<IUIRefreshable>())
                .Where(i => i != null);
        }

        void IEventDefaultActionHandler.DefaultAction(Event evt)
        {
            switch (evt.Type)
            {
                case XFormsEvents.ModelConstruct:
                    OnModelConstruct();
                    break;
                case XFormsEvents.ModelConstructDone:
                    OnModelConstructDone();
                    break;
                case XFormsEvents.Ready:
                    OnReady();
                    break;
                case XFormsEvents.Rebuild:
                    OnRebuild();
                    break;
                case XFormsEvents.Recalculate:
                    OnRecalculate();
                    break;
                case XFormsEvents.Revalidate:
                    OnRevalidate();
                    break;
                case XFormsEvents.Refresh:
                    OnRefresh();
                    break;
                case XFormsEvents.Reset:
                    OnReset();
                    break;
            }
        }

        /// <summary>
        /// Default action for the xforms-model-construct event.
        /// </summary>
        void OnModelConstruct()
        {
            // mark step as complete, regardless of outcome
            State.Construct = true;

            // validate model version, we only support 1.0
            var versions = Module.GetAttributeValue(Xml, "version");
            if (versions != null)
                foreach (var version in versions.Split(' ').Select(i => i.Trim()).Where(i => !string.IsNullOrEmpty(i)))
                    if (version != "1.0")
                    {
                        this.Interface<IEventTarget>().DispatchEvent(new VersionExceptionEvent(this).Event);
                        return;
                    }

            var schema = Module.GetAttributeValue(Xml, "schema");
            if (schema != null)
                foreach (var item in schema.Split(' ').Select(i => i.Trim()).Where(i => !string.IsNullOrEmpty(i)))
                    continue; // TODO

            // attempt to load model instance data, if possible; if no instance loaded, exit
            Module.ProcessModelInstance(this);
            if (!Instances.Any(i => i.State.Document != null))
                return;

            OnRebuild();
            OnRecalculate();
            OnRevalidate();
        }

        /// <summary>
        /// Default action for the xforms-model-construct-done event.
        /// </summary>
        void OnModelConstructDone()
        {
            State.ConstructDone = true;

            if (Document.Root.GetState<ModuleState>().ConstructDoneOnce)
                return;

            // refresh values
            foreach (var item1 in GetUIBindingNodes())
                if (item1.UIBinding != null)
                    item1.UIBinding.Refresh();

            foreach (var item2 in GetUIRefreshableNodes())
                item2.Refresh();

            // dispatch required events
            foreach (var item3 in GetUIBindingNodes())
                if (item3.UIBinding != null)
                    item3.UIBinding.ClearEvents();

            Document.Root.GetState<ModuleState>().ConstructDoneOnce = true;
        }

        /// <summary>
        /// Default action for the xforms-ready event.
        /// </summary>
        internal void OnReady()
        {
            State.Ready = true;
        }

        /// <summary>
        /// Default action for the xforms-rebuild event.
        /// </summary>
        internal void OnRebuild()
        {
            do
            {
                State.RebuildFlag = false;
                State.RecalculateFlag = true;
            }
            while (State.RebuildFlag);
        }

        /// <summary>
        /// Default action for the xforms-recalculate event.
        /// </summary>
        internal void OnRecalculate()
        {
            do
            {
                State.RecalculateFlag = false;
                State.RevalidateFlag = true;

                // for each each instance underneath the model
                foreach (var instance in Instances)
                {
                    var instanceModelItems = instance.State.Document.Root
                        .DescendantsAndSelf()
                        .SelectMany(i => i.Attributes().Cast<XObject>().Prepend(i))
                        .Where(i => i is XElement || i is XAttribute)
                        .Select(i => new ModelItem(Module, i));

                    // for each model item underneath the instance
                    foreach (var modelItem in instanceModelItems)
                    {
                        if (modelItem.State.Clear)
                        {
                            if (modelItem.Xml is XElement)
                                ((XElement)modelItem.Xml).RemoveNodes();
                            else if (modelItem.Xml is XAttribute)
                                ((XAttribute)modelItem.Xml).SetValue("");
                            else
                                throw new InvalidOperationException();

                            modelItem.State.Clear = false;
                            modelItem.State.DispatchValueChanged = true;

                            // prompt model to act
                            State.RevalidateFlag = true;
                            State.RefreshFlag = true;
                        }

                        // model item contains a new value
                        if (modelItem.State.NewContents != null)
                        {
                            if (modelItem.Xml is XElement)
                                ((XElement)modelItem.Xml).ReplaceAll(modelItem.State.NewContents);
                            else if (modelItem.Xml is XAttribute)
                                ((XAttribute)modelItem.Xml).SetValue(modelItem.State.NewValue);
                            else
                                throw new InvalidOperationException();

                            modelItem.State.NewContents = null;
                            modelItem.State.DispatchValueChanged = true;

                            // prompt model to act
                            State.RevalidateFlag = true;
                            State.RefreshFlag = true;
                        }

                        if (modelItem.State.NewValue != null)
                        {
                            if (modelItem.Xml is XElement)
                                ((XElement)modelItem.Xml).SetValue(modelItem.State.NewValue);
                            else if (modelItem.Xml is XAttribute)
                                ((XAttribute)modelItem.Xml).SetValue(modelItem.State.NewValue);
                            else
                                throw new Exception();

                            modelItem.State.NewContents = null;
                            modelItem.State.DispatchValueChanged = true;

                            // prompt model to act
                            State.RevalidateFlag = true;
                            State.RefreshFlag = true;
                        }
                    }
                }

                // apply binding expressions
                foreach (var bind in this.Descendants(false).OfType<BindElement>())
                {
                    if (bind.Binding == null)
                        continue;

                    bind.Binding.Refresh();
                    if (bind.Binding.ModelItems == null ||
                        bind.Binding.ModelItems.Length == 0)
                        continue;

                    var typeAttr = Module.GetAttributeValue(bind.Xml, "type");
                    var calculateAttr = Module.GetAttributeValue(bind.Xml, "calculate");
                    var readonlyAttr = Module.GetAttributeValue(bind.Xml, "readonly");
                    var requiredAttr = Module.GetAttributeValue(bind.Xml, "required");
                    var relevantAttr = Module.GetAttributeValue(bind.Xml, "relevant");
                    var constraintAttr = Module.GetAttributeValue(bind.Xml, "constraint");

                    for (int i = 0; i < bind.Binding.ModelItems.Length; i++)
                    {
                        var modelItem = bind.Binding.ModelItems[i];
                        var modelItemState = modelItem.State;

                        var ec = new EvaluationContext(bind.Binding.Context.Model, bind.Binding.Context.Instance, modelItem, i + 1, bind.Binding.ModelItems.Length);
                        var nc = new XFormsXsltContext(bind, ec);

                        // get existing values
                        var oldReadOnly = modelItem.ReadOnly;
                        var oldRequired = modelItem.Required;
                        var oldRelevant = modelItem.Relevant;
                        var oldValue = modelItem.Value;

                        if (typeAttr != null)
                        {
                            // lookup namespace of type specifier
                            var st = typeAttr.Split(':');
                            var ns = st.Length == 2 ? nc.LookupNamespace(st[0]) : null;
                            var lp = st.Length == 2 ? st[1] : st[0];
                            modelItemState.Type = XName.Get(lp, ns);
                        }

                        // recalculate read-only value
                        if (readonlyAttr != null)
                        {
                            var obj = Module.EvaluateXPath(bind, ec, readonlyAttr, XPathResultType.Any);
                            if (obj is bool)
                                modelItemState.ReadOnly = (bool)obj;
                            else if (obj is string && !string.IsNullOrWhiteSpace((string)obj))
                                modelItemState.ReadOnly = bool.Parse((string)obj);
                            else
                                throw new Exception();
                        }

                        // recalculate required value
                        if (requiredAttr != null)
                        {
                            var obj = Module.EvaluateXPath(bind, ec, requiredAttr, XPathResultType.Any);
                            if (obj is bool)
                                modelItemState.Required = (bool)obj;
                            else if (obj is string && !string.IsNullOrWhiteSpace((string)obj))
                                modelItemState.Required = bool.Parse((string)obj);
                        }

                        // recalculate relevant value
                        if (relevantAttr != null)
                        {
                            var obj = Module.EvaluateXPath(bind, ec, relevantAttr, XPathResultType.Any);
                            if (obj is bool)
                                modelItemState.Relevant = (bool)obj;
                            else if (obj is string && !string.IsNullOrWhiteSpace((string)obj))
                                modelItemState.Relevant = bool.Parse((string)obj);
                        }

                        // calculate before setting read-only, so read-only can be overridden
                        if (calculateAttr != null)
                        {
                            // calculated nodes are readonly
                            modelItemState.ReadOnly = true;

                            var calculateBinding = new Binding(bind, ec, calculateAttr);
                            if (calculateBinding.Value != null)
                                if (oldValue != calculateBinding.Value)
                                    modelItem.Value = calculateBinding.Value;
                        }

                        if (constraintAttr != null)
                        {
                            var obj = Module.EvaluateXPath(bind, ec, constraintAttr, XPathResultType.Any);
                            if (obj is bool)
                                modelItemState.Constraint = (bool)obj;
                            else if (obj is string && !string.IsNullOrWhiteSpace((string)obj))
                                modelItemState.Constraint = bool.Parse((string)obj);
                        }

                        // get new read-only value; raise event on change
                        if (modelItem.ReadOnly != oldReadOnly)
                            if (modelItem.ReadOnly)
                                modelItemState.DispatchReadOnly = true;
                            else
                                modelItemState.DispatchReadWrite = true;

                        // get new required value; raise event on change
                        if (modelItem.Required != oldRequired)
                            if (modelItem.Required)
                                modelItemState.DispatchRequired = true;
                            else
                                modelItemState.DispatchOptional = true;

                        // get new relevant value; raise event on change
                        if (modelItem.Relevant != oldRelevant)
                            if (modelItem.Relevant)
                                modelItemState.DispatchEnabled = true;
                            else
                                modelItemState.DispatchDisabled = true;
                    }
                }
            }
            while (State.RecalculateFlag);
        }

        /// <summary>
        /// Default action for the xforms-revalidate event.
        /// </summary>
        internal void OnRevalidate()
        {
            do
            {
                State.RevalidateFlag = false;

                foreach (var instance in Instances)
                {
                    // all model items
                    var modelItems = instance.State.Document.Root.DescendantNodesAndSelf()
                        .OfType<XElement>()
                        .SelectMany(i => i.Attributes().Cast<XObject>().Prepend(i))
                        .Select(i => new ModelItem(Module, i));

                    foreach (var modelItem in modelItems)
                    {
                        var oldValid = modelItem.Valid;

                        modelItem.State.Valid = (modelItem.Required ? modelItem.Value.TrimToNull() != null : true) && modelItem.Constraint;
                        if (oldValid != modelItem.Valid)
                            if (modelItem.Valid)
                                modelItem.State.DispatchValid = true;
                            else
                                modelItem.State.DispatchInvalid = true;
                    }
                }
            }
            while (State.RevalidateFlag);
        }

        /// <summary>
        /// Default action for the xforms-refresh event.
        /// </summary>
        internal void OnRefresh()
        {
            do
            {
                State.RefreshFlag = false;

                // all available ui bindings
                var bindingNodes = GetUIBindingNodes()
                    .Select(i => i.UIBinding)
                    .Where(i => i != null);

                // refresh values
                foreach (var item in bindingNodes)
                    item.Refresh();

                foreach (var item in GetUIRefreshableNodes())
                    item.Refresh();

                // dispatch required events
                foreach (var item in bindingNodes)
                    item.DispatchEvents();
            }
            while (State.RefreshFlag);

            // clear any notification events
            foreach (var instance in Instances)
            {
                // all model items
                var items = instance.State.Document.Root.DescendantNodesAndSelf()
                    .OfType<XElement>()
                    .SelectMany(i => i.Attributes().Cast<XObject>().Prepend(i))
                    .Select(i => new ModelItem(Module, i));

                // clear notifications
                foreach (var item in items)
                    item.State.Reset();
            }
        }

        /// <summary>
        /// Default action for the xforms-reset event.
        /// </summary>
        internal void OnReset()
        {

        }

    }

}