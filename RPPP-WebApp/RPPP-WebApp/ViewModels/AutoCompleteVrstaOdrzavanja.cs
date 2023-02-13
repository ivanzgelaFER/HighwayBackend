using System.Text.Json.Serialization;

namespace RPPP_WebApp.ViewModels

{
    public class AutoCompleteVrstaOdrzavanja
    {
        [JsonPropertyName("label")]
        public string Label { get; set; }
        [JsonPropertyName("id")]
        public int Id { get; set; }

        public AutoCompleteVrstaOdrzavanja() { }
        public AutoCompleteVrstaOdrzavanja(int id, string label)
        {
          Id = id;
          Label = label;
        }
    }
}
