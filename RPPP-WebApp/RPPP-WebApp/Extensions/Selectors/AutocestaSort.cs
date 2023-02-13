using RPPP_WebApp.Models;
using System.Linq.Expressions;

namespace RPPP_WebApp.Extensions.Selectors
{
    public static class AutocestaSort
    {
        public static IQueryable<Autocesta> ApplySort(this IQueryable<Autocesta> query, int sort, bool ascending)
        {
            System.Linq.Expressions.Expression<Func<Autocesta, object>> orderSelector = null;
            switch (sort)
            {
                case 1:
                    orderSelector = a => a.Oznaka;
                    break;
                case 2:
                    orderSelector = a => a.Naziv;
                    break;
                case 3:
                    orderSelector = a => a.Koncesionar.Naziv;
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

        /*public static IQueryable<ViewAutocestaInfo> ApplySort(this IQueryable<ViewAutocestaInfo> query, int sort, bool ascending)
        {
            System.Linq.Expressions.Expression<Func<ViewAutocestaInfo, object>> orderSelector = null;
            switch (sort)
            {
                case 1:
                    orderSelector = a => a.Oznaka;
                    break;
                case 2:
                    orderSelector = a => a.Naziv;
                    break;
                case 3:
                    //orderSelector = a => a.Koncesionar.Naziv;
                    break;

            }
            if (orderSelector != null)
            {
                query = ascending ?
                       query.OrderBy(orderSelector) :
                       query.OrderByDescending(orderSelector);
            }

            return query;
        }*/
    }
}