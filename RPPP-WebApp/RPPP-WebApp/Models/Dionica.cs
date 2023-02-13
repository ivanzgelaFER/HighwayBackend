﻿// <auto-generated> This file has been auto generated by EF Core Power Tools. </auto-generated>
#nullable disable
using System;
using System.Collections.Generic;

namespace RPPP_WebApp.Models
{
    public partial class Dionica
    {
        public Dionica()
        {
            CestovniObjekt = new HashSet<CestovniObjekt>();
            Incident = new HashSet<Incident>();
            Kamera = new HashSet<Kamera>();
            Odmoriste = new HashSet<Odmoriste>();
        }

        public int Id { get; set; }
        public int UlaznaPostajaId { get; set; }
        public int IzlaznaPostajaId { get; set; }
        public int BrojTraka { get; set; }
        public bool ZaustavnaTraka { get; set; }
        public bool DozvolaTeretnimVozilima { get; set; }
        public bool OtvorenZaProlaz { get; set; }
        public int GodinaOtvaranja { get; set; }
        public int Duljina { get; set; }
        public string Naziv { get; set; }
        public int OgranicenjeBrzine { get; set; }
        public int AutocestaId { get; set; }

        public virtual Autocesta Autocesta { get; set; }
        public virtual NaplatnaPostaja IzlaznaPostaja { get; set; }
        public virtual NaplatnaPostaja UlaznaPostaja { get; set; }
        public virtual ICollection<CestovniObjekt> CestovniObjekt { get; set; }
        public virtual ICollection<Incident> Incident { get; set; }
        public virtual ICollection<Kamera> Kamera { get; set; }
        public virtual ICollection<Odmoriste> Odmoriste { get; set; }
    }
}