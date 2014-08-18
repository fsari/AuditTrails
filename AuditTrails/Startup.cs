using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(AuditTrails.Startup))]
namespace AuditTrails
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
