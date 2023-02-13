namespace RPPP_WebApp.Models
{
    public class ViewCestovniObjekt
    {
        public int Id { get; set; }
        public string Naziv { get; set; }
        public int DionicaId { get; set; }
        public string TipObjekta { get; set; }
        public int OgranicenjeBrzine { get; set; }
        public int BrojPrometnihTraka { get; set; } 
        public int DuljinaObjekta { get; set; }
        public bool ZaustavniTrak { get; set; }
        public bool DozvolaTeretnimVozilima { get; set; }
        public string Zanimljivost { get; set; }    
        public int GodinaIzgradnje { get; set; }
        public bool PjesackaStaza { get; set; }
        public bool NaplataPrijelaza { get; set; }
    }
}
