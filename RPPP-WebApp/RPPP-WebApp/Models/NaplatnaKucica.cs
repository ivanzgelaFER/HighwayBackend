﻿// <auto-generated> This file has been auto generated by EF Core Power Tools. </auto-generated>
#nullable disable
using System;
using System.Collections.Generic;

namespace RPPP_WebApp.Models
{
    public partial class NaplatnaKucica
    {
        public NaplatnaKucica()
        {
            ProlazakVozila = new HashSet<ProlazakVozila>();
        }

        public int Id { get; set; }
        public int NaplatnaPostajaId { get; set; }
        public int VrstaNaplateId { get; set; }
        public bool Otvorena { get; set; }

        public virtual NaplatnaPostaja NaplatnaPostaja { get; set; }
        public virtual VrstaNaplate VrstaNaplate { get; set; }
        public virtual ICollection<ProlazakVozila> ProlazakVozila { get; set; }
    }
}