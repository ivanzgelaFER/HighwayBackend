using RPPP_WebApp.Models;
namespace RPPP_WebApp.ViewModels {
    public class VrsteReakcijaViewModel {
        public IEnumerable<VrstaReakcijeViewModel> VrsteReakcija { get; set; }
        public PagingInfo PagingInfo { get; set; }
    }
}
