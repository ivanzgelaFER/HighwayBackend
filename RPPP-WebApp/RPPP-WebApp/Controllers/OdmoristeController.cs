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
    public class OdmoristeController : Controller
    {
        private readonly RPPP04Context ctx;
        private readonly AppSettings appData;

        public OdmoristeController(RPPP04Context ctx, IOptionsSnapshot<AppSettings> options)
        {
            this.ctx = ctx;
            appData = options.Value;
        }

        public async Task<IActionResult> Index(int page = 1, int sort = 1, bool ascending = true)
        {
            int pagesize = appData.PageSize;

            var query = ctx.Odmoriste.AsNoTracking();
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

            var odmorista = await query
                          .Select(o => new OdmoristeViewModel
                          {
                              IdOdmorista = o.Id,
                              NazivOdmorista = o.Naziv,
                              KoordinataX = (int)o.KoordinataX,
                              KoordinataY = (int)o.KoordinataY,
                              GodinaOtvaranja = (int)o.GodinaOtvaranja,
                              Dionica = o.Dionica.Naziv

                          })
                          .Skip((page - 1) * pagesize)
                          .Take(pagesize)
                          .ToListAsync();

            var model = new OdmoristaViewModel
            {
                Odmorista = odmorista,
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
        public async Task<IActionResult> Create(Odmoriste odmoriste)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    ctx.Add(odmoriste);
                    await ctx.SaveChangesAsync();

                    TempData[Constants.Message] = $"Odmorište {odmoriste.Naziv} dodana.";
                    TempData[Constants.ErrorOccurred] = false;
                    return RedirectToAction(nameof(Index));
                }
                catch (Exception exc)
                {
                    ModelState.AddModelError(string.Empty, exc.CompleteExceptionMessage());
                    await PrepareDropDownList(); 
                    return View(odmoriste);
                }
            }
            else
            {
                await PrepareDropDownList();
                return View(odmoriste);
            }
        }

        private async Task PrepareDropDownList()
        {

            var dionice = await ctx.Dionica.OrderBy(d => d.Naziv).Select(d => new { d.Id, d.Naziv }).ToListAsync();

            ViewBag.Odmorista = new SelectList(dionice, nameof(Dionica.Id), nameof(Dionica.Naziv));

        }

        // METODE ZA DINAMICKO BRISANJE I AZURIRANJE PODATAKA

        [HttpDelete]
        public async Task<IActionResult> Delete(int id)
        {
            ActionResponseMessage response;
            var odmoriste
                = await ctx.Odmoriste.FindAsync(id);
            
            if (odmoriste != null)
            {
                try
                {
                    string naziv = odmoriste.Naziv;
                    ctx.Remove(odmoriste);
                    await ctx.SaveChangesAsync();
                    response = new ActionResponseMessage(MessageType.Success, $"Odmorište {odmoriste.Naziv} uspješno obrisan.");
                }
                catch (Exception exc)
                {
                    response = new ActionResponseMessage(MessageType.Error, "Pogreška prilikom brisanja odmorišta: " + exc.CompleteExceptionMessage());
                    
                }
            }
            else
            {
                response = new ActionResponseMessage(MessageType.Error, "Ne postoji odmorište s id-om: " + id);
                
            }
            Response.Headers["HX-Trigger"] = JsonSerializer.Serialize(new { showMessage = response });
            return response.MessageType == MessageType.Success ? new EmptyResult() : await Get(id);
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {

            var odmoriste = await ctx.Odmoriste.FindAsync(id); 
            

            if (odmoriste != null)
            {
                await PrepareDropDownList();
                return PartialView(odmoriste);
                
            }
            else
            {
                return NotFound("Ne postoji odmoriste s oznakom: " + id);

            }
        }

        [HttpPost]
        public async Task<IActionResult> Edit(Odmoriste odmoriste)
        {
            if (odmoriste == null)
            {
                return NotFound("Nema poslanih podataka");
            }

            bool checkId = await ctx.Odmoriste.AnyAsync(o => o.Id == o.Id);
            if (!checkId)
            {
                return NotFound("Ne postoji odmoriste s id-om: " + odmoriste.Id);
            }

            if (ModelState.IsValid)
            {
                try
                {
                    ctx.Update(odmoriste);
                    await ctx.SaveChangesAsync();
                    return RedirectToAction(nameof(Get), new { id = odmoriste.Id}); 

                }
                catch (Exception exc)
                {
                    ModelState.AddModelError(string.Empty, exc.CompleteExceptionMessage());
                    await PrepareDropDownList();
                    return PartialView(odmoriste);
                }
            }
            else
            {
                await PrepareDropDownList(); 
                return PartialView(odmoriste);
            }
        }

        [HttpGet]
        public async Task<IActionResult> Get(int id)
        {
            var odmoriste
                = await ctx.Odmoriste
                                  .Where(o => o.Id == id)
                                  .Select(o => new OdmoristeViewModel
                                  {
                                      IdOdmorista = o.Id,
                                      NazivOdmorista = o.Naziv,
                                      KoordinataX = (int)o.KoordinataX,
                                      KoordinataY = (int)o.KoordinataY,
                                      GodinaOtvaranja = (int)o.GodinaOtvaranja,
                                      Dionica = o.Dionica.Naziv
                                  })
                                  .SingleOrDefaultAsync();
            if (odmoriste != null)
            {
                return PartialView(odmoriste);
            }
            else
            {
                return NotFound($"Neispravan id odmorista: {id}");
            }
        }


    }
}
