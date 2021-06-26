using System.Text.Json.Serialization;

namespace StreamlabsLib.Models
{
    public class DonationModel
    {
        [JsonPropertyName("id")]
        public long Id { get; set; }
        [JsonPropertyName("name")]
        public string Name { get; set; }
        [JsonPropertyName("amount")]
        public decimal Amount { get; set; }
        [JsonPropertyName("donation_currency")]
        public string DonationCurrency { get; set; }
        [JsonPropertyName("message")]
        public string Message { get; set; }
        [JsonPropertyName("from")]
        public string From { get; set; }
        [JsonPropertyName("to")]
        public NameModel To { get; set; }
    }
}
