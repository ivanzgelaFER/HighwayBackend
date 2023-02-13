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
    public class DionicaController : Controller
    {
        private readonly RPPP04Context ctx;
        private readonly AppSettings appData;
        private readonly ILogger<DionicaController> logger;

        public DionicaController(RPPP04Context ctx, IOptionsSnapshot<AppSettings> options, ILogger<DionicaController> logger)
        {
            appData = options.Value;
            this.ctx = ctx;
            this.logger = logger;
        }

        public async Task<IActionResult> Index(int page = 1, int sort = 1, bool ascending = true)
        {

            var query = ctx.Dionica.AsNoTracking();
            int count = await query.CountAsync();

            int pagesize = appData.PageSize;
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
                return RedirectToAction(nameof(Index), new { page = 1, sort = sort, ascending = ascending });
            }

            query = query.ApplySort(sort, ascending);

            var dionice = await query
                            .Select(d => new DionicaViewModel
                            {
                                Id = d.Id,
                                UlaznaPostaja = d.UlaznaPostaja.Naziv,
                                IzlaznaPostaja = d.IzlaznaPostaja.Naziv,
                                BrojTraka = d.BrojTraka,
                                ZaustavnaTraka = d.ZaustavnaTraka,
                                DozvolaTeretnimVozilima = d.DozvolaTeretnimVozilima,
                                OtvorenZaProlaz = d.OtvorenZaProlaz,
                                GodinaOtvaranja = d.GodinaOtvaranja,
                                Duljina = d.Duljina,
                                Naziv = d.Naziv,
                                OgranicenjeBrzine = d.OgranicenjeBrzine,
                                Autocesta = d.Autocesta.Naziv
                            })
                            .Skip((page - 1) * pagesize)
                            .Take(pagesize)
                            .ToListAsync();

            var model = new DioniceViewModel
            {
                Dionice = dionice,
                PagingInfo = pagingInfo,
            };

            return View(model);
        }
        [HttpGet]
        public async Task<IActionResult> Get(int id)
        {
            var dionica = await ctx.Dionica
                            .Where(d => d.Id == id)
                            .Select(d => new DionicaViewModel
                            {
                                Id = d.Id,
                                UlaznaPostaja = d.UlaznaPostaja.Naziv,
                                IzlaznaPostaja = d.IzlaznaPostaja.Naziv,
                                BrojTraka = d.BrojTraka,
                                ZaustavnaTraka = d.ZaustavnaTraka,
                                DozvolaTeretnimVozilima = d.DozvolaTeretnimVozilima,
                                OtvorenZaProlaz = d.OtvorenZaProlaz,
                                GodinaOtvaranja = d.GodinaOtvaranja,
                                Duljina = d.Duljina,
                                Naziv = d.Naziv,
                                OgranicenjeBrzine = d.OgranicenjeBrzine,
                                Autocesta = d.Autocesta.Naziv
                            })
                            .SingleOrDefaultAsync();
            if (dionica != null)
            {
                return PartialView(dionica);
            }
            else
            {
                return NotFound($"Neispravan id dionice: {id}");
            }
        }

        [HttpGet]
        public async Task<IActionResult> Create()
        {
            await PrepareDropDownLists();
            return View();
        }


        [HttpPost]
        public async Task<IActionResult> Create(Dionica dionica)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    ctx.Add(dionica);
                    await ctx.SaveChangesAsync();
                    logger.LogInformation(new EventId(1000), $"Dionica {dionica.Naziv} dodana.");
                    TempData[Constants.Message] = $"Dionica {dionica.Naziv} uspješno dodana.";
                    TempData[Constants.ErrorOccurred] = false;
                    return RedirectToAction(nameof(Index));
                }
                catch (Exception exc)
                {
                    logger.LogError("Pogreška prilikom dodavanje nove dionice: {0}", exc.CompleteExceptionMessage());
                    ModelState.AddModelError(string.Empty, exc.CompleteExceptionMessage());
                    await PrepareDropDownLists();
                    return View(dionica);
                }
            }
            else
            {
                await PrepareDropDownLists();
                return View(dionica);
            }
        }

        private async Task PrepareDropDownLists()
        {
            var ulaznePostaje = await ctx.NaplatnaPostaja
                            .Select(up => new { up.Naziv, up.Id })
                            .ToListAsync();
            ViewBag.UlaznePostaje = new SelectList(ulaznePostaje, nameof(NaplatnaPostaja.Id), nameof(NaplatnaPostaja.Naziv));

            var izlaznePostaje = await ctx.NaplatnaPostaja
                            .Select(up => new { up.Naziv, up.Id })
                            .ToListAsync();
            ViewBag.IzlaznePostaje = new SelectList(izlaznePostaje, nameof(NaplatnaPostaja.Id), nameof(NaplatnaPostaja.Naziv));

            var autoceste = await ctx.Autocesta
                            .Select(up => new { up.Naziv, up.Id })
                            .ToListAsync();
            ViewBag.Autoceste = new SelectList(autoceste, nameof(Autocesta.Id), nameof(Autocesta.Naziv));
        }

        [HttpDelete]
        public async Task<IActionResult> Delete(int id)
        {
            var dionica = await ctx.Dionica.FindAsync(id);
            ActionResponseMessage responseMessage;
            if (dionica != null)
            {
                try
                {
                    string naziv = dionica.Naziv;
                    ctx.Remove(dionica);
                    await ctx.SaveChangesAsync();
                    logger.LogInformation($"Dionica: {naziv} uspješno obrisana");
                    responseMessage = new ActionResponseMessage(MessageType.Success, $"Dionica ({naziv}) sa šifrom {id} uspješno obrisana.");
                }
                catch (Exception exc)
                {
                    logger.LogError("Pogreška prilikom brisanja dionice: " + exc.CompleteExceptionMessage());
                    responseMessage = new ActionResponseMessage(MessageType.Error, $"Pogreška prilikom brisanja dionice: {exc.CompleteExceptionMessage()}");
                }
            }
            else
            {
                responseMessage = new ActionResponseMessage(MessageType.Error, $"Dionica sa šifrom {id} ne postoji");
            }

            Response.Headers["HX-Trigger"] = JsonSerializer.Serialize(new { showMessage = responseMessage });
            return responseMessage.MessageType == MessageType.Success ? new EmptyResult() : await Get(id);
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var dionica = await ctx.Dionica.AsNoTracking().Where(d => d.Id == id).SingleOrDefaultAsync();
            if (dionica != null)
            {
                await PrepareDropDownLists();
                return PartialView(dionica);
            }
            else
            {
                return NotFound($"Neispravan id dionice: {id}");
            }
        }

        [HttpPost]
        public async Task<IActionResult> Edit(Dionica dionica)
        {
            if (dionica == null)
            {
                return NotFound("Nema poslanih podataka");
            }
            bool checkId = await ctx.Dionica.AnyAsync(m => m.Id == dionica.Id);
            if (!checkId)
            {
                return NotFound($"Neispravan id dionice: {dionica?.Id}");
            }

            if (ModelState.IsValid)
            {
                try
                {
                    ctx.Update(dionica);
                    await ctx.SaveChangesAsync();
                    logger.LogInformation($"Dionica: {dionica.Naziv} uspješno ažurirana");
                    TempData[Constants.Message] = $"Dionica {dionica.Naziv} uspješno ažurirana";
                    TempData[Constants.ErrorOccurred] = false;
                    return RedirectToAction(nameof(Get), new { id = dionica.Id });
                }
                catch (Exception exc)
                {
                    logger.LogError("Pogreška prilikom ažuriranja dionice: " + exc.CompleteExceptionMessage());
                    ModelState.AddModelError(string.Empty, exc.CompleteExceptionMessage());
                    await PrepareDropDownLists();
                    return PartialView(dionica);
                }
            }
            else
            {
                await PrepareDropDownLists();
                return PartialView(dionica);
            }
        }

        private void CopyValues(Dionica dionica, Dionica dionicaRes)
        {
            //ovdje bi elegantnije rijesenje bilo preko nekog autoMappera, ali trenutno sam ostavio ovako
            dionica.Naziv = dionicaRes.Naziv;
            dionica.OgranicenjeBrzine = dionicaRes.OgranicenjeBrzine;
            dionica.Duljina = dionicaRes.Duljina;
            dionica.GodinaOtvaranja = dionicaRes.GodinaOtvaranja;
            dionica.OtvorenZaProlaz = dionicaRes.OtvorenZaProlaz;
            dionica.DozvolaTeretnimVozilima = dionicaRes.DozvolaTeretnimVozilima;
            dionica.ZaustavnaTraka = dionicaRes.ZaustavnaTraka;
            dionica.BrojTraka = dionicaRes.BrojTraka;
            dionica.Autocesta = dionicaRes.Autocesta;
            dionica.UlaznaPostaja = dionicaRes.UlaznaPostaja;
            dionica.IzlaznaPostaja = dionicaRes.IzlaznaPostaja;
        }
    }
}
