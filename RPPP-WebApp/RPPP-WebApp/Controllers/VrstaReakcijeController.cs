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

namespace RPPP_WebApp.Controllers {
    public class VrstaReakcijeController : Controller {
        private readonly RPPP04Context ctx;
        private readonly AppSettings appSettings;

        public VrstaReakcijeController(RPPP04Context ctx, IOptionsSnapshot<AppSettings> options) {
            this.ctx = ctx;
            appSettings = options.Value;
        }

        public async Task<IActionResult> Index(int page = 1, int sort = 1, bool ascending = true) {
            int pagesize = appSettings.PageSize;

            var query = ctx.VrstaReakcije
                           .AsNoTracking();

            int count = await query.CountAsync();

            var pagingInfo = new PagingInfo {
                CurrentPage = page,
                Sort = sort,
                Ascending = ascending,
                ItemsPerPage = pagesize,
                TotalItems = count
            };
            if (page < 1 || page > pagingInfo.TotalPages) {
                return RedirectToAction(nameof(Index), new { page = 1, sort, ascending });
            }

            query = query.ApplySort(sort, ascending);

            var vrsteReakcija = await query
                        .Select(np => new VrstaReakcijeViewModel {
                            Id = np.Id,
                            Naziv = np.Naziv,
                            BrojTelefona = np.BrojTelefona
                        })
                        .Skip((page - 1) * pagesize)
                        .Take(pagesize)
                        .ToListAsync();

            var model = new VrsteReakcijaViewModel {
                VrsteReakcija = vrsteReakcija,
                PagingInfo = pagingInfo
            };

            return View(model);
        }


        [HttpGet]
        public async Task<IActionResult> Create() {
            return View();
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(VrstaReakcije vrstaReakcije) {
            if (ModelState.IsValid) {
                try {
                    ctx.Add(vrstaReakcije);
                    await ctx.SaveChangesAsync();

                    TempData[Constants.Message] = $"Vrsta reakcije {vrstaReakcije.Naziv} dodana.";
                    TempData[Constants.ErrorOccurred] = false;
                    return RedirectToAction(nameof(Index));
                } catch (Exception exc) {
                    ModelState.AddModelError(string.Empty, exc.CompleteExceptionMessage());
                    return View(vrstaReakcije);
                }
            } else 
                return View(vrstaReakcije);
            }
        


        [HttpDelete]
        public async Task<IActionResult> Delete(int id) {
            ActionResponseMessage responseMessage;
            var vrstaReakcije = await ctx.VrstaReakcije.FindAsync(id);
            if (vrstaReakcije != null) {
                try {
                    string naziv = vrstaReakcije.Naziv;
                    ctx.Remove(vrstaReakcije);
                    await ctx.SaveChangesAsync();
                    responseMessage = new ActionResponseMessage(MessageType.Success, $"Vrsta reakcije {naziv} sa šifrom {id} uspješno obrisana.");
                } catch (Exception exc) {
                    responseMessage = new ActionResponseMessage(MessageType.Error, $"Pogreška prilikom brisanja vrste reakcije: {exc.CompleteExceptionMessage()}");
                }
            } else {
                responseMessage = new ActionResponseMessage(MessageType.Error, $"Vrsta reakcije sa šifrom {id} ne postoji");
            }
            Response.Headers["HX-Trigger"] = JsonSerializer.Serialize(new { showMessage = responseMessage });
            return new EmptyResult();
        }


        [HttpGet]
        public async Task<IActionResult> Edit(int id) {
            var vrstaReakcije = await ctx.VrstaReakcije.AsNoTracking().Where(np => np.Id == id).SingleOrDefaultAsync();
            if (vrstaReakcije != null) {
                return PartialView(vrstaReakcije);
            } else {
                return NotFound($"Neispravan id vrste reakcije: {id}");
            }
        }

        [HttpPost]

        public async Task<IActionResult> Edit(VrstaReakcije vrstaReakcije) {
            if (vrstaReakcije == null) {
                return NotFound("Nema poslanih podataka");
            }
            bool checkId = await ctx.VrstaReakcije.AnyAsync(np => np.Id == vrstaReakcije.Id);
            if (!checkId) {
                return NotFound($"Neispravan id vrste reakcije: {vrstaReakcije?.Id}");
            }

            if (ModelState.IsValid) {
                try {
                    ctx.Update(vrstaReakcije);
                    await ctx.SaveChangesAsync();
                    return RedirectToAction(nameof(Get), new { id = vrstaReakcije.Id });
                } catch (Exception exc) {
                    ModelState.AddModelError(string.Empty, exc.CompleteExceptionMessage());
                    return PartialView(vrstaReakcije);
                }
            } else {
                return PartialView(vrstaReakcije);
            }
        }


        [HttpGet]
        public async Task<IActionResult> Get(int id) {
            var vrstaReakcije = await ctx.VrstaReakcije
                                  .Where(np => np.Id == id)
                                  .Select(np => new VrstaReakcijeViewModel {
                                      Id = np.Id,
                                      Naziv = np.Naziv,
                                      BrojTelefona = np.BrojTelefona
                                  })
                                  .SingleOrDefaultAsync();
            if (vrstaReakcije != null) {
                Console.WriteLine(vrstaReakcije);
                return PartialView(vrstaReakcije);
            } else {
                return NotFound($"Neispravan id vrste reakcije: {id}");
            }
        }
    }
}
