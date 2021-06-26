using Microsoft.AspNetCore.Hosting;

[assembly: HostingStartup(typeof(DoberDogBot.UI.Areas.Identity.IdentityHostingStartup))]
namespace DoberDogBot.UI.Areas.Identity
{
    public class IdentityHostingStartup : IHostingStartup
    {
        public void Configure(IWebHostBuilder builder)
        {
            builder.ConfigureServices((context, services) => {
            });
        }
    }
}