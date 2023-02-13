using RPPP_WebApp.Models;
using System.Linq.Expressions;

namespace RPPP_WebApp.Extensions.Selectors {
    public static class VrstaIncidentaSort {
        public static IQueryable<VrstaIncidenta> ApplySort(this IQueryable<VrstaIncidenta> query, int sort, bool ascending) {
            System.Linq.Expressions.Expression<Func<VrstaIncidenta, object>> orderSelector = null;
            switch (sort) {
                case 1:
                    orderSelector = k => k.Naziv;
                    break;
                case 2:
                    orderSelector = k => k.OpisPravilaPonasanja;
                    break;
    }
            if (orderSelector != null) {
                query = ascending ?
                       query.OrderBy(orderSelector) :
                       query.OrderByDescending(orderSelector);
            }

            return query;
        }
    }
}