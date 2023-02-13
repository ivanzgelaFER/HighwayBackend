using RPPP_WebApp.Models;
namespace RPPP_WebApp.ViewModels
{
    public class ProlaziVozilaViewModel
    {
        public IEnumerable<ProlazakVozilaTS> ProlaziVozila { get; set; }
        public PagingInfo PagingInfo { get; set; }

    }
}