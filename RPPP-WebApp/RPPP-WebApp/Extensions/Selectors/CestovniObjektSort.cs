using RPPP_WebApp.Models;

namespace RPPP_WebApp.Extensions.Selectors
{
    public static class CestovniObjektSort
    {
        public static IQueryable<CestovniObjekt> ApplySort(this IQueryable<CestovniObjekt> query, int sort, bool ascending)
        {
            System.Linq.Expressions.Expression<Func<CestovniObjekt, object>> orderSelector = null;
            switch (sort)
            {
                case 1:
                    orderSelector = co => co.Naziv;
                    break;
                case 2:
                    orderSelector = co => co.TipObjekta;
                    break;
                case 3:
                    orderSelector = co => co.OgranicenjeBrzine;
                    break;
                case 4:
                    orderSelector = co => co.BrojPrometnihTraka;
                    break;
                case 5:
                    orderSelector = co => co.DuljinaObjekta;
                    break;
                case 6:
                    orderSelector = co => co.ZaustavniTrak;
                    break;
                case 7:
                    orderSelector = co => co.DozvolaTeretnimVozilima;
                    break;
                case 8:
                    orderSelector = co => co.Zanimljivost;
                    break;
                case 9:
                    orderSelector = co => co.GodinaIzgradnje;
                    break;
                case 10:
                    orderSelector = co => co.PjesackaStaza;
                    break;
                case 11:
                    orderSelector = co => co.NaplataPrijelaza;
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
