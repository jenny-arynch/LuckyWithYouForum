using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(LuckyWithYouForum.Startup))]
namespace LuckyWithYouForum
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
