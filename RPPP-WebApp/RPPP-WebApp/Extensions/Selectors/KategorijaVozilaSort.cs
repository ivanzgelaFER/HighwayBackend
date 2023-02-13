using RPPP_WebApp.Models;
using System.Linq;
using System;
using System.Linq.Expressions;

namespace RPPP_WebApp.Extensions.Selectors
{
    public static class KategorijaVozilaSort
    {
        public static IQueryable<KategorijaVozila> ApplySort(this IQueryable<KategorijaVozila> query, int sort, bool ascending)
        {
            System.Linq.Expressions.Expression<Func<KategorijaVozila, object>> orderSelector = null;
            switch (sort)
            {
                case 1:
                    orderSelector = kv => kv.Id;
                    break;
                case 2:
                    orderSelector = kv => kv.Naziv;
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