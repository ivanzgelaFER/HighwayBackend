using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.VisualBasic;
using RPPP_WebApp.Extensions;
using RPPP_WebApp.Extensions.Selectors;
using NLog.LayoutRenderers;
using RPPP_WebApp.Models;
using RPPP_WebApp.ViewModels;
using System.Text.Json;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Reflection.Metadata;

namespace RPPP_WebApp.Controllers
{
    public class AutocestaMDController : Controller
    {
        private readonly RPPP04Context ctx;
        private readonly ILogger<AutocestaMDController> logger;
        private readonly AppSettings appSettings;

        public AutocestaMDController(RPPP04Context ctx, IOptionsSnapshot<AppSettings> options, ILogger<AutocestaMDController> logger)
        {
            this.ctx = ctx;
            this.logger = logger;
            appSettings = options.Value;
        }

        public async Task<IActionResult> Index(int page = 1, int sort = 1, bool ascending = true)
        {
            int pagesize = appSettings.PageSize;

            var query = ctx.Autocesta.AsQueryable();

            int count = await query.CountAsync();


            var pagingInfo = new PagingInfo
            {
                CurrentPage = page,
                Sort = sort,
                Ascending = ascending,
                ItemsPerPage = pagesize,
                TotalItems = count
            };
            if (page < 1 || page > pagingInfo.TotalPages)
            {
                return RedirectToAction(nameof(Index), new { page = 1, sort, ascending });
            }

            query = query.ApplySort(sort, ascending);

            
            var autoceste = await query
                         .Select(a => new AutocestaViewModel
                         {
                             Id = a.Id,
                             Naziv = a.Naziv,
                             Oznaka = a.Oznaka,
                             KoncesionarId = a.Koncesionar.Id,
                             KoncesionarObj = a.Koncesionar,
                             Koncesionar = a.Koncesionar.Naziv,
                          })
                        .Skip((page - 1) * pagesize)
                        .Take(pagesize)
            .ToListAsync();

            foreach(var a in autoceste)
            {
                var listaNaziva = await ctx.Dionica
                                      .Where(d => d.AutocestaId == a.Id)
                                      .Select(d => d.Naziv)
                                      .ToListAsync();
                string naziv = string.Join(", ", listaNaziva);
                
                a.NaziviDionica = naziv;
            }


            var model = new AutocesteViewModel
            {
                Autoceste = autoceste,
                PagingInfo = pagingInfo
            };

            return View(model);
        }

       

        public async Task<IActionResult> Show(int id, string filter, int page = 1, int sort = 1, bool ascending = true, string viewName = nameof(Show))
        {
            var autocesta = await ctx.Autocesta
                                    .Where(a => a.Id == id)
                                    .Select(a => new AutocestaViewModel
                                    {
                                        Id = a.Id,
                                        Oznaka = a.Oznaka,
                                        Naziv = a.Naziv,
                                        KoncesionarId = a.KoncesionarId,
                                        KoncesionarObj = a.Koncesionar,
                                        Koncesionar = a.Koncesionar.Naziv,
                                    })
                                    .FirstOrDefaultAsync();
            if (autocesta == null)
            {
                return NotFound($"Autocesta s id {id} ne postoji");
            }
            else
            {
               
                //učitavanje stavki - dionica
                var stavke = await ctx.Dionica
                                      .Where(d => d.AutocestaId == autocesta.Id)
                                      .OrderBy(d => d.Id)
                                      .Select(d => new DionicaViewModel
                                      {
                                          Id = d.Id,
                                          UlaznaPostaja = d.UlaznaPostaja.Naziv,
                                          IzlaznaPostaja = d.IzlaznaPostaja.Naziv,
                                          BrojTraka = d.BrojTraka,
                                          ZaustavnaTraka = d.ZaustavnaTraka,
                                          DozvolaTeretnimVozilima = d.DozvolaTeretnimVozilima,
                                          OtvorenZaProlaz = d.OtvorenZaProlaz,
                                          GodinaOtvaranja = d.GodinaOtvaranja,
                                          Duljina = d.Duljina,
                                          Naziv = d.Naziv,
                                          OgranicenjeBrzine = d.OgranicenjeBrzine,
                                          Autocesta = d.Autocesta.Naziv
                                      })
                                      .ToListAsync();
                autocesta.Dionice = stavke;

                /*if (position.HasValue)
                {
                    page = 1 + position.Value / appSettings.PageSize;
                    await SetPreviousAndNext(position.Value, sort, ascending);
                }*/

                ViewBag.Page = page;
                ViewBag.Sort = sort;
                ViewBag.Ascending = ascending;
                ViewBag.Filter = filter;
                //ViewBag.Position = position;

                return View(viewName, autocesta);
            }
        }


       

        private async Task PrepareDropDownList()
        {
          
            var koncesionari = await ctx.Koncesionar.OrderBy(k => k.Naziv).Select(k => new { k.Id, k.Naziv }).ToListAsync();
            
            ViewBag.Autoceste = new SelectList(koncesionari, nameof(Koncesionar.Id), nameof(Koncesionar.Naziv));
            // new SelectList(koncesionari, nameof(koncesionari.Id), nameof(koncesionari.Naziv));
        }

        [HttpGet]
        public async Task<IActionResult> Create()
        {
            await PrepareDropDownList();
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(AutocestaViewModel model)
        {
            if (ModelState.IsValid)
            {
                Autocesta a = new Autocesta();
                //a.Id = model.Id;
                a.Naziv = model.Naziv;
                a.Oznaka = model.Oznaka;
                a.KoncesionarId = model.KoncesionarId;
                
                foreach (var dionica in model.Dionice)
                {
                    Dionica novaDionica = new Dionica();
                    //novaDionica.Id = stavka.Id;
                    novaDionica.Naziv = dionica.Naziv;
                    novaDionica.BrojTraka = dionica.BrojTraka;
                    novaDionica.Duljina = dionica.Duljina;
                    novaDionica.ZaustavnaTraka = dionica.ZaustavnaTraka;
                    novaDionica.DozvolaTeretnimVozilima = dionica.DozvolaTeretnimVozilima;
                    novaDionica.OgranicenjeBrzine = dionica.OgranicenjeBrzine;
                    novaDionica.OtvorenZaProlaz = dionica.OtvorenZaProlaz;
                    //novaDionica.UlaznaPostaja.Naziv = dionica.UlaznaPostaja;
                    //novaDionica.IzlaznaPostaja.Naziv = dionica.IzlaznaPostaja;
                    novaDionica.UlaznaPostaja = dionica.UlaznaPostajaObj;
                    novaDionica.IzlaznaPostaja = dionica.IzlaznaPostajaObj;
                    // novaDionica.Autocesta.Naziv = dionica.Autocesta;
                    novaDionica.Autocesta = dionica.AutocestaObj;
                    novaDionica.GodinaOtvaranja = dionica.GodinaOtvaranja;
                    a.Dionica.Add(novaDionica);
                }

                try
                {
                    ctx.Add(a);
                    await ctx.SaveChangesAsync();
                    logger.LogInformation(new EventId(1000), $"MD autocesta  {a.Naziv} dodan.");
                    TempData[Constants.Message] = $"Dokument uspješno dodan. Id={a.Id}";
                    TempData[Constants.ErrorOccurred] = false;
                    return RedirectToAction(nameof(Edit), new { id = a.Id });

                }
                catch (Exception exc)
                {
                    logger.LogError("Pogreška prilikom dodavanje nove autoceste: {0}", exc.CompleteExceptionMessage());
                    ModelState.AddModelError(string.Empty, exc.CompleteExceptionMessage());
                    return View(model);
                }
            }
            else
            {
                return View(model);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int IdAutoceste, string filter, int page = 1, int sort = 1, bool ascending = true)
        {
            var autocesta = await ctx.Autocesta
                                    .Where(a => a.Id == IdAutoceste)
                                    .SingleOrDefaultAsync();
            if (autocesta != null)
            {
                try
                {
                    string naziv = autocesta.Naziv;
                    ctx.Remove(autocesta);
                    await ctx.SaveChangesAsync();
                    logger.LogInformation($"Autocesta: {naziv} uspješno obrisana");
                    TempData[Constants.Message] = $"Autocesta {autocesta.Id} uspješno obrisana.";
                    TempData[Constants.ErrorOccurred] = false;
                }
                catch (Exception exc)
                {
                    logger.LogError("Pogreška prilikom brisanja cestovnog objekta: " + exc.CompleteExceptionMessage());
                    TempData[Constants.Message] = "Pogreška prilikom brisanja autoceste: " + exc.CompleteExceptionMessage();
                    TempData[Constants.ErrorOccurred] = true;
                }
            }
            else
            {
                TempData[Constants.Message] = "Ne postoji autocesta s id-om: " + IdAutoceste;
                TempData[Constants.ErrorOccurred] = true;
            }
            return RedirectToAction(nameof(Index), new { filter, page, sort, ascending });
        }


    [HttpGet]
        public Task<IActionResult> Edit(int id, string filter, int page = 1, int sort = 1, bool ascending = true)
        {
            return Show(id, filter, page, sort, ascending, viewName: nameof(Edit));
        }

        [HttpPost]

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(AutocestaViewModel model, string filter, int page = 1, int sort = 1, bool ascending = true)
        {
            ViewBag.Page = page;
            ViewBag.Sort = sort;
            ViewBag.Ascending = ascending;
            ViewBag.Filter = filter;
            if (ModelState.IsValid)
            {
                var autocesta = await ctx.Autocesta
                                        .Include(a => a.Dionica)
                                        .Where(a => a.Id == model.Id)
                                        .FirstOrDefaultAsync();
                if (autocesta == null)
                {
                    logger.LogWarning("Ne postoji autocesta  s oznakom: {0} ", model.Id);
                    return NotFound("Ne postoji autocesta s id-om: " + model.Id);
                }


                autocesta.Naziv = model.Naziv;
                autocesta.Oznaka = model.Oznaka;
                autocesta.Koncesionar = model.KoncesionarObj;

                List<int> idDionica = model.Dionice
                                          .Where(a => a.Id > 0)
                                          .Select(a => a.Id)
                                          .ToList();
                //izbaci sve koje su nisu više u modelu
                ctx.RemoveRange(autocesta.Dionica.Where(d => !idDionica.Contains(d.Id)));

                foreach (var dionica in model.Dionice)
                {
                    //ažuriraj postojeće i dodaj nove
                    Dionica novaDionica; // potpuno nova ili dohvaćena ona koju treba izmijeniti
                    if (dionica.Id > 0)
                    {
                        novaDionica = autocesta.Dionica.First(d => d.Id == dionica.Id);
                    }
                    else
                    {
                        novaDionica = new Dionica();
                        autocesta.Dionica.Add(novaDionica);
                    }
                    novaDionica.Naziv = dionica.Naziv;
                    novaDionica.BrojTraka = dionica.BrojTraka;
                    novaDionica.Duljina = dionica.Duljina;
                    novaDionica.ZaustavnaTraka = dionica.ZaustavnaTraka;
                    novaDionica.DozvolaTeretnimVozilima = dionica.DozvolaTeretnimVozilima;
                    novaDionica.OgranicenjeBrzine = dionica.OgranicenjeBrzine;
                    novaDionica.OtvorenZaProlaz = dionica.OtvorenZaProlaz;
                    novaDionica.UlaznaPostaja = dionica.UlaznaPostajaObj;
                    novaDionica.IzlaznaPostaja = dionica.IzlaznaPostajaObj;
                    //novaDionica.UlaznaPostaja.Naziv = dionica.UlaznaPostaja;
                    //novaDionica.IzlaznaPostaja.Naziv = dionica.IzlaznaPostaja;
                    //novaDionica.Autocesta.Naziv = dionica.Autocesta;
                    novaDionica.Autocesta = dionica.AutocestaObj;
                    novaDionica.GodinaOtvaranja = dionica.GodinaOtvaranja;
                    ctx.Update(novaDionica);
                }

                
                try
                {
                    ctx.Update(autocesta);
                    await ctx.SaveChangesAsync();
                    logger.LogInformation($"(MD) autocesta: {autocesta.Naziv} uspješno ažuriran");
                    TempData[Constants.Message] = $"Dokument {autocesta.Id} uspješno ažuriran.";
                    TempData[Constants.ErrorOccurred] = false;
                    return RedirectToAction(nameof(Edit), new
                    {
                        id = autocesta.Id,
                        filter,
                        page,
                        sort,
                        ascending
                    });

                }
                catch (Exception exc)
                {
                    logger.LogError("Pogreška prilikom ažuriranja autoceste: " + exc.CompleteExceptionMessage());
                    ModelState.AddModelError(string.Empty, exc.CompleteExceptionMessage());
                    return View(model);
                }
            }
            else
            {
                return View(model);
            }
        }



        [HttpGet]
        public async Task<IActionResult> Get(int id)
        {
            var autocesta = await ctx.Autocesta
                                  .Where(a => a.Id == id)
                                  .Select(a => new AutocestaViewModel
                                  {
                                      Id = a.Id,
                                      Naziv = a.Naziv,
                                      Oznaka = a.Oznaka,
                                      Koncesionar = a.Koncesionar.Naziv
                                  })
                                  .SingleOrDefaultAsync();
            if (autocesta != null)
            {
                return PartialView(autocesta);
            }
            else
            {
                return NotFound($"Neispravan id autoceste: {id}");
            }
        }
    }
}
