using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using RPPP_WebApp.Extensions;
using RPPP_WebApp.ViewModels;
using RPPP_WebApp.Extensions.Selectors;
using System.Text.Json;
using RPPP_WebApp.Models;
using Microsoft.AspNetCore.Mvc.Rendering;
using NLog.LayoutRenderers;

namespace RPPP_WebApp.Controllers
{
    public class ProlazakVozilaController : Controller
    {
        private readonly RPPP04Context ctx;
        private readonly AppSettings appSettings;

        public ProlazakVozilaController(RPPP04Context ctx, IOptionsSnapshot<AppSettings> options)
        {
            this.ctx = ctx;
            appSettings = options.Value;
        }

        public async Task<IActionResult> Index(int page = 1, int sort = 1, bool ascending = true)
        {
            int pagesize = appSettings.PageSize;

            var query = ctx.ProlazakVozila.AsNoTracking();
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

            var prolazakVozila = await query
                          .Select(vp => new ProlazakVozila
                          {
                              Id = vp.Id,
                              RegistracijskaOznaka = vp.RegistracijskaOznaka,
                              KategorijaVozilaId = vp.KategorijaVozilaId,
                              VrijemeProlaska = vp.VrijemeProlaska,
                              NaplatnaKucica = vp.NaplatnaKucica
                          })
                          .Skip((page - 1) * pagesize)
                          .Take(pagesize)
                          .ToListAsync();

            var model = new ProlaziVozilaViewModel
            {
                ProlaziVozila = ProlazakVozila.ConvertToProlazakVozilaTS(prolazakVozila),
                PagingInfo = pagingInfo
            };

            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> Create()
        {
            await PrepareDropdownLists();
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(ProlazakVozilaTS prolazakVozilaTS)
        {
            var prolazakVozila = prolazakVozilaTS.ConvertToProlazakVozila();

            if (ModelState.IsValid)
            {
                try
                {
                    ctx.Add(prolazakVozila);
                    await ctx.SaveChangesAsync();

                    TempData[Constants.Message] = $"Prolazak vozila {prolazakVozila.Id} dodan.";
                    TempData[Constants.ErrorOccurred] = false;
                    return RedirectToAction(nameof(Index));
                }
                catch (Exception exc)
                {
                    ModelState.AddModelError(string.Empty, exc.CompleteExceptionMessage());
                    return View(prolazakVozila);
                }
            }
            else
            {
                return View(prolazakVozila);
            }
        }

        [HttpDelete]
        public async Task<IActionResult> Delete(int id)
        {
            ActionResponseMessage response;
            var prolazakVozila
                = await ctx.ProlazakVozila.FindAsync(id);

            if (prolazakVozila != null)
            {
                try
                {
                    ctx.Remove(prolazakVozila);
                    await ctx.SaveChangesAsync();
                    response = new ActionResponseMessage(MessageType.Success, $"Prolazak vozila {prolazakVozila.Id} uspješno obrisan.");
                }
                catch (Exception exc)
                {
                    response = new ActionResponseMessage(MessageType.Error, "Pogreška prilikom brisanja prolaska vozila: " + exc.CompleteExceptionMessage());

                }
            }
            else
            {
                response = new ActionResponseMessage(MessageType.Error, "Ne postoji prolaz vozila s id-om: " + id);

            }
            Response.Headers["HX-Trigger"] = JsonSerializer.Serialize(new { showMessage = response });
            return response.MessageType == MessageType.Success ? new EmptyResult() : await Get(id);
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {

            var vrstaNaplate = await ctx.VrstaNaplate.FindAsync(id);


            if (vrstaNaplate != null)
            {

                return PartialView(vrstaNaplate);

            }
            else
            {
                return NotFound("Ne postoji vrsta naplate s oznakom: " + id);

            }
        }

        [HttpPost]
        public async Task<IActionResult> Edit(VrstaNaplate vrstaNaplate)
        {
            if (vrstaNaplate == null)
            {
                return NotFound("Nema poslanih podataka");
            }

            bool checkId = await ctx.VrstaNaplate.AnyAsync(vn => vn.Id == vn.Id);
            if (!checkId)
            {
                return NotFound("Ne postoji vrsta naplate s id-om: " + vrstaNaplate.Id);
            }

            if (ModelState.IsValid)
            {
                try
                {
                    ctx.Update(vrstaNaplate);
                    await ctx.SaveChangesAsync();
                    return RedirectToAction(nameof(Get), new { id = vrstaNaplate.Id });

                }
                catch (Exception exc)
                {
                    ModelState.AddModelError(string.Empty, exc.CompleteExceptionMessage());
                    return PartialView(vrstaNaplate);
                }
            }
            else
            {
                return PartialView(vrstaNaplate);
            }
        }

        [HttpGet]
        public async Task<IActionResult> Get(int id)
        {
            var vrstaNaplate
                = await ctx.VrstaNaplate
                                  .Where(p => p.Id == id)
                                  .Select(p => new VrstaNaplate
                                  {
                                      Id = p.Id,
                                      Naziv = p.Naziv
                                  })
                                  .SingleOrDefaultAsync();

            if (vrstaNaplate != null)
            {
                return PartialView(vrstaNaplate);
            }
            else
            {
                return NotFound($"Neispravan id vrste naplate: {id}");
            }
        }

        private async Task PrepareDropdownLists()
        {
            var kategorijeVozila = await ctx.KategorijaVozila
                            .Select(d => new { d.Naziv, d.Id })
                            .ToListAsync();
            ViewBag.KategorijaVozila = new SelectList(kategorijeVozila, nameof(KategorijaVozila.Id), nameof(KategorijaVozila.Naziv));

            var naplatneKucice = await ctx.NaplatnaKucica
                            .Select(d => new { d.Id, d.Otvorena })
                            .ToListAsync();
            ViewBag.NaplatnaKucica = new SelectList(naplatneKucice, nameof(NaplatnaKucica.Id), nameof(NaplatnaKucica.Otvorena));
        }
    }
}