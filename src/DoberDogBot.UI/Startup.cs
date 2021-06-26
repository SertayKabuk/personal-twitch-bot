using Autofac;
using DoberDogBot.Application.AutofacModules;
using DoberDogBot.Application.Queries;
using DoberDogBot.Infrastructure.AppDb;
using DoberDogBot.UI.Data;
using DoberDogBot.UI.Hubs;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Server;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.ResponseCompression;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace DoberDogBot.UI
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }
        public ILifetimeScope AutofacContainer { get; private set; }

        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddDbContextFactory<AppDbContext>(options =>
            {
                // Heroku provides PostgreSQL connection URL via env variable
                var connUrl = Configuration.GetValue<string>("DATABASE_URL");

                // Parse connection URL to connection string for Npgsql
                connUrl = connUrl.Replace("postgres://", string.Empty);

                var pgUserPass = connUrl.Split("@")[0];
                var pgHostPortDb = connUrl.Split("@")[1];
                var pgHostPort = pgHostPortDb.Split("/")[0];

                var pgDb = pgHostPortDb.Split("/")[1];
                var pgUser = pgUserPass.Split(":")[0];
                var pgPass = pgUserPass.Split(":")[1];
                var pgHost = pgHostPort.Split(":")[0];
                var pgPort = pgHostPort.Split(":")[1];

                string connStr = $"Server={pgHost};Port={pgPort};User Id={pgUser};Password={pgPass};Database={pgDb};Sslmode=Require;Trust Server Certificate=true;";

                options.UseNpgsql(connStr);
            });

            services.AddScoped(p => p.GetRequiredService<IDbContextFactory<AppDbContext>>().CreateDbContext());

            services.AddDefaultIdentity<IdentityUser>().AddEntityFrameworkStores<AppDbContext>().AddDefaultTokenProviders();

            services.AddScoped<AuthenticationStateProvider, ServerAuthenticationStateProvider>();
            services.AddSingleton<ISubscriberQueries, SubscriberQueries>();

            services.AddRazorPages();
            services.AddServerSideBlazor();
            services.AddSingleton<TwitchService>();
            services.AddResponseCompression(opts =>
            {
                opts.MimeTypes = ResponseCompressionDefaults.MimeTypes.Concat(
                    new[] { "application/octet-stream" });
            });

            services.AddAuthentication()
             .AddTwitch(options =>
             {
                 options.ClientId = Configuration.GetValue<string>("TWITCH_CLIENTID");
                 options.ClientSecret = Configuration.GetValue<string>("TWITCH_CLIENTSECRET");
                 options.Scope.Add("bits:read");
                 options.Scope.Add("channel:read:subscriptions");
                 options.Scope.Add("channel:read:redemptions");
                 options.Scope.Add("chat:read");
                 options.Scope.Add("chat:edit");
                 options.SaveTokens = true;

                 options.Events.OnCreatingTicket = ctx =>
                 {
                     List<AuthenticationToken> tokens = ctx.Properties.GetTokens().ToList();

                     foreach (var item in tokens)
                     {
                         ctx.Identity.AddClaim(new Claim("urn:twitch:" + item.Name, item.Value));
                     }

                     tokens.Add(new AuthenticationToken()
                     {
                         Name = "TwitchTicketCreated",
                         Value = DateTime.UtcNow.ToString()
                     });

                     ctx.Properties.StoreTokens(tokens);

                     return Task.CompletedTask;
                 };
             }).AddStreamlabs(options =>
             {
                 options.ClientId = Configuration.GetValue<string>("STREAMLABS_CLIENTID");
                 options.ClientSecret = Configuration.GetValue<string>("STREAMLABS_CLIENTSECRET");
                 
                 options.Scope.Add("donations.read");
                 options.Scope.Add("socket.token");
                 
                 options.SaveTokens = true;

                 options.Events.OnCreatingTicket = ctx =>
                 {
                     List<AuthenticationToken> tokens = ctx.Properties.GetTokens().ToList();

                     foreach (var item in tokens)
                     {
                         ctx.Identity.AddClaim(new Claim("urn:streamlabs:" + item.Name, item.Value));
                     }

                     tokens.Add(new AuthenticationToken()
                     {
                         Name = "StreamlabsTicketCreated",
                         Value = DateTime.UtcNow.ToString()
                     });

                     ctx.Properties.StoreTokens(tokens);

                     return Task.CompletedTask;
                 };
             });

            services.AddHttpClient();
            services.AddSingleton<TokenProvider>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            app.UseResponseCompression();

            app.UseForwardedHeaders(new ForwardedHeadersOptions
            {
                ForwardedHeaders = ForwardedHeaders.XForwardedProto
            });

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            //app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
                endpoints.MapBlazorHub();
                endpoints.MapHub<TwitchSubHub>("/twitchsubhub");
                endpoints.MapFallbackToPage("/_Host");
            });
        }

        public void ConfigureContainer(ContainerBuilder builder)
        {
            builder.RegisterModule(new MediatorModule());
        }
    }
}
