using System.Text.Json.Serialization;

namespace RPPP_WebApp.ViewModels
{
    public class AutoCompleteVrstaPopratnog
    {
        [JsonPropertyName("label")]
        public string Label { get; set; }
        [JsonPropertyName("id")]
        public int Id { get; set; }

        public AutoCompleteVrstaPopratnog() { }
        public AutoCompleteVrstaPopratnog(int id, string label)
        {
            Id = id;
            Label = label;
        }

    }
}
