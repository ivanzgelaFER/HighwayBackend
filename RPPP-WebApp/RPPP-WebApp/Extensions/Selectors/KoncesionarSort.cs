using RPPP_WebApp.Models;
using System.Linq.Expressions;

namespace RPPP_WebApp.Extensions.Selectors
{
    public static class KoncesionarSort
    {
        public static IQueryable<Koncesionar> ApplySort(this IQueryable<Koncesionar> query, int sort, bool ascending)
        {
            System.Linq.Expressions.Expression<Func<Koncesionar, object>> orderSelector = null;
            switch (sort)
            {
                case 1:
                    orderSelector = k => k.Naziv;
                    break;
                case 2:
                    orderSelector = k => k.Adresa;
                    break;
                case 3:
                    orderSelector = k => k.Email;
                    break;
                case 4:
                    orderSelector = k => k.KoncesijaOd;
                    break;
                case 5:
                    orderSelector = k => k.KoncesijaDo;
                    break;
               
            }
            if (orderSelector != null)
            {
                query = ascending ?
                       query.OrderBy(orderSelector) :
                       query.OrderByDescending(orderSelector);
            }

            return query;
        }
    }
}