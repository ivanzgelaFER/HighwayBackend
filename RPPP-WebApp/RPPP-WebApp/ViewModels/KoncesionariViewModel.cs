using RPPP_WebApp.Models;
namespace RPPP_WebApp.ViewModels
{
    public class KoncesionariViewModel
    {
        public IEnumerable<Koncesionar> Koncesionari { get; set; }
        public PagingInfo PagingInfo { get; set; }
    }
}
