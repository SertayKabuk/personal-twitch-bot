using Microsoft.Extensions.Logging;
using StreamlabsLib.Events;
using StreamlabsLib.Models;
using StreamlabsLib.StreamlabsEvents;
using System;
using System.Collections.Concurrent;
using System.Text.Json;

namespace StreamlabsLib
{
    public class StreamlabsClient
    {
        private ConcurrentDictionary<long, DonationModel> donationList;
        private readonly StreamlabsSocketClient streamlabsSocketClient;
        private readonly ILogger logger;

        public event EventHandler<DonationEventArgs> OnDonationReceived;

        public StreamlabsClient(ILogger logger)
        {
            this.streamlabsSocketClient = new StreamlabsSocketClient(logger);
            this.logger = logger;
        }

        public void Connect(string socketToken)
        {
            donationList = new();
            streamlabsSocketClient.OnMessage += MessageReceived;
            streamlabsSocketClient.Connect(socketToken);
        }

        private void MessageReceived(object sender, OnMessageEventArgs @event)
        {
            var message = JsonDocument.Parse(@event.Payload);

            string eventType = "";

            if (message.RootElement.ValueKind == JsonValueKind.Array)
            {
                var arrayLength = message.RootElement.GetArrayLength();

                if (arrayLength == 2)
                {
                    for (int i = 0; i < arrayLength; i++)
                    {
                        var jElement = message.RootElement[i];

                        if (i == 0)
                            eventType = jElement.ToString();
                        else if (eventType == "donations")
                        {
                            for (int d = 0; d < jElement.GetArrayLength(); d++)
                            {
                                var donation = JsonSerializer.Deserialize<DonationModel>(jElement[d].ToString());

                                if (!donationList.ContainsKey(donation.Id))
                                {
                                    donationList.TryAdd(donation.Id, donation);
                                    OnDonationReceived?.Invoke(this, new DonationEventArgs(donation));
                                }
                            }
                        }
                    }
                }
            }
        }
    }
}
