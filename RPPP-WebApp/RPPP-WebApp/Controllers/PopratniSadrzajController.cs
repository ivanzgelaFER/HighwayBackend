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
    public class PopratniSadrzajController : Controller
    {
        private readonly RPPP04Context ctx;
        private readonly AppSettings appData;

        public PopratniSadrzajController(RPPP04Context ctx, IOptionsSnapshot<AppSettings> options)
        {
            this.ctx = ctx;
            appData = options.Value;
        }

        public async Task<IActionResult> Index(int page = 1, int sort = 1, bool ascending = true)
        {
            int pagesize = appData.PageSize;

            var query = ctx.PopratniSadrzaj.AsNoTracking();
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

            var popratnisadrzaji = await query
                          .Select(p => new PopratniSadrzajViewModel
                          {
                              IdPopratnogSadrzaja = p.Id,
                              NazivPopratnogSadrzaja = p.Naziv,
                              RadnimDanomOd = p.RadnimDanomOd,
                              RadninDanomDo = p.RadninDanomDo,
                              VikendimaDo = p.VikendimaDo,
                              VikendimaOd = p.VikendimaOd,
                              SlikaPopratnogSadrzaja = p.Slika,
                              OdmoristeId = p.OdmoristeId, 
                              VrstaPopratnog = p.VrstaSadrzaja.Naziv,
                              Odmoriste = p.Odmoriste.Naziv
                          })
                          .Skip((page - 1) * pagesize)
                          .Take(pagesize)
                          .ToListAsync();

            var model = new PopratniSadrzajiViewModel
            {
                
                PopratniSadrzaji = popratnisadrzaji,
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
        public async Task<IActionResult> Create(PopratniSadrzaj popratniSadrzaj)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    ctx.Add(popratniSadrzaj);
                    await ctx.SaveChangesAsync();

                    TempData[Constants.Message] = $"Popratni sadržaj {popratniSadrzaj.Naziv} dodan.";
                    TempData[Constants.ErrorOccurred] = false;
                    return RedirectToAction(nameof(Index));
                }
                catch (Exception exc)
                {
                    ModelState.AddModelError(string.Empty, exc.CompleteExceptionMessage());
                    await PrepareDropDownList();
                    return View(popratniSadrzaj);
                }
            }
            else
            {
                await PrepareDropDownList();
                return View(popratniSadrzaj);
            }
        }

        private async Task PrepareDropDownList()
        {

            var vrstePopratnogSadrzaja = await ctx.VrstaPopratnog.OrderBy(v => v.Naziv).Select(v => new { v.Id, v.Naziv }).ToListAsync();
            var odmorista = await ctx.Odmoriste.OrderBy(o => o.Naziv).Select(o => new { o.Id, o.Naziv }).ToListAsync();

            ViewBag.PopratniSadrzaji = new SelectList(vrstePopratnogSadrzaja, nameof(VrstaPopratnog.Id), nameof(VrstaPopratnog.Naziv));
            ViewBag.Odmorista = new SelectList(odmorista, nameof(Odmoriste.Id), nameof(Odmoriste.Naziv));

        }


        [HttpDelete]
        public async Task<IActionResult> Delete(int id)
        {
            ActionResponseMessage response;
            var popratniSadrzaj
                = await ctx.PopratniSadrzaj.FindAsync(id);

            if (popratniSadrzaj != null)
            {
                try
                {
                    string naziv = popratniSadrzaj.Naziv;
                    ctx.Remove(popratniSadrzaj);
                    await ctx.SaveChangesAsync();
                    response = new ActionResponseMessage(MessageType.Success, $"Odmorište {popratniSadrzaj.Naziv} uspješno obrisan.");
                }
                catch (Exception exc)
                {
                    response = new ActionResponseMessage(MessageType.Error, "Pogreška prilikom brisanja popratnog sadržaja: " + exc.CompleteExceptionMessage());

                }
            }
            else
            {
                response = new ActionResponseMessage(MessageType.Error, "Ne postoji popratni sadržaj s id-om: " + id);

            }
            Response.Headers["HX-Trigger"] = JsonSerializer.Serialize(new { showMessage = response });
            return response.MessageType == MessageType.Success ? new EmptyResult() : await Get(id);
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {

            var popratniSadrzaj = await ctx.PopratniSadrzaj.FindAsync(id);


            if (popratniSadrzaj != null)
            {
                await PrepareDropDownList();

                return PartialView(popratniSadrzaj);

            }
            else
            {
                return NotFound("Ne postoji popratni sadržaj s oznakom: " + id);

            }
        }

        [HttpPost]
        public async Task<IActionResult> Edit(PopratniSadrzaj popratniSadrzaj)
        {
            if (popratniSadrzaj == null)
            {
                return NotFound("Nema poslanih podataka");
            }

            bool checkId = await ctx.PopratniSadrzaj.AnyAsync(p => p.Id == p.Id);
            if (!checkId)
            {
                return NotFound("Ne postoji popratni sadržaj s id-om: " + popratniSadrzaj.Id);
            }

            if (ModelState.IsValid)
            {
                try
                {
                    ctx.Update(popratniSadrzaj);
                    await ctx.SaveChangesAsync();
                    return RedirectToAction(nameof(Get), new { id = popratniSadrzaj.Id }); 

                }
                catch (Exception exc)
                {
                    ModelState.AddModelError(string.Empty, exc.CompleteExceptionMessage());
                    await PrepareDropDownList();
                    return PartialView(popratniSadrzaj);
                }
            }
            else
            {
                await PrepareDropDownList();
                return PartialView(popratniSadrzaj);
            }
        }

        [HttpGet]
        public async Task<IActionResult> Get(int id)
        {
            var popratniSadrzaj
                = await ctx.PopratniSadrzaj
                                  .Where(p => p.Id == id)
                                  .Select(p => new PopratniSadrzajViewModel
                                  {
                                      IdPopratnogSadrzaja = p.Id,
                                      Odmoriste = p.Odmoriste.Naziv,
                                      NazivPopratnogSadrzaja = p.Naziv,
                                      RadnimDanomOd = p.RadnimDanomOd,
                                      RadninDanomDo = p.RadninDanomDo,
                                      VikendimaDo = p.VikendimaDo,
                                      VikendimaOd = p.VikendimaOd,
                                      SlikaPopratnogSadrzaja = p.Slika,
                                      OdmoristeId = p.OdmoristeId,
                                      VrstaSadrzajaId = p.VrstaSadrzajaId,
                                      VrstaPopratnog = p.VrstaSadrzaja.Naziv
                                  })
                                  .SingleOrDefaultAsync();

            if (popratniSadrzaj != null)
            {
                return PartialView(popratniSadrzaj);
            }
            else
            {
                return NotFound($"Neispravan id popratnog sadrzaja: {id}");
            }
        }


    }
}
