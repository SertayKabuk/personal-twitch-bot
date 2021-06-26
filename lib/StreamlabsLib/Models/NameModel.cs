using System.Text.Json.Serialization;

namespace StreamlabsLib.Models
{
    public class NameModel
    {
        [JsonPropertyName("name")]
        public string Name { get; set; }
    }
}
