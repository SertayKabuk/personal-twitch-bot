using Autofac;
using Autofac.Extensions.DependencyInjection;
using DoberDogBot.Application.AutofacModules;
using DoberDogBot.Application.Commands;
using DoberDogBot.Application.Extensions;
using DoberDogBot.Application.Models;
using DoberDogBot.Application.Queries;
using DoberDogBot.Domain.AggregatesModel.BitAggregate;
using DoberDogBot.Domain.AggregatesModel.BotAggregate;
using DoberDogBot.Domain.AggregatesModel.SubscriberAggregate;
using DoberDogBot.Infrastructure.AppDb;
using DoberDogBot.Infrastructure.AppDb.Repositories;
using DoberDogBot.Infrastructure.BotDb.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Serilog;

namespace DoberDogBot.Worker
{
    public class Program
    {
        public static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureAppConfiguration((context, builder) =>
                {
                    builder.LoadEnvVariables();
                })
                .UseServiceProviderFactory(new AutofacServiceProviderFactory())
                .ConfigureContainer<ContainerBuilder>(builder =>
                {
                    builder.RegisterModule<MediatorModule>();
                })
                .ConfigureServices((hostContext, services) =>
                {
                    services.PostConfigure<Application.Models.BotOption>(options =>
                    {
                        options.AutoSleepEnabled = hostContext.Configuration.GetValue<bool>("AutoSleepEnabled");
                        options.PokeChattersEnabled = hostContext.Configuration.GetValue<bool>("PokeChattersEnabled");
                        options.MinSleepIntervalInMinutes = hostContext.Configuration.GetValue<int>("MinSleepIntervalInMinutes");
                        options.MaxSleepIntervalInMinutes = hostContext.Configuration.GetValue<int>("MaxSleepIntervalInMinutes");
                        options.MinSleepDurationInMinutes = hostContext.Configuration.GetValue<int>("MinSleepDurationInMinutes");
                        options.MaxSleepDurationInMinutes = hostContext.Configuration.GetValue<int>("MaxSleepDurationInMinutes");
                        options.MinChattersIntervalInMinutes = hostContext.Configuration.GetValue<int>("MinChattersIntervalInMinutes");
                        options.MaxChattersIntervalInMinutes = hostContext.Configuration.GetValue<int>("MaxChattersIntervalInMinutes");
                    });

                    services.PostConfigure<TwitchOptions>(options =>
                    {
                        options.BotName = hostContext.Configuration.GetValue<string>("BotName");
                        options.BotChannelId = hostContext.Configuration.GetValue<string>("BotChannelId");
                        options.ClientId = hostContext.Configuration.GetValue<string>("ClientId");
                        options.ClientSecret = hostContext.Configuration.GetValue<string>("ClientSecret");
                        options.Channel = hostContext.Configuration.GetValue<string>("Channel");
                        options.ChannelId = hostContext.Configuration.GetValue<string>("ChannelId");
                        options.DonationKeyword = hostContext.Configuration.GetValue<string>("DonationKeyword");
                        options.MessagesAllowedInPeriod = hostContext.Configuration.GetValue<int>("MessagesAllowedInPeriod");
                        options.ThrottlingPeriodInSeconds = hostContext.Configuration.GetValue<int>("ThrottlingPeriodInSeconds");
                    });

                    services.Configure<Application.Models.BotOption>(hostContext.Configuration.GetSection(Application.Models.BotOption.OptionsName));
                    services.Configure<TwitchOptions>(hostContext.Configuration.GetSection(TwitchOptions.OptionsName));

                    services.AddLogging();
                    services.AddAutoMapper(typeof(BotOnJoinedCommandProfile).Assembly);

                    services.AddScoped<IMessageService, MessageService>();
                    services.AddScoped<IBotRepository, BotRepository>();
                    services.AddScoped<ISubscriberRepository, SubscriberRepository>();
                    services.AddScoped<IBitRepository, BitRepository>();
                    services.AddScoped<IStreamerRepository, StreamerRepository>();
                    services.AddScoped<IStreamerQueries, StreamerQueries>();

                    services.AddHttpClient();

                    services.AddDbContext<AppDbContext>((serviceProvider, options) =>
                    {
                        // Heroku provides PostgreSQL connection URL via env variable
                        var connUrl = hostContext.Configuration.GetValue<string>("DATABASE_URL");

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

                    services.AddHostedService<Worker>();
                })
                .UseSerilog((builderContext, config) =>
                {
                    config.ReadFrom.Configuration(builderContext.Configuration)
                    .Enrich.FromLogContext()
                    .Enrich.WithProperty("ApplicationName", typeof(Program).Assembly.GetName().Name)
                    .Enrich.WithProperty("Environment", builderContext.HostingEnvironment);
                });
    }
}