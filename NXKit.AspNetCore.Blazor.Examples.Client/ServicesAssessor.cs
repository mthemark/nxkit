using Microsoft.Extensions.DependencyInjection;
using NXKit.Extensions.DependencyInjection;

namespace NXKit.AspNetCore.Blazor.Examples.Client
{
    public class ServicesAssessor : IServicesAssessor
    {
        public ServicesAssessor(IServiceCollection serviceCollection)
        {
            Services = serviceCollection;
        }
        public IServiceCollection Services { get; }
    }

}
