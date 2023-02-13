using RPPP_WebApp.Models;
using System.Linq.Expressions;

namespace RPPP_WebApp.Extensions.Selectors
{
    public static class PopratniSadrzajSort
    {
        public static IQueryable<PopratniSadrzaj> ApplySort(this IQueryable<PopratniSadrzaj> query, int sort, bool ascending)
        {
            Expression<Func<PopratniSadrzaj, object>> orderSelector = null;
            switch (sort)
            {
                case 1:
                    orderSelector = k => k.Naziv;
                    break;
                case 2:
                    orderSelector = k => k.Id;
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