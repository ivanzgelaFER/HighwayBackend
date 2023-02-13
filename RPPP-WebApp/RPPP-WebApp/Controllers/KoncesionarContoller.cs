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
    public class KoncesionarController : Controller
    {
        private readonly RPPP04Context ctx;
        private readonly ILogger<KoncesionarController> logger;
        private readonly AppSettings appSettings;

        public KoncesionarController(RPPP04Context ctx, IOptionsSnapshot<AppSettings> options, ILogger<KoncesionarController> logger)
        {
            this.ctx = ctx;
            this.logger = logger;
            appSettings = options.Value;
        }

        public IActionResult Index(int page = 1, int sort = 1, bool ascending = true)
        {
            int pagesize = appSettings.PageSize;

            var query = ctx.Koncesionar
                           .AsNoTracking();

            int count = query.Count();

            if (count == 0)
            {
                logger.LogInformation("Ne postoji nijedan koncesionar");
                TempData[Constants.Message] = "Ne postoji niti jedan koncesionar.";
                TempData[Constants.ErrorOccurred] = false;
                return RedirectToAction(nameof(Create));
            }



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

            var koncesionari = query
                        .Skip((page - 1) * pagesize)
                        .Take(pagesize)
                        .ToList();

            var model = new KoncesionariViewModel
            {
                Koncesionari = koncesionari,
                PagingInfo = pagingInfo
            };

            return View(model);
        }


        [HttpGet]
        public IActionResult Create()
        {
            //await PrepareDropDownList();
            return View();
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(Koncesionar koncesionar)
        {
            logger.LogTrace(JsonSerializer.Serialize(koncesionar));
            if (ModelState.IsValid)
            {
                try
                {
                    ctx.Add(koncesionar);
                    ctx.SaveChanges();

                    logger.LogInformation(new EventId(1000), $"Koncesionar {koncesionar.Naziv} dodan.");

                    TempData[Constants.Message] = $"Koncesionar {koncesionar.Naziv} dodan.";
                    TempData[Constants.ErrorOccurred] = false;
                    return RedirectToAction(nameof(Index));
                }
                catch (Exception exc)
                {
                    logger.LogError("Pogreška prilikom dodavanje novog koncesionara: {0}", exc.CompleteExceptionMessage());
                    ModelState.AddModelError(string.Empty, exc.CompleteExceptionMessage());
                    //await PrepareDropDownList();
                    return View(koncesionar);
                }
            }
            else
            {
                //await PrepareDropDownList();
                return View(koncesionar);
            }
        }

        /*private async Task PrepareDropDownList()
        {

            var autoceste = await ctx.Koncesionar.OrderBy(a => a.Naziv).Select(a => new { a.Id, a.Naziv }).ToListAsync();

            ViewBag.Koncesionari = new SelectList(autoceste, nameof(Autocesta.Id), nameof(Autocesta.Naziv));
            // ako zatreba
        }


        [HttpDelete]
        public async Task<IActionResult> Delete(int id)
        {
            ActionResponseMessage responseMessage;
            var koncesionar = await ctx.Koncesionar.FindAsync(id);
            if (koncesionar != null)
            {
                try
                {
                    string naziv = koncesionar.Naziv;
                    ctx.Remove(koncesionar);
                    await ctx.SaveChangesAsync();
                    responseMessage = new ActionResponseMessage(MessageType.Success, $"Koncesionar {naziv} sa šifrom {id} uspješno obrisan.");
                }
                catch (Exception exc)
                {
                    responseMessage = new ActionResponseMessage(MessageType.Error, $"Pogreška prilikom brisanja koncesionara: {exc.CompleteExceptionMessage()}");
                }
            }
            else
            {
                responseMessage = new ActionResponseMessage(MessageType.Error, $"Koncesionar sa šifrom {id} ne postoji");
            }
            Response.Headers["HX-Trigger"] = JsonSerializer.Serialize(new { showMessage = responseMessage });
            return new EmptyResult();
        } */

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Delete(int id, int page = 1, int sort = 1, bool ascending = true)
        {
            var koncesionar = ctx.Koncesionar.Find(id);
            if (koncesionar != null)
            {
                try
                {
                    string naziv = koncesionar.Naziv;
                    ctx.Remove(koncesionar);
                    ctx.SaveChanges();
                    logger.LogInformation($"Koncesionar {naziv} uspješno obrisan");
                    TempData[Constants.Message] = $"Koncesionar {naziv} uspješno obrisan";
                    TempData[Constants.ErrorOccurred] = false;
                }
                catch (Exception exc)
                {
                    TempData[Constants.Message] = "Pogreška prilikom brisanja koncesionara: " + exc.CompleteExceptionMessage();
                    TempData[Constants.ErrorOccurred] = true;
                    logger.LogError("Pogreška prilikom brisanja koncesionara: " + exc.CompleteExceptionMessage());
                }
            }
            else
            {
                logger.LogWarning("Ne postoji koncesionar s id: {0} ", id);
                TempData[Constants.Message] = "Ne postoji koncesionar s id: " + id;
                TempData[Constants.ErrorOccurred] = true;
            }
            return RedirectToAction(nameof(Index), new { page = page, sort = sort, ascending = ascending });
        }

        /*
        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var koncesionar = await ctx.Koncesionar.AsNoTracking().Where(k => k.Id == id).SingleOrDefaultAsync();
            if (koncesionar != null)
            {
                await PrepareDropDownList();
                return PartialView(koncesionar);
            }
            else
            {
                return NotFound($"Neispravan id koncesionara: {id}");
            }
        }

        [HttpPost]

        public async Task<IActionResult> Edit(Koncesionar koncesionar)
        {
            if (koncesionar == null)
            {
                return NotFound("Nema poslanih podataka");
            }
            bool checkId = await ctx.Koncesionar.AnyAsync(m => m.Id == koncesionar.Id);
            if (!checkId)
            {
                return NotFound($"Neispravan id mjesta: {koncesionar?.Id}");
            }

            if (ModelState.IsValid)
            {
                try
                {
                    ctx.Update(koncesionar);
                    await ctx.SaveChangesAsync();
                    return RedirectToAction(nameof(Get), new { id = koncesionar.Id });
                }
                catch (Exception exc)
                {
                    ModelState.AddModelError(string.Empty, exc.CompleteExceptionMessage());
                    await PrepareDropDownList();
                    return PartialView(koncesionar);
                }
            }
            else
            {
                await PrepareDropDownList();
                return PartialView(koncesionar);
            }
        } */


        [HttpGet]
        public IActionResult Edit(int id, int page = 1, int sort = 1, bool ascending = true)
        {
            var koncesionar = ctx.Koncesionar.AsNoTracking().Where(k => k.Id == id).SingleOrDefault();
            if (koncesionar == null)
            {
                logger.LogWarning("Ne postoji koncesionar s id: {0} ", id);
                return NotFound("Ne postoji koncesionar s id: " + id);
            }
            else
            {
                ViewBag.Page = page;
                ViewBag.Sort = sort;
                ViewBag.Ascending = ascending;
                return View(koncesionar);
            }
        }


        
            [HttpPost, ActionName("Edit")]
            [ValidateAntiForgeryToken]
            public async Task<IActionResult> Update(int id, int page = 1, int sort = 1, bool ascending = true)
            {
                //za različite mogućnosti ažuriranja pogledati
                //attach, update, samo id, ...
                //https://docs.microsoft.com/en-us/aspnet/core/data/ef-mvc/crud#update-the-edit-page

                try
                {
                    Koncesionar koncesionar = await ctx.Koncesionar
                                      .Where(k => k.Id == id)
                                      .FirstOrDefaultAsync();
                    if (koncesionar == null)
                    {
                        return NotFound("Neispravan Id koncesionara: " + id);
                    }

                    if (await TryUpdateModelAsync<Koncesionar>(koncesionar, "",
                       k => k.Naziv, k => k.Adresa, k => k.Email, k => k.KoncesijaOd, k => k.KoncesijaDo
                    ))
                    {
                        ViewBag.Page = page;
                        ViewBag.Sort = sort;
                        ViewBag.Ascending = ascending;
                        try
                        {
                            await ctx.SaveChangesAsync();
                        logger.LogInformation($"Koncesionar: {koncesionar.Naziv} uspješno ažuriran");
                        TempData[Constants.Message] = "Koncesionar ažuriran.";
                            TempData[Constants.ErrorOccurred] = false;
                            return RedirectToAction(nameof(Index), new { page = page, sort = sort, ascending = ascending });
                        }
                        catch (Exception exc)
                        {
                        logger.LogError("Pogreška prilikom ažuriranja koncesionara: " + exc.CompleteExceptionMessage());
                        ModelState.AddModelError(string.Empty, exc.CompleteExceptionMessage());
                            return View(koncesionar);
                        }
                    }
                    else
                    {
                        ModelState.AddModelError(string.Empty, "Podatke o koncesiji nije moguće povezati s forme");
                        return View(koncesionar);
                    }
                }
                catch (Exception exc)
                {
                    TempData[Constants.Message] = exc.CompleteExceptionMessage();
                    TempData[Constants.ErrorOccurred] = true;
                    return RedirectToAction(nameof(Edit), id);
                }
            }

        /*


        [HttpGet]
        public async Task<IActionResult> Get(int id)
        {
            var koncesionar = await ctx.Koncesionar
                                  .Where(k => k.Id == id)
                                  .SingleOrDefaultAsync();
            if (koncesionar != null)
            {
                return PartialView(koncesionar);
            }
            else
            {
                return NotFound($"Neispravan id autoceste: {id}");
            }
        }
        
        */

    }
}
    