using RPPP_WebApp.Models;
namespace RPPP_WebApp.ViewModels
{
    public class NaplatnePostajeViewModel
    {
        public IEnumerable<NaplatnaPostajaViewModel> NaplatnePostaje { get; set; }
        public PagingInfo PagingInfo { get; set; }
    }
}
