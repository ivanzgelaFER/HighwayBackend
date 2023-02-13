using RPPP_WebApp.Models;

namespace RPPP_WebApp.Extensions.Selectors {
    public static class VrstaReakcijeSort {

        public static IQueryable<VrstaReakcije> ApplySort(this IQueryable<VrstaReakcije> query, int sort, bool ascending) {
            System.Linq.Expressions.Expression<Func<VrstaReakcije, object>> orderSelector = null;
            switch (sort) {
                case 1:
                    orderSelector = np => np.Naziv;
                    break;
                case 2:
                    orderSelector = np => np.BrojTelefona;
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
