using RPPP_WebApp.Models;

namespace RPPP_WebApp.ViewModels
{
    public class CestovniObjektiViewModel
    {
        public IEnumerable<CestovniObjekt> CestovniObjekt { get; set; }
        public PagingInfo PagingInfo { get; set; }
    }
}
