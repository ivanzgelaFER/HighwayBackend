using RPPP_WebApp.Models;

namespace RPPP_WebApp.Extensions.Selectors
{
    public static class DionicaSort
    {
        public static IQueryable<Dionica> ApplySort(this IQueryable<Dionica> query, int sort, bool ascending)
        {
            System.Linq.Expressions.Expression<Func<Dionica, object>> orderSelector = null;
            switch (sort)
            {
                case 1:
                    orderSelector = d => d.Naziv;
                    break;
                case 2:
                    orderSelector = d => d.UlaznaPostaja.Naziv;
                    break;
                case 3:
                    orderSelector = d => d.IzlaznaPostaja.Naziv;
                    break;
                case 4:
                    orderSelector = d => d.Autocesta.Naziv;
                    break;
                case 5:
                    orderSelector = d => d.BrojTraka;
                    break;
                case 6:
                    orderSelector = d => d.ZaustavnaTraka;
                    break;
                case 7:
                    orderSelector = d => d.DozvolaTeretnimVozilima;
                    break;
                case 8:
                    orderSelector = d => d.OtvorenZaProlaz;
                    break;
                case 9:
                    orderSelector = d => d.GodinaOtvaranja;
                    break;
                case 10:
                    orderSelector = d => d.Duljina;
                    break;
                case 11:
                    orderSelector = d => d.OgranicenjeBrzine;
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
