using System.Text.Json.Serialization;

namespace RPPP_WebApp.ViewModels
{
    public class AutoCompleteAutocestaDionica
    {
        [JsonPropertyName("label")]
        public string Label { get; set; }
        
        [JsonPropertyName("id")]
        public int Id { get; set; }
        
        [JsonPropertyName("ulaznaPostaja")]
        public int UlaznaPostajaId { get; set; }
        
        [JsonPropertyName("izlaznaPostaja")]
        public int IzlaznaPostajaId { get; set; }
       
        [JsonPropertyName("brojTraka")]
        public int BrojTraka { get; set; }
        
        [JsonPropertyName("zaustavnaTraka")]
        public bool ZaustavnaTraka { get; set; }

        [JsonPropertyName("dozvolaTeretnimVozilima")]
        public bool DozvolaTeretnimVozilima { get; set; }

        [JsonPropertyName("otvorenZaProlaz")]
        public bool OtvorenZaProlaz { get; set; }

        [JsonPropertyName("godinaOtvaranja")]
        public int GodinaOtvaranja { get; set; }
       
        [JsonPropertyName("duljina")]
        public int Duljina { get; set; }
        
        [JsonPropertyName("naziv")]
        public string Naziv { get; set; }
        
        [JsonPropertyName("ogranicenjeBrzine")]
        public int OgranicenjeBrzine { get; set; }
        
        [JsonPropertyName("autocesta")]
        public int AutocestaId { get; set; }
        
        public AutoCompleteAutocestaDionica() { }
        public AutoCompleteAutocestaDionica(int id, string label, int ulaznaPostajaId, int izlaznaPostajaId,
                                           int brojTraka, bool zaustavnaTraka, bool dozvolaTeretnimVozilima,
                                           bool otvorenZaProlaz, int godinaOtvaranja, int duljina, string naziv,
                                           int ogranicenjeBrzine, int autocestaId)
        {
            Id = id;
            Label = label;
            UlaznaPostajaId = ulaznaPostajaId;
            IzlaznaPostajaId = izlaznaPostajaId;
            BrojTraka = brojTraka;
            ZaustavnaTraka = zaustavnaTraka;
            DozvolaTeretnimVozilima = dozvolaTeretnimVozilima;
            OtvorenZaProlaz = otvorenZaProlaz;
            GodinaOtvaranja = godinaOtvaranja;
            Duljina = duljina;
            Naziv = naziv;
            OgranicenjeBrzine = ogranicenjeBrzine;
            AutocestaId = autocestaId;
        }
    }
}
