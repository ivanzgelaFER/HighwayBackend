using RPPP_WebApp.Models;
namespace RPPP_WebApp.ViewModels
{
    public class NaplatneKuciceViewModel
    {
        public IEnumerable<NaplatnaKucica> NaplatneKucice { get; set; }
        public PagingInfo PagingInfo { get; set; }
    }
}