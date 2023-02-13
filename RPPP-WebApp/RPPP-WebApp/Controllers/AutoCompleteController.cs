using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using RPPP_WebApp.Models;
using RPPP_WebApp.ViewModels;
using Microsoft.EntityFrameworkCore;
using MVC.ViewModels;

namespace RPPP_WebApp.Controllers
{
    public class AutoCompleteController : Controller
    {
        private readonly RPPP04Context ctx;
        private readonly ILogger<IvanMasterDetailController> logger;
        private readonly AppSettings appData;

        public AutoCompleteController(RPPP04Context ctx, IOptionsSnapshot<AppSettings> options, ILogger<IvanMasterDetailController> logger)
        {
            appData = options.Value;
            this.ctx = ctx;
            this.logger = logger;
        }

        public async Task<IEnumerable<IdLabel>> Dionica(string term)
        {
            var query = ctx.Dionica
                     .Where(d => d.Naziv.Contains(term))
                     .OrderBy(d => d.Naziv)
                     .Select(d => new IdLabel
                     {
                         Id = d.Id,
                         Label = d.Naziv
                     });

            var list = await query.OrderBy(l => l.Label)
                                  .ThenBy(l => l.Id)
                                  .Take(appData.AutoCompleteCount)
                                  .ToListAsync();
            return list;
        }

        public async Task<IEnumerable<AutoCompleteVrstaOdrzavanja>> VrstaOdrzavanja(string term)
        {
            var query = ctx.VrstaOdrzavanja
                     .Where(vo => vo.Naziv.Contains(term))
                     .OrderBy(vo => vo.Naziv)
                     .Select(vo => new AutoCompleteVrstaOdrzavanja
                     {
                         Id = vo.Id,
                         Label = vo.Naziv
                     });

            var list = await query.OrderBy(l => l.Label)
                                  .ThenBy(l => l.Id)
                                  .Take(appData.AutoCompleteCount)
                                  .ToListAsync();
            return list;
        }

        public async Task<IEnumerable<AutoCompleteOdrzavanjeObjekta>> OdrzavanjeObjekta(string term)
        {
            var query = ctx.OdrzavanjeObjekta
                     .Where(oo => oo.ImeFirme.Contains(term))
                     .OrderBy(oo => oo.ImeFirme)
                     .Select(oo => new AutoCompleteOdrzavanjeObjekta
                     {
                         Id = oo.Id,
                         Label = oo.ImeFirme,
                         Cijena = oo.Cijena,
                         VrstaId = oo.VrstaId,
                         Vrsta = oo.Vrsta.Naziv,
                         CestovniObjektId = oo.CestovniObjektId,
                         RadnimDanomOd = oo.RadnimDanomOd,
                         RadnimDanomDo = oo.RadnimDanomDo,
                         VikendimaOd = oo.VikendimaOd,
                         VikendimaDo = oo.VikendimaDo,
                         BrojLjudi = oo.BrojLjudi,
                         PredvidenoDana = oo.PredvidenoDana
                     });

            var list = await query.OrderBy(l => l.Label)
                                  .ThenBy(l => l.Id)
                                  .Take(appData.AutoCompleteCount)
                                  .ToListAsync();
            return list;
        }

        public async Task<IEnumerable<AutoCompleteVrstaPopratnog>> VrstaPopratnog(string term)
        {
            var query = ctx.VrstaPopratnog
                     .Where(vp => vp.Naziv.Contains(term))
                     .OrderBy(vp => vp.Naziv)
                     .Select(vp => new AutoCompleteVrstaPopratnog
                     {
                         Id = vp.Id,
                         Label = vp.Naziv
                     });

            var list = await query.OrderBy(l => l.Label)
                                  .ThenBy(l => l.Id)
                                  .Take(appData.AutoCompleteCount)
                                  .ToListAsync();
            return list;
        }


        public async Task<IEnumerable<AutoCompletePopratniSadrzaj>> PopratniSadrzaj(string term)
        {
            var query = ctx.PopratniSadrzaj
                     .Where(ps => ps.Naziv.Contains(term))
                     .OrderBy(ps => ps.Naziv)
                     .Select(ps => new AutoCompletePopratniSadrzaj
                     {
                         Id = ps.Id,
                         Label = ps.Naziv,
                         RadnimDanomOd = ps.RadnimDanomOd,
                         RadninDanomDo = ps.RadninDanomDo,
                         VikendimaDo = ps.VikendimaDo,
                         VikendimaOd = ps.VikendimaOd,
                         OdmoristeId = ps.OdmoristeId,
                         VrstaSadrzajaId = ps.VrstaSadrzajaId,
                         VrstaPopratnog = ps.VrstaSadrzaja.Naziv
                     });

            var list = await query.OrderBy(l => l.Label)
                                  .ThenBy(l => l.Id)
                                  .Take(appData.AutoCompleteCount)
                                  .ToListAsync();
            return list;
        }

        public async Task<IEnumerable<AutoCompleteKoncesionar>> Koncesionar(string term)
        {
            var query = ctx.Koncesionar
                     .Where(k => k.Naziv.Contains(term))
                     .OrderBy(k => k.Naziv)
                     .Select(k => new AutoCompleteKoncesionar
                     {
                         Id = k.Id,
                         Label = k.Naziv
                     });

            var list = await query.OrderBy(l => l.Label)
                                  .ThenBy(l => l.Id)
                                  .Take(appData.AutoCompleteCount)
                                  .ToListAsync();
            return list;
        }

        public async Task<IEnumerable<AutoCompleteAutocestaDionica>> AutocestaDionica(string term)
        {
            var query = ctx.Dionica
                     .Where(d => d.Naziv.Contains(term))
                     .OrderBy(d => d.Naziv)
                     .Select(d => new AutoCompleteAutocestaDionica
                     {
                         Id = d.Id,
                        Label =  d.Naziv,
                        UlaznaPostajaId = d.UlaznaPostajaId,
                        IzlaznaPostajaId = d.IzlaznaPostajaId,
                        BrojTraka = d.BrojTraka,
                        ZaustavnaTraka = d.ZaustavnaTraka,
                        DozvolaTeretnimVozilima = d.DozvolaTeretnimVozilima,
                        OtvorenZaProlaz = d.OtvorenZaProlaz,
                        GodinaOtvaranja = d.GodinaOtvaranja,
                        Duljina = d.Duljina,
                        Naziv = d.Naziv,
                        OgranicenjeBrzine = d.OgranicenjeBrzine,
                        AutocestaId = d.AutocestaId
                        
                     });

            var list = await query.OrderBy(l => l.Label)
                                  .ThenBy(l => l.Id)
                                  .Take(appData.AutoCompleteCount)
                                  .ToListAsync();
            return list;
        }
    }
}
