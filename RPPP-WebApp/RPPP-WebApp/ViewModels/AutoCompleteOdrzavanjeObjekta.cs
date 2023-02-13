using System.Text.Json.Serialization;

namespace RPPP_WebApp.ViewModels

{
    public class AutoCompleteOdrzavanjeObjekta
    {
        [JsonPropertyName("label")]
        public string Label { get; set; }
        [JsonPropertyName("id")]
        public int Id { get; set; }
        [JsonPropertyName("cijena")]
        public decimal Cijena { get; set; }
        [JsonPropertyName("vrstaId")]
        public int VrstaId { get; set; }
        [JsonPropertyName("vrsta")]
        public string Vrsta { get; set; }
        [JsonPropertyName("cestovniObjekt")]
        public int? CestovniObjektId { get; set; }
        [JsonPropertyName("radnimDanomOd")]
        public TimeSpan RadnimDanomOd { get; set; }
        [JsonPropertyName("radnimDanomDo")]
        public TimeSpan RadnimDanomDo { get; set; }
        [JsonPropertyName("vikendimaOd")]
        public TimeSpan? VikendimaOd { get; set; }
        [JsonPropertyName("vikendimaDo")]
        public TimeSpan? VikendimaDo { get; set; }
        [JsonPropertyName("brojLjudi")]
        public int? BrojLjudi { get; set; }
        [JsonPropertyName("predvidenoDana")]
        public int? PredvidenoDana { get; set; }
        public AutoCompleteOdrzavanjeObjekta() { }
        public AutoCompleteOdrzavanjeObjekta(int id, string label, int cijena, int vrstaId, string vrsta, int cestovniObjektId,
            TimeSpan radnimDanomOd, TimeSpan radnimDanomDo, TimeSpan vikendimaOd, TimeSpan vikendimaDo, int brojLjudi,
            int predvidenoDana)
        {
            Id = id;
            Label = label;
            Cijena = cijena;
            VrstaId = vrstaId;
            Vrsta = vrsta;
            CestovniObjektId = cestovniObjektId;
            RadnimDanomOd = radnimDanomOd;
            RadnimDanomDo = radnimDanomDo;
            VikendimaOd = vikendimaOd;
            VikendimaDo = vikendimaDo;
            BrojLjudi = brojLjudi;
            PredvidenoDana = predvidenoDana;
        }
    }
}
