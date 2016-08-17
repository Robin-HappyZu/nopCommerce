using Autofac;
using Nop.Core.Configuration;
using Nop.Core.Infrastructure;
using Nop.Core.Infrastructure.DependencyManagement;
using Nop.Plugin.ExternalAuth.WeiXin.Core;

namespace Nop.Plugin.ExternalAuth.WeiXin
{
    public class DependencyRegistrar : IDependencyRegistrar
    {
        public void Register(ContainerBuilder builder, ITypeFinder typeFinder, NopConfig config)
        {
            builder.RegisterType<WeiXinProviderAuthorizer>()
                .As<IOAuthProviderWeiXinAuthorizer>()
                .InstancePerLifetimeScope();
        }

        public int Order => 1;
    }
}