using System.Text.Json.Serialization;

namespace DoberDogBot.UI.Models
{
    public class TipModel
    {
        [JsonPropertyName("donation_id")]
        public string DonationId { get; set; }
        [JsonPropertyName("created_at")]
        public string CreatedAt { get; set; }
        [JsonPropertyName("currency")]
        public string Currency { get; set; }
        [JsonPropertyName("amount")]
        public string Amount { get; set; }
        [JsonPropertyName("name")]
        public string Name { get; set; }
        [JsonPropertyName("message")] 
        public string Message { get; set; }
    }
}
