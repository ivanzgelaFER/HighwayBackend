using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using RPPP_WebApp.Extensions;
using RPPP_WebApp.Extensions.Selectors;
using RPPP_WebApp.Models;
using RPPP_WebApp.ViewModels;
using System.Text.Json;

namespace RPPP_WebApp.Controllers
{
    public class IvanMasterDetailController : Controller
    {
        private readonly RPPP04Context ctx;
        private readonly ILogger<IvanMasterDetailController> logger;
        private readonly AppSettings appData;

        public IvanMasterDetailController(RPPP04Context ctx, IOptionsSnapshot<AppSettings> options, ILogger<IvanMasterDetailController> logger)
        {
            appData = options.Value;
            this.ctx = ctx;
            this.logger = logger;
        }
        public async Task<IActionResult> Index(int page = 1, int sort = 1, bool ascending = true)
        {

            var query = ctx.CestovniObjekt.AsQueryable();
            int count = await query.CountAsync();

            int pagesize = appData.PageSize;
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
                return RedirectToAction(nameof(Index), new { page = 1, sort = sort, ascending = ascending });
            }

            query = query.ApplySort(sort, ascending);

            var cestovniObjekti = await query
                                      .Select(co => new IvanMasterDetailViewModel
                                      {
                                        Id = co.Id,
                                        NazivCestovnogObjekta = co.Naziv,
                                        Dionica = co.Dionica
                                      })
                                      .Skip((page - 1) * pagesize)
                                      .Take(pagesize)
                                      .ToListAsync();

            foreach (var co in cestovniObjekti)
            {
                var listaOdrzavanjaObjekta = await ctx.OdrzavanjeObjekta
                                      .Where(oo => oo.CestovniObjektId == co.Id)
                                      .Include(oo => oo.Vrsta)
                                      .Select(oo => oo.Vrsta.Naziv)
                                      .ToListAsync();
                string nazivi = string.Join(", ", listaOdrzavanjaObjekta);

                co.NaziviOdrzavanjaObjekata = nazivi;
            }

            var model = new IvanMasterDetailsViewModel
            {
                IvanMasterDetail = cestovniObjekti,
                PagingInfo = pagingInfo
            };

            return View(model);
        }

        public async Task<IActionResult> Show(int id, string filter, int page = 1, int sort = 1, bool ascending = true, string viewName = nameof(Show))
        {
            IvanMasterDetailViewModel cestovniObjekt = await ctx.CestovniObjekt
                                    .Where(co => co.Id == id)
                                    .Select(co => new IvanMasterDetailViewModel
                                    {
                                        Id = co.Id,
                                        NazivCestovnogObjekta = co.Naziv,
                                        TipObjekta = co.TipObjekta,
                                        OgranicenjeBrzine = co.OgranicenjeBrzine,
                                        BrojPrometnihTraka = co.BrojPrometnihTraka,
                                        DuljinaObjekta = co.DuljinaObjekta,
                                        ZaustavniTrak = co.ZaustavniTrak,
                                        DozvolaTeretnimVozilima = co.DozvolaTeretnimVozilima,
                                        Zanimljivost = co.Zanimljivost,
                                        GodinaIzgradnje = co.GodinaIzgradnje,
                                        PjesackaStaza = co.PjesackaStaza,
                                        NaplataPrijelaza = co.NaplataPrijelaza,
                                        Dionica = co.Dionica,
                                        DionicaId = co.DionicaId,
                                    })
                                    .FirstOrDefaultAsync();

            if (cestovniObjekt != null) {
                List<OdrzavanjeObjekta> odrzavanjaObjekata = await ctx.OdrzavanjeObjekta
                                      .Where(oo => oo.CestovniObjektId == cestovniObjekt.Id)
                                      .Include(oo => oo.Vrsta)
                                      .OrderBy(d => d.Id)
                                      .ToListAsync();
                cestovniObjekt.OdrzavanjeObjekata = odrzavanjaObjekata;

                ViewBag.Page = page;
                ViewBag.Sort = sort;
                ViewBag.Ascending = ascending;
                ViewBag.Filter = filter;

                return View(viewName, cestovniObjekt);
            }
            else {
                return NotFound($"Cestovni objekt s id = {id} ne postoji");
            }
        }


        [HttpGet]
        public IActionResult Create()
        {
            var ivanMasterDetailViewModel = new IvanMasterDetailViewModel();
            return View(ivanMasterDetailViewModel);
        }

        [HttpPost]
        public async Task<IActionResult> Create(IvanMasterDetailViewModel masterDetailViewModel)
        {
            if (ModelState.IsValid)
            {
                CestovniObjekt cestovniObjekt = new CestovniObjekt();
                CopyValuesMaster(cestovniObjekt, masterDetailViewModel);

                foreach (var odrzavanjeObjekta in masterDetailViewModel.OdrzavanjeObjekata)
                {
                    OdrzavanjeObjekta odrzavanjeObjektaNew = new OdrzavanjeObjekta();
                    CopyValuesDetail(odrzavanjeObjektaNew, odrzavanjeObjekta);
                    cestovniObjekt.OdrzavanjeObjekta.Add(odrzavanjeObjektaNew);
                }

                try
                {
                    ctx.Add(cestovniObjekt);
                    await ctx.SaveChangesAsync();
                    logger.LogInformation(new EventId(1000), $"MD cestovni objekt {cestovniObjekt.Naziv} dodan.");
                    TempData[Constants.Message] = $"MD cestovni objekt uspješno dodan. Id={cestovniObjekt.Id}";
                    TempData[Constants.ErrorOccurred] = false;
                    return RedirectToAction(nameof(Edit), new { id = cestovniObjekt.Id });

                }
                catch (Exception exc)
                {
                    logger.LogError("Pogreška prilikom dodavanje novog cestovnog objekta: {0}", exc.CompleteExceptionMessage());
                    ModelState.AddModelError(string.Empty, exc.CompleteExceptionMessage());
                    return View(masterDetailViewModel);
                }
            }
            else
            {
                return View(masterDetailViewModel);
            }
        }

        [HttpDelete]
        public async Task<IActionResult> Delete(int id)
        {
            var cestovniObjekt = await ctx.CestovniObjekt
                   .Include(co => co.OdrzavanjeObjekta).Where(co => co.Id == id).SingleOrDefaultAsync();
            ActionResponseMessage responseMessage;
            if (cestovniObjekt != null)
            {
                try
                {
                    string naziv = cestovniObjekt.Naziv;
                    ctx.Remove(cestovniObjekt);
                    await ctx.SaveChangesAsync();
                    logger.LogInformation($"Cestovni objekt: {naziv} uspješno obrisan");
                    responseMessage = new ActionResponseMessage(MessageType.Success, $"Cestovni objekt {naziv} sa šifrom {id} uspješno obrisan.");
                }
                catch (Exception exc)
                {
                    logger.LogError("Pogreška prilikom brisanja cestovnog objekta: " + exc.CompleteExceptionMessage());
                    responseMessage = new ActionResponseMessage(MessageType.Error, $"Pogreška prilikom brisanja cestovnog objekta: {exc.CompleteExceptionMessage()}");

                }
            }
            else
            {
                responseMessage = new ActionResponseMessage(MessageType.Error, $"Cestovni objekt sa šifrom {id} ne postoji");
            }

            Response.Headers["HX-Trigger"] = JsonSerializer.Serialize(new { showMessage = responseMessage });
            return responseMessage.MessageType == MessageType.Success ? new EmptyResult() : await Get(id);
        }
        [HttpGet]
        public Task<IActionResult> Edit(int id, int? position, string filter, int page = 1, int sort = 1, bool ascending = true)
        {
            return Show(id, filter, page, sort, ascending, viewName: nameof(Edit));
        }

        [HttpPost]
        public async Task<IActionResult> Edit(IvanMasterDetailViewModel masterDetailViewModel, int? position, string filter, int page = 1, int sort = 1, bool ascending = true)
        {
            ViewBag.Page = page;
            ViewBag.Sort = sort;
            ViewBag.Ascending = ascending;
            ViewBag.Filter = filter;
            ViewBag.Position = position;

            if (ModelState.IsValid)
            {
                var cestovniObjekt = await ctx.CestovniObjekt
                                        .Include(co => co.OdrzavanjeObjekta)
                                        .Where(co => co.Id == masterDetailViewModel.Id)
                                        .FirstOrDefaultAsync();
                if (cestovniObjekt == null)
                {
                    logger.LogWarning("Ne postoji cestovni objekt s oznakom: {0} ", masterDetailViewModel.Id);
                    return NotFound("Ne postoji cestovni objekt s id-om: " + masterDetailViewModel.Id);
                }

                CopyValuesMaster(cestovniObjekt, masterDetailViewModel);

                List<int> idOdrzavanjaObjekata = masterDetailViewModel.OdrzavanjeObjekata
                                            .Where(oo => oo.Id > 0)
                                            .Select(s => s.Id)
                                            .ToList();
                ctx.RemoveRange(cestovniObjekt.OdrzavanjeObjekta.Where(oo => !idOdrzavanjaObjekata.Contains(oo.Id)));

                foreach (var odrzavanjeObjekta in masterDetailViewModel.OdrzavanjeObjekata)
                {
                    OdrzavanjeObjekta odrzavanjeObjektaNew;
                    if (odrzavanjeObjekta.Id > 0) {
                        odrzavanjeObjektaNew = await ctx.OdrzavanjeObjekta.Where(co => co.Id == odrzavanjeObjekta.Id).SingleOrDefaultAsync();
                        //cestovniObjekt.OdrzavanjeObjekta.First(oo => oo.Id == odrzavanjeObjekta.Id);
                    }
                    else {
                        odrzavanjeObjektaNew = new OdrzavanjeObjekta();
                    }
                    CopyValuesDetail(odrzavanjeObjektaNew, odrzavanjeObjekta);
                    cestovniObjekt.OdrzavanjeObjekta.Add(odrzavanjeObjektaNew);
                    ctx.Update(odrzavanjeObjektaNew);
                }

                try
                {
                    ctx.Update(cestovniObjekt);
                    await ctx.SaveChangesAsync();
                    logger.LogInformation($"(MD) cestovni objekt: {cestovniObjekt.Naziv} uspješno ažuriran");
                    TempData[Constants.Message] = $"(MD) Cestovni objekt s oznakom: {cestovniObjekt.Id} uspješno ažuriran.";
                    TempData[Constants.ErrorOccurred] = false;
                    return RedirectToAction(nameof(Edit), new
                    {
                        id = cestovniObjekt.Id,
                        position,
                        filter,
                        page,
                        sort,
                        ascending
                    });
                }
                catch (Exception exc)
                {
                    logger.LogError("Pogreška prilikom ažuriranja cestovnog objekta: " + exc.CompleteExceptionMessage());
                    ModelState.AddModelError(string.Empty, exc.CompleteExceptionMessage());
                    return View(masterDetailViewModel);
                }
            }
            else
            {
                return View(masterDetailViewModel);
            }
        }

        [HttpGet]
        public async Task<IActionResult> Get(int id)
        {
            var cestovniObjekt = await ctx.CestovniObjekt
                                  .Where(co => co.Id == id)
                                  .SingleOrDefaultAsync();
            if (cestovniObjekt != null)
            {
                return PartialView(cestovniObjekt);
            }
            else
            {
                return NotFound($"Neispravan id cestovnog objekta: {id}");
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetDetail(int id)
        {
            var odrzavanjeObjekta = await ctx.OdrzavanjeObjekta
                                 .Where(co => co.Id == id)
                                 .SingleOrDefaultAsync();
            if (odrzavanjeObjekta != null)
            {
                return PartialView(odrzavanjeObjekta);
            }
            else
            {
                return NotFound($"Neispravan id održavanja objekta: {id}");
            }
        }

        private static void CopyValuesMaster(CestovniObjekt cestovniObjekt, IvanMasterDetailViewModel cestovniObjektDto)
        {
            cestovniObjekt.Naziv = cestovniObjektDto.NazivCestovnogObjekta;
            cestovniObjekt.DionicaId = cestovniObjektDto.DionicaId;
            cestovniObjekt.TipObjekta = cestovniObjektDto.TipObjekta;
            cestovniObjekt.OgranicenjeBrzine = cestovniObjektDto.OgranicenjeBrzine;
            cestovniObjekt.BrojPrometnihTraka = cestovniObjektDto.BrojPrometnihTraka;
            cestovniObjekt.DuljinaObjekta = cestovniObjektDto.DuljinaObjekta;
            cestovniObjekt.ZaustavniTrak = cestovniObjektDto.ZaustavniTrak;
            cestovniObjekt.DozvolaTeretnimVozilima = cestovniObjektDto.DozvolaTeretnimVozilima;
            cestovniObjekt.Zanimljivost = cestovniObjektDto.Zanimljivost;
            cestovniObjekt.GodinaIzgradnje = cestovniObjektDto.GodinaIzgradnje;
            cestovniObjekt.PjesackaStaza = cestovniObjektDto.PjesackaStaza;
            cestovniObjekt.NaplataPrijelaza = cestovniObjektDto.NaplataPrijelaza;
        }

        private static void CopyValuesDetail(OdrzavanjeObjekta odrzavanjeObjekta, OdrzavanjeObjekta odrzavanjeObjektaRes)
        {
            odrzavanjeObjekta.VrstaId = odrzavanjeObjektaRes.VrstaId;
            odrzavanjeObjekta.ImeFirme = odrzavanjeObjektaRes.ImeFirme;
            odrzavanjeObjekta.RadnimDanomOd = odrzavanjeObjektaRes.RadnimDanomOd;
            odrzavanjeObjekta.RadnimDanomDo = odrzavanjeObjektaRes.RadnimDanomDo;
            odrzavanjeObjekta.VikendimaOd = odrzavanjeObjektaRes.VikendimaOd;
            odrzavanjeObjekta.VikendimaDo = odrzavanjeObjektaRes.VikendimaDo;
            odrzavanjeObjekta.BrojLjudi = odrzavanjeObjektaRes.BrojLjudi;
            odrzavanjeObjekta.Cijena = odrzavanjeObjektaRes.Cijena;
            odrzavanjeObjekta.PredvidenoDana = odrzavanjeObjektaRes.PredvidenoDana;
            odrzavanjeObjekta.CestovniObjektId = odrzavanjeObjektaRes.CestovniObjektId;
        }
    }
}
