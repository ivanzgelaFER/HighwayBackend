﻿// <auto-generated> This file has been auto generated by EF Core Power Tools. </auto-generated>
#nullable disable
using System;
using System.Collections.Generic;

namespace RPPP_WebApp.Models
{
    public partial class CestovniObjekt
    {
        public CestovniObjekt()
        {
            OdrzavanjeObjekta = new HashSet<OdrzavanjeObjekta>();
        }

        public int Id { get; set; }
        public string Naziv { get; set; }
        public int DionicaId { get; set; }
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

        public virtual Dionica Dionica { get; set; }
        public virtual ICollection<OdrzavanjeObjekta> OdrzavanjeObjekta { get; set; }
    }
}