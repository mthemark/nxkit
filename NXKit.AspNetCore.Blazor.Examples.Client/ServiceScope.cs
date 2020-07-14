using Microsoft.Extensions.DependencyInjection;
using NXKit.Extensions.DependencyInjection;
using System;

namespace NXKit.AspNetCore.Blazor.Examples.Client
{

    public class ServiceScope : IServiceScope
    {
        public IServicesAssessor ServicesAssessor { get; }
        public IServiceProvider ServiceProvider { get; }

        public IServiceCollection Services { get => ServicesAssessor.Services; }

        public ServiceScope(IServiceProvider serviceProvider, IServicesAssessor servicesAssessor)
        {
            ServiceProvider = serviceProvider;
            ServicesAssessor = servicesAssessor;
        }

        public void Dispose()
        {
            //throw new NotImplementedException();
        }
    }

}
