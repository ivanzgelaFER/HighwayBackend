using RPPP_WebApp.Models;

namespace RPPP_WebApp.Extensions.Selectors
{
    public static class VrstaOdrzavanjaSort
    {
        public static IQueryable<VrstaOdrzavanja> ApplySort(this IQueryable<VrstaOdrzavanja> query, int sort, bool ascending)
        {
            System.Linq.Expressions.Expression<Func<VrstaOdrzavanja, object>> orderSelector = null;
            switch (sort)
            {
                case 1:
                    orderSelector = vo => vo.Naziv;
                    break;
                case 2:
                    orderSelector = vo => vo.Izvanredno;
                    break;
                case 3:
                    orderSelector = vo => vo.Preventivno;
                    break;
                case 4:
                    orderSelector = vo => vo.Periodicnost;
                    break;
                case 5:
                    orderSelector = vo => vo.GodisnjeDoba;
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
