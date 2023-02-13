using RPPP_WebApp.Models;

namespace RPPP_WebApp.Extensions.Selectors
{
    public static class OdrzavanjeObjektaSort
    {
        public static IQueryable<OdrzavanjeObjekta> ApplySort(this IQueryable<OdrzavanjeObjekta> query, int sort, bool ascending)
        {
            System.Linq.Expressions.Expression<Func<OdrzavanjeObjekta, object>> orderSelector = null;
            switch (sort)
            {
                case 1:
                    orderSelector = oo => oo.VrstaId;
                    break;
                case 2:
                    orderSelector = oo => oo.ImeFirme;
                    break;
                case 3:
                    orderSelector = oo => oo.RadnimDanomOd;
                    break;
                case 4:
                    orderSelector = oo => oo.RadnimDanomDo;
                    break;
                case 5:
                    orderSelector = oo => oo.VikendimaOd;
                    break;
                case 6:
                    orderSelector = oo => oo.VikendimaDo;
                    break;
                case 7:
                    orderSelector = oo => oo.BrojLjudi;
                    break;
                case 8:
                    orderSelector = oo => oo.Cijena;
                    break;
                case 9:
                    orderSelector = oo => oo.PredvidenoDana;
                    break;
                case 10:
                    orderSelector = oo => oo.CestovniObjektId;
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
