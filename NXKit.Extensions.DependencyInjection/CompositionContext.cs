using System;

using Microsoft.Extensions.DependencyInjection;

using NXKit.Composition;

namespace NXKit.Extensions.DependencyInjection
{

    /// <summary>
    /// Microsoft Dependency Injection based implementation of <see cref="ICompositionContext"/>.
    /// </summary>
    class CompositionContext : ICompositionContext
    {

        readonly IServiceProvider provider;
        readonly IServiceScope scope;
        readonly IServicesAssessor servicesAssessor;

        /// <summary>
        /// Initializes a new instance.
        /// </summary>
        /// <param name="provider"></param>
        /// <param name="scope"></param>
        public CompositionContext(IServiceProvider provider, IServiceScope scope, IServicesAssessor servicesAssessor)
        {
            this.provider = provider ?? throw new ArgumentNullException(nameof(provider));
            this.scope = scope ?? throw new ArgumentNullException(nameof(scope));
            this.servicesAssessor = servicesAssessor ?? throw new ArgumentNullException(nameof(servicesAssessor));
        }

        /// <summary>
        /// Initializes a new instance.
        /// </summary>
        /// <param name="scope"></param>
        public CompositionContext(IServiceScope scope, IServicesAssessor servicesAssessor) :
            this(scope.ServiceProvider, scope, servicesAssessor)
        {

        }

        public ICompositionContext BeginContext(CompositionScope scope)
        {
            return new CompositionContext(provider.GetRequiredService<IServiceScopeFactory>().CreateScope(), servicesAssessor);
        }

        public ICompositionContext BeginContext(CompositionScope scope, Action<ICompositionContextBuilder> builder)
        {
            try
            {
                var c = BeginContext(scope);
                var cbuilder = new CompositionContextBuilder(servicesAssessor.Services);
                builder(cbuilder);
                return c;                
            }
            catch (Exception e)
            {
                Console.WriteLine($"{nameof(CompositionContext)}.{nameof(BeginContext)} builder failed: {e}");
            }
            return BeginContext(scope);
        }

        public T Resolve<T>()
        {
            return provider.GetRequiredService<T>();
        }

        public object Resolve(Type type)
        {
            return provider.GetRequiredService(type);
        }

        public void Dispose()
        {
            if (scope != null)
                scope.Dispose();
        }

    }

}
