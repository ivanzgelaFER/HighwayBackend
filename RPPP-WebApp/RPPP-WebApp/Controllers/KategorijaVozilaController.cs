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
    public class KategorijaVozilaController : Controller
    {
        private readonly RPPP04Context ctx;
        private readonly AppSettings appSettings;

        public KategorijaVozilaController(RPPP04Context ctx, IOptionsSnapshot<AppSettings> options)
        {
            appSettings = options.Value;
            this.ctx = ctx;
        }

        public async Task<IActionResult> Index(int page = 1, int sort = 1, bool ascending = true)
        {
            int pagesize = appSettings.PageSize;

            var query = ctx.KategorijaVozila.AsNoTracking();

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

            var kategorijeVozila = await query
                        .Select(q => new KategorijaVozila
                        {
                            Id = q.Id,
                            Naziv = q.Naziv
                        })
                        .Skip((page - 1) * pagesize)
                        .Take(pagesize)
                        .ToListAsync();

            var model = new KategorijeVozilaViewModel
            {
                KategorijeVozila = kategorijeVozila,
                PagingInfo = pagingInfo
            };

            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> Create()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(KategorijaVozila kategorijaVozila)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    ctx.Add(kategorijaVozila);
                    await ctx.SaveChangesAsync();

                    TempData[Constants.Message] = $"Kategorija vozila: {kategorijaVozila.Naziv} dodana.";
                    TempData[Constants.ErrorOccurred] = false;
                    return RedirectToAction(nameof(Index));
                }
                catch (Exception exc)
                {
                    ModelState.AddModelError(string.Empty, exc.CompleteExceptionMessage());
                    return View(kategorijaVozila);
                }
            }
            else
            {
                return View(kategorijaVozila);
            }
        }

        [HttpDelete]
        public async Task<IActionResult> Delete(int id)
        {
            ActionResponseMessage response;
            var kategorijaVozila
                = await ctx.KategorijaVozila.FindAsync(id);

            if (kategorijaVozila != null)
            {
                try
                {
                    string naziv = kategorijaVozila.Naziv;
                    ctx.Remove(kategorijaVozila);
                    await ctx.SaveChangesAsync();
                    response = new ActionResponseMessage(MessageType.Success, $"Kategorija vozila: {kategorijaVozila.Naziv} uspješno obrisana.");
                }
                catch (Exception exc)
                {
                    response = new ActionResponseMessage(MessageType.Error, "Pogreška prilikom brisanja kategorije vozila: " + exc.CompleteExceptionMessage());

                }
            }
            else
            {
                response = new ActionResponseMessage(MessageType.Error, "Ne postoji kategorija vozila s id-om: " + id);

            }
            Response.Headers["HX-Trigger"] = JsonSerializer.Serialize(new { showMessage = response });
            return response.MessageType == MessageType.Success ? new EmptyResult() : await Get(id);
        }

        [HttpGet]
        public async Task<IActionResult> Get(int id)
        {
            var kategorijaVozila
                = await ctx.KategorijaVozila
                                  .Where(p => p.Id == id)
                                  .Select(p => new KategorijaVozila
                                  {
                                      Id = p.Id,
                                      Naziv = p.Naziv
                                  })
                                  .SingleOrDefaultAsync();

            if (kategorijaVozila != null)
            {
                return PartialView(kategorijaVozila);
            }
            else
            {
                return NotFound($"Neispravan id kategorije vozila: {id}");
            }
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int id, int page = 1, int sort = 1, bool ascending = true)
        {

            var kategorijaVozila = await ctx.KategorijaVozila.FindAsync(id);
            if (kategorijaVozila != null)
            {
                ViewBag.Page = page;
                ViewBag.Sort = sort;
                ViewBag.Ascending = ascending;
                return View(kategorijaVozila);
            }
            else
            {
                return NotFound($"Neispravan id kategorije vozila: {id}");
            }
        }

        [HttpPost]
        public async Task<IActionResult> Edit(KategorijaVozila kategorijaVozila)
        {
            if (kategorijaVozila == null)
            {
                return NotFound("Nema poslanih podataka");
            }

            bool checkId = await ctx.VrstaNaplate.AnyAsync(vn => vn.Id == vn.Id);
            if (!checkId)
            {
                return NotFound("Ne postoji kategorija vozila s id-om: " + kategorijaVozila.Id);
            }

            if (ModelState.IsValid)
            {
                try
                {
                    ctx.Update(kategorijaVozila);
                    await ctx.SaveChangesAsync();
                    return RedirectToAction(nameof(Index), new { id = kategorijaVozila.Id });

                }
                catch (Exception exc)
                {
                    ModelState.AddModelError(string.Empty, exc.CompleteExceptionMessage());
                    return PartialView(kategorijaVozila);
                }
            }
            else
            {
                return PartialView(kategorijaVozila);
            }
        }
    }
}