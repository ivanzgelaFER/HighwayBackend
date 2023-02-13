using RPPP_WebApp.Models;

namespace RPPP_WebApp.Extensions.Selectors
{
	public static class NaplatnaKucicaSort
	{
		public static IQueryable<NaplatnaKucica> ApplySort(this IQueryable<NaplatnaKucica> query, int sort, bool ascending)
		{
			System.Linq.Expressions.Expression<Func<NaplatnaKucica, object>> orderSelector = null;
			switch (sort)
			{
				case 1:
					orderSelector = oo => oo.Id;
					break;
				case 2:
					orderSelector = oo => oo.NaplatnaPostaja.Naziv;
					break;
				case 3:
					orderSelector = oo => oo.VrstaNaplate.Naziv;
					break;
				case 4:
					orderSelector = oo => oo.Otvorena;
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
