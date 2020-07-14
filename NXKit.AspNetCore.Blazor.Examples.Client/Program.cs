using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.Extensions.DependencyInjection;
using NXKit.Extensions.DependencyInjection;

namespace NXKit.AspNetCore.Blazor.Examples.Client
{

    public static class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebAssemblyHostBuilder.CreateDefault(args);
            //.UseServiceProviderFactory(new AutofacServiceProviderFactory(BuildContainer))
            //builder.ConfigureContainer(new AutofacServiceProviderFactory(BuildContainer));

            builder.Services.AddSingleton<IServicesAssessor, ServicesAssessor>(a => new ServicesAssessor(builder.Services));
            builder.Services.AddSingleton<IServiceScope, ServiceScope>();
            builder.Services.AddNXKitAssemblies(new[]
            {
                typeof(NXKit.DocumentEnvironment).Assembly,
                typeof(NXKit.AspNetCore.Components._Imports).Assembly,
                typeof(NXKit.DOM.DOMException).Assembly,
                typeof(NXKit.DOMEvents.ActionEventListener).Assembly,
                typeof(NXKit.NXInclude.Include).Assembly,
                typeof(NXKit.Scripting.Console).Assembly,
                typeof(NXKit.XInclude.Include).Assembly,
                typeof(NXKit.XMLEvents.Events).Assembly,
                typeof(NXKit.XPath.DefaultXsltContextFunctionProvider).Assembly,
                typeof(NXKit.XPath2.Functions.Flags).Assembly,
                typeof(NXKit.XForms.VarProperties).Assembly,
                typeof(NXKit.AspNetCore.Components.XForms.XFormsComponentTypeProvider).Assembly,
                typeof(NXKit.XForms.Layout.Paragraph).Assembly,
                typeof(NXKit.AspNetCore.Components.XForms.Layout.XFormsLayoutComponentTypeProvider).Assembly,
                typeof(NXKit.XForms.Examples.ExampleIOTransport).Assembly,
                typeof(NXKit.XHtml.Div).Assembly,
                typeof(NXKit.AspNetCore.Components.XHtml._Imports).Assembly,
            });

            builder.RootComponents.Add<App>("app");
            builder
                .Build()
                .RunAsync();
        }

    }

}
