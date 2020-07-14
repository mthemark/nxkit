using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using NXKit.Composition;

using System;
using System.Xml.Linq;

namespace NXKit.Extensions.DependencyInjection
{

    /// <summary>
    /// Implements the <see cref="ICompositionContextBuilder"/> interface for Microsoft Dependency Injection scopes.
    /// </summary>
    class CompositionContextBuilder : ICompositionContextBuilder
    {

        readonly IServiceCollection services;

        /// <summary>
        /// Initializes a new instance.
        /// </summary>
        /// <param name="builder"></param>
        public CompositionContextBuilder(IServiceCollection services)
        {
            this.services = services ?? throw new ArgumentNullException(nameof(services));
        }

        public ICompositionContextBuilder AddInstance<T>(T instance) 
            where T : class
        {
            Console.WriteLine($"AutoF@cked-NetCore {nameof(AddInstance)} for : {instance.GetType()}");
            //services.AddScoped(a => instance as XObject);
            services.AddScoped(a => instance);
            return this;
        }

    }

}
