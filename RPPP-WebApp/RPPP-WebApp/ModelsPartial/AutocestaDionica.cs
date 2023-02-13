namespace RPPP_WebApp.Models
{
    public class AutocestaDionica
    {
        public int Id { get; set; }
        public string Oznaka { get; set; }
        public string Naziv { get; set; }
        public string Koncesionar {  get; set; }
           
        public string UlaznaPostaja { get; set; }
        public string IzlaznaPostaja { get; set; }
        public int BrojTraka { get; set; }
        public bool ZaustavnaTraka { get; set; }
        public bool DozvolaTeretnimVozilima { get; set; }
        public bool OtvorenZaProlaz { get; set; }
        public int GodinaOtvaranja { get; set; }
        public int Duljina { get; set; }
        public string NazivDionice { get; set; }
        public int OgranicenjeBrzine { get; set; }
    }
}
