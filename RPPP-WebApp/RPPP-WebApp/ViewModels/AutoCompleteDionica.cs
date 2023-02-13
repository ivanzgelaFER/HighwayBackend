using System.Text.Json.Serialization;

namespace RPPP_WebApp.ViewModels

{
    public class AutoCompleteDionica
    {
        [JsonPropertyName("label")]
        public string Label { get; set; }
        [JsonPropertyName("id")]
        public int Id { get; set; }
        [JsonPropertyName("cijena")]
        public decimal Cijena { get; set; }
        public AutoCompleteDionica() { }
        public AutoCompleteDionica(int id, string label, decimal cijena)
        {
          Id = id;
          Label = label;
          Cijena = cijena;
        }
    }
}
