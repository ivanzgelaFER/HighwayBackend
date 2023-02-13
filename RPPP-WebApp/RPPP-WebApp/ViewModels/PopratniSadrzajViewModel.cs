using RPPP_WebApp.Models;

namespace RPPP_WebApp.ViewModels
{
    public class PopratniSadrzajViewModel
    {
        public int IdPopratnogSadrzaja { get; set; }
        public string NazivPopratnogSadrzaja { get; set; }
        public TimeSpan RadnimDanomOd { get; set; }
        public TimeSpan RadninDanomDo { get; set; }
        public TimeSpan VikendimaDo { get; set; }
        public TimeSpan VikendimaOd { get; set; }
        public byte[] SlikaPopratnogSadrzaja { get; set; }
        public int OdmoristeId { get; set; }
        public int VrstaSadrzajaId { get; set; }
        public string VrstaPopratnog { get; set; }

        public VrstaPopratnog VrstaPopratnogObj { get; set; }

        public string Odmoriste { get; set; }

    }
}
