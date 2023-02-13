using RPPP_WebApp.Models;

namespace RPPP_WebApp.Extensions.Selectors
{
    public static class VrstaPopratnogSort
    {
        public static IQueryable<VrstaPopratnog> ApplySort(this IQueryable<VrstaPopratnog> query, int sort, bool ascending)
        {
            System.Linq.Expressions.Expression<Func<VrstaPopratnog, object>> orderSelector = null;
            switch (sort)
            {
                case 1:
                    orderSelector = a => a.Naziv;
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
