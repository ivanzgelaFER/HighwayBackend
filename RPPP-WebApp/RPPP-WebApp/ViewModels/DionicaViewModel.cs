using RPPP_WebApp.Models;

namespace RPPP_WebApp.ViewModels
{
    public class DionicaViewModel
    {
        public int Id { get; set; }
        public string UlaznaPostaja { get; set; }

        public NaplatnaPostaja UlaznaPostajaObj { get; set; }
        public string IzlaznaPostaja { get; set; }

        public NaplatnaPostaja IzlaznaPostajaObj { get; set; }
        public int BrojTraka { get; set; }
        public bool ZaustavnaTraka { get; set; }
        public bool DozvolaTeretnimVozilima { get; set; }
        public bool OtvorenZaProlaz { get; set; }
        public int GodinaOtvaranja { get; set; }
        public int Duljina { get; set; }
        public string Naziv { get; set; }
        public int OgranicenjeBrzine { get; set; }
        public string Autocesta { get; set; }
        public Autocesta AutocestaObj { get; set; }
    }
}
