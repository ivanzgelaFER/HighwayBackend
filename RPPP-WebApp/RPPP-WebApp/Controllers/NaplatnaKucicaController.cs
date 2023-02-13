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
    public class NaplatnaKucicaController : Controller
    {
        private readonly RPPP04Context ctx;
        private readonly AppSettings appSettings;

        public NaplatnaKucicaController(RPPP04Context ctx, IOptionsSnapshot<AppSettings> options)
        {
            this.ctx = ctx;
            appSettings = options.Value;
        }

        public async Task<IActionResult> Index(int page = 1, int sort = 1, bool ascending = true)
        {
            int pagesize = appSettings.PageSize;

            var query = ctx.NaplatnaKucica
                           .AsNoTracking();

            int count = await query.CountAsync();

            var pagingInfo = new PagingInfo
            {
                CurrentPage = page,
                Sort = sort,
                Ascending = ascending,
                ItemsPerPage = pagesize,
                TotalItems = count
            };
            if (page < 1)
            {
                return RedirectToAction(nameof(Index), new { page = 1, sort, ascending });
            }

            query = query.ApplySort(sort, ascending);

            var naplatneKucice = await query
                                      .Include(oo => oo.NaplatnaPostaja)
                                      .Include(oo => oo.VrstaNaplate)
                                      .Skip((page - 1) * pagesize)
                                      .Take(pagesize)
                                      .ToListAsync();

            var model = new NaplatneKuciceViewModel
            {
                NaplatneKucice = naplatneKucice,
                PagingInfo = pagingInfo
            };

            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int id, int page = 1, int sort = 1, bool ascending = true)
        {
            var naplatnaKucica = await ctx.NaplatnaKucica.FindAsync(id);
            if (naplatnaKucica != null)
            {
                await PrepareDropdownLists();
                ViewBag.Page = page;
                ViewBag.Sort = sort;
                ViewBag.Ascending = ascending;
                return View(naplatnaKucica);
            }
            else
            {
                return NotFound($"Neispravan id naplatne kucice: {id}");
            }
        }



        [HttpDelete]
        public async Task<IActionResult> Delete(int id)
        {
            ActionResponseMessage responseMessage;
            var naplatnaKucica = await ctx.NaplatnaKucica.FindAsync(id);
            if (naplatnaKucica != null)
            {
                try
                {
                    var idDeleted = naplatnaKucica.Id;
                    ctx.Remove(naplatnaKucica);
                    await ctx.SaveChangesAsync();
                    responseMessage = new ActionResponseMessage(MessageType.Success, $"Naplatna kuæica sa šifrom {idDeleted} uspješno obrisana.");
                }
                catch (Exception exc)
                {
                    responseMessage = new ActionResponseMessage(MessageType.Error, $"Pogreška prilikom brisanja naplatne kuæice: {exc.CompleteExceptionMessage()}");
                }
            }
            else
            {
                responseMessage = new ActionResponseMessage(MessageType.Error, $"Naplatna kuæica sa šifrom {id} ne postoji");
            }
            Response.Headers["HX-Trigger"] = JsonSerializer.Serialize(new { showMessage = responseMessage });
            return new EmptyResult();
        }

        [HttpPost]

        public async Task<IActionResult> Edit(NaplatnaKucica naplatnaKucica)
        {
            if (naplatnaKucica == null)
            {
                return NotFound("Nema poslanih podataka");
            }
            bool checkId = await ctx.NaplatnaPostaja.AnyAsync(np => np.Id == naplatnaKucica.Id);
            if (!checkId)
            {
                return NotFound($"Neispravan id mjesta: {naplatnaKucica?.Id}");
            }

            if (ModelState.IsValid)
            {
                try
                {
                    ctx.Update(naplatnaKucica);
                    await ctx.SaveChangesAsync();
                    return RedirectToAction(nameof(Index), new { id = naplatnaKucica.Id });
                }
                catch (Exception exc)
                {
                    ModelState.AddModelError(string.Empty, exc.CompleteExceptionMessage());
                    return PartialView(naplatnaKucica);
                }
            }
            else
            {
                return PartialView(naplatnaKucica);
            }
        }

        [HttpPost]
        public async Task<IActionResult> Create(NaplatnaKucica naplatnaKucica)
        {
            //logger.LogTrace(JsonSerializer.Serialize(autocesta));
            if (ModelState.IsValid)
            {
                try
                {
                    ctx.Add(naplatnaKucica);
                    await ctx.SaveChangesAsync();

                    //logger.LogInformation(new EventId(1000), $"Autocesta {autocesta.Naziv} dodana.");

                    TempData[Constants.Message] = $"Naplatna kucica {naplatnaKucica.Id} dodana.";
                    TempData[Constants.ErrorOccurred] = false;
                    return RedirectToAction(nameof(Index));
                }
                catch (Exception exc)
                {
                    //logger.LogError("Pogreška prilikom dodavanje nove autoceste: {0}", exc.CompleteExceptionMessage());
                    ModelState.AddModelError(string.Empty, exc.CompleteExceptionMessage());
                    return View(naplatnaKucica);
                }
            }
            else
            {
                return View(naplatnaKucica);
            }
        }

        [HttpGet]
        public async Task<IActionResult> Get(int id)
        {
            var naplatnaKucica = await ctx.NaplatnaKucica
                                  .Where(nk => nk.Id == id)
                                  .Select(np => new NaplatnaKucica
                                  {
                                      Id = np.Id,
                                      VrstaNaplateId = np.VrstaNaplateId,
                                      Otvorena = np.Otvorena,
                                      NaplatnaPostajaId = np.NaplatnaPostajaId
                                  })
                                  .SingleOrDefaultAsync();
            if (naplatnaKucica != null)
            {
                return PartialView(naplatnaKucica);
            }
            else
            {
                return NotFound($"Neispravan id naplatne kucice: {id}");
            }
        }

        [HttpGet]
        public async Task<IActionResult> Create()
        {
            await PrepareDropdownLists();
            return View();
        }

       private async Task PrepareDropdownLists()
        {
            var naplatnePostaje = await ctx.NaplatnaPostaja
                            .Select(d => new { d.Naziv, d.Id })
                            .ToListAsync();
            ViewBag.NaplatnaPostaja = new SelectList(naplatnePostaje, nameof(NaplatnaPostaja.Id), nameof(NaplatnaPostaja.Naziv));

            var vrsteNaplate = await ctx.VrstaNaplate
                                .Select(d => new { d.Naziv, d.Id })
                                .ToListAsync();
            ViewBag.VrstaNaplate = new SelectList(vrsteNaplate, nameof(VrstaNaplate.Id), nameof(VrstaNaplate.Naziv));
        }
    }

}