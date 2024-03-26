using System.Text.Json.Serialization;

namespace DemoAuth.Models
{
    public record Select2Option
    {
        [JsonPropertyName("id")]
        public int Id { get; set; }
        [JsonPropertyName("text")]
        public string Text { get; set; } = string.Empty;
    }

    public record Pagination
    {
        [JsonPropertyName("more")]
        public bool More { get; set; }
    }

    public record Select2Response
    {
        [JsonPropertyName("results")]
        public List<Select2Option> Results { get; set; } = [];
        [JsonPropertyName("pagination")]
        public Pagination Pagination { get; set; } = new() { More = false };
    }

}