namespace RPPP_WebApp.ViewModels {
    public class IncidentViewModel {
        public int Id { get; set; }
        public string Opis { get; set; }
        public DateTime? Datum { get; set; }
        public string MeteoroloskiUvjeti { get; set; }
        public string StanjeNaCesti { get; set; }
        public string Prohodnost { get; set; }
        public string Dionica { get; set; }
        public string VrstaIncidenta { get; set; }
        public string NaziviReakcija { get; set; }
        public IEnumerable<ReakcijaViewModel> Reakcije { get; set; }
        public IncidentViewModel() {
            this.Reakcije = new List<ReakcijaViewModel>();
        }
    }
}
