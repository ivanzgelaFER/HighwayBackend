using RPPP_WebApp.Models;
namespace RPPP_WebApp.ViewModels
{
    public class AutocesteViewModel
    {
        public IEnumerable<AutocestaViewModel> Autoceste { get; set; }
        public PagingInfo PagingInfo { get; set; }
    }
}
