using RPPP_WebApp.Models;
using System.Linq;
using System;
using System.Linq.Expressions;

namespace RPPP_WebApp.Extensions.Selectors
{
    public static class ProlazakVozilaSort
    {
        public static IQueryable<ProlazakVozila> ApplySort(this IQueryable<ProlazakVozila> query, int sort, bool ascending)
        {
            System.Linq.Expressions.Expression<Func<ProlazakVozila, object>> orderSelector = null;
            switch (sort)
            {
                case 1:
                    orderSelector = pv => pv.Id;
                    break;
                case 2:
                    orderSelector = pv => pv.RegistracijskaOznaka;
                    break;
                case 3:
                    orderSelector = pv => pv.KategorijaVozila.Naziv;
                    break;
                case 4:
                    orderSelector = pv => pv.VrijemeProlaska;
                    break;
                case 5:
                    orderSelector = pv => pv.NaplatnaKucica.Id;
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