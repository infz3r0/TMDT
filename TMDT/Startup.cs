using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(TMDT.Startup))]
namespace TMDT
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
