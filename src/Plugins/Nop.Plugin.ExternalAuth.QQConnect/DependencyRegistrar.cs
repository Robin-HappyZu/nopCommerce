using Autofac;
using Nop.Core.Configuration;
using Nop.Core.Infrastructure;
using Nop.Core.Infrastructure.DependencyManagement;
using Nop.Plugin.ExternalAuth.QQConnect.Core;
using Nop.Services.Authentication;
using Nop.Services.Authentication.External;
using Nop.Services.Customers;


namespace Nop.Plugin.ExternalAuth.QQConnect
{
    public class DependencyRegistrar : IDependencyRegistrar
    {
        public virtual void Register(ContainerBuilder builder, ITypeFinder typeFinde, NopConfig configr)
        {
            builder.RegisterType<QQConnectProviderAuthorizer>().As<IOAuthProviderQQConnectAuthorizer>().InstancePerLifetimeScope();
        }

        public int Order
        {
            get { return 1; }
        }
    }
}
