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
    public class VrstaNaplateController : Controller
    {
        private readonly RPPP04Context ctx;
        private readonly AppSettings appSettings;

        public VrstaNaplateController(RPPP04Context ctx, IOptionsSnapshot<AppSettings> options)
        {
            this.ctx = ctx;
            appSettings = options.Value;
        }

        public async Task<IActionResult> Index(int page = 1, int sort = 1, bool ascending = true)
        {
            int pagesize = appSettings.PageSize;

            var query = ctx.VrstaNaplate.AsNoTracking();
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

            var vrsteNaplate = await query
                          .Select(vp => new VrstaNaplate
                          {
                              Id = vp.Id,
                              Naziv = vp.Naziv
                          })
                          .Skip((page - 1) * pagesize)
                          .Take(pagesize)
                          .ToListAsync();

            var model = new VrsteNaplataViewModel
            {
                VrsteNaplata = vrsteNaplate,
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
        public async Task<IActionResult> Create(VrstaNaplate vrstaNaplate)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    ctx.Add(vrstaNaplate);
                    await ctx.SaveChangesAsync();

                    TempData[Constants.Message] = $"Vrsta naplate {vrstaNaplate.Naziv} dodana.";
                    TempData[Constants.ErrorOccurred] = false;
                    return RedirectToAction(nameof(Index));
                }
                catch (Exception exc)
                {
                    ModelState.AddModelError(string.Empty, exc.CompleteExceptionMessage());
                    return View(vrstaNaplate);
                }
            }
            else
            {
                return View(vrstaNaplate);
            }
        }

        [HttpDelete]
        public async Task<IActionResult> Delete(int id)
        {
            ActionResponseMessage response;
            var vrstaNaplate
                = await ctx.VrstaNaplate.FindAsync(id);

            if (vrstaNaplate != null)
            {
                try
                {
                    string naziv = vrstaNaplate.Naziv;
                    ctx.Remove(vrstaNaplate);
                    await ctx.SaveChangesAsync();
                    response = new ActionResponseMessage(MessageType.Success, $"Vrsta naplate {vrstaNaplate.Naziv} uspješno obrisana.");
                }
                catch (Exception exc)
                {
                    response = new ActionResponseMessage(MessageType.Error, "Pogreška prilikom brisanja vrste naplate: " + exc.CompleteExceptionMessage());

                }
            }
            else
            {
                response = new ActionResponseMessage(MessageType.Error, "Ne postoji vrsta naplate s id-om: " + id);

            }
            Response.Headers["HX-Trigger"] = JsonSerializer.Serialize(new { showMessage = response });
            return response.MessageType == MessageType.Success ? new EmptyResult() : await Get(id);
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int id, int page = 1, int sort = 1, bool ascending = true)
        {
            var vrstaNaplate = await ctx.VrstaNaplate.FindAsync(id);
            if (vrstaNaplate != null)
            {
                ViewBag.Page = page;
                ViewBag.Sort = sort;
                ViewBag.Ascending = ascending;
                return View(vrstaNaplate);
            }
            else
            {
                return NotFound($"Neispravan id vrste naplate: {id}");
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
                    return RedirectToAction(nameof(Index), new { id = vrstaNaplate.Id });

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


    }
}