using RPPP_WebApp.Models;
using iTextSharp.text;
using iTextSharp.text.pdf;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PdfRpt.ColumnsItemsTemplates;
using PdfRpt.Core.Contracts;
using PdfRpt.Core.Helper;
using PdfRpt.FluentInterface;
using System;
using System.Collections.Generic;
using Microsoft.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using System.IO;
using Microsoft.AspNetCore.Hosting;
using OfficeOpenXml;
using RPPP_WebApp.Extensions;
using System.Drawing;

namespace MVC.Controllers
{
    public class LukaReportController : Controller
    {
        private readonly RPPP04Context ctx;
        private readonly IWebHostEnvironment environment;
        private const string ExcelContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";

        public LukaReportController(RPPP04Context ctx, IWebHostEnvironment environment)
        {
            this.ctx = ctx;
            this.environment = environment;
        }

        public IActionResult Index()
        {
            return View();
        }

        public async Task<IActionResult> Autoceste()
        {
            string naslov = "Popis autocesta";
            var autoceste = await ctx.Autocesta
                                      .Include(a => a.Koncesionar)
                                      .AsNoTracking()
                                      .OrderBy(a => a.Naziv)
                                      .ToListAsync();
            PdfReport report = CreateReport(naslov);
            #region Podnožje i zaglavlje
            report.PagesFooter(footer =>
            {
                footer.DefaultFooter(DateTime.Now.ToString("dd.MM.yyyy."));
            })
            .PagesHeader(header =>
            {
                header.CacheHeader(cache: true); // It's a default setting to improve the performance.
                header.DefaultHeader(defaultHeader =>
                {
                    defaultHeader.RunDirection(PdfRunDirection.LeftToRight);
                    defaultHeader.Message(naslov);
                });
            });
            #endregion
            #region Postavljanje izvora podataka i stupaca
            report.MainTableDataSource(dataSource => dataSource.StronglyTypedList(autoceste));

            report.MainTableColumns(columns =>
            {
                columns.AddColumn(column =>
                {
                    column.IsRowNumber(true);
                    column.CellsHorizontalAlignment(HorizontalAlignment.Right);
                    column.IsVisible(true);
                    column.Order(0);
                    column.Width(1);
                    column.HeaderCell("#", horizontalAlignment: HorizontalAlignment.Right);
                });

                columns.AddColumn(column =>
                {
                    column.PropertyName<Autocesta>(a => a.Oznaka);
                    column.CellsHorizontalAlignment(HorizontalAlignment.Center);
                    column.IsVisible(true);
                    column.Order(1);
                    column.Width(2);
                    column.HeaderCell("Oznaka autoceste", horizontalAlignment: HorizontalAlignment.Center);
                });

                columns.AddColumn(column =>
                {
                    column.PropertyName<Autocesta>(a => a.Naziv);
                    column.CellsHorizontalAlignment(HorizontalAlignment.Center);
                    column.IsVisible(true);
                    column.Order(1);
                    column.Width(2);
                    column.HeaderCell("Naziv autoceste", horizontalAlignment: HorizontalAlignment.Center);
                });

                columns.AddColumn(column =>
                {
                    column.PropertyName<Autocesta>(k => k.Koncesionar.Naziv);
                    column.CellsHorizontalAlignment(HorizontalAlignment.Center);
                    column.IsVisible(true);
                    column.Order(4);
                    column.Width(2);
                    column.HeaderCell("Koncesionar", horizontalAlignment: HorizontalAlignment.Center);
                });

            });

            #endregion
            byte[] pdf = report.GenerateAsByteArray();

            if (pdf != null)
            {
                Response.Headers.Add("content-disposition", "inline; filename=autoceste.pdf");
                return File(pdf, "application/pdf");
                //return File(pdf, "application/pdf", "drzave.pdf"); //Otvara save as dialog
            }
            else
            {
                return NotFound();
            }
        }

        public async Task<IActionResult> Dionice()
        {
            string naslov = "Popis dionica autoceste";
            var dionice = await ctx.Dionica
                                    .AsNoTracking()
                                    .Include(d => d.Autocesta)
                                    .Include(d => d.UlaznaPostaja)
                                    .Include(d => d.IzlaznaPostaja)
                                    .OrderBy(d => d.Naziv)
                                    .ToListAsync();
            PdfReport report = CreateReport(naslov);
            #region Podnožje i zaglavlje
            report.PagesFooter(footer =>
            {
                footer.DefaultFooter(DateTime.Now.ToString("dd.MM.yyyy."));
            })
            .PagesHeader(header =>
            {
                header.CacheHeader(cache: true); // It's a default setting to improve the performance.
                header.DefaultHeader(defaultHeader =>
                {
                    defaultHeader.RunDirection(PdfRunDirection.LeftToRight);
                    defaultHeader.Message(naslov);
                });
            });
            #endregion
            #region Postavljanje izvora podataka i stupaca
            report.MainTableDataSource(dataSource => dataSource.StronglyTypedList(dionice));

            report.MainTableColumns(columns =>
            {
                columns.AddColumn(column =>
                {
                    column.IsRowNumber(true);
                    column.CellsHorizontalAlignment(HorizontalAlignment.Right);
                    column.IsVisible(true);
                    column.Order(0);
                    column.Width(1);
                    column.HeaderCell("#", horizontalAlignment: HorizontalAlignment.Right);
                });

                columns.AddColumn(column =>
                {
                    column.PropertyName<Dionica>(d => d.Naziv);
                    column.CellsHorizontalAlignment(HorizontalAlignment.Center);
                    column.IsVisible(true);
                    column.Order(1);
                    column.Width(2);
                    column.HeaderCell("Naziv dionice", horizontalAlignment: HorizontalAlignment.Center);
                });

                columns.AddColumn(column =>
                {
                    column.PropertyName<Dionica>(d => d.Autocesta.Naziv);
                    column.CellsHorizontalAlignment(HorizontalAlignment.Center);
                    column.IsVisible(true);
                    column.Order(1);
                    column.Width(2);
                    column.HeaderCell("Naziv autoceste", horizontalAlignment: HorizontalAlignment.Center);
                });

                columns.AddColumn(column =>
                {
                    column.PropertyName<Dionica>(d => d.UlaznaPostaja.Naziv);
                    column.CellsHorizontalAlignment(HorizontalAlignment.Center);
                    column.IsVisible(true);
                    column.Order(2);
                    column.Width(1);
                    column.HeaderCell("Ulazna postaja", horizontalAlignment: HorizontalAlignment.Center);
                });

                columns.AddColumn(column =>
                {
                    column.PropertyName<Dionica>(d => d.IzlaznaPostaja.Naziv);
                    column.CellsHorizontalAlignment(HorizontalAlignment.Center);
                    column.IsVisible(true);
                    column.Order(2);
                    column.Width(1);
                    column.HeaderCell("Izlazna postaja", horizontalAlignment: HorizontalAlignment.Center);
                });

                columns.AddColumn(column =>
                {
                    column.PropertyName<Dionica>(d => d.BrojTraka);
                    column.CellsHorizontalAlignment(HorizontalAlignment.Center);
                    column.IsVisible(true);
                    column.Order(2);
                    column.Width(1);
                    column.HeaderCell("Broj traka", horizontalAlignment: HorizontalAlignment.Center);
                });

                columns.AddColumn(column =>
                {
                    column.PropertyName<Dionica>(d => d.ZaustavnaTraka);
                    column.CellsHorizontalAlignment(HorizontalAlignment.Center);
                    column.IsVisible(true);
                    column.Order(3);
                    column.Width(1);
                    column.HeaderCell("Zaustavna traka", horizontalAlignment: HorizontalAlignment.Center);
                });

                columns.AddColumn(column =>
                {
                    column.PropertyName<Dionica>(d => d.DozvolaTeretnimVozilima);
                    column.CellsHorizontalAlignment(HorizontalAlignment.Center);
                    column.IsVisible(true);
                    column.Order(4);
                    column.Width(1);
                    column.HeaderCell("Dozvola teretnim vozilima", horizontalAlignment: HorizontalAlignment.Center);
                });

                columns.AddColumn(column =>
                {
                    column.PropertyName<Dionica>(d => d.OtvorenZaProlaz);
                    column.CellsHorizontalAlignment(HorizontalAlignment.Center);
                    column.IsVisible(true);
                    column.Order(4);
                    column.Width(1);
                    column.HeaderCell("Otvorena za prolaz", horizontalAlignment: HorizontalAlignment.Center);
                });

                columns.AddColumn(column =>
                {
                    column.PropertyName<Dionica>(d => d.GodinaOtvaranja);
                    column.CellsHorizontalAlignment(HorizontalAlignment.Center);
                    column.IsVisible(true);
                    column.Order(4);
                    column.Width(1);
                    column.HeaderCell("Godina otvaranja", horizontalAlignment: HorizontalAlignment.Center);
                });

                columns.AddColumn(column =>
                {
                    column.PropertyName<Dionica>(d => d.Duljina);
                    column.CellsHorizontalAlignment(HorizontalAlignment.Center);
                    column.IsVisible(true);
                    column.Order(4);
                    column.Width(1);
                    column.HeaderCell("Duljina", horizontalAlignment: HorizontalAlignment.Center);
                });

                columns.AddColumn(column =>
                {
                    column.PropertyName<Dionica>(d => d.OgranicenjeBrzine);
                    column.CellsHorizontalAlignment(HorizontalAlignment.Center);
                    column.IsVisible(true);
                    column.Order(4);
                    column.Width(1);
                    column.HeaderCell("Ogr. brzine", horizontalAlignment: HorizontalAlignment.Center);
                });
            });

            #endregion
            byte[] pdf = report.GenerateAsByteArray();

            if (pdf != null)
            {
                Response.Headers.Add("content-disposition", "inline; filename=dionice.pdf");
                return File(pdf, "application/pdf");
                //return File(pdf, "application/pdf", "drzave.pdf"); //Otvara save as dialog
            }
            else
            {
                return NotFound();
            }
        }

        public async Task<IActionResult> Koncesionari()
        {
            string naslov = "Popis koncesionara";
            var kocesionari = await ctx.Koncesionar
                                    .AsNoTracking()
                                    .OrderBy(d => d.Naziv)
                                    .ToListAsync();
            PdfReport report = CreateReport(naslov);
            #region Podnožje i zaglavlje
            report.PagesFooter(footer =>
            {
                footer.DefaultFooter(DateTime.Now.ToString("dd.MM.yyyy."));
            })
            .PagesHeader(header =>
            {
                header.CacheHeader(cache: true); // It's a default setting to improve the performance.
                header.DefaultHeader(defaultHeader =>
                {
                    defaultHeader.RunDirection(PdfRunDirection.LeftToRight);
                    defaultHeader.Message(naslov);
                });
            });
            #endregion
            #region Postavljanje izvora podataka i stupaca
            report.MainTableDataSource(dataSource => dataSource.StronglyTypedList(kocesionari));

            report.MainTableColumns(columns =>
            {
                columns.AddColumn(column =>
                {
                    column.IsRowNumber(true);
                    column.CellsHorizontalAlignment(HorizontalAlignment.Right);
                    column.IsVisible(true);
                    column.Order(0);
                    column.Width(1);
                    column.HeaderCell("#", horizontalAlignment: HorizontalAlignment.Right);
                });

                columns.AddColumn(column =>
                {
                    column.PropertyName<Koncesionar>(d => d.Naziv);
                    column.CellsHorizontalAlignment(HorizontalAlignment.Center);
                    column.IsVisible(true);
                    column.Order(1);
                    column.Width(2);
                    column.HeaderCell("Naziv koncesionara", horizontalAlignment: HorizontalAlignment.Center);
                });

                columns.AddColumn(column =>
                {
                    column.PropertyName<Koncesionar>(oo => oo.Adresa);
                    column.CellsHorizontalAlignment(HorizontalAlignment.Center);
                    column.IsVisible(true);
                    column.Order(1);
                    column.Width(2);
                    column.HeaderCell("Adresa koncesionara", horizontalAlignment: HorizontalAlignment.Center);
                });

                columns.AddColumn(column =>
                {
                    column.PropertyName<Koncesionar>(oo => oo.Email);
                    column.CellsHorizontalAlignment(HorizontalAlignment.Center);
                    column.IsVisible(true);
                    column.Order(2);
                    column.Width(1);
                    column.HeaderCell("Email koncesionara", horizontalAlignment: HorizontalAlignment.Center);
                });

                columns.AddColumn(column =>
                {
                    column.PropertyName<Koncesionar>(oo => oo.KoncesijaOd);
                    column.CellsHorizontalAlignment(HorizontalAlignment.Center);
                    column.IsVisible(true);
                    column.Order(3);
                    column.Width(1);
                    column.HeaderCell("Koncesija od", horizontalAlignment: HorizontalAlignment.Center);
                });

                columns.AddColumn(column =>
                {
                    column.PropertyName<Koncesionar>(oo => oo.KoncesijaDo);
                    column.CellsHorizontalAlignment(HorizontalAlignment.Center);
                    column.IsVisible(true);
                    column.Order(4);
                    column.Width(1);
                    column.HeaderCell("Koncesija do", horizontalAlignment: HorizontalAlignment.Center);
                });

                
            });

            #endregion
            byte[] pdf = report.GenerateAsByteArray();

            if (pdf != null)
            {
                Response.Headers.Add("content-disposition", "inline; filename=koncesionari.pdf");
                return File(pdf, "application/pdf");
                //return File(pdf, "application/pdf", "drzave.pdf"); //Otvara save as dialog
            }
            else
            {
                return NotFound();
            }
        }

        public async Task<IActionResult> NaplatnePostaje()
        {
            string naslov = "Naplatne postaje";
            var naplatnePostaje = await ctx.NaplatnaPostaja
                                    .AsNoTracking()
                                    .OrderBy(d => d.Naziv)
                                    .Include(np => np.Autocesta)
                                    .ToListAsync();
            PdfReport report = CreateReport(naslov);
            #region Podnožje i zaglavlje
            report.PagesFooter(footer =>
            {
                footer.DefaultFooter(DateTime.Now.ToString("dd.MM.yyyy."));
            })
            .PagesHeader(header =>
            {
                header.CacheHeader(cache: true); // It's a default setting to improve the performance.
                header.DefaultHeader(defaultHeader =>
                {
                    defaultHeader.RunDirection(PdfRunDirection.LeftToRight);
                    defaultHeader.Message(naslov);
                });
            });
            #endregion
            #region Postavljanje izvora podataka i stupaca
            report.MainTableDataSource(dataSource => dataSource.StronglyTypedList(naplatnePostaje));

            report.MainTableColumns(columns =>
            {
                columns.AddColumn(column =>
                {
                    column.IsRowNumber(true);
                    column.CellsHorizontalAlignment(HorizontalAlignment.Right);
                    column.IsVisible(true);
                    column.Order(0);
                    column.Width(1);
                    column.HeaderCell("#", horizontalAlignment: HorizontalAlignment.Right);
                });

                columns.AddColumn(column =>
                {
                    column.PropertyName<NaplatnaPostaja>(vo => vo.Naziv);
                    column.CellsHorizontalAlignment(HorizontalAlignment.Center);
                    column.IsVisible(true);
                    column.Order(1);
                    column.Width(2);
                    column.HeaderCell("Naziv naplatne postaje", horizontalAlignment: HorizontalAlignment.Center);
                });

                columns.AddColumn(column =>
                {
                    column.PropertyName<NaplatnaPostaja>(vo => vo.KoordinataX);
                    column.CellsHorizontalAlignment(HorizontalAlignment.Center);
                    column.IsVisible(true);
                    column.Order(2);
                    column.Width(1);
                    column.HeaderCell("Koordinata x", horizontalAlignment: HorizontalAlignment.Center);
                });

                columns.AddColumn(column =>
                {
                    column.PropertyName<NaplatnaPostaja>(d => d.KoordinataY);
                    column.CellsHorizontalAlignment(HorizontalAlignment.Center);
                    column.IsVisible(true);
                    column.Order(3);
                    column.Width(1);
                    column.HeaderCell("Koordinata y", horizontalAlignment: HorizontalAlignment.Center);
                });

                columns.AddColumn(column =>
                {
                    column.PropertyName<NaplatnaPostaja>(d => d.GodinaOtvaranja);
                    column.CellsHorizontalAlignment(HorizontalAlignment.Center);
                    column.IsVisible(true);
                    column.Order(4);
                    column.Width(1);
                    column.HeaderCell("Godina otvaranja", horizontalAlignment: HorizontalAlignment.Center);
                });

            });

            #endregion
            byte[] pdf = report.GenerateAsByteArray();

            if (pdf != null)
            {
                Response.Headers.Add("content-disposition", "inline; filename=naplatnePostaje.pdf");
                return File(pdf, "application/pdf");
                //return File(pdf, "application/pdf", "drzave.pdf"); //Otvara save as dialog
            }
            else
            {
                return NotFound();
            }
        }

        #region Export u Excel
        public async Task<IActionResult> ExcelSimpleAutocesta()
        {
            var autoceste = await ctx.Autocesta
                                  .Include(co => co.Koncesionar)
                                  .AsNoTracking()
                                  .OrderBy(co => co.Naziv)
                                  .ToListAsync();
            byte[] content;
            using (ExcelPackage excel = new ExcelPackage())
            {
                excel.Workbook.Properties.Title = "Popis autocesta";
                excel.Workbook.Properties.Author = "Luka autoceste";
                var worksheet = excel.Workbook.Worksheets.Add("Autoceste");

                //First add the headers
                worksheet.Cells[1, 1].Value = "Oznaka";
                worksheet.Cells[1, 2].Value = "Naziv";
                worksheet.Cells[1, 3].Value = "Koncesionar";
                

                for (int i = 0; i < autoceste.Count; i++)
                {
                    worksheet.Cells[i + 2, 1].Value = autoceste[i].Oznaka;
                    worksheet.Cells[i + 2, 2].Value = autoceste[i].Naziv;
                    worksheet.Cells[i + 2, 3].Value = autoceste[i].Koncesionar.Naziv;
                }

                worksheet.Cells[1, 1, autoceste.Count + 1, 3].AutoFitColumns();

                content = excel.GetAsByteArray();
            }
            return File(content, ExcelContentType, "autoceste.xlsx");
        }

        public async Task<IActionResult> ExcelSimpleDionice()
        {
            var dionice = await ctx.Dionica
                                    .AsNoTracking()
                                    .Include(d => d.Autocesta)
                                    .Include(d => d.UlaznaPostaja)
                                    .Include(d => d.IzlaznaPostaja)
                                    .OrderBy(co => co.Naziv)
                                    .ToListAsync();
            byte[] content;
            using (ExcelPackage excel = new ExcelPackage())
            {
                excel.Workbook.Properties.Title = "Popis dionica";
                excel.Workbook.Properties.Author = "Luka dionice";
                var worksheet = excel.Workbook.Worksheets.Add("Dionice");

                //First add the headers
                worksheet.Cells[1, 1].Value = "Naziv";
                worksheet.Cells[1, 2].Value = "Broj traka";
                worksheet.Cells[1, 3].Value = "Zaustavna traka";
                worksheet.Cells[1, 4].Value = "Dozvola teretnim vozilima";
                worksheet.Cells[1, 5].Value = "Otvorena za prolaz";
                worksheet.Cells[1, 6].Value = "Godina otvaranja";
                worksheet.Cells[1, 7].Value = "Duljina";
                worksheet.Cells[1, 8].Value = "Ograničenje brzine";
                worksheet.Cells[1, 9].Value = "Autocesta";
                worksheet.Cells[1, 10].Value = "Ulaz";
                worksheet.Cells[1, 11].Value = "Izlaz";

                for (int i = 0; i < dionice.Count; i++)
                {
                    worksheet.Cells[i + 2, 1].Value = dionice[i].Naziv;
                    worksheet.Cells[i + 2, 2].Value = dionice[i].BrojTraka;
                    worksheet.Cells[i + 2, 3].Value = dionice[i].ZaustavnaTraka;
                    worksheet.Cells[i + 2, 4].Value = dionice[i].DozvolaTeretnimVozilima;
                    worksheet.Cells[i + 2, 5].Value = dionice[i].OtvorenZaProlaz;
                    worksheet.Cells[i + 2, 6].Value = dionice[i].GodinaOtvaranja;
                    worksheet.Cells[i + 2, 7].Value = dionice[i].Duljina;
                    worksheet.Cells[i + 2, 8].Value = dionice[i].OgranicenjeBrzine;
                    worksheet.Cells[i + 2, 9].Value = dionice[i].Autocesta.Naziv;
                    worksheet.Cells[i + 2, 10].Value = dionice[i].UlaznaPostaja.Naziv;
                    worksheet.Cells[i + 2, 11].Value = dionice[i].IzlaznaPostaja.Naziv;
                }

                worksheet.Cells[1, 1, dionice.Count + 1, 11].AutoFitColumns();

                content = excel.GetAsByteArray();
            }
            return File(content, ExcelContentType, "dionice.xlsx");
        }

        public async Task<IActionResult> ExcelSimpleKoncesionar()
        {
            var koncesionari = await ctx.Koncesionar
                                    .AsNoTracking()
                                    .OrderBy(co => co.Naziv)
                                    .ToListAsync();
            byte[] content;
            using (ExcelPackage excel = new ExcelPackage())
            {
                excel.Workbook.Properties.Title = "Popis koncesionara";
                excel.Workbook.Properties.Author = "Luka";
                var worksheet = excel.Workbook.Worksheets.Add("Koncesionari");

                //First add the headers
                worksheet.Cells[1, 1].Value = "Naziv";
                worksheet.Cells[1, 2].Value = "Adresa";
                worksheet.Cells[1, 3].Value = "Email";
                worksheet.Cells[1, 4].Value = "Koncesija od";
                worksheet.Cells[1, 5].Value = "Koncesija do";
                

                for (int i = 0; i < koncesionari.Count; i++)
                {
                    worksheet.Cells[i + 2, 1].Value = koncesionari[i].Naziv;
                    worksheet.Cells[i + 2, 2].Value = koncesionari[i].Adresa;
                    worksheet.Cells[i + 2, 3].Value = koncesionari[i].Email;
                    worksheet.Cells[i + 2, 4].Value = koncesionari[i].KoncesijaOd.ToString();
                    worksheet.Cells[i + 2, 5].Value = koncesionari[i].KoncesijaOd.ToString();
                   
                }

                worksheet.Cells[1, 1, koncesionari.Count + 1, 5].AutoFitColumns();

                content = excel.GetAsByteArray();
            }
            return File(content, ExcelContentType, "koncesionari.xlsx");
        }

        public async Task<IActionResult> ExcelSimpleNaplatnePostaje()
        {
            var postaje = await ctx.NaplatnaPostaja
                                    .Include(np => np.Autocesta)
                                    .AsNoTracking()
                                    .OrderBy(vo => vo.Naziv)
                                    .ToListAsync();
            byte[] content;
            using (ExcelPackage excel = new ExcelPackage())
            {
                excel.Workbook.Properties.Title = "Popis naplatnih postaja";
                excel.Workbook.Properties.Author = "Luka";
                var worksheet = excel.Workbook.Worksheets.Add("Naplatne postaje");

                //First add the headers
                worksheet.Cells[1, 1].Value = "Naziv";
                worksheet.Cells[1, 2].Value = "Autocesta";
                worksheet.Cells[1, 3].Value = "Koordinata x";
                worksheet.Cells[1, 4].Value = "Koordinata y";
                worksheet.Cells[1, 5].Value = "Godina otvaanja";

                for (int i = 0; i < postaje.Count; i++)
                {
                    worksheet.Cells[i + 2, 1].Value = postaje[i].Naziv;
                    worksheet.Cells[i + 2, 2].Value = postaje[i].Autocesta.Naziv;
                    worksheet.Cells[i + 2, 3].Value = postaje[i].KoordinataX;
                    worksheet.Cells[i + 2, 4].Value = postaje[i].KoordinataY;
                    worksheet.Cells[i + 2, 5].Value = postaje[i].GodinaOtvaranja;
                }

                worksheet.Cells[1, 1, postaje.Count + 1, 5].AutoFitColumns();

                content = excel.GetAsByteArray();
            }
            return File(content, ExcelContentType, "naplatnePostaje.xlsx");
        }

        
        public async Task<IActionResult> ExcelComplexAutoceste()
        {
            var autoceste = await ctx.Autocesta.Include(a => a.Koncesionar).ToListAsync();

            byte[] content;
            using (ExcelPackage excel = new ExcelPackage())
            {
                excel.Workbook.Properties.Title = "MD autoceste";
                excel.Workbook.Properties.Author = "Luka";
                var worksheet = excel.Workbook.Worksheets.Add("MD autoceste");
                worksheet.Cells[2, 1].Value = "Autocesta";
                worksheet.Cells[4, 1].Value = "Dionica";
                worksheet.Cells[2, 1].AutoFitColumns();
                worksheet.Cells[4, 1].AutoFitColumns();


                //First add the headers
                for (int i = 0; i < autoceste.Count; i++)
                {
                    worksheet.Cells[1, i * 12 + 2].Value = "Oznaka";
                    worksheet.Cells[1, i * 12 + 3].Value = "Naziv";
                    worksheet.Cells[1, i * 12 + 4].Value = "Koncesionar";
                    worksheet.Cells[2, i * 12 + 2].Value = autoceste[i].Oznaka;
                    worksheet.Cells[2, i * 12 + 3].Value = autoceste[i].Naziv;
                    worksheet.Cells[2, i * 12 + 4].Value = autoceste[i].Koncesionar.Naziv;

                    var dionice = await ctx.Dionica.
                                    Where(d => d.AutocestaId == autoceste[i].Id)
                                    .Include(d => d.Autocesta)
                                    .Include(d => d.UlaznaPostaja)
                                    .Include(d => d.IzlaznaPostaja)
                                    .ToListAsync();
                    //var odrzavanja = await ctx.OdrzavanjeObjekta.Where(oo => oo.CestovniObjektId == cestovniObjekti[i].Id).Include(oo => oo.Vrsta).ToListAsync();
                    worksheet.Cells[4, i * 12 + 2].Value = "Naziv";
                    worksheet.Cells[4, i * 12 + 3].Value = "Ulaz";
                    worksheet.Cells[4, i * 12 + 4].Value = "Izlaz";
                    worksheet.Cells[4, i * 12 + 5].Value = "Autocesta";
                    worksheet.Cells[4, i * 12 + 6].Value = "Broj traka";
                    worksheet.Cells[4, i * 12 + 7].Value = "Zaustavna traka";
                    worksheet.Cells[4, i * 12 + 8].Value = "Dozvola teretnim vozilima";
                    worksheet.Cells[4, i * 12 + 9].Value = "Otvoren za prolaz";
                    worksheet.Cells[4, i * 12 + 10].Value = "Godina otvaranja";
                    worksheet.Cells[4, i * 12 + 11].Value = "Duljina";
                    worksheet.Cells[4, i * 12 + 12].Value = "Ograničenje brzine";
                    worksheet.Cells[4, i * 12 + 13].Value = " ";
                    worksheet.Cells[4, i * 12 + 2].AutoFitColumns();
                    worksheet.Cells[4, i * 12 + 3].AutoFitColumns();
                    worksheet.Cells[4, i * 12 + 4].AutoFitColumns();
                    worksheet.Cells[4, i * 12 + 5].AutoFitColumns();
                    worksheet.Cells[4, i * 12 + 6].AutoFitColumns();
                    worksheet.Cells[4, i * 12 + 7].AutoFitColumns();
                    worksheet.Cells[4, i * 12 + 8].AutoFitColumns();
                    worksheet.Cells[4, i * 12 + 9].AutoFitColumns();
                    worksheet.Cells[4, i * 12 + 10].AutoFitColumns();
                    worksheet.Cells[4, i * 12 + 11].AutoFitColumns();
                    worksheet.Cells[4, i * 12 + 12].AutoFitColumns();
                    worksheet.Cells[4, i * 12 + 13].AutoFitColumns();

                    for (int j = 0; j < dionice.Count; j++)
                    {
                        worksheet.Cells[j + 5, i * 12 + 2].Value = dionice[j].Naziv;
                        worksheet.Cells[j + 5, i * 12 + 3].Value = dionice[j].UlaznaPostaja.Naziv;
                        worksheet.Cells[j + 5, i * 12 + 4].Value = dionice[j].IzlaznaPostaja.Naziv;
                        worksheet.Cells[j + 5, i * 12 + 5].Value = dionice[j].Autocesta.Naziv;
                        worksheet.Cells[j + 5, i * 12 + 6].Value = dionice[j].BrojTraka;
                        worksheet.Cells[j + 5, i * 12 + 7].Value = dionice[j].ZaustavnaTraka;
                        worksheet.Cells[j + 5, i * 12 + 8].Value = dionice[j].DozvolaTeretnimVozilima;
                        worksheet.Cells[j + 5, i * 12 + 9].Value = dionice[j].OtvorenZaProlaz;
                        worksheet.Cells[j + 5, i * 12 + 10].Value = dionice[j].GodinaOtvaranja;
                        worksheet.Cells[j + 5, i * 12 + 11].Value = dionice[j].Duljina;
                        worksheet.Cells[j + 5, i * 12 + 12].Value = dionice[j].OgranicenjeBrzine;
                        worksheet.Cells[j + 5, i * 12 + 2].AutoFitColumns();
                        worksheet.Cells[j + 5, i * 12 + 3].AutoFitColumns();
                        worksheet.Cells[j + 5, i * 12 + 4].AutoFitColumns();
                        worksheet.Cells[j + 5, i * 12 + 5].AutoFitColumns();
                        worksheet.Cells[j + 5, i * 12 + 6].AutoFitColumns();
                        worksheet.Cells[j + 5, i * 12 + 7].AutoFitColumns();
                        worksheet.Cells[j + 5, i * 12 + 8].AutoFitColumns();
                        worksheet.Cells[j + 5, i * 12 + 9].AutoFitColumns();
                        worksheet.Cells[j + 5, i * 12 + 10].AutoFitColumns();
                        worksheet.Cells[j + 5, i * 12 + 11].AutoFitColumns();
                        worksheet.Cells[j + 5, i * 12 + 12].AutoFitColumns();
                    }
                }

                content = excel.GetAsByteArray();
            }
            return File(content, ExcelContentType, "LukaSlugecicMDAutoceste.xlsx");
        }

        
        
        #endregion
        
        public async Task<IActionResult> MDAutoceste()
        {
            string naslov = "Luka MD Autoceste";
            var autoceste = await ctx.Autocesta.Include(a => a.Koncesionar).ToListAsync();
            List<AutocestaDionica> autocestaDionice = new List<AutocestaDionica>();
            foreach (var a in autoceste)
            {
                List<Dionica> dionice = await ctx.Dionica
                        .Where(d => d.AutocestaId == a.Id)
                        .Include(d => d.UlaznaPostaja)
                        .Include(d => d.IzlaznaPostaja)
                        .ToListAsync();
                foreach (var dionica in dionice)
                {
                    autocestaDionice.Add(new AutocestaDionica
                    {
                        Id = a.Id,
                        Oznaka = a.Oznaka,
                        Naziv = a.Naziv,
                        Koncesionar = a.Koncesionar.Naziv,
                        NazivDionice = dionica.Naziv,
                        UlaznaPostaja = dionica.UlaznaPostaja.Naziv,
                        IzlaznaPostaja = dionica.IzlaznaPostaja.Naziv,
                        BrojTraka = dionica.BrojTraka,
                        ZaustavnaTraka = dionica.ZaustavnaTraka,
                        DozvolaTeretnimVozilima = dionica.DozvolaTeretnimVozilima,
                        OtvorenZaProlaz = dionica.OtvorenZaProlaz,
                        GodinaOtvaranja = dionica.GodinaOtvaranja,
                        Duljina = dionica.Duljina,
                        OgranicenjeBrzine = dionica.OgranicenjeBrzine
                    });
                }
            }
            autocestaDionice.OrderBy(ad => ad.Id).OrderBy(ad => ad.Naziv);
            PdfReport report = CreateReport(naslov);
            #region Podnožje i zaglavlje
            report.PagesFooter(footer =>
            {
                footer.DefaultFooter(DateTime.Now.ToString("dd.MM.yyyy."));
            })
            .PagesHeader(header =>
            {
                header.CacheHeader(cache: true); // It's a default setting to improve the performance.
                header.CustomHeader(new MasterDetailsHeaders(naslov)
                {
                    PdfRptFont = header.PdfFont
                });
            });
            #endregion
            #region Postavljanje izvora podataka i stupaca
            report.MainTableDataSource(dataSource => dataSource.StronglyTypedList(autocestaDionice));

            report.MainTableColumns(columns =>
            {
                #region Stupci po kojima se grupira
                columns.AddColumn(column =>
                {
                    column.PropertyName<AutocestaDionica>(ad => ad.Id);
                    column.Group(
                        (val1, val2) =>
                        {
                            return (int)val1 == (int)val2;
                        });
                });
                #endregion

                columns.AddColumn(column =>
                {
                    column.IsRowNumber(true);
                    column.CellsHorizontalAlignment(HorizontalAlignment.Right);
                    column.IsVisible(true);
                    column.Width(1);
                    column.HeaderCell("#", horizontalAlignment: HorizontalAlignment.Right);
                });
                columns.AddColumn(column =>
                {
                    column.PropertyName<AutocestaDionica>(ad => ad.NazivDionice);
                    column.CellsHorizontalAlignment(HorizontalAlignment.Center);
                    column.IsVisible(true);
                    column.Width(2);
                    column.HeaderCell("Naziv dionice", horizontalAlignment: HorizontalAlignment.Center);
                });

                columns.AddColumn(column =>
                {
                    column.PropertyName<AutocestaDionica>(ad => ad.UlaznaPostaja);
                    column.CellsHorizontalAlignment(HorizontalAlignment.Right);
                    column.IsVisible(true);
                    column.Width(1);
                    column.HeaderCell("Ulazna postaja", horizontalAlignment: HorizontalAlignment.Center);
                });

                columns.AddColumn(column =>
                {
                    column.PropertyName<AutocestaDionica>(ad => ad.IzlaznaPostaja);
                    column.CellsHorizontalAlignment(HorizontalAlignment.Right);
                    column.IsVisible(true);
                    column.Width(1);
                    column.HeaderCell("Izlazna postaja", horizontalAlignment: HorizontalAlignment.Center);
                });

                columns.AddColumn(column =>
                {
                    column.PropertyName<AutocestaDionica>(ad => ad.BrojTraka);
                    column.CellsHorizontalAlignment(HorizontalAlignment.Right);
                    column.IsVisible(true);
                    column.Width(1);
                    column.HeaderCell("Broj traka", horizontalAlignment: HorizontalAlignment.Center);
                });

                columns.AddColumn(column =>
                {
                    column.PropertyName<AutocestaDionica>(ad => ad.ZaustavnaTraka);
                    column.CellsHorizontalAlignment(HorizontalAlignment.Right);
                    column.IsVisible(true);
                    column.Width(1);
                    column.HeaderCell("Zaustavna traka", horizontalAlignment: HorizontalAlignment.Center);
                });

                columns.AddColumn(column =>
                {
                    column.PropertyName<AutocestaDionica>(ad => ad.DozvolaTeretnimVozilima);
                    column.CellsHorizontalAlignment(HorizontalAlignment.Right);
                    column.IsVisible(true);
                    column.Width(1);
                    column.HeaderCell("Dozvola teretnim vozilima", horizontalAlignment: HorizontalAlignment.Center);
                });
                columns.AddColumn(column =>
                {
                    column.PropertyName<AutocestaDionica>(ad => ad.OtvorenZaProlaz);
                    column.CellsHorizontalAlignment(HorizontalAlignment.Right);
                    column.IsVisible(true);
                    column.Width(1);
                    column.HeaderCell("Otvoren za prolaz", horizontalAlignment: HorizontalAlignment.Center);
                });
                columns.AddColumn(column =>
                {
                    column.PropertyName<AutocestaDionica>(ad => ad.GodinaOtvaranja);
                    column.CellsHorizontalAlignment(HorizontalAlignment.Right);
                    column.IsVisible(true);
                    column.Width(1);
                    column.HeaderCell("Godina otvaranja", horizontalAlignment: HorizontalAlignment.Center);
                });
                columns.AddColumn(column =>
                {
                    column.PropertyName<AutocestaDionica>(ad => ad.Duljina);
                    column.CellsHorizontalAlignment(HorizontalAlignment.Right);
                    column.IsVisible(true);
                    column.Width(1);
                    column.HeaderCell("Duljina", horizontalAlignment: HorizontalAlignment.Center);
                });
                columns.AddColumn(column =>
                {
                    column.PropertyName<AutocestaDionica>(ad => ad.OgranicenjeBrzine);
                    column.CellsHorizontalAlignment(HorizontalAlignment.Right);
                    column.IsVisible(true);
                    column.Width(1);
                    column.HeaderCell("Ograničenje brzine", horizontalAlignment: HorizontalAlignment.Center);
                });

            });

            #endregion
            byte[] pdf = report.GenerateAsByteArray();

            if (pdf != null)
            {
                Response.Headers.Add("content-disposition", "inline; filename=MDAutoceste.pdf");
                return File(pdf, "application/pdf");
            }
            else
                return NotFound();
        }

        #region Master-detail header
        public class MasterDetailsHeaders : IPageHeader
        {
            private string naslov;
            public MasterDetailsHeaders(string naslov)
            {
                this.naslov = naslov;
            }
            public IPdfFont PdfRptFont { set; get; }

            public PdfGrid RenderingGroupHeader(Document pdfDoc, PdfWriter pdfWriter, IList<CellData> newGroupInfo, IList<SummaryCellData> summaryData)
            {
                var oznaka = newGroupInfo.GetSafeStringValueOf(nameof(AutocestaDionica.Oznaka));
                var naziv = newGroupInfo.GetSafeStringValueOf(nameof(AutocestaDionica.Naziv));
                var koncesionar = newGroupInfo.GetValueOf(nameof(AutocestaDionica.Koncesionar));
                

                var table = new PdfGrid(relativeWidths: new[] { 1f, 1f, 1f, 4f ,2f, 2f}) { WidthPercentage = 100 };

                table.AddSimpleRow(
                    (cellData, cellProperties) => {
                        cellData.Value = "Oznaka:";
                        cellProperties.PdfFont = PdfRptFont;
                        cellProperties.PdfFontStyle = DocumentFontStyle.Bold;
                        cellProperties.HorizontalAlignment = HorizontalAlignment.Left;
                    },
                    (cellData, cellProperties) => {
                        cellData.TableRowData = newGroupInfo; //postavi podatke retka za ćeliju
                        var cellTemplate = new HyperlinkField(BaseColor.Black, false)
                        {
                            TextPropertyName = nameof(AutocestaDionica.Oznaka),
                            BasicProperties = new CellBasicProperties
                            {
                                HorizontalAlignment = HorizontalAlignment.Left,
                                PdfFontStyle = DocumentFontStyle.Bold,
                                PdfFont = PdfRptFont
                            }
                        };

                        cellData.CellTemplate = cellTemplate;
                        cellProperties.PdfFont = PdfRptFont;
                        cellProperties.HorizontalAlignment = HorizontalAlignment.Left;
                    },
                    (cellData, cellProperties) => {
                        cellData.Value = "Naziv:";
                        cellProperties.PdfFont = PdfRptFont;
                        cellProperties.PdfFontStyle = DocumentFontStyle.Bold;
                        cellProperties.HorizontalAlignment = HorizontalAlignment.Left;
                    },
                    (cellData, cellProperties) => {
                        cellData.Value = naziv;
                        cellProperties.PdfFont = PdfRptFont;
                        cellProperties.HorizontalAlignment = HorizontalAlignment.Left;
                    },
                    (cellData, cellProperties) =>
                    {
                        cellData.Value = "Koncesionar";
                        cellProperties.PdfFont = PdfRptFont;
                        cellProperties.PdfFontStyle = DocumentFontStyle.Bold;
                        cellProperties.HorizontalAlignment = HorizontalAlignment.Left;
                    },
                    (cellData, cellProperties) =>
                    {
                        cellData.Value = koncesionar;
                        cellProperties.PdfFont = PdfRptFont;
                        cellProperties.HorizontalAlignment = HorizontalAlignment.Left;
                    });

               
                    
                   
                return table.AddBorderToTable(borderColor: BaseColor.LightGray, spacingBefore: 5f);
            }

            public PdfGrid RenderingReportHeader(Document pdfDoc, PdfWriter pdfWriter, IList<SummaryCellData> summaryData)
            {
                var table = new PdfGrid(numColumns: 1) { WidthPercentage = 100 };
                table.AddSimpleRow(
                    (cellData, cellProperties) =>
                    {
                        cellData.Value = naslov;
                        cellProperties.PdfFont = PdfRptFont;
                        cellProperties.PdfFontStyle = DocumentFontStyle.Bold;
                        cellProperties.HorizontalAlignment = HorizontalAlignment.Center;
                    });
                return table.AddBorderToTable();
            }
        }
        

        
        #endregion

        private PdfReport CreateReport(string naslov)
        {
            var pdf = new PdfReport();

            pdf.DocumentPreferences(doc =>
            {
                doc.Orientation(PageOrientation.Portrait);
                doc.PageSize(PdfPageSize.A4);
                doc.DocumentMetadata(new DocumentMetadata
                {
                    Author = "Najjaca Grupa 04",
                    Application = "RPPP_WebApp.MVC Core",
                    Title = naslov
                });
                doc.Compression(new CompressionSettings
                {
                    EnableCompression = true,
                    EnableFullCompression = true
                });
            })
            //fix za linux https://github.com/VahidN/PdfReport.Core/issues/40
            .DefaultFonts(fonts => {
                fonts.Path(Path.Combine(environment.WebRootPath, "fonts", "verdana.ttf"),
                           Path.Combine(environment.WebRootPath, "fonts", "tahoma.ttf"));
                fonts.Size(7);
                fonts.Color(System.Drawing.Color.Black);
            })
            //
            .MainTableTemplate(template =>
            {
                template.BasicTemplate(BasicTemplate.ProfessionalTemplate);
            })
            .MainTablePreferences(table =>
            {
                table.ColumnsWidthsType(TableColumnWidthType.Relative);
                //table.NumberOfDataRowsPerPage(20);
                table.GroupsPreferences(new GroupsPreferences
                {
                    GroupType = GroupType.HideGroupingColumns,
                    RepeatHeaderRowPerGroup = true,
                    ShowOneGroupPerPage = true,
                    SpacingBeforeAllGroupsSummary = 5f,
                    NewGroupAvailableSpacingThreshold = 150,
                    SpacingAfterAllGroupsSummary = 5f
                });
                table.SpacingAfter(4f);
            });

            return pdf;
        }
    }
}
