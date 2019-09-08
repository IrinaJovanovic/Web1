using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(SistemZaRezervacijeApartmana.Startup))]
namespace SistemZaRezervacijeApartmana
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
