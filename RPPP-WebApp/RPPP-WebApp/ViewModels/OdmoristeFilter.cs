using RPPP_WebApp.Models;
using RPPP_WebApp.ModelsPartial;
using System.Globalization;

namespace RPPP_WebApp.ViewModels
{
    public class OdmoristeFilter : IPageFilter
    {
        public int? DionicaId { get; set; }

        public string NazDionice { get; set; }

        public int? GodinaOtvaranjaOd { get; set; }

        public int? GodinaOtvaranjaDo { get; set; }

        public int? KoordinataXOd { get; set; }

        public int? KoordinataXDo { get; set; }

        public int? KoordinataYOd { get; set; }

        public int? KoordinataYDo { get; set; }

        public bool IsEmpty()
        {
            bool active = DionicaId.HasValue
                            || GodinaOtvaranjaOd.HasValue
                            || GodinaOtvaranjaDo.HasValue
                            || KoordinataXOd.HasValue
                            || KoordinataXDo.HasValue
                            || KoordinataYOd.HasValue
                            || KoordinataYDo.HasValue; 
            return !active; 
        }


        public override string ToString()
        {
            return string.Format("{0}-{1}-{2}-{3}-{4}-{5}-{6}",
                  DionicaId,
                  GodinaOtvaranjaOd,
                  GodinaOtvaranjaDo,
                  KoordinataXOd,
                  KoordinataXDo,
                  KoordinataYOd,
                  KoordinataYDo
                  );
        }

        public static OdmoristeFilter FromString(string s)
        {
            var filter = new OdmoristeFilter();
            if (!string.IsNullOrEmpty(s))
            {
                string[] arr = s.Split('-', StringSplitOptions.None);

                if (arr.Length == 7)
                {
                    filter.DionicaId = string.IsNullOrWhiteSpace(arr[0]) ? new int?() : int.Parse(arr[0]);
                    filter.GodinaOtvaranjaOd = string.IsNullOrWhiteSpace(arr[0]) ? new int?() : int.Parse(arr[0]);
                    filter.GodinaOtvaranjaDo = string.IsNullOrWhiteSpace(arr[0]) ? new int?() : int.Parse(arr[0]);
                    filter.KoordinataXOd = string.IsNullOrWhiteSpace(arr[0]) ? new int?() : int.Parse(arr[0]);
                    filter.KoordinataXDo = string.IsNullOrWhiteSpace(arr[0]) ? new int?() : int.Parse(arr[0]);
                    filter.KoordinataYOd = string.IsNullOrWhiteSpace(arr[0]) ? new int?() : int.Parse(arr[0]);
                    filter.KoordinataYDo = string.IsNullOrWhiteSpace(arr[0]) ? new int?() : int.Parse(arr[0]);
                }
            }

            return filter;
        }

        public IQueryable<Odmoriste> Apply(IQueryable<Odmoriste> query)
        {
            if (DionicaId.HasValue)
            {
                query = query.Where(d => d.DionicaId == DionicaId.Value);
            }
            if (GodinaOtvaranjaOd.HasValue)
            {
                query = query.Where(d => d.GodinaOtvaranja >= GodinaOtvaranjaOd.Value);
            }
            if (GodinaOtvaranjaDo.HasValue)
            {
                query = query.Where(d => d.GodinaOtvaranja <= GodinaOtvaranjaDo.Value);
            }
            if (KoordinataXOd.HasValue)
            {
                query = query.Where(d => d.KoordinataX >= KoordinataXOd.Value);
            }
            if (KoordinataXDo.HasValue)
            {
                query = query.Where(d => d.KoordinataX <= KoordinataXDo.Value);
            }
            if (KoordinataYOd.HasValue)
            {
                query = query.Where(d => d.KoordinataY >= KoordinataYOd.Value);
            }
            if (KoordinataYDo.HasValue)
            {
                query = query.Where(d => d.KoordinataY <= KoordinataYDo.Value);
            }



            return query;
        }
    }
}
