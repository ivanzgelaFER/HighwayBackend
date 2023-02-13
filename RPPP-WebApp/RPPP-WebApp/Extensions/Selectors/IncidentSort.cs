using RPPP_WebApp.Models;

namespace RPPP_WebApp.Extensions.Selectors {
    public static class IncidentSort {
        public static IQueryable<Incident> ApplySort(this IQueryable<Incident> query, int sort, bool ascending) {
            System.Linq.Expressions.Expression<Func<Incident, object>> orderSelector = null;
            switch (sort) {
                case 1:
                    orderSelector = a => a.Opis;
                    break;
                case 2:
                    orderSelector = a => a.Datum;
                    break;
                case 3:
                    orderSelector = a => a.MeteoroloskiUvjeti;
                    break;
                case 4:
                    orderSelector = a => a.StanjeNaCesti;
                    break;
                case 5:
                    orderSelector = a => a.Prohodnost;
                    break;
                case 6:
                    orderSelector = a => a.Dionica;
                    break;
                case 7:
                    orderSelector = a => a.VrstaIncidenta;
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
