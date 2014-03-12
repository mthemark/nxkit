﻿using System.Diagnostics.Contracts;
using System.Linq;
using System.Xml.Linq;

namespace NXKit.XForms
{

    /// <summary>
    /// Base implementation for an XForms visual which implements Single-Node Binding.
    /// </summary>
    public class XFormsSingleNodeBindingVisual : 
        XFormsBindingVisual
    {

        XFormsEvaluationContext context;
        bool contextCached;

        /// <summary>
        /// Gets the evaluation context contributed to visuals within this visual's scope.
        /// </summary>
        public override XFormsEvaluationContext Context
        {
            get
            {
                if (!contextCached)
                {
                    context = null;

                    if (Binding != null &&
                        Binding.Node != null &&
                        Binding.Node.Document != null)
                    {
                        var model = Binding.Node.Document.Annotation<XFormsModelVisual>();
                        Contract.Assert(model != null);

                        var instance = Binding.Node.Document.Annotation<XFormsInstanceVisual>();
                        Contract.Assert(instance != null);

                        context = new XFormsEvaluationContext(model, instance, Binding.Node, 1, 1);
                    }

                    contextCached = true;
                }

                return context;
            }
        }

        /// <summary>
        /// Gets the type of the bound data.
        /// </summary>
        public XName Type
        {
            get { return Binding != null ? Binding.Type : null; }
        }

        /// <summary>
        /// Gets whether or not this visual is enabled.
        /// </summary>
        public virtual bool Relevant
        {
            get
            {
                // 8.1.1 Implementation Requirements Common to All Form Controls

                // the Single Node Binding is expressed and resolves to empty nodeset
                if (Binding == null)
                    return true;

                // the Single Node Binding is expressed and resolves to empty nodeset
                if (Binding.Node == null)
                    return false;

                // the Single Node Binding is expressed and resolves to a non-relevant instance node
                if (Binding.Relevant == false)
                    return false;

                // the form control is contained by a non-relevant switch or group (which includes a non-relevant repeat item)
                var scope = Ascendants().OfType<IRelevancyScope>().FirstOrDefault();
                if (scope != null)
                    if (scope.Relevant == false)
                        return false;

                // the form control is contained by a non-selected case element of a switch
                // TODO

                return true;
            }
        }

        /// <summary>
        /// Gets whether or not this visual is read-only.
        /// </summary>
        public bool ReadOnly
        {
            get { return Binding != null ? Binding.ReadOnly : true; }
        }

        /// <summary>
        /// Gets whether or not this visual is required.
        /// </summary>
        public bool Required
        {
            get { return Binding != null ? Binding.Required : false; }
        }

        /// <summary>
        /// Refreshes the visual's state from the model.
        /// </summary>
        public override void Refresh()
        {
            // rebuild binding
            Binding = Module.ResolveSingleNodeBinding(this);

            // rebuild cached values
            context = null;
            contextCached = false;

            base.Refresh();
        }

    }

}
