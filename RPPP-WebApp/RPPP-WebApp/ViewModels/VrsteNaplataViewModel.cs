using RPPP_WebApp.Models;
namespace RPPP_WebApp.ViewModels
{
    public class VrsteNaplataViewModel
    {
        public IEnumerable<VrstaNaplate> VrsteNaplata { get; set; }
        public PagingInfo PagingInfo { get; set; }
    }
}