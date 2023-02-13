using System.Text.Json.Serialization;

namespace RPPP_WebApp.ViewModels
{
    public class AutoCompletePopratniSadrzaj
    {
        [JsonPropertyName("label")]
        public string Label { get; set; }
        [JsonPropertyName("id")]
        public int Id { get; set; }
        public TimeSpan RadnimDanomOd { get; set; }
        public TimeSpan RadninDanomDo { get; set; }
        public TimeSpan VikendimaDo { get; set; }
        public TimeSpan VikendimaOd { get; set; }
        public int OdmoristeId { get; set; }
        public int VrstaSadrzajaId { get; set; }
        public string VrstaPopratnog { get; set; }

        public string Odmoriste { get; set; }

        public AutoCompletePopratniSadrzaj() { }
        public AutoCompletePopratniSadrzaj(int id, string label, TimeSpan radnimDanomOd, TimeSpan radninDanomDo, TimeSpan vikendimaDo, TimeSpan vikendimaOd, int odmoristeId, int vrstaSadrzajaId, string vrstaPopratnog, string odmoriste)
        {
            Id = id;
            Label = label;
            RadnimDanomOd = radnimDanomOd;
            RadninDanomDo = radninDanomDo;
            VikendimaDo = vikendimaDo;
            VikendimaOd = vikendimaOd;
            OdmoristeId = odmoristeId;
            VrstaSadrzajaId = vrstaSadrzajaId;
            VrstaPopratnog = vrstaPopratnog;
            Odmoriste = odmoriste; 
        }
    }
}
