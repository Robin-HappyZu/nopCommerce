using Autofac;
using Nop.Core.Configuration;
using Nop.Core.Infrastructure;
using Nop.Core.Infrastructure.DependencyManagement;
using Nop.Plugin.ExternalAuth.WeixinConnect.Core;

namespace Nop.Plugin.ExternalAuth.WeixinConnect
{
    public class DependencyRegistrar : IDependencyRegistrar
    {
        public void Register(ContainerBuilder builder, ITypeFinder typeFinder, NopConfig config)
        {
            builder.RegisterType<WeixinConnectProviderAuthorizer>()
                .As<IOAuthProviderWeixinConnectAuthorizer>()
                .InstancePerLifetimeScope();
        }

        public int Order => 1;
    }
}