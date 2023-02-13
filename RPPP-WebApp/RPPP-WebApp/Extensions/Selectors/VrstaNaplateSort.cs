using RPPP_WebApp.Models;

namespace RPPP_WebApp.Extensions.Selectors
{
    public static class VrstaNaplateSort
    {
        public static IQueryable<VrstaNaplate> ApplySort(this IQueryable<VrstaNaplate> query, int sort, bool ascending)
        {
            System.Linq.Expressions.Expression<Func<VrstaNaplate, object>> orderSelector = null;
            switch (sort)
            {
                case 1:
                    orderSelector = oo => oo.Id;
                    break;
                case 2:
                    orderSelector = oo => oo.Naziv;
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
