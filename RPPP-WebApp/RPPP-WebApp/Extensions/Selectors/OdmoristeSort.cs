using RPPP_WebApp.Models;
using System.Linq.Expressions;

namespace RPPP_WebApp.Extensions.Selectors
{
    public static class OdmoristeSort
    {
        public static IQueryable<Odmoriste> ApplySort(this IQueryable<Odmoriste> query, int sort, bool ascending)
        {
            Expression<Func<Odmoriste, object>> orderSelector = null;
            switch (sort)
            {
                case 1:
                    orderSelector = k => k.Naziv;
                    break; 
                case 2:
                    orderSelector= k => k.Dionica.Naziv;
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