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

namespace RPPP_WebApp.Controllers {
    public class VrstaIncidentaController : Controller {
        private readonly RPPP04Context ctx;
        private readonly ILogger<VrstaIncidentaController> logger;
        private readonly AppSettings appSettings;

        public VrstaIncidentaController(RPPP04Context ctx, IOptionsSnapshot<AppSettings> options, ILogger<VrstaIncidentaController> logger) {
            this.ctx = ctx;
            this.logger = logger;
            appSettings = options.Value;
        }

        public IActionResult Index(int page = 1, int sort = 1, bool ascending = true) {
            int pagesize = appSettings.PageSize;

            var query = ctx.VrstaIncidenta
                           .AsNoTracking();

            int count = query.Count();

            if (count == 0) {
                logger.LogInformation("Ne postoji nijedna vrsta incidenta");
                TempData[Constants.Message] = "Ne postoji niti jedna v.";
                TempData[Constants.ErrorOccurred] = false;
                return RedirectToAction(nameof(Create));
            }



            var pagingInfo = new PagingInfo {
                CurrentPage = page,
                Sort = sort,
                Ascending = ascending,
                ItemsPerPage = pagesize,
                TotalItems = count
            };
            if (page < 1 || page > pagingInfo.TotalPages) {
                return RedirectToAction(nameof(Index), new { page = 1, sort, ascending });
            }

            query = query.ApplySort(sort, ascending);

            var vrsteIncidenata = query
                        .Skip((page - 1) * pagesize)
                        .Take(pagesize)
                        .ToList();

            var model = new VrsteIncidenataViewModel {
                VrsteIncidenata = vrsteIncidenata,
                PagingInfo = pagingInfo
            };

            return View(model);
        }


        [HttpGet]
        public IActionResult Create() {
            return View();
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(VrstaIncidenta vrstaIncidenta) {
            logger.LogTrace(JsonSerializer.Serialize(vrstaIncidenta));
            if (ModelState.IsValid) {
                try {
                    ctx.Add(vrstaIncidenta);
                    ctx.SaveChanges();

                    logger.LogInformation(new EventId(1000), $"Vrsta incidenta {vrstaIncidenta.Naziv} dodana.");

                    TempData[Constants.Message] = $"Vrsta incidenta {vrstaIncidenta.Naziv} dodana.";
                    TempData[Constants.ErrorOccurred] = false;
                    return RedirectToAction(nameof(Index));
                } catch (Exception exc) {
                    logger.LogError("Pogreška prilikom dodavanje novog koncesionara: {0}", exc.CompleteExceptionMessage());
                    ModelState.AddModelError(string.Empty, exc.CompleteExceptionMessage());
                    return View(vrstaIncidenta);
                }
            } else {
                return View(vrstaIncidenta);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Delete(int id, int page = 1, int sort = 1, bool ascending = true) {
            var vrstaIncidenta = ctx.VrstaIncidenta.Find(id);
            if (vrstaIncidenta != null) {
                try {
                    string naziv = vrstaIncidenta.Naziv;
                    ctx.Remove(vrstaIncidenta);
                    ctx.SaveChanges();
                    logger.LogInformation($"Vrsta incidenta {naziv} uspješno obrisana");
                    TempData[Constants.Message] = $"Vrsta incidenta {naziv} uspješno obrisana";
                    TempData[Constants.ErrorOccurred] = false;
                } catch (Exception exc) {
                    TempData[Constants.Message] = "Pogreška prilikom brisanja vrste incidenta: " + exc.CompleteExceptionMessage();
                    TempData[Constants.ErrorOccurred] = true;
                    logger.LogError("Pogreška prilikom brisanja vrste incidenta: " + exc.CompleteExceptionMessage());
                }
            } else {
                logger.LogWarning("Ne postoji vrsta incidenta s id: {0} ", id);
                TempData[Constants.Message] = "Ne postoji vrsta incidenta s id: " + id;
                TempData[Constants.ErrorOccurred] = true;
            }
            return RedirectToAction(nameof(Index), new { page = page, sort = sort, ascending = ascending });
        }


        [HttpGet]
        public IActionResult Edit(int id, int page = 1, int sort = 1, bool ascending = true) {
            var vrstaIncidenta = ctx.VrstaIncidenta.AsNoTracking().Where(k => k.Id == id).SingleOrDefault();
            if (vrstaIncidenta == null) {
                logger.LogWarning("Ne postoji vrsta incidenta s id: {0} ", id);
                return NotFound("Ne postoji vrsta incidenta s id: " + id);
            } else {
                ViewBag.Page = page;
                ViewBag.Sort = sort;
                ViewBag.Ascending = ascending;
                return View(vrstaIncidenta);
            }
        }



        [HttpPost, ActionName("Edit")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Update(int id, int page = 1, int sort = 1, bool ascending = true) {

            try {
                VrstaIncidenta vrstaIncidenta = await ctx.VrstaIncidenta
                                  .Where(k => k.Id == id)
                                  .FirstOrDefaultAsync();
                if (vrstaIncidenta == null) {
                    return NotFound("Neispravan Id vrste incidenta: " + id);
                }

                if (await TryUpdateModelAsync<VrstaIncidenta>(vrstaIncidenta, "",
                    k => k.Naziv, k => k.OpisPravilaPonasanja)) {
                        ViewBag.Page = page;
                    ViewBag.Sort = sort;
                    ViewBag.Ascending = ascending;
                    try {
                        await ctx.SaveChangesAsync();
                        TempData[Constants.Message] = "Vrsta incidenta ažuriran.";
                        TempData[Constants.ErrorOccurred] = false;
                        return RedirectToAction(nameof(Index), new { page = page, sort = sort, ascending = ascending });
                    } catch (Exception exc) {
                        ModelState.AddModelError(string.Empty, exc.CompleteExceptionMessage());
                        return View(vrstaIncidenta);
                    }
                } else {
                    ModelState.AddModelError(string.Empty, "Podatke o koncesiji nije moguće povezati s forme");
                    return View(vrstaIncidenta);
                }
            } catch (Exception exc) {
                TempData[Constants.Message] = exc.CompleteExceptionMessage();
                TempData[Constants.ErrorOccurred] = true;
                return RedirectToAction(nameof(Edit), id);
            }
        }

    }
}
