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

namespace RPPP_WebApp.Controllers
{
    public class AutocestaController : Controller
    {
        private readonly RPPP04Context ctx;
        private readonly ILogger<AutocestaController> logger;
        private readonly AppSettings appSettings;

        public AutocestaController(RPPP04Context ctx, IOptionsSnapshot<AppSettings> options, ILogger<AutocestaController> logger)
        {
            this.ctx = ctx;
            this.logger = logger;
            appSettings = options.Value;
        }

        public async Task<IActionResult> Index(int page = 1, int sort = 1, bool ascending = true)
        {
            int pagesize = appSettings.PageSize;

            var query = ctx.Autocesta
                           .AsNoTracking();

            int count = await query.CountAsync();

            /*if (count == 0)
            {
                logger.LogInformation("Ne postoji nijedna autocesta");
                TempData[Constants.Message] = "Ne postoji niti jedna autocesta.";
                TempData[Constants.ErrorOccurred] = false;
                return RedirectToAction(nameof(Create));
            }*/



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
                            Koncesionar = a.Koncesionar.Naziv
                        })
                        .Skip((page - 1) * pagesize)
                        .Take(pagesize)
                        .ToListAsync();

            var model = new AutocesteViewModel
            {
                Autoceste = autoceste,
                PagingInfo = pagingInfo
            };

            return View(model);
        }


        [HttpGet]
        public async Task<IActionResult> Create()
        {
            await PrepareDropDownList();
            return View();
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Autocesta autocesta)
        {
            logger.LogTrace(JsonSerializer.Serialize(autocesta));
            if (ModelState.IsValid)
            {
                try
                {
                    ctx.Add(autocesta);
                    await ctx.SaveChangesAsync();

                    logger.LogInformation(new EventId(1000), $"Autocesta {autocesta.Naziv} dodana.");

                    TempData[Constants.Message] = $"Autocesta {autocesta.Naziv} dodana.";
                    TempData[Constants.ErrorOccurred] = false;
                    return RedirectToAction(nameof(Index));
                }
                catch (Exception exc)
                {
                    logger.LogError("Pogreška prilikom dodavanje nove autoceste: {0}", exc.CompleteExceptionMessage());
                    ModelState.AddModelError(string.Empty, exc.CompleteExceptionMessage());
                    await PrepareDropDownList();
                    return View(autocesta);
                }
            }
            else
            {
                await PrepareDropDownList();
                return View(autocesta);
            }
        }

        private async Task PrepareDropDownList()
        {

            var koncesionari = await ctx.Koncesionar.OrderBy(k => k.Naziv).Select(k => new { k.Id, k.Naziv }).ToListAsync();

            ViewBag.Autoceste = new SelectList(koncesionari, nameof(Koncesionar.Id), nameof(Koncesionar.Naziv));
            // new SelectList(koncesionari, nameof(koncesionari.Id), nameof(koncesionari.Naziv));
        }


        [HttpDelete]
        public async Task<IActionResult> Delete(int id)
        {
            ActionResponseMessage responseMessage;
            var autocesta = await ctx.Autocesta.FindAsync(id);
            if (autocesta != null)
            {
                try
                {
                    string naziv = autocesta.Naziv;
                    ctx.Remove(autocesta);
                    await ctx.SaveChangesAsync();
                    logger.LogInformation($"Autocesta: {naziv} uspješno obrisana");
                    responseMessage = new ActionResponseMessage(MessageType.Success, $"Autocesta {naziv} sa šifrom {id} uspješno obrisana.");
                }
                catch (Exception exc)
                {
                    logger.LogError("Pogreška prilikom brisanja autoceste: " + exc.CompleteExceptionMessage());
                    responseMessage = new ActionResponseMessage(MessageType.Error, $"Pogreška prilikom brisanja autoceste: {exc.CompleteExceptionMessage()}");
                }
            }
            else
            {
                responseMessage = new ActionResponseMessage(MessageType.Error, $"Autocesta sa šifrom {id} ne postoji");
            }
            Response.Headers["HX-Trigger"] = JsonSerializer.Serialize(new { showMessage = responseMessage });
            return new EmptyResult();
        }


        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var autocesta = await ctx.Autocesta.AsNoTracking().Where(a => a.Id == id).SingleOrDefaultAsync();
            if (autocesta != null)
            {
                await PrepareDropDownList();
                return PartialView(autocesta);
            }
            else
            {
                return NotFound($"Neispravan id autoceste: {id}");
            }
        }

        [HttpPost]

        public async Task<IActionResult> Edit(Autocesta autocesta)
        {
            if (autocesta == null)
            {
                return NotFound("Nema poslanih podataka");
            }
            bool checkId = await ctx.Autocesta.AnyAsync(m => m.Id == autocesta.Id);
            if (!checkId)
            {
                return NotFound($"Neispravan id autoceste: {autocesta?.Id}");
            }

            if (ModelState.IsValid)
            {
                try
                {
                    ctx.Update(autocesta);
                    logger.LogInformation($"Autocesta: {autocesta.Naziv} uspješno ažurirana");
                    await ctx.SaveChangesAsync();
                    return RedirectToAction(nameof(Get), new { id = autocesta.Id });
                }
                catch (Exception exc)
                {
                    logger.LogError("Pogreška prilikom ažuriranja autoceste: " + exc.CompleteExceptionMessage());
                    ModelState.AddModelError(string.Empty, exc.CompleteExceptionMessage());
                    await PrepareDropDownList();
                    return PartialView(autocesta);
                }
            }
            else
            {
                await PrepareDropDownList();
                return PartialView(autocesta);
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