using Autofac;
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
        public virtual void Register(ContainerBuilder builder, ITypeFinder typeFinder)
        {
            builder.RegisterType<QQConnectProviderAuthorizer>().As<IOAuthProviderQQConnectAuthorizer>().InstancePerLifetimeScope();

            builder.RegisterType<Nop.Plugin.ExternalAuth.QQConnect.Authentication.ExternalAuthorizer>().As<IExternalAuthorizer>().InstancePerLifetimeScope();
            builder.RegisterType<Nop.Plugin.ExternalAuth.QQConnect.Services.CustomerRegistrationService>().As<ICustomerRegistrationService>().InstancePerLifetimeScope();
            builder.RegisterType<Nop.Plugin.ExternalAuth.QQConnect.Authentication.External.OpenAuthenticationService>().As<IOpenAuthenticationService>().InstancePerLifetimeScope();
        }

        public int Order
        {
            get { return 1; }
        }
    }
}
