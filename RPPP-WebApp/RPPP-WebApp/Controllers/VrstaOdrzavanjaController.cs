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
    public class VrstaOdrzavanjaController : Controller
    {
        private readonly RPPP04Context ctx;
        private readonly AppSettings appData;
        private readonly ILogger<CestovniObjektController> logger;

        public VrstaOdrzavanjaController(RPPP04Context ctx, IOptionsSnapshot<AppSettings> options, ILogger<CestovniObjektController> logger)
        {
            appData = options.Value;
            this.ctx = ctx;
            this.logger = logger;
        }
        public async Task<IActionResult> Index(int page = 1, int sort = 1, bool ascending = true)
        {

            var query = ctx.VrstaOdrzavanja.AsQueryable();
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

            var vrsteOdrzavanja = await query
                                      .Skip((page - 1) * pagesize)
                                      .Take(pagesize)
                                      .ToListAsync();

            var model = new VrsteOdrzavanjaViewModel
            {
                VrsteOdrzavanja = vrsteOdrzavanja,
                PagingInfo = pagingInfo
            };

            return View(model);
        }
        [HttpGet]
        public async Task<IActionResult> Get(int id)
        {
            var vrstaOdrzavanja = await ctx.VrstaOdrzavanja
                                  .Where(vo => vo.Id == id)
                                  .SingleOrDefaultAsync();
            if (vrstaOdrzavanja != null)
            {
                return PartialView(vrstaOdrzavanja);
            }
            else
            {
                return NotFound($"Neispravan id vrste održavanja: {id}");
            }
        }

        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }


        [HttpPost]
        public async Task<IActionResult> Create(VrstaOdrzavanja vrstaOdrzavanja)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    ctx.Add(vrstaOdrzavanja);
                    await ctx.SaveChangesAsync();
                    logger.LogInformation(new EventId(1000), $"Vrsta održavanja s oznakom {vrstaOdrzavanja.Id} uspješno dodana");
                    TempData[Constants.Message] = $"Vrsta održavanja s oznakom {vrstaOdrzavanja.Id} uspješno dodana";
                    TempData[Constants.ErrorOccurred] = false;
                    return RedirectToAction(nameof(Index));
                }
                catch (Exception exc)
                {
                    logger.LogError("Pogreška prilikom dodavanja nove vrste održavanja: {0}", exc.CompleteExceptionMessage());
                    ModelState.AddModelError(string.Empty, exc.CompleteExceptionMessage());
                    return View(vrstaOdrzavanja);
                }
            }
            else
            {
                return View(vrstaOdrzavanja);
            }
        }

        [HttpDelete]
        public async Task<IActionResult> Delete(int id)
        {
            var vrstaOdrzavanja = await ctx.VrstaOdrzavanja.FindAsync(id);
            ActionResponseMessage responseMessage;
            if (vrstaOdrzavanja != null)
            {
                try
                {
                    ctx.Remove(vrstaOdrzavanja);
                    await ctx.SaveChangesAsync();
                    logger.LogInformation($"Vrsta održavanja: {vrstaOdrzavanja.Naziv} uspješno obrisana");
                    responseMessage = new ActionResponseMessage(MessageType.Success, $"Vrsta održavanja: {vrstaOdrzavanja.Naziv} uspješno obrisana");
                }
                catch (Exception exc)
                {
                    logger.LogError("Pogreška prilikom brisanja vrste održavanja: " + exc.CompleteExceptionMessage());
                    responseMessage = new ActionResponseMessage(MessageType.Error, $"Pogreška prilikom brisanja vrste održavanja: {exc.CompleteExceptionMessage()}");
                }
            }
            else
            {
                responseMessage = new ActionResponseMessage(MessageType.Error, $"Vrsta održavanja sa šifrom {id} ne postoji");
            }

            Response.Headers["HX-Trigger"] = JsonSerializer.Serialize(new { showMessage = responseMessage });
            return responseMessage.MessageType == MessageType.Success ? new EmptyResult() : await Get(id);
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int id, int page = 1, int sort = 1, bool ascending = true)
        {
            var vrstaOdrzavanja = await ctx.VrstaOdrzavanja.FindAsync(id);
            if (vrstaOdrzavanja != null)
            {
                ViewBag.Page = page;
                ViewBag.Sort = sort;
                ViewBag.Ascending = ascending;
                return View(vrstaOdrzavanja);
            }
            else
            {
                return NotFound($"Neispravan id vrste održavanja: {id}");
            }
        }

        [HttpPost]
        public async Task<IActionResult> Edit(VrstaOdrzavanja vrstaOdrzavanjaRes, int page = 1, int sort = 1, bool ascending = true)
        {
            if (vrstaOdrzavanjaRes == null)
            {
                return NotFound("Nema poslanih podataka");
            }
            var vrstaOdrzavanja = await ctx.VrstaOdrzavanja.FindAsync(vrstaOdrzavanjaRes.Id);

            if (vrstaOdrzavanja == null)
            {
                logger.LogWarning("Ne postoji vrsta održavanja s oznakom: " + vrstaOdrzavanjaRes.Id);
                return NotFound("Ne postoji vrsta održavanja s oznakom: " + vrstaOdrzavanjaRes.Id);
            }

            ViewBag.Page = page;
            ViewBag.Sort = sort;
            ViewBag.Ascending = ascending;

            if (ModelState.IsValid)
            {
                try
                {
                    CopyValues(vrstaOdrzavanja, vrstaOdrzavanjaRes);
                    ctx.Update(vrstaOdrzavanja);
                    await ctx.SaveChangesAsync();
                    //return RedirectToAction(nameof(Index), new { id = cestovniObjekt.Id });
                    logger.LogInformation($"Vrsta održavanja s oznakom {vrstaOdrzavanja.Id} uspješno ažurirana");
                    TempData[Constants.Message] = $"Vrsta održavanja s oznakom {vrstaOdrzavanja.Id} uspješno ažurirana";
                    TempData[Constants.ErrorOccurred] = false;
                    return RedirectToAction(nameof(Index), new { page = page, sort = sort, ascending = ascending });
                }
                catch (Exception exc)
                {
                    logger.LogError("Pogreška prilikom ažuriranja vrste održavanja: " + exc.CompleteExceptionMessage());
                    ModelState.AddModelError(string.Empty, exc.CompleteExceptionMessage());
                    return View(vrstaOdrzavanja);
                }
            }
            else
            {
                return View(vrstaOdrzavanja);
            }
        }

        private void CopyValues(VrstaOdrzavanja vrstaOdrzavanja, VrstaOdrzavanja vrstaOdrzavanjaRes)
        {
            //ovdje bi elegantnije rijesenje bilo preko nekog autoMappera, ali trenutno sam ostavio ovako
            vrstaOdrzavanja.Naziv = vrstaOdrzavanjaRes.Naziv;
            vrstaOdrzavanja.Izvanredno = vrstaOdrzavanjaRes.Izvanredno;
            vrstaOdrzavanja.Preventivno = vrstaOdrzavanjaRes.Preventivno;
            vrstaOdrzavanja.Periodicnost = vrstaOdrzavanjaRes.Periodicnost;
            vrstaOdrzavanja.GodisnjeDoba = vrstaOdrzavanjaRes.GodisnjeDoba;
        } 
    }
}
