namespace NXKit.Extensions.DependencyInjection
{
    public interface IServicesAssessor
    {
        Microsoft.Extensions.DependencyInjection.IServiceCollection Services { get; }
    }
}
