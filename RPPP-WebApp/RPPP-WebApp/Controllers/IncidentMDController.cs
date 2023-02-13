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
using System.Reflection.Metadata;

namespace RPPP_WebApp.Controllers {
    public class IncidentMDController : Controller {
        private readonly RPPP04Context ctx;
        //private readonly ILogger<AutocestaController> logger;
        private readonly AppSettings appSettings;

        public IncidentMDController(RPPP04Context ctx, IOptionsSnapshot<AppSettings> options) {
            this.ctx = ctx;
            //this.logger = logger;
            appSettings = options.Value;
        }

        public async Task<IActionResult> Index(int page = 1, int sort = 1, bool ascending = true) {
            int pagesize = appSettings.PageSize;

            var query = ctx.Incident
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


            var incidenti = await query
                         .Select(a => new IncidentViewModel {
                             Id = a.Id,
                             Opis = a.Opis,
                             Datum = a.Datum,
                             MeteoroloskiUvjeti = a.MeteoroloskiUvjeti,
                             StanjeNaCesti = a.StanjeNaCesti,
                             Prohodnost = a.Prohodnost,
                             Dionica = a.Dionica.Naziv,
                             VrstaIncidenta = a.VrstaIncidenta.Naziv
                         })
                         .Skip((page - 1) * pagesize)
                         .Take(pagesize)
                         .ToListAsync();

            foreach (var incident in incidenti) {
                var lista = await ctx.Reakcija
                                      .Where(d => d.IncidentId == incident.Id)
                                      .Select(d => d.Opis)
                                      .ToListAsync();
                string naziv = string.Join(", ", lista);

                incident.NaziviReakcija = naziv;
            }

            var model = new IncidentiViewModel {
                Incidenti = incidenti,
                PagingInfo = pagingInfo
            };

            return View(model);
        }


        public async Task<IActionResult> Show(int id, string filter, int page = 1, int sort = 1, bool ascending = true, string viewName = nameof(Show)) {
            var incident = await ctx.Incident
                                    .Where(a => a.Id == id)
                                      .Select(a => new IncidentViewModel {
                                          Id = a.Id,
                                          Opis = a.Opis,
                                          Datum = a.Datum,
                                          MeteoroloskiUvjeti = a.MeteoroloskiUvjeti,
                                          StanjeNaCesti = a.StanjeNaCesti,
                                          Prohodnost = a.Prohodnost,
                                          Dionica = a.Dionica.Naziv,
                                          VrstaIncidenta = a.VrstaIncidenta.Naziv
                                      })
                                    .FirstOrDefaultAsync();
            if (incident == null) {
                return NotFound($"Incident s id {id} ne postoji");
            } else {

                //učitavanje stavki - dionica
                var stavke = await ctx.Reakcija
                                      .Where(d => d.IncidentId == incident.Id)
                                      .OrderBy(d => d.Id)
                                      .Select(d => new ReakcijaViewModel {
                                          Id = d.Id,
                                          Datum = d.Datum,
                                          Opis = d.Opis,
                                          Incident = d.Incident.VrstaIncidenta.Naziv,
                                          VrstaReakcije = d.VrstaReakcije.Naziv
                                      })
                                      .ToListAsync();
                incident.Reakcije = stavke;

                ViewBag.Page = page;
                ViewBag.Sort = sort;
                ViewBag.Ascending = ascending;
                ViewBag.Filter = filter;
                //ViewBag.Position = position;

                return View(viewName, incident);
            }
        }




        private async Task PrepareDropDownList() {


            var vrsteIncidenata = await ctx.VrstaIncidenta.OrderBy(vi => vi.Naziv).Select(vi => new { vi.Id, vi.Naziv }).ToListAsync();
            var dionice = await ctx.Dionica.OrderBy(dionica => dionica.Naziv).Select(dionica => new { dionica.Id, dionica.Naziv }).ToListAsync();

            ViewBag.Incidenti2 = new SelectList(dionice, nameof(Dionica.Id), nameof(Dionica.Naziv));
            ViewBag.Incidenti1 = new SelectList(vrsteIncidenata, nameof(VrstaIncidenta.Id), nameof(VrstaIncidenta.Naziv));
        }

        [HttpGet]
        public async Task<IActionResult> Create() {
            await PrepareDropDownList();
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Incident incident) {
            if (ModelState.IsValid) {
                try {
                    ctx.Add(incident);
                    await ctx.SaveChangesAsync();


                    TempData[Constants.Message] = $"Autocesta {incident.Id} dodana.";
                    TempData[Constants.ErrorOccurred] = false;
                    return RedirectToAction(nameof(Index));
                } catch (Exception exc) {

                    ModelState.AddModelError(string.Empty, exc.CompleteExceptionMessage());
                    await PrepareDropDownList();
                    return View(incident);
                }
            } else {
                await PrepareDropDownList();
                return View(incident);
            }
        }

        [HttpDelete]
        public async Task<IActionResult> Delete(int id) {
            ActionResponseMessage responseMessage;
            var incident = await ctx.Incident.FindAsync(id);
            if (incident != null) {
                try {
                    ctx.Remove(incident);
                    await ctx.SaveChangesAsync();
                    responseMessage = new ActionResponseMessage(MessageType.Success, $"Incident sa šifrom {id} uspješno obrisan.");
                } catch (Exception exc) {
                    responseMessage = new ActionResponseMessage(MessageType.Error, $"Pogreška prilikom brisanja incidenta: {exc.CompleteExceptionMessage()}");
                }
            } else {
                responseMessage = new ActionResponseMessage(MessageType.Error, $"Incident sa šifrom {id} ne postoji");
            }
            Response.Headers["HX-Trigger"] = JsonSerializer.Serialize(new { showMessage = responseMessage });
            return new EmptyResult();
        }


        [HttpGet]
        public async Task<IActionResult> Edit(int id) {
            var incident = await ctx.Incident.AsNoTracking().Where(a => a.Id == id).SingleOrDefaultAsync();
            if (incident != null) {
                await PrepareDropDownList();
                return PartialView(incident);
            } else {
                return NotFound($"Neispravan id incidenta: {id}");
            }
        }


        [HttpPost]

        public async Task<IActionResult> Edit(Incident incident) {
            if (incident == null) {
                return NotFound("Nema poslanih podataka");
            }
            bool checkId = await ctx.Incident.AnyAsync(m => m.Id == incident.Id);
            if (!checkId) {
                return NotFound($"Neispravan id autoceste: {incident?.Id}");
            }

            if (ModelState.IsValid) {
                try {
                    ctx.Update(incident);
                    await ctx.SaveChangesAsync();
                    return RedirectToAction(nameof(Get), new { id = incident.Id });
                } catch (Exception exc) {
                    ModelState.AddModelError(string.Empty, exc.CompleteExceptionMessage());
                    await PrepareDropDownList();
                    return PartialView(incident);
                }
            } else {
                await PrepareDropDownList();
                return PartialView(incident);
            }
        }


        [HttpGet]
        public async Task<IActionResult> Get(int id) {
            var incident = await ctx.Incident
                                  .Where(a => a.Id == id)
                                  .Select(a => new IncidentViewModel {
                                      Id = a.Id,
                                      Opis = a.Opis,
                                      Datum = a.Datum,
                                      MeteoroloskiUvjeti = a.MeteoroloskiUvjeti,
                                      StanjeNaCesti = a.StanjeNaCesti,
                                      Prohodnost = a.Prohodnost,
                                      Dionica = a.Dionica.Naziv,
                                      VrstaIncidenta = a.VrstaIncidenta.Naziv
                                  })
                                  .SingleOrDefaultAsync();
            if (incident != null) {
                return PartialView(incident);
            } else {
                return NotFound($"Neispravan id incidenta: {id}");
            }
        }
    }
}
