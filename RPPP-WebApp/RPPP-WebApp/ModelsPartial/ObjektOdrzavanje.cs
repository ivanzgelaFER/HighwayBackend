using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace RPPP_WebApp.Models
{
  public class ObjektOdrzavanje
  {
    public int Id { get; set; }
    public string Naziv { get; set; }
    public string TipObjekta { get; set; }
    public int OgranicenjeBrzine { get; set; }
    public int BrojPrometnihTraka { get; set; }
    public int DuljinaObjekta { get; set; }
    public bool ZaustavniTrak { get; set; }
    public bool? DozvolaTeretnimVozilima { get; set; }
    public string Zanimljivost { get; set; }
    public int? GodinaIzgradnje { get; set; }
    public bool? PjesackaStaza { get; set; }
    public bool? NaplataPrijelaza { get; set; }
    public string ImeFirme { get; set; }
    public TimeSpan RadnimDanomOd { get; set; }
    public TimeSpan RadnimDanomDo { get; set; }
    public TimeSpan? VikendimaOd { get; set; }
    public TimeSpan? VikendimaDo { get; set; }
    public int? BrojLjudi { get; set; }
    public int Cijena { get; set; }
    public int? PredvidenoDana { get; set; }
    }
}
