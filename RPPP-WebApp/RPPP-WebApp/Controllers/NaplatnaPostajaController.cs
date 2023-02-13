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
    public class NaplatnaPostajaController : Controller
    {
        private readonly RPPP04Context ctx;
        private readonly ILogger<NaplatnaPostajaController> logger;
        private readonly AppSettings appSettings;

        public NaplatnaPostajaController(RPPP04Context ctx, IOptionsSnapshot<AppSettings> options, ILogger<NaplatnaPostajaController> logger)
        {
            this.ctx = ctx;
            this.logger = logger;
            appSettings = options.Value;
        }

        public async Task<IActionResult> Index(int page = 1, int sort = 1, bool ascending = true)
        {
            int pagesize = appSettings.PageSize;

            var query = ctx.NaplatnaPostaja
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

            var naplatnePostaje = await query
                        .Select(np => new NaplatnaPostajaViewModel
                        {
                            Id = np.Id,
                            Naziv = np.Naziv,
                            Autocesta = np.Autocesta.Naziv,
                            KoordinataX= np.KoordinataX,
                            KoordinataY= np.KoordinataY,
                            GodinaOtvaranja = np.GodinaOtvaranja
                        })
                        .Skip((page - 1) * pagesize)
                        .Take(pagesize)
                        .ToListAsync();

            var model = new NaplatnePostajeViewModel
            {
                NaplatnePostaje = naplatnePostaje,
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
        public async Task<IActionResult> Create(NaplatnaPostaja naplatnaPostaja)
        {
            logger.LogTrace(JsonSerializer.Serialize(naplatnaPostaja));
            if (ModelState.IsValid)
            {
                try
                {
                    ctx.Add(naplatnaPostaja);
                    await ctx.SaveChangesAsync();

                    logger.LogInformation(new EventId(1000), $"Naplatna postaja {naplatnaPostaja.Naziv} dodana.");

                    TempData[Constants.Message] = $"Naplatna postaja {naplatnaPostaja.Naziv} dodana.";
                    TempData[Constants.ErrorOccurred] = false;
                    return RedirectToAction(nameof(Index));
                }
                catch (Exception exc)
                {
                    logger.LogError("Pogreška prilikom dodavanje nove naplatne postaje: {0}", exc.CompleteExceptionMessage());
                    ModelState.AddModelError(string.Empty, exc.CompleteExceptionMessage());
                    await PrepareDropDownList();
                    return View(naplatnaPostaja);
                }
            }
            else
            {
                await PrepareDropDownList();
                return View(naplatnaPostaja);
            }
        }

        private async Task PrepareDropDownList()
        {

            var autoceste = await ctx.Autocesta.OrderBy(a => a.Naziv).Select(a => new { a.Id, a.Naziv }).ToListAsync();

            ViewBag.NaplatnePostaje = new SelectList(autoceste, nameof(Autocesta.Id), nameof(Autocesta.Naziv));
        }


        [HttpDelete]
        public async Task<IActionResult> Delete(int id)
        {
            ActionResponseMessage responseMessage;
            var naplatnaPostaja = await ctx.NaplatnaPostaja.FindAsync(id);
            if (naplatnaPostaja != null)
            {
                try
                {
                    string naziv = naplatnaPostaja.Naziv;
                    ctx.Remove(naplatnaPostaja);
                    await ctx.SaveChangesAsync();
                    logger.LogInformation($"Naplatna postaja: {naziv} uspješno obrisana");
                    responseMessage = new ActionResponseMessage(MessageType.Success, $"Naplatna postaja {naziv} sa šifrom {id} uspješno obrisana.");
                }
                catch (Exception exc)
                {
                    logger.LogError("Pogreška prilikom brisanja naplatne postaje: " + exc.CompleteExceptionMessage());
                    responseMessage = new ActionResponseMessage(MessageType.Error, $"Pogreška prilikom brisanja naplatne postaje: {exc.CompleteExceptionMessage()}");
                }
            }
            else
            {
                responseMessage = new ActionResponseMessage(MessageType.Error, $"Naplatna postaja sa šifrom {id} ne postoji");
            }
            Response.Headers["HX-Trigger"] = JsonSerializer.Serialize(new { showMessage = responseMessage });
            return new EmptyResult();
        }


        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var naplatnaPostaja = await ctx.NaplatnaPostaja.AsNoTracking().Where(np => np.Id == id).SingleOrDefaultAsync();
            if (naplatnaPostaja != null)
            {
                await PrepareDropDownList();
                return PartialView(naplatnaPostaja);
            }
            else
            {
                return NotFound($"Neispravan id naplatne postaje: {id}");
            }
        }

        [HttpPost]

        public async Task<IActionResult> Edit(NaplatnaPostaja naplatnaPostaja)
        {
            if (naplatnaPostaja == null)
            {
                return NotFound("Nema poslanih podataka");
            }
            bool checkId = await ctx.NaplatnaPostaja.AnyAsync(np => np.Id == naplatnaPostaja.Id);
            if (!checkId)
            {
                return NotFound($"Neispravan id mjesta: {naplatnaPostaja?.Id}");
            }

            if (ModelState.IsValid)
            {
                try
                {
                    ctx.Update(naplatnaPostaja);
                    await ctx.SaveChangesAsync();
                    logger.LogInformation($"Naplatna postaja: {naplatnaPostaja.Naziv} uspješno ažurirana");
                    return RedirectToAction(nameof(Get), new { id = naplatnaPostaja.Id });
                }
                catch (Exception exc)
                {
                    logger.LogError("Pogreška prilikom ažuriranja naplatne postaje: " + exc.CompleteExceptionMessage());
                    ModelState.AddModelError(string.Empty, exc.CompleteExceptionMessage());
                    await PrepareDropDownList();
                    return PartialView(naplatnaPostaja);
                }
            }
            else
            {
                await PrepareDropDownList();
                return PartialView(naplatnaPostaja);
            }
        }

        /*

            [HttpPost, ActionName("Edit")]
            [ValidateAntiForgeryToken]
            public async Task<IActionResult> Update(int id, int page = 1, int sort = 1, bool ascending = true)
            {
                //za različite mogućnosti ažuriranja pogledati
                //attach, update, samo id, ...
                //https://docs.microsoft.com/en-us/aspnet/core/data/ef-mvc/crud#update-the-edit-page

                try
                {
                    Autocesta autocesta = await ctx.Autocesta
                                      .Where(a => a.Id == id)
                                      .FirstOrDefaultAsync();
                    if (autocesta == null)
                    {
                        return NotFound("Neispravan Id autoceste: " + id);
                    }

                    if (await TryUpdateModelAsync<Autocesta>(autocesta, ""
                       // d => d.NazDrzave, d => d.SifDrzave, d => d.Iso3drzave
                    ))
                    {
                        ViewBag.Page = page;
                        ViewBag.Sort = sort;
                        ViewBag.Ascending = ascending;
                        try
                        {
                            await ctx.SaveChangesAsync();
                            TempData[Constants.Message] = "Autocesta ažurirana.";
                            TempData[Constants.ErrorOccurred] = false;
                            return RedirectToAction(nameof(Index), new { page = page, sort = sort, ascending = ascending });
                        }
                        catch (Exception exc)
                        {
                            ModelState.AddModelError(string.Empty, exc.CompleteExceptionMessage());
                            return View(autocesta);
                        }
                    }
                    else
                    {
                        ModelState.AddModelError(string.Empty, "Podatke o autocesti nije moguće povezati s forme");
                        return View(autocesta);
                    }
                }
                catch (Exception exc)
                {
                    TempData[Constants.Message] = exc.CompleteExceptionMessage();
                    TempData[Constants.ErrorOccurred] = true;
                    return RedirectToAction(nameof(Edit), id);
                }
            }
        */



        [HttpGet]
        public async Task<IActionResult> Get(int id)
        {
            var naplatnaPostaja = await ctx.NaplatnaPostaja
                                  .Where(np => np.Id == id)
                                  .Select(np => new NaplatnaPostajaViewModel
                                  {
                                      Id = np.Id,
                                      Naziv = np.Naziv,
                                      Autocesta = np.Autocesta.Naziv,
                                      KoordinataX = np.KoordinataX,
                                      KoordinataY = np.KoordinataY,
                                      GodinaOtvaranja = np.GodinaOtvaranja
                                  })
                                  .SingleOrDefaultAsync();
            if (naplatnaPostaja != null)
            {
                return PartialView(naplatnaPostaja);
            }
            else
            {
                return NotFound($"Neispravan id naplatne postaje: {id}");
            }
        }
    }
}
