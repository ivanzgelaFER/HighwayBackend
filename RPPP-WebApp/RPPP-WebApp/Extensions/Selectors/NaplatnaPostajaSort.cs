using RPPP_WebApp.Models;
using System.Linq.Expressions;

namespace RPPP_WebApp.Extensions.Selectors
{
    public static class NaplatnaPostajaSort
    {
        public static IQueryable<NaplatnaPostaja> ApplySort(this IQueryable<NaplatnaPostaja> query, int sort, bool ascending)
        {
            System.Linq.Expressions.Expression<Func<NaplatnaPostaja, object>> orderSelector = null;
            switch (sort)
            {
                case 1:
                    orderSelector = np => np.Naziv;
                    break;
                case 2:
                    orderSelector = np => np.Autocesta.Naziv;
                    break;
                case 3:
                    orderSelector = np => np.KoordinataX;
                    break;
                case 4:
                    orderSelector = np => np.KoordinataY;
                    break;
                case 5:
                    orderSelector = np => np.GodinaOtvaranja;
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