﻿// <auto-generated> This file has been auto generated by EF Core Power Tools. </auto-generated>
#nullable disable
using System;
using System.Collections.Generic;

namespace RPPP_WebApp.Models
{
    public partial class Incident
    {
        public Incident()
        {
            Reakcija = new HashSet<Reakcija>();
        }

        public int Id { get; set; }
        public string Opis { get; set; }
        public DateTime? Datum { get; set; }
        public string MeteoroloskiUvjeti { get; set; }
        public string StanjeNaCesti { get; set; }
        public string Prohodnost { get; set; }
        public int DionicaId { get; set; }
        public int VrstaIncidentaId { get; set; }

        public virtual Dionica Dionica { get; set; }
        public virtual VrstaIncidenta VrstaIncidenta { get; set; }
        public virtual ICollection<Reakcija> Reakcija { get; set; }
    }
}