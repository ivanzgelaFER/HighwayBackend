using RPPP_WebApp.Models;
namespace RPPP_WebApp.ViewModels
{
    public class VrsteOdrzavanjaViewModel
    {
        public IEnumerable<VrstaOdrzavanja> VrsteOdrzavanja { get; set; }
        public PagingInfo PagingInfo { get; set; }
    }
}
