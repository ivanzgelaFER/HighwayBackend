using RPPP_WebApp.Models;

namespace RPPP_WebApp.ViewModels
{
    public class OdmoristaViewModel
    {
        public IEnumerable<OdmoristeViewModel> Odmorista { get; set; }
        public PagingInfo PagingInfo { get; set; }

        public OdmoristeFilter Filter { get; set; }
    }
}
    