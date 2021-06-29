using DoberDogBot.Application.Queries;
using DoberDogBot.Domain.AggregatesModel.BitAggregate;
using DoberDogBot.Domain.AggregatesModel.SubscriberAggregate;
using DoberDogBot.Domain.AggregatesModel.TipAggregate;
using DoberDogBot.UI.Models;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;

namespace DoberDogBot.UI.Data
{
    public class TwitchService
    {
        private readonly HttpClient _http;
        private readonly IServiceScopeFactory _scopeFactory;
        private readonly ILogger<TwitchService> _logger;
        private readonly string _channelId;
        private readonly string _channel;

        public TwitchService(IHttpClientFactory clientFactory, IServiceScopeFactory scopeFactory, ILogger<TwitchService> logger,
            IConfiguration configuration)
        {
            _http = clientFactory.CreateClient();
            _scopeFactory = scopeFactory;
            _logger = logger;

            _channelId = configuration.GetValue<string>("BROADCASTER_CHANNELID");
            _channel = configuration.GetValue<string>("BROADCASTER_CHANNEL");
        }

        public async Task<int> GetLastSessionSubCount(string channelId)
        {
            _logger.LogInformation("GetLastSessionSubCount: {ChannelId}", channelId);

            using var scope = _scopeFactory.CreateScope();

            var subscriberQueries = scope.ServiceProvider.GetRequiredService<ISubscriberQueries>();

            var subCount = await subscriberQueries.GetActiveSessionSubCount(channelId);

            return subCount;
        }

        public Task<DonationBaseModel> GetAllSupportDonation()
        {
            DonationBaseModel donationBase = new();
            donationBase.TotalDonation = new();
            donationBase.Donations = new();

            return Task.FromResult(donationBase);
        }

        public Task<string> GetChannelId(AuthenticationState authenticationState)
        {
            return Task.FromResult(string.Empty);
        }

        public Task<Subscriber[]> GetAllSubscribers(string channelId)
        {
            return Task.FromResult(Array.Empty<Subscriber>());
        }

        public Task<Bit[]> GetAllBits(string channelId)
        {
            return Task.FromResult(Array.Empty<Bit>());
        }

        public Task<Tip[]> GetAllTips(AuthenticationState authenticationState)
        {
            return Task.FromResult(Array.Empty<Tip>());
        }

        public async Task<TipModel[]> SyncTipsFromExternalApi(AuthenticationState authenticationState)
        {
            List<TipModel> tips = new();

            try
            {
                using var scope = _scopeFactory.CreateScope();

                var streamlabsToken = authenticationState.User.Claims.SingleOrDefault(x => x.Type == "urn:streamlabs:access_token");

                if (!string.IsNullOrEmpty(streamlabsToken.Value))
                {
                    _http.DefaultRequestHeaders.Add("Authorization", "Bearer " + streamlabsToken);

                    var tipsResponse = await _http.GetAsync("https://streamlabs.com/api/v1.0/donations");

                    if (tipsResponse.IsSuccessStatusCode)
                    {
                        var tipsJson = await tipsResponse.Content.ReadAsStringAsync();
                        tips = JsonSerializer.Deserialize<List<TipModel>>(tipsJson);
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
            }

            return (tips ?? new()).ToArray();
        }
    }
}
