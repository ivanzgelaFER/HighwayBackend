using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RPPP_WebApp.Models
{
    public class ViewAutocestaInfo
    {
        public int Id { get; set; }
        public string Oznaka { get; set; }
        public string Naziv { get; set; }
        public string Koncesionar { get; set; }

        //public int? IdPrethAutoceste { get; set; }

       // [NotMapped]
       // public int Position { get; set; } //Position in the result
    }
}
