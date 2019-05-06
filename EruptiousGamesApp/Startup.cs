using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(EruptiousGamesApp.Startup))]
namespace EruptiousGamesApp
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
