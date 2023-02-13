using RPPP_WebApp.Models;

namespace RPPP_WebApp.Extensions.Selectors {
    public static class ReakcijaSort {
        public static IQueryable<Reakcija> ApplySort(this IQueryable<Reakcija> query, int sort, bool ascending) {
            System.Linq.Expressions.Expression<Func<Reakcija, object>> orderSelector = null;
            switch (sort) {
                case 1:
                    orderSelector = d => d.Opis;
                    break;
                case 2:
                    orderSelector = d => d.Datum;
                    break;
                case 3:
                    orderSelector = d => d.Incident;
                    break;
                case 4:
                    orderSelector = d => d.VrstaReakcije;
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
