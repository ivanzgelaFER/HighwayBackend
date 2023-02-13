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
    public class OdrzavanjeObjektaController : Controller
    {
        private readonly RPPP04Context ctx;
        private readonly AppSettings appData;
        private readonly ILogger<CestovniObjektController> logger;


        public OdrzavanjeObjektaController(RPPP04Context ctx, IOptionsSnapshot<AppSettings> options, ILogger<CestovniObjektController> logger)
        {
            appData = options.Value;
            this.ctx = ctx;
            this.logger = logger;
        }

        public async Task<IActionResult> Index(int page = 1, int sort = 1, bool ascending = true)
        {

            var query = ctx.OdrzavanjeObjekta.AsQueryable();
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

            var odrzavanjaObjekata = await query
                                      .Include(oo => oo.Vrsta)
                                      .Include(oo => oo.CestovniObjekt)
                                      .Skip((page - 1) * pagesize)
                                      .Take(pagesize)
                                      .ToListAsync();

            var model = new OdrzavanjeObjekataViewModel
            {
                OdrzavanjaObjekata = odrzavanjaObjekata,
                PagingInfo = pagingInfo
            };

            return View(model);
        }
        [HttpGet]
        public async Task<IActionResult> Get(int id)
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

        [HttpGet]
        public async Task<IActionResult> Create()
        {
            await PrepareDropDownLists();
            return View();
        }


        [HttpPost]
        public async Task<IActionResult> Create(OdrzavanjeObjekta odrzavanjeObjekta)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    ctx.Add(odrzavanjeObjekta);
                    await ctx.SaveChangesAsync();
                    logger.LogInformation(new EventId(1000), $"Održavanje objekta s oznakom {odrzavanjeObjekta.Id} dodano.");
                    TempData[Constants.Message] = $"Održavanje objekta s oznakom {odrzavanjeObjekta.Id} dodano.";
                    TempData[Constants.ErrorOccurred] = false;
                    return RedirectToAction(nameof(Index));
                }
                catch (Exception exc)
                {
                    logger.LogError("Pogreška prilikom dodavanje novog održavanja objekta: {0}", exc.CompleteExceptionMessage());
                    ModelState.AddModelError(string.Empty, exc.CompleteExceptionMessage());
                    await PrepareDropDownLists();
                    return View(odrzavanjeObjekta);
                }
            }
            else
            {
                await PrepareDropDownLists();
                return View(odrzavanjeObjekta);
            }
        }

        private async Task PrepareDropDownLists()
        {
            var vrsteOdrzavanja = await ctx.VrstaOdrzavanja
                            .Select(d => new { d.Naziv, d.Id })
                            .ToListAsync();
            ViewBag.VrstaOdrzavanja = new SelectList(vrsteOdrzavanja, nameof(VrstaOdrzavanja.Id), nameof(VrstaOdrzavanja.Naziv));

            var cestovniObjekti = await ctx.CestovniObjekt
                            .Select(d => new { d.Naziv, d.Id })
                            .ToListAsync();
            ViewBag.CestovniObjekti = new SelectList(cestovniObjekti, nameof(CestovniObjekt.Id), nameof(CestovniObjekt.Naziv));
        }

        [HttpDelete]
        public async Task<IActionResult> Delete(int id)
        {
            var odrzavanjeObjekta = await ctx.OdrzavanjeObjekta.FindAsync(id);
            ActionResponseMessage responseMessage;
            if (odrzavanjeObjekta != null)
            {
                try
                {
                    ctx.Remove(odrzavanjeObjekta);
                    await ctx.SaveChangesAsync();
                    logger.LogInformation($"Održavanje objekta: oznaka {odrzavanjeObjekta.Id} uspješno obrisano");
                    responseMessage = new ActionResponseMessage(MessageType.Success, $"Održavanje objekta s oznakom {id} uspješno obrisano.");
                }
                catch (Exception exc)
                {
                    responseMessage = new ActionResponseMessage(MessageType.Error, $"Pogreška prilikom brisanja održavanja objekta: {exc.CompleteExceptionMessage()}");
                }
            }
            else
            {
                responseMessage = new ActionResponseMessage(MessageType.Error, $"Održavanje objekta sa šifrom {id} ne postoji");
            }

            Response.Headers["HX-Trigger"] = JsonSerializer.Serialize(new { showMessage = responseMessage });
            return responseMessage.MessageType == MessageType.Success ? new EmptyResult() : await Get(id);
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int id, int page = 1, int sort = 1, bool ascending = true)
        {
            var odrzavanjeObjekta = await ctx.OdrzavanjeObjekta.FindAsync(id);
            if (odrzavanjeObjekta != null)
            {
                await PrepareDropDownLists();
                ViewBag.Page = page;
                ViewBag.Sort = sort;
                ViewBag.Ascending = ascending;
                return View(odrzavanjeObjekta);
            }
            else
            {
                return NotFound($"Neispravan id održavanja objekta: {id}");
            }
        }

        [HttpPost]
        public async Task<IActionResult> Edit(OdrzavanjeObjekta odrzavanjeObjektaRes, int page = 1, int sort = 1, bool ascending = true)
        {
            if (odrzavanjeObjektaRes == null)
            {
                return NotFound("Nema poslanih podataka");
            }
            var odrzavanjeObjekta = await ctx.OdrzavanjeObjekta.FindAsync(odrzavanjeObjektaRes.Id);

            if (odrzavanjeObjekta == null)
            {
                logger.LogWarning("Ne postoji održavanje objekta s oznakom: " + odrzavanjeObjektaRes.Id);
                return NotFound("Ne postoji održavanje objekta s oznakom: " + odrzavanjeObjektaRes.Id);
            }

            ViewBag.Page = page;
            ViewBag.Sort = sort;
            ViewBag.Ascending = ascending;

            if (ModelState.IsValid)
            {
                try
                {
                    CopyValues(odrzavanjeObjekta, odrzavanjeObjektaRes);
                    ctx.Update(odrzavanjeObjekta);
                    await ctx.SaveChangesAsync();
                    //return RedirectToAction(nameof(Index), new { id = cestovniObjekt.Id });
                    logger.LogInformation($"Održavanje objekta: oznaka {odrzavanjeObjekta.Id} uspješno ažurirano");
                    TempData[Constants.Message] = $"Održavanje objekta s oznakom {odrzavanjeObjekta.Id} uspješno ažurirano";
                    TempData[Constants.ErrorOccurred] = false;
                    return RedirectToAction(nameof(Index), new { page = page, sort = sort, ascending = ascending });
                }
                catch (Exception exc)
                {
                    ModelState.AddModelError(string.Empty, exc.CompleteExceptionMessage());
                    await PrepareDropDownLists();
                    return View(odrzavanjeObjekta);
                }
            }
            else
            {
                await PrepareDropDownLists();
                return View(odrzavanjeObjekta);
            }
        }

        private void CopyValues(OdrzavanjeObjekta odrzavanjeObjekta, OdrzavanjeObjekta odrzavanjeObjektaRes)
        {
            //ovdje bi elegantnije rijesenje bilo preko nekog autoMappera, ali trenutno sam ostavio ovako
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
