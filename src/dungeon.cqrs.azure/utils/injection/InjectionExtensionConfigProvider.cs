using System;
using System.Collections.Generic;
using Microsoft.Azure.WebJobs.Host.Config;

namespace dungeon.cqrs.azure.utils.injection
{

    public class InjectionExtensionConfigProvider : IExtensionConfigProvider
    {
        private readonly IServiceProvider serviceProvider;

        public InjectionExtensionConfigProvider(IServiceProvider serviceProvider)
        {
            this.serviceProvider = serviceProvider;
        }

        public void Initialize(ExtensionConfigContext context)
        {
            context.AddBindingRule<DungeonInjectAttribute>().BindToInput(atr => serviceProvider);
        }
    }
}
