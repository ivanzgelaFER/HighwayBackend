using RPPP_WebApp.Models;

namespace RPPP_WebApp.ViewModels
{
    public class DioniceViewModel
    {
        public IEnumerable<DionicaViewModel> Dionice { get; set; }
        public PagingInfo PagingInfo { get; set; }
    }
}
