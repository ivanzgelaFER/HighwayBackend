using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using RPPP_WebApp.Extensions;
using RPPP_WebApp.Extensions.Selectors;
using RPPP_WebApp.Models;
using RPPP_WebApp.ViewModels;
using System.Text.Json;

namespace RPPP_WebApp.Controllers {
    public class ReakcijaController : Controller {
        private readonly RPPP04Context ctx;
        private readonly AppSettings appData;

        public ReakcijaController(RPPP04Context ctx, IOptionsSnapshot<AppSettings> options) {
            appData = options.Value;
            this.ctx = ctx;
        }

        public async Task<IActionResult> Index(int page = 1, int sort = 1, bool ascending = true) {

            var query = ctx.Reakcija.AsNoTracking();
            int count = await query.CountAsync();

            int pagesize = appData.PageSize;
            var pagingInfo = new PagingInfo {
                CurrentPage = page,
                Sort = sort,
                Ascending = ascending,
                ItemsPerPage = pagesize,
                TotalItems = count
            };


            if (page < 1 || page > pagingInfo.TotalPages) {
                return RedirectToAction(nameof(Index), new { page = 1, sort = sort, ascending = ascending });
            }

            query = query.ApplySort(sort, ascending);

            var reakcije = await query
                            .Select(d => new ReakcijaViewModel {
                                Id = d.Id,
                                Datum = d.Datum,
                                Opis = d.Opis,
                                Incident = d.Incident.VrstaIncidenta.Naziv,
                                VrstaReakcije = d.VrstaReakcije.Naziv
                            })
                            .Skip((page - 1) * pagesize)
                            .Take(pagesize)
                            .ToListAsync();

            var model = new ReakcijeViewModel {
                Reakcije = reakcije,
                PagingInfo = pagingInfo,
            };

            return View(model);
        }
        [HttpGet]
        public async Task<IActionResult> Get(int id) {
            var dionica = await ctx.Reakcija
                            .Where(d => d.Id == id)
                            .Select(d => new ReakcijaViewModel {
                                Id = d.Id,
                                Datum = d.Datum,
                                Opis = d.Opis,
                                Incident = d.Incident.VrstaIncidenta.Naziv,
                                VrstaReakcije = d.VrstaReakcije.Naziv
                            })
                            .SingleOrDefaultAsync();
            if (dionica != null) {
                return PartialView(dionica);
            } else {
                return NotFound($"Neispravan id reakcije: {id}");
            }
        }

        [HttpGet]
        public async Task<IActionResult> Create() {
            await PrepareDropDownLists();
            return View();
        }


        [HttpPost]
        public async Task<IActionResult> Create(Reakcija reakcija) {
            if (ModelState.IsValid) {
                try {
                    ctx.Add(reakcija);
                    await ctx.SaveChangesAsync();
                    TempData[Constants.Message] = $"Reakcija uspješno dodana. Id={reakcija.Id}";
                    TempData[Constants.ErrorOccurred] = false;
                    return RedirectToAction(nameof(Index));
                } catch (Exception exc) {
                    ModelState.AddModelError(string.Empty, exc.CompleteExceptionMessage());
                    await PrepareDropDownLists();
                    return View(reakcija);
                }
            } else {
                await PrepareDropDownLists();
                return View(reakcija);
            }
        }

        private async Task PrepareDropDownLists() {
            var incidenti = await ctx.Incident
                            .Select(up => new { up.VrstaIncidenta.Naziv, up.Id })
                            .ToListAsync();
            ViewBag.Incidenti = new SelectList(incidenti, nameof(Incident.Id), nameof(Incident.VrstaIncidenta.Naziv));

            var vrsteReakcija = await ctx.VrstaReakcije
                            .Select(up => new { up.Naziv, up.Id })
                            .ToListAsync();
            ViewBag.VrsteReakcija = new SelectList(vrsteReakcija, nameof(VrstaReakcije.Id), nameof(VrstaReakcije.Naziv));

           
        }

        [HttpDelete]
        public async Task<IActionResult> Delete(int id) {
            var reakcija = await ctx.Reakcija.FindAsync(id);
            ActionResponseMessage responseMessage;
            if (reakcija != null) {
                try {
                    ctx.Remove(reakcija);
                    await ctx.SaveChangesAsync();
                    responseMessage = new ActionResponseMessage(MessageType.Success, $"Reakcija sa šifrom {id} uspješno obrisana.");
                } catch (Exception exc) {
                    responseMessage = new ActionResponseMessage(MessageType.Error, $"Pogreška prilikom brisanja reakcije: {exc.CompleteExceptionMessage()}");
                }
            } else {
                responseMessage = new ActionResponseMessage(MessageType.Error, $"Reakcija sa šifrom {id} ne postoji");
            }

            Response.Headers["HX-Trigger"] = JsonSerializer.Serialize(new { showMessage = responseMessage });
            return responseMessage.MessageType == MessageType.Success ? new EmptyResult() : await Get(id);
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int id) {
            var dionica = await ctx.Reakcija.AsNoTracking().Where(d => d.Id == id).SingleOrDefaultAsync();
            if (dionica != null) {
                await PrepareDropDownLists();
                return PartialView(dionica);
            } else {
                return NotFound($"Neispravan id reakcije: {id}");
            }
        }

        [HttpPost]
        public async Task<IActionResult> Edit(Reakcija reakcija) {
            if (reakcija == null) {
                return NotFound("Nema poslanih podataka");
            }
            bool checkId = await ctx.Reakcija.AnyAsync(m => m.Id == reakcija.Id);
            if (!checkId) {
                return NotFound($"Neispravan id reakcije: {reakcija?.Id}");
            }

            if (ModelState.IsValid) {
                try {
                    ctx.Update(reakcija);
                    await ctx.SaveChangesAsync();
                    return RedirectToAction(nameof(Get), new { id = reakcija.Id });
                } catch (Exception exc) {
                    ModelState.AddModelError(string.Empty, exc.CompleteExceptionMessage());
                    await PrepareDropDownLists();
                    return PartialView(reakcija);
                }
            } else {
                await PrepareDropDownLists();
                return PartialView(reakcija);
            }
        }

        private void CopyValues(Reakcija reakcija, Reakcija reakcijaRes) {
            reakcija.Datum = reakcijaRes.Datum;
            reakcija.Opis = reakcijaRes.Opis;
            reakcija.Incident = reakcijaRes.Incident;
            reakcija.VrstaReakcije = reakcijaRes.VrstaReakcije;
        }
    }
}
