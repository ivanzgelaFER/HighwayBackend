using RPPP_WebApp.Models;

namespace RPPP_WebApp.ViewModels {
    public class VrsteIncidenataViewModel {
        public IEnumerable<VrstaIncidenta> VrsteIncidenata { get; set; }
        public PagingInfo PagingInfo { get; set; }
    }
}
