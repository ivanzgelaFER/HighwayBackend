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
    public class VrstaPopratnogController : Controller
    {
        private readonly RPPP04Context ctx;
        private readonly AppSettings appData;

        public VrstaPopratnogController(RPPP04Context ctx, IOptionsSnapshot<AppSettings> options)
        {
            this.ctx = ctx;
            appData = options.Value;
        }

        public async Task<IActionResult> Index(int page = 1, int sort = 1, bool ascending = true)
        {
            int pagesize = appData.PageSize;

            var query = ctx.VrstaPopratnog.AsNoTracking();
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

            var vrstePopratnog = await query
                          .Select(vp => new VrstaPopratnogViewModel
                          {
                              IdVrstePopratnog = vp.Id,
                              NazivVrstePopratnog = vp.Naziv
                          })
                          .Skip((page - 1) * pagesize)
                          .Take(pagesize)
                          .ToListAsync();

            var model = new VrstePopratnogViewModel
            {
                VrstePopratnog = vrstePopratnog,
                PagingInfo = pagingInfo
            };

            return View(model);
        }

        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(VrstaPopratnog vrstaPopratnog)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    ctx.Add(vrstaPopratnog);
                    await ctx.SaveChangesAsync();

                    TempData[Constants.Message] = $"Vrsta popratnog sadržaja {vrstaPopratnog.Naziv} dodana.";
                    TempData[Constants.ErrorOccurred] = false;
                    return RedirectToAction(nameof(Index));
                }
                catch (Exception exc)
                {
                    ModelState.AddModelError(string.Empty, exc.CompleteExceptionMessage());
                    return View(vrstaPopratnog);
                }
            }
            else
            {
                return View(vrstaPopratnog);
            }
        }


        [HttpDelete]
        public async Task<IActionResult> Delete(int id)
        {
            ActionResponseMessage response;
            var vrstaPopratnog
                = await ctx.VrstaPopratnog.FindAsync(id);

            if (vrstaPopratnog != null)
            {
                try
                {
                    string naziv = vrstaPopratnog.Naziv;
                    ctx.Remove(vrstaPopratnog);
                    await ctx.SaveChangesAsync();
                    response = new ActionResponseMessage(MessageType.Success, $"Vrsta popratnog sadržaja: {vrstaPopratnog.Naziv} uspješno obrisan.");
                }
                catch (Exception exc)
                {
                    response = new ActionResponseMessage(MessageType.Error, "Pogreška prilikom brisanja vrste popratnog sadržaja: " + exc.CompleteExceptionMessage());

                }
            }
            else
            {
                response = new ActionResponseMessage(MessageType.Error, "Ne postoji vrsta popratnog sadržaja s id-om: " + id);

            }
            Response.Headers["HX-Trigger"] = JsonSerializer.Serialize(new { showMessage = response });
            return response.MessageType == MessageType.Success ? new EmptyResult() : await Get(id);
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {

            var vrstaPopratnog = await ctx.VrstaPopratnog.FindAsync(id);


            if (vrstaPopratnog != null)
            {

                return PartialView(vrstaPopratnog);

            }
            else
            {
                return NotFound("Ne postoji vrsta popratnog sadržaja s oznakom: " + id);

            }
        }

        [HttpPost]
        public async Task<IActionResult> Edit(VrstaPopratnog vrstaPopratnog)
        {
            if (vrstaPopratnog == null)
            {
                return NotFound("Nema poslanih podataka");
            }

            bool checkId = await ctx.VrstaPopratnog.AnyAsync(vp => vp.Id == vp.Id);
            if (!checkId)
            {
                return NotFound("Ne postoji vrsta popratnog sadržaja s id-om: " + vrstaPopratnog.Id);
            }

            if (ModelState.IsValid)
            {
                try
                {
                    ctx.Update(vrstaPopratnog);
                    await ctx.SaveChangesAsync();
                    return RedirectToAction(nameof(Get), new { id = vrstaPopratnog.Id }); 

                }
                catch (Exception exc)
                {
                    ModelState.AddModelError(string.Empty, exc.CompleteExceptionMessage());
                    return PartialView(vrstaPopratnog);
                }
            }
            else
            {
                return PartialView(vrstaPopratnog);
            }
        }

        [HttpGet]
        public async Task<IActionResult> Get(int id)
        {
            var vrstaPopratnog
                = await ctx.VrstaPopratnog
                                  .Where(vp => vp.Id == id)
                                  .Select(vp => new VrstaPopratnogViewModel
                                  {
                                        IdVrstePopratnog = vp.Id,
                                        NazivVrstePopratnog = vp.Naziv
                                  })
                                  .SingleOrDefaultAsync();
            if (vrstaPopratnog != null)
            {
                return PartialView(vrstaPopratnog);
            }
            else
            {
                return NotFound($"Neispravan id vrste popratnog sadržaja: {id}");
            }
        }


    }
}
