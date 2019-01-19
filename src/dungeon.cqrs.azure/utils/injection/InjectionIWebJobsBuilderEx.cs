using System;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.DependencyInjection;

namespace dungeon.cqrs.azure.utils.injection
{
    public static class InjectionIWebJobsBuilderEx
    {
        
        public static IWebJobsBuilder SetServiceProvider(this IWebJobsBuilder builder, IServiceCollection serviceCollection)
        {
            return builder.SetServiceProvider(serviceCollection.BuildServiceProvider());
        }

        public static IWebJobsBuilder SetServiceProvider(this IWebJobsBuilder builder, Action<IServiceCollection> onRegister)
        {
            var serviceCollection = new ServiceCollection();
            onRegister(serviceCollection);
            return builder.SetServiceProvider(serviceCollection.BuildServiceProvider());
        }

        public static IWebJobsBuilder SetServiceProvider(this IWebJobsBuilder builder, IServiceProvider provider)
        {
            builder.AddExtension(new InjectionExtensionConfigProvider(provider));
            return builder;
        }
    }
}