using System.Text.Json.Serialization;

namespace RPPP_WebApp.ViewModels
{
    public class AutoCompleteKoncesionar
    {
        [JsonPropertyName("label")]
        public string Label { get; set; }
        [JsonPropertyName("id")]
        public int Id { get; set; }

        public AutoCompleteKoncesionar() { }
        public AutoCompleteKoncesionar(int id, string label)
        {
            Id = id;
            Label = label;
        }
    }
}
