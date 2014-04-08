﻿using System;
using System.Diagnostics.Contracts;
using System.Xml.Linq;
using NXKit.DOMEvents;

namespace NXKit.XForms
{

    /// <summary>
    /// Provides a <see cref="Binding"/> for a UI element.
    /// </summary>
    [NXElementInterface("http://www.w3.org/2002/xforms", null)]
    public class BindingNode :
        IBindingNode,
        IEvaluationContextScope
    {

        readonly XElement element;
        readonly BindingAttributes attributes;
        readonly Lazy<Binding> binding;

        /// <summary>
        /// Initializes a new instance.
        /// </summary>
        /// <param name="element"></param>
        public BindingNode(XElement element)
        {
            Contract.Requires<ArgumentNullException>(element != null);

            this.element = element;
            this.attributes = new BindingAttributes(element);
            this.binding = new Lazy<Binding>(() => GetOrCreateBinding());
        }

        /// <summary>
        /// Gets the node binding attributes of the element.
        /// </summary>
        public BindingAttributes Attributes
        {
            get { return attributes; }
        }

        /// <summary>
        /// Gets the evaluation context to be used for the binding.
        /// </summary>
        public EvaluationContext EvaluationContext
        {
            get { return GetEvaluationContext(); }
        }

        /// <summary>
        /// Implements the getter for EvaluationContext.
        /// </summary>
        /// <returns></returns>
        EvaluationContext GetEvaluationContext()
        {
            var model = element.InterfaceOrDefault<NodeEvaluationContext>();
            if (model != null &&
                model.Context != null)
                return model.Context;

            return null;
        }

        /// <summary>
        /// Gets the binding provided.
        /// </summary>
        public Binding Binding
        {
            get { return binding.Value; }
        }

        /// <summary>
        /// Creates the binding.
        /// </summary>
        /// <returns></returns>
        Binding GetOrCreateBinding()
        {
            // bind attribute overrides
            var bindIdRef = Attributes.Bind;
            if (bindIdRef != null)
                return GetBindBinding(bindIdRef);

            // otherwise 'ref' or 'nodeset'
            var expression = Attributes.Ref ?? Attributes.NodeSet;
            if (expression == null)
                return null;

            // obtain evaluation context
            var context = EvaluationContext;
            if (context == null)
                return null;

            return new Binding(element, context, expression);
        }

        /// <summary>
        /// Gets the <see cref="Binding"/> returned by the referenced 'bind' element.
        /// </summary>
        /// <returns></returns>
        Binding GetBindBinding(string bindIdRef)
        {
            // resolve bind element
            var bind = element.ResolveId(bindIdRef);
            if (bind == null)
            {
                element.Interface<INXEventTarget>().DispatchEvent(Events.BindingException);
                return null;
            }

            var binding = bind.InterfaceOrDefault<IBindingNode>();
            if (binding != null)
                return binding.Binding;

            return null;
        }

        /// <summary>
        /// Gets the <see cref="EvaluationContext"/> provided to further children elements.
        /// </summary>
        public EvaluationContext Context
        {
            get { return Binding != null ? new EvaluationContext(Binding.ModelItem.Model, Binding.ModelItem.Instance, Binding.ModelItem, 1, 1) : null; }
        }

    }

}
