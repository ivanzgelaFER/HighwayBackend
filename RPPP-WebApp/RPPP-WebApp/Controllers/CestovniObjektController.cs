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
    public class CestovniObjektController : Controller
    {
        private readonly RPPP04Context ctx;
        private readonly AppSettings appData;
        private readonly ILogger<CestovniObjektController> logger;

        public CestovniObjektController(RPPP04Context ctx, IOptionsSnapshot<AppSettings> options, ILogger<CestovniObjektController> logger)
        {
            appData = options.Value;
            this.ctx = ctx;
            this.logger = logger;
        }

        public async Task<IActionResult> Index(int page = 1, int sort = 1, bool ascending = true)
        {

            var query = ctx.CestovniObjekt.AsQueryable();
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

            var cestovniObjekti = await query
                                      .Include(co => co.Dionica)
                                      .Skip((page - 1) * pagesize)
                                      .Take(pagesize)
                                      .ToListAsync();
            
            var model = new CestovniObjektiViewModel
            {
                CestovniObjekt = cestovniObjekti,
                PagingInfo = pagingInfo
            };

            return View(model);
        }
        [HttpGet]
        public async Task<IActionResult> Get(int id)
        {
            var cestovniObjekt = await ctx.CestovniObjekt
                                  .Where(co => co.Id == id)
                                  .SingleOrDefaultAsync();
            if (cestovniObjekt != null)
            {
                return PartialView(cestovniObjekt);
            }
            else
            {
                return NotFound($"Neispravan id cestovnog objekta: {id}");
            }
        }

        [HttpGet]
        public async Task<IActionResult> Create()
        {
            await PrepareDropDownLists();
            return View();
        }


        [HttpPost]
        public async Task<IActionResult> Create(CestovniObjekt cestovniObjekt)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    ctx.Add(cestovniObjekt);
                    await ctx.SaveChangesAsync();
                    logger.LogInformation(new EventId(1000), $"Cestovni objekt {cestovniObjekt.Naziv} dodan.");
                    TempData[Constants.Message] = $"Cestovni objekt uspješno dodan. Id={cestovniObjekt.Id}";
                    TempData[Constants.ErrorOccurred] = false;
                    return RedirectToAction(nameof(Index));
                }
                catch (Exception exc)
                {
                    logger.LogError("Pogreška prilikom dodavanje novog cestovnog objekta: {0}", exc.CompleteExceptionMessage());
                    ModelState.AddModelError(string.Empty, exc.CompleteExceptionMessage());
                    await PrepareDropDownLists();
                    return View(cestovniObjekt);
                }
            }
            else
            {
                await PrepareDropDownLists();
                return View(cestovniObjekt);
            }
        }

        private async Task PrepareDropDownLists()
        {
            var dionice = await ctx.Dionica
                            .Select(d => new {d.Naziv, d.Id})
                            .ToListAsync();
            ViewBag.Dionice = new SelectList(dionice, nameof(Dionica.Id), nameof(Dionica.Naziv));
        }

        [HttpDelete]
        public async Task<IActionResult> Delete(int id)
        {
            var cestovniObjekt = await ctx.CestovniObjekt.FindAsync(id);
            ActionResponseMessage responseMessage;
            if (cestovniObjekt != null)
            {
                try
                {
                    string naziv = cestovniObjekt.Naziv;
                    ctx.Remove(cestovniObjekt);
                    await ctx.SaveChangesAsync();
                    logger.LogInformation($"Cestovni objekt: {naziv} uspješno obrisan");
                    responseMessage = new ActionResponseMessage(MessageType.Success, $"Cestovni objekt {naziv} sa šifrom {id} uspješno obrisan.");
                }
                catch (Exception exc)
                {
                    logger.LogError("Pogreška prilikom brisanja cestovnog objekta: " + exc.CompleteExceptionMessage());
                    responseMessage = new ActionResponseMessage(MessageType.Error, $"Pogreška prilikom brisanja cestovnog objekta: {exc.CompleteExceptionMessage()}");
                }
            }
            else
            {
                responseMessage = new ActionResponseMessage(MessageType.Error, $"Cestovni objekt sa šifrom {id} ne postoji");
            }

            Response.Headers["HX-Trigger"] = JsonSerializer.Serialize(new { showMessage = responseMessage });
            return responseMessage.MessageType == MessageType.Success ? new EmptyResult() : await Get(id);
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int id, int page = 1, int sort = 1, bool ascending = true)
        {
            var cestovniObjekt = await ctx.CestovniObjekt.FindAsync(id);
            if (cestovniObjekt != null)
            {
                await PrepareDropDownLists();
                ViewBag.Page = page;
                ViewBag.Sort = sort;
                ViewBag.Ascending = ascending;
                return View(cestovniObjekt);
            }
            else
            {
                return NotFound($"Neispravan id cestovnog objekta: {id}");
            }
        }

        [HttpPost]
        public async Task<IActionResult> Edit(CestovniObjekt cestovniObjektRes, int page = 1, int sort = 1, bool ascending = true)
        {
            if (cestovniObjektRes == null)
            {
                return NotFound("Nema poslanih podataka");
            }
            var cestovniObjekt = await ctx.CestovniObjekt.FindAsync(cestovniObjektRes.Id);

            if (cestovniObjekt == null)
            {
                logger.LogWarning("Ne postoji cestovni objekt s oznakom: " + cestovniObjekt.Id);
                return NotFound("Ne postoji cestovni objekt s oznakom: " + cestovniObjektRes.Id);
            }

            ViewBag.Page = page;
            ViewBag.Sort = sort;
            ViewBag.Ascending = ascending;

            if (ModelState.IsValid)
            {
                try
                {
                    CopyValues(cestovniObjekt, cestovniObjektRes);
                    ctx.Update(cestovniObjekt);
                    await ctx.SaveChangesAsync();
                    logger.LogInformation($"Cestovni objekt: {cestovniObjekt.Naziv} uspješno ažuriran");
                    //return RedirectToAction(nameof(Index), new { id = cestovniObjekt.Id });
                    TempData[Constants.Message] = $"Cestovni objekt {cestovniObjekt.Naziv} uspješno ažuriran";
                    TempData[Constants.ErrorOccurred] = false;
                    return RedirectToAction(nameof(Index), new { page = page, sort = sort, ascending = ascending });
                }
                catch (Exception exc)
                {
                    logger.LogError("Pogreška prilikom ažuriranja cestovnog objekta: " + exc.CompleteExceptionMessage());
                    ModelState.AddModelError(string.Empty, exc.CompleteExceptionMessage());
                    await PrepareDropDownLists();
                    return View(cestovniObjekt);
                }
            }
            else
            {
                await PrepareDropDownLists();
                return View(cestovniObjekt);
            }
        }

        private void CopyValues(CestovniObjekt cestovniObjekt, CestovniObjekt cestovniObjektRes)
        {
            //ovdje bi elegantnije rijesenje bilo preko nekog autoMappera, ali trenutno sam ostavio ovako
            cestovniObjekt.Naziv = cestovniObjektRes.Naziv;
            cestovniObjekt.DionicaId = cestovniObjektRes.DionicaId;
            cestovniObjekt.TipObjekta = cestovniObjektRes.TipObjekta;
            cestovniObjekt.OgranicenjeBrzine = cestovniObjektRes.OgranicenjeBrzine;
            cestovniObjekt.BrojPrometnihTraka = cestovniObjektRes.BrojPrometnihTraka;
            cestovniObjekt.DuljinaObjekta = cestovniObjektRes.DuljinaObjekta;
            cestovniObjekt.ZaustavniTrak = cestovniObjektRes.ZaustavniTrak;
            cestovniObjekt.DozvolaTeretnimVozilima = cestovniObjektRes.DozvolaTeretnimVozilima;
            cestovniObjekt.Zanimljivost = cestovniObjektRes.Zanimljivost;
            cestovniObjekt.GodinaIzgradnje = cestovniObjektRes.GodinaIzgradnje;
            cestovniObjekt.PjesackaStaza = cestovniObjektRes.PjesackaStaza;
            cestovniObjekt.NaplataPrijelaza = cestovniObjektRes.NaplataPrijelaza;
        }
    }
}
