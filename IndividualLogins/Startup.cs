using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(IndividualLogins.Startup))]
namespace IndividualLogins
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
