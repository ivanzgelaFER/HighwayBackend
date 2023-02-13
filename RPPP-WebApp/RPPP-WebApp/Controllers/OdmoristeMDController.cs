using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using RPPP_WebApp.Extensions;
using RPPP_WebApp.ViewModels;
using RPPP_WebApp;
using System.Collections;
using RPPP_WebApp.Models;
using RPPP_WebApp.Extensions.Selectors;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Text.Json;
using System.Reflection.Metadata;
using System.Xml.Linq;
using static iTextSharp.text.pdf.XfaForm;
using NLog.Web.LayoutRenderers;

namespace MVC.Controllers
{
    public class OdmoristeMDController : Controller
    {
        private readonly RPPP04Context ctx;
        private readonly AppSettings appData;

        public OdmoristeMDController(RPPP04Context ctx, IOptionsSnapshot<AppSettings> options)
        {
            this.ctx = ctx;
            appData = options.Value;
        }

        public async Task<IActionResult> Index(string filter, int page = 1, int sort = 1, bool ascending = true)
        {
            int pagesize = appData.PageSize;
            var query = ctx.Odmoriste.AsQueryable();

            #region Apply filter
            OdmoristeFilter of = OdmoristeFilter.FromString(filter);
            if (!of.IsEmpty())
            {
                if (of.DionicaId.HasValue)
                {
                    of.NazDionice = await ctx.Dionica
                                              .Where(d => d.Id == of.DionicaId)
                                              .Select(vd => vd.Naziv)
                                              .FirstOrDefaultAsync();
                }
                query = of.Apply(query);
            }
            #endregion

            int count = await query.CountAsync();

            var pagingInfo = new PagingInfo
            {
                CurrentPage = page,
                Sort = sort,
                Ascending = ascending,
                ItemsPerPage = pagesize,
                TotalItems = count
            };

            if (count > 0 && (page < 1 || page > pagingInfo.TotalPages))
            {
                return RedirectToAction(nameof(Index), new { page = 1, sort, ascending, filter });
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
                              Dionica = o.Dionica.Naziv,
                              DionicaId = o.Dionica.Id,
                              DionicaObj = o.Dionica

                          })
                          .Skip((page - 1) * pagesize)
                          .Take(pagesize)
                          .ToListAsync();

            foreach (var o in odmorista)
            {
                var listaNaziva = await ctx.PopratniSadrzaj
                                      .Where(p => p.OdmoristeId == o.IdOdmorista)
                                      .Select(p => p.Naziv)
                                      .ToListAsync();
                string naziv = string.Join(", ", listaNaziva);

                o.NaziviPopratnihSadrzaja = naziv;
            }

            var model = new OdmoristaViewModel
            {
                Odmorista = odmorista,
                PagingInfo = pagingInfo,
                Filter = of
            };

            return View(model);
        }

        [HttpPost]
        public IActionResult Filter(OdmoristeFilter filter)
        {
            return RedirectToAction(nameof(Index), new { filter = filter.ToString() });
        }

        public async Task<IActionResult> Show(int id, int? position, string filter, int page = 1, int sort = 1, bool ascending = true, string viewName = nameof(Show))
        {
            var odmoriste = await ctx.Odmoriste
                                    .Where(o => o.Id == id)
                                    .Select(o => new OdmoristeViewModel
                                    {
                                        IdOdmorista = o.Id,
                                        NazivOdmorista = o.Naziv,
                                        KoordinataX = (int)o.KoordinataX,
                                        KoordinataY = (int)o.KoordinataY,
                                        GodinaOtvaranja = (int)o.GodinaOtvaranja,
                                        Dionica = o.Dionica.Naziv,
                                        DionicaId = o.Dionica.Id,
                                        DionicaObj = o.Dionica
                                    })
                                    .FirstOrDefaultAsync();
            if (odmoriste == null)
            {
                return NotFound($"Odmoriste {id} ne postoji");
            }
            else
            {
                var popratniSadrzaji = await ctx.PopratniSadrzaj
                                      .Where(s => s.OdmoristeId == odmoriste.IdOdmorista)
                                      .OrderBy(s => s.Id)
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
                                          Odmoriste = p.Odmoriste.Naziv,
                                          VrstaPopratnogObj = p.VrstaSadrzaja 

                                      })
                                      .ToListAsync();

                odmoriste.PopratniSadrzaji = popratniSadrzaji;

                ViewBag.Page = page;
                ViewBag.Sort = sort;
                ViewBag.Ascending = ascending;
                ViewBag.Filter = filter;
                ViewBag.Position = position;

                return View(viewName, odmoriste);
            }
        }

        private async Task PrepareDropDownList()
        {

            var dionice = await ctx.Dionica.OrderBy(d => d.Naziv).Select(d => new { d.Id, d.Naziv }).ToListAsync();

            ViewBag.Odmorista = new SelectList(dionice, nameof(Dionica.Id), nameof(Dionica.Naziv));
        }

        [HttpGet]
        public async Task<IActionResult> Create()
        {
            await PrepareDropDownList();
            var odmoristeViewModel = new OdmoristeViewModel(); 
            return View(odmoristeViewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(OdmoristeViewModel odmoristeModel)
        {
            if (ModelState.IsValid)
            {
                Odmoriste odmoriste = new Odmoriste();

                odmoriste.Naziv = odmoristeModel.NazivOdmorista; 
                odmoriste.GodinaOtvaranja = odmoristeModel.GodinaOtvaranja;
                odmoriste.KoordinataX = odmoristeModel.KoordinataX;
                odmoriste.KoordinataY = odmoristeModel.KoordinataY;
                odmoriste.Dionica = odmoristeModel.DionicaObj;
                odmoriste.DionicaId = odmoristeModel.DionicaId;

                foreach(var popratniSadrzaj in odmoristeModel.PopratniSadrzaji)
                {
                    PopratniSadrzaj noviPopratniSadrzaj = new PopratniSadrzaj();
                    noviPopratniSadrzaj.Naziv = popratniSadrzaj.NazivPopratnogSadrzaja;
                    noviPopratniSadrzaj.VrstaSadrzaja = popratniSadrzaj.VrstaPopratnogObj;
                    /*noviPopratniSadrzaj.VrstaSadrzaja.Naziv = popratniSadrzaj.VrstaPopratnog;*/
                    noviPopratniSadrzaj.RadnimDanomOd = popratniSadrzaj.RadnimDanomOd;
                    noviPopratniSadrzaj.RadninDanomDo = popratniSadrzaj.RadninDanomDo;
                    noviPopratniSadrzaj.VikendimaOd = popratniSadrzaj.VikendimaOd;
                    noviPopratniSadrzaj.VikendimaDo = popratniSadrzaj.VikendimaDo;
                    odmoriste.PopratniSadrzaj.Add(noviPopratniSadrzaj);
                }

                try
                {
                    ctx.Add(odmoriste);
                    await ctx.SaveChangesAsync();

                    TempData[Constants.Message] = $"Odmorište {odmoriste.Naziv} dodana.";
                    TempData[Constants.ErrorOccurred] = false;
                    return RedirectToAction(nameof(Edit), new { id = odmoriste.Id });

                }
                catch (Exception exc)
                {
                    ModelState.AddModelError(string.Empty, exc.CompleteExceptionMessage());
                    return View(odmoristeModel);
                }
            }
            else
            {
                return View(odmoristeModel);
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
                                      Dionica = o.Dionica.Naziv,
                                      DionicaId = o.DionicaId,
                                      DionicaObj = o.Dionica
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

        [HttpGet]
        public Task<IActionResult> Edit(int id, int? position, string filter, int page = 1, int sort = 1, bool ascending = true)
        {
            return Show(id, position, filter, page, sort, ascending, viewName: nameof(Edit));
        }

        [HttpPost]
        public async Task<IActionResult> Edit(OdmoristeViewModel model, int? position, string filter, int page = 1, int sort = 1, bool ascending = true)
        {
            ViewBag.Page = page;
            ViewBag.Sort = sort;
            ViewBag.Ascending = ascending;
            ViewBag.Filter = filter;
            ViewBag.Position = position;

            if (ModelState.IsValid)
            {
                var odmoriste = await ctx.Odmoriste
                                        .Include(o => o.PopratniSadrzaj)
                                        .Where(o => o.Id == model.IdOdmorista)
                                        .FirstOrDefaultAsync();
                if (odmoriste == null)
                {
                    return NotFound("Ne postoji odmoriste s id-om: " + model.IdOdmorista);
                }

                odmoriste.GodinaOtvaranja = model.GodinaOtvaranja;
                odmoriste.KoordinataX = model.KoordinataX;
                odmoriste.KoordinataY = model.KoordinataY;
                odmoriste.Dionica = model.DionicaObj;
                odmoriste.DionicaId = model.DionicaId;

                List<int> idPopratnogSadrzaja = model.PopratniSadrzaji
                                          .Where(p => p.IdPopratnogSadrzaja > 0)
                                          .Select(p => p.IdPopratnogSadrzaja)
                                          .ToList();

                //izbaci sve koje su nisu više u modelu
                ctx.RemoveRange(odmoriste.PopratniSadrzaj.Where(p => !idPopratnogSadrzaja.Contains(p.Id)));

                foreach (var popratniSadrzaj in model.PopratniSadrzaji)
                {
                    PopratniSadrzaj noviPopratniSadrzaj;

                    if (popratniSadrzaj.IdPopratnogSadrzaja > 0)
                    {
                        noviPopratniSadrzaj = odmoriste.PopratniSadrzaj.First(p => p.Id == popratniSadrzaj.IdPopratnogSadrzaja);
                    }
                    else
                    {
                        noviPopratniSadrzaj = new PopratniSadrzaj();
                        odmoriste.PopratniSadrzaj.Add(noviPopratniSadrzaj);
                    }

                    noviPopratniSadrzaj.Naziv = popratniSadrzaj.NazivPopratnogSadrzaja;
                    noviPopratniSadrzaj.VrstaSadrzaja = popratniSadrzaj.VrstaPopratnogObj;
                    /*noviPopratniSadrzaj.VrstaSadrzaja.Naziv = popratniSadrzaj.VrstaPopratnog;*/
                    noviPopratniSadrzaj.RadnimDanomOd = popratniSadrzaj.RadnimDanomOd;
                    noviPopratniSadrzaj.RadninDanomDo = popratniSadrzaj.RadninDanomDo;
                    noviPopratniSadrzaj.VikendimaOd = popratniSadrzaj.VikendimaOd;
                    noviPopratniSadrzaj.VikendimaDo = popratniSadrzaj.VikendimaDo;
                    ctx.Update(noviPopratniSadrzaj);
                }

                try
                {
                    ctx.Update(odmoriste);
                    await ctx.SaveChangesAsync();

                    TempData[Constants.Message] = $"Odmoriste {odmoriste.Id} uspješno ažurirano.";
                    TempData[Constants.ErrorOccurred] = false;
                    return RedirectToAction(nameof(Edit), new
                    {
                        id = odmoriste.Id,
                        position,
                        filter,
                        page,
                        sort,
                        ascending
                    });

                }
                catch (Exception exc)
                {
                    ModelState.AddModelError(string.Empty, exc.CompleteExceptionMessage());
                    return View(model);
                }
            }
            else
            {
                return View(model);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int IdOdmorista, int page = 1, int sort = 1, bool ascending = true)
        {
            /*ActionResponseMessage response;*/
            var odmoriste
                = await ctx.Odmoriste.FindAsync(IdOdmorista);

            if (odmoriste != null)
            {
                try
                {
                    string naziv = odmoriste.Naziv;
                    ctx.Remove(odmoriste);
                    await ctx.SaveChangesAsync();
                    /*response = new ActionResponseMessage(MessageType.Success, $"Odmorište {odmoriste.Naziv} uspješno obrisan.");*/
                    TempData[Constants.Message] = $"Odmorište {naziv} uspješno obrisano";
                    TempData[Constants.ErrorOccurred] = false;
                }
                catch (Exception exc)
                {
                    /*response = new ActionResponseMessage(MessageType.Error, "Pogreška prilikom brisanja odmorišta: " + exc.CompleteExceptionMessage());*/
                    TempData[Constants.Message] = "Pogreška prilikom brisanja odmorišta: " + exc.CompleteExceptionMessage();
                    TempData[Constants.ErrorOccurred] = true;
                }
            }
            else
            {
                /*response = new ActionResponseMessage(MessageType.Error, "Ne postoji odmorište s id-om: " + id);*/
                TempData[Constants.Message] = "Ne postoji odmorište s oznakom: " + IdOdmorista;
                TempData[Constants.ErrorOccurred] = true;

            }
            /*Response.Headers["HX-Trigger"] = JsonSerializer.Serialize(new { showMessage = response });
            return response.MessageType == MessageType.Success ? new EmptyResult() : await Get(id);*/

            return RedirectToAction(nameof(Index), new { page = page, sort = sort, ascending = ascending });
        }

        
    }
}
