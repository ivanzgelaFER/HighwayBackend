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
using RPPP_WebApp.ModelsPartial;

namespace MVC.Controllers
{
    public class ZizicReportController : Controller
    {
        private readonly RPPP04Context ctx;
        private readonly IWebHostEnvironment environment;
        private const string ExcelContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";

        public ZizicReportController(RPPP04Context ctx, IWebHostEnvironment environment)
        {
            this.ctx = ctx;
            this.environment = environment;
        }

        public IActionResult Index()
        {
            return View();
        }

        public async Task<IActionResult> Odmoriste()
        {
            string naslov = "Popis odmorista";
            var odmorista = await ctx.Odmoriste
                                      .Include(o => o.Dionica)
                                      .AsNoTracking()
                                      .OrderBy(o => o.Naziv)
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
            report.MainTableDataSource(dataSource => dataSource.StronglyTypedList(odmorista));

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
                    column.PropertyName<Odmoriste>(o => o.Naziv);
                    column.CellsHorizontalAlignment(HorizontalAlignment.Center);
                    column.IsVisible(true);
                    column.Order(1);
                    column.Width(2);
                    column.HeaderCell("Naziv odmorista", horizontalAlignment: HorizontalAlignment.Center);
                });

                columns.AddColumn(column =>
                {
                    column.PropertyName<Odmoriste>(o => o.Dionica.Naziv);
                    column.CellsHorizontalAlignment(HorizontalAlignment.Center);
                    column.IsVisible(true);
                    column.Order(4);
                    column.Width(2);
                    column.HeaderCell("Dionica", horizontalAlignment: HorizontalAlignment.Center);
                });

                columns.AddColumn(column =>
                {
                    column.PropertyName<Odmoriste>(o => o.KoordinataX);
                    column.CellsHorizontalAlignment(HorizontalAlignment.Center);
                    column.IsVisible(true);
                    column.Order(4);
                    column.Width(1);
                    column.HeaderCell("KoordinataX", horizontalAlignment: HorizontalAlignment.Center);
                });

                columns.AddColumn(column =>
                {
                    column.PropertyName<Odmoriste>(o => o.KoordinataY);
                    column.CellsHorizontalAlignment(HorizontalAlignment.Center);
                    column.IsVisible(true);
                    column.Order(4);
                    column.Width(1);
                    column.HeaderCell("KoordinataY", horizontalAlignment: HorizontalAlignment.Center);
                });

                columns.AddColumn(column =>
                {
                    column.PropertyName<Odmoriste>(o => o.GodinaOtvaranja);
                    column.CellsHorizontalAlignment(HorizontalAlignment.Center);
                    column.IsVisible(true);
                    column.Order(4);
                    column.Width(1);
                    column.HeaderCell("GodinaOtvaranja", horizontalAlignment: HorizontalAlignment.Center);
                });
            });

            #endregion

            byte[] pdf = report.GenerateAsByteArray();

            if (pdf != null)
            {
                Response.Headers.Add("content-disposition", "inline; filename=odmorista.pdf");
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
            string naslov = "Popis dionica autocestes";

            var dionice = await ctx.Dionica
                                    .AsNoTracking()
                                    .Include(d => d.Autocesta)
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

            /* tu baca error */ 
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

        public async Task<IActionResult> PopratniSadrzaji()
        {
            string naslov = "Popis popratnih sadrzaja";
            var sadrzaji = await ctx.PopratniSadrzaj
                                    .AsNoTracking()
                                    .Include(p => p.VrstaSadrzaja)
                                    .OrderBy(p => p.Id)
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
            report.MainTableDataSource(dataSource => dataSource.StronglyTypedList(sadrzaji));

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
                    column.PropertyName<PopratniSadrzaj>(p => p.VrstaSadrzaja.Naziv);
                    column.CellsHorizontalAlignment(HorizontalAlignment.Center);
                    column.IsVisible(true);
                    column.Order(1);
                    column.Width(2);
                    column.HeaderCell("Vrsta popratnog sadrzaja", horizontalAlignment: HorizontalAlignment.Center);
                });

                columns.AddColumn(column =>
                {
                    column.PropertyName<PopratniSadrzaj>(p => p.Naziv);
                    column.CellsHorizontalAlignment(HorizontalAlignment.Center);
                    column.IsVisible(true);
                    column.Order(1);
                    column.Width(2);
                    column.HeaderCell("Naziv popratnog sadrzaja", horizontalAlignment: HorizontalAlignment.Center);
                });

                columns.AddColumn(column =>
                {
                    column.PropertyName<PopratniSadrzaj>(p => p.RadnimDanomOd);
                    column.CellsHorizontalAlignment(HorizontalAlignment.Center);
                    column.IsVisible(true);
                    column.Order(2);
                    column.Width(1);
                    column.HeaderCell("Radnim danom od", horizontalAlignment: HorizontalAlignment.Center);
                });

                columns.AddColumn(column =>
                {
                    column.PropertyName<PopratniSadrzaj>(p => p.RadninDanomDo);
                    column.CellsHorizontalAlignment(HorizontalAlignment.Center);
                    column.IsVisible(true);
                    column.Order(3);
                    column.Width(1);
                    column.HeaderCell("Radnim danom do", horizontalAlignment: HorizontalAlignment.Center);
                });

                columns.AddColumn(column =>
                {
                    column.PropertyName<PopratniSadrzaj>(p => p.VikendimaOd);
                    column.CellsHorizontalAlignment(HorizontalAlignment.Center);
                    column.IsVisible(true);
                    column.Order(4);
                    column.Width(1);
                    column.HeaderCell("Vikendima od", horizontalAlignment: HorizontalAlignment.Center);
                });

                columns.AddColumn(column =>
                {
                    column.PropertyName<PopratniSadrzaj>(p => p.VikendimaDo);
                    column.CellsHorizontalAlignment(HorizontalAlignment.Center);
                    column.IsVisible(true);
                    column.Order(4);
                    column.Width(1);
                    column.HeaderCell("Vikendima do", horizontalAlignment: HorizontalAlignment.Center);
                });
            });

            #endregion
            byte[] pdf = report.GenerateAsByteArray();

            if (pdf != null)
            {
                Response.Headers.Add("content-disposition", "inline; filename=popratniSadrzaji.pdf");
                return File(pdf, "application/pdf");
                //return File(pdf, "application/pdf", "drzave.pdf"); //Otvara save as dialog
            }
            else
            {
                return NotFound();
            }
        }

        public async Task<IActionResult> VrstePopratnih()
        {
            string naslov = "Vrste popratnih sadrzaja";
            var vrste = await ctx.VrstaPopratnog
                                    .AsNoTracking()
                                    .OrderBy(v => v.Naziv)
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
            report.MainTableDataSource(dataSource => dataSource.StronglyTypedList(vrste));

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
                    column.PropertyName<VrstaPopratnog>(vo => vo.Naziv);
                    column.CellsHorizontalAlignment(HorizontalAlignment.Center);
                    column.IsVisible(true);
                    column.Order(1);
                    column.Width(2);
                    column.HeaderCell("Naziv vrste popratnog", horizontalAlignment: HorizontalAlignment.Center);
                });

            });

            #endregion
            byte[] pdf = report.GenerateAsByteArray();

            if (pdf != null)
            {
                Response.Headers.Add("content-disposition", "inline; filename=vrsteOdrzavanja.pdf");
                return File(pdf, "application/pdf");
                //return File(pdf, "application/pdf", "drzave.pdf"); //Otvara save as dialog
            }
            else
            {
                return NotFound();
            }
        }

        #region Export u Excel
        public async Task<IActionResult> ExcelSimpleOdmorista()
        {
            var odmorista = await ctx.Odmoriste
                                  .Include(o => o.Dionica)
                                  .AsNoTracking()
                                  .OrderBy(o => o.Naziv)
                                  .ToListAsync();
            byte[] content;
            using (ExcelPackage excel = new ExcelPackage())
            {
                excel.Workbook.Properties.Title = "Popis odmorista";
                excel.Workbook.Properties.Author = "Zizic odmorista";
                var worksheet = excel.Workbook.Worksheets.Add("Odmorista");

                //First add the headers
                worksheet.Cells[1, 1].Value = "Naziv";
                worksheet.Cells[1, 2].Value = "Dionica";
                worksheet.Cells[1, 3].Value = "KoordinataX";
                worksheet.Cells[1, 4].Value = "KoordinataY";
                worksheet.Cells[1, 5].Value = "Godina otvaranja";

                for (int i = 0; i < odmorista.Count; i++)
                {
                    worksheet.Cells[i + 2, 1].Value = odmorista[i].Naziv;
                    worksheet.Cells[i + 2, 2].Value = odmorista[i].Dionica.Naziv;
                    worksheet.Cells[i + 2, 3].Value = odmorista[i].KoordinataX;
                    worksheet.Cells[i + 2, 4].Value = odmorista[i].KoordinataY;
                    worksheet.Cells[i + 2, 5].Value = odmorista[i].GodinaOtvaranja;
                }

                worksheet.Cells[1, 1, odmorista.Count + 1, 5].AutoFitColumns();

                content = excel.GetAsByteArray();
            }   

            return File(content, ExcelContentType, "odmorista.xlsx");
        }

        public async Task<IActionResult> ExcelSimpleDionice()
        {
            var dionice = await ctx.Dionica
                                    .AsNoTracking()
                                    .Include(d => d.Autocesta)
                                    .OrderBy(co => co.Naziv)
                                    .ToListAsync();
            byte[] content;
            using (ExcelPackage excel = new ExcelPackage())
            {
                excel.Workbook.Properties.Title = "Popis dionica";
                excel.Workbook.Properties.Author = "Zizic dionice";
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
                }

                worksheet.Cells[1, 1, dionice.Count + 1, 9].AutoFitColumns();

                content = excel.GetAsByteArray();
            }
            return File(content, ExcelContentType, "dionice.xlsx");
        }

        public async Task<IActionResult> ExcelSimpleSadrzaji()
        {
            var sadrzaji = await ctx.PopratniSadrzaj
                                    .Include(ps => ps.VrstaSadrzaja)
                                    .AsNoTracking()
                                    .OrderBy(ps => ps.Naziv)
                                    .ToListAsync();
            byte[] content;
            using (ExcelPackage excel = new ExcelPackage())
            {
                excel.Workbook.Properties.Title = "Popis popratnih sadrzaja";
                excel.Workbook.Properties.Author = "Zizic popratni sadrzaji";
                var worksheet = excel.Workbook.Worksheets.Add("Popratni sadrzaji");

                //First add the headers
                worksheet.Cells[1, 1].Value = "Vrsta";
                worksheet.Cells[1, 2].Value = "Naziv";
                worksheet.Cells[1, 3].Value = "Radnim danom od";
                worksheet.Cells[1, 4].Value = "Radnim danom do";
                worksheet.Cells[1, 5].Value = "Vikendima od";
                worksheet.Cells[1, 6].Value = "Vikendima do";

                for (int i = 0; i < sadrzaji.Count; i++)
                {
                    worksheet.Cells[i + 2, 1].Value = sadrzaji[i].VrstaSadrzaja.Naziv;
                    worksheet.Cells[i + 2, 2].Value = sadrzaji[i].Naziv;
                    worksheet.Cells[i + 2, 3].Value = sadrzaji[i].RadnimDanomOd.ToString();
                    worksheet.Cells[i + 2, 4].Value = sadrzaji[i].RadninDanomDo.ToString();
                    worksheet.Cells[i + 2, 5].Value = sadrzaji[i].VikendimaOd.ToString();
                    worksheet.Cells[i + 2, 6].Value = sadrzaji[i].VikendimaDo.ToString();
                }

                worksheet.Cells[1, 1, sadrzaji.Count + 1, 6].AutoFitColumns();

                content = excel.GetAsByteArray();
            }
            return File(content, ExcelContentType, "popratniSadrzaji.xlsx");
        }

        public async Task<IActionResult> ExcelSimpleVrstePopratnih()
        {
            var vrste = await ctx.VrstaPopratnog
                                    .AsNoTracking()
                                    .OrderBy(vp => vp.Naziv)
                                    .ToListAsync();
            byte[] content;
            using (ExcelPackage excel = new ExcelPackage())
            {
                excel.Workbook.Properties.Title = "Popis vrsta sadrzaja";
                excel.Workbook.Properties.Author = "Zizic vrste popratnih sadrzaja";
                var worksheet = excel.Workbook.Worksheets.Add("Vrste sadrzaja");

                //First add the headers
                worksheet.Cells[1, 1].Value = "Naziv";

                for (int i = 0; i < vrste.Count; i++)
                {
                    worksheet.Cells[i + 2, 1].Value = vrste[i].Naziv;
                }

                worksheet.Cells[1, 1, vrste.Count + 1, 1].AutoFitColumns();

                content = excel.GetAsByteArray();
            }
            return File(content, ExcelContentType, "vrsteSadrzaja.xlsx");
        }

        public async Task<IActionResult> ExcelComplexOdmorista()
        {
            var odmorista = await ctx.Odmoriste.ToListAsync();

            byte[] content;
            using (ExcelPackage excel = new ExcelPackage())
            {
                excel.Workbook.Properties.Title = "MD odmorista";
                excel.Workbook.Properties.Author = "Zizic Odmorista";
                var worksheet = excel.Workbook.Worksheets.Add("MD Odmorista");
                worksheet.Cells[2, 1].Value = "Odmoriste";
                worksheet.Cells[4, 1].Value = "Popratni Sadrzaj";
                worksheet.Cells[2, 1].AutoFitColumns();
                worksheet.Cells[4, 1].AutoFitColumns();

                //First add the headers
                for (int i = 0; i < odmorista.Count; i++)
                {
                    worksheet.Cells[1, i * 11 + 2].Value = "Naziv";
                    worksheet.Cells[1, i * 11 + 3].Value = "KoordinataX";
                    worksheet.Cells[1, i * 11 + 4].Value = "KoordinataY";
                    worksheet.Cells[1, i * 11 + 5].Value = "Godina Otvaranja";
                    worksheet.Cells[1, i * 11 + 6].Value = "|";
                    worksheet.Cells[2, i * 11 + 2].Value = odmorista[i].Naziv;
                    worksheet.Cells[2, i * 11 + 3].Value = odmorista[i].KoordinataX;
                    worksheet.Cells[2, i * 11 + 4].Value = odmorista[i].KoordinataY;
                    worksheet.Cells[2, i * 11 + 5].Value = odmorista[i].GodinaOtvaranja;
                    worksheet.Cells[2, i * 11 + 6].Value = "|";

                    var sadrzaji = await ctx.PopratniSadrzaj.Where(ps => ps.OdmoristeId == odmorista[i].Id).Include(ps => ps.VrstaSadrzaja).ToListAsync();
                    worksheet.Cells[4, i * 11 + 2].Value = "Naziv";
                    worksheet.Cells[4, i * 11 + 3].Value = "Radnim danom od";
                    worksheet.Cells[4, i * 11 + 4].Value = "Radnim danom do";
                    worksheet.Cells[4, i * 11 + 5].Value = "Vikendima od";
                    worksheet.Cells[4, i * 11 + 6].Value = "Vikendima do";
                    worksheet.Cells[4, i * 11 + 2].AutoFitColumns();
                    worksheet.Cells[4, i * 11 + 3].AutoFitColumns();
                    worksheet.Cells[4, i * 11 + 4].AutoFitColumns();
                    worksheet.Cells[4, i * 11 + 5].AutoFitColumns();
                    worksheet.Cells[4, i * 11 + 6].AutoFitColumns();

                    for (int j = 0; j < sadrzaji.Count; j++)
                    {
                        worksheet.Cells[j + 5, i * 11 + 2].Value = sadrzaji[j].Naziv;
                        worksheet.Cells[j + 5, i * 11 + 3].Value = sadrzaji[j].RadnimDanomOd.ToString();
                        worksheet.Cells[j + 5, i * 11 + 4].Value = sadrzaji[j].RadninDanomDo.ToString();
                        worksheet.Cells[j + 5, i * 11 + 5].Value = sadrzaji[j].VikendimaOd.ToString();
                        worksheet.Cells[j + 5, i * 11 + 6].Value = sadrzaji[j].VikendimaDo.ToString();
                        worksheet.Cells[j + 5, i * 11 + 2].AutoFitColumns();
                        worksheet.Cells[j + 5, i * 11 + 3].AutoFitColumns();
                        worksheet.Cells[j + 5, i * 11 + 4].AutoFitColumns();
                        worksheet.Cells[j + 5, i * 11 + 5].AutoFitColumns();
                        worksheet.Cells[j + 5, i * 11 + 6].AutoFitColumns();
                    }
                }

                content = excel.GetAsByteArray();
            }
            return File(content, ExcelContentType, "OdmoristaMD.xlsx");
        }

        #endregion

        public async Task<IActionResult> MDOdmoriste()
        {
            string naslov = "Zizic MD Odmorista";
            var odmorista = await ctx.Odmoriste.ToListAsync();
            List<SadrzajPopratni> sadrzajPopratni = new List<SadrzajPopratni>();
            foreach (var o in odmorista)
            {
                List<PopratniSadrzaj> sadrzaji = await ctx.PopratniSadrzaj.Where(ps => ps.OdmoristeId == o.Id).ToListAsync();
                foreach (var sadrzaj in sadrzaji)
                {
                    sadrzajPopratni.Add(new SadrzajPopratni
                    {
                        Naziv = o.Naziv,
                        KoordinataX = o.KoordinataX,
                        KoordinataY = o.KoordinataY,
                        GodinaOtvaranja = o.GodinaOtvaranja,
                        NazivPopratnog = sadrzaj.Naziv,
                        RadninDanomDo = sadrzaj.RadninDanomDo,
                        RadnimDanomOd = sadrzaj.RadnimDanomOd,
                        VikendimaDo = sadrzaj.VikendimaDo,
                        VikendimaOd = sadrzaj.VikendimaOd,
                        Id = o.Id             }); 
                }
            }
            sadrzajPopratni.OrderBy(sp => sp.Id).OrderBy(sp => sp.NazivPopratnog);
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
            report.MainTableDataSource(dataSource => dataSource.StronglyTypedList(sadrzajPopratni));

            report.MainTableColumns(columns =>
            {
            #region Stupci po kojima se grupira
            columns.AddColumn(column =>
            {
                column.PropertyName<SadrzajPopratni>(sp => sp.Id);
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
                column.PropertyName<SadrzajPopratni>(sp => sp.NazivPopratnog);
                column.CellsHorizontalAlignment(HorizontalAlignment.Center);
                column.IsVisible(true);
                column.Width(2);
                column.HeaderCell("Naziv popratnog", horizontalAlignment: HorizontalAlignment.Center);
            });

            columns.AddColumn(column =>
            {
                column.PropertyName<SadrzajPopratni>(sp => sp.RadnimDanomOd);
                column.CellsHorizontalAlignment(HorizontalAlignment.Right);
                column.IsVisible(true);
                column.Width(1);
                column.HeaderCell("Radnim danom od", horizontalAlignment: HorizontalAlignment.Center);
            });

            columns.AddColumn(column =>
            {
                column.PropertyName<SadrzajPopratni>(sp => sp.RadninDanomDo);
                column.CellsHorizontalAlignment(HorizontalAlignment.Right);
                column.IsVisible(true);
                column.Width(1);
                column.HeaderCell("Radnim danom do", horizontalAlignment: HorizontalAlignment.Center);
            });

            columns.AddColumn(column =>
            {
                column.PropertyName<SadrzajPopratni>(sp => sp.VikendimaOd);
                column.CellsHorizontalAlignment(HorizontalAlignment.Right);
                column.IsVisible(true);
                column.Width(1);
                column.HeaderCell("Vikendima od", horizontalAlignment: HorizontalAlignment.Center);
            });

            columns.AddColumn(column =>
            {
                column.PropertyName<SadrzajPopratni>(sp => sp.VikendimaDo);
                column.CellsHorizontalAlignment(HorizontalAlignment.Right);
                column.IsVisible(true);
                column.Width(1);
                column.HeaderCell("Vikendima do", horizontalAlignment: HorizontalAlignment.Center);
            });
            });

            #endregion
            byte[] pdf = report.GenerateAsByteArray();

            if (pdf != null)
            {
                Response.Headers.Add("content-disposition", "inline; filename=MDOdmorista.pdf");
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
                var nazivOdmorista = newGroupInfo.GetSafeStringValueOf(nameof(SadrzajPopratni.Naziv));
                var koordinataX = newGroupInfo.GetSafeStringValueOf(nameof(SadrzajPopratni.KoordinataX));
                var koordinataY = newGroupInfo.GetSafeStringValueOf(nameof(SadrzajPopratni.KoordinataY));
                var godinaOtvaranja = newGroupInfo.GetSafeStringValueOf(nameof(SadrzajPopratni.GodinaOtvaranja));

                var table = new PdfGrid(relativeWidths: new[] { 2f, 4f, 2f, 4f }) { WidthPercentage = 100 };

                table.AddSimpleRow(
                    (cellData, cellProperties) => {
                        cellData.Value = "Naziv:";
                        cellProperties.PdfFont = PdfRptFont;
                        cellProperties.PdfFontStyle = DocumentFontStyle.Bold;
                        cellProperties.HorizontalAlignment = HorizontalAlignment.Left;
                    },
                    (cellData, cellProperties) => {
                        cellData.TableRowData = newGroupInfo;
                        var cellTemplate = new HyperlinkField(BaseColor.Black, false)
                        {
                            TextPropertyName = nameof(SadrzajPopratni.Naziv),
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
                        cellData.Value = "KoordinataX";
                        cellProperties.PdfFont = PdfRptFont;
                        cellProperties.PdfFontStyle = DocumentFontStyle.Bold;
                        cellProperties.HorizontalAlignment = HorizontalAlignment.Left;
                    },
                    (cellData, cellProperties) => {
                        cellData.Value = koordinataX;
                        cellProperties.PdfFont = PdfRptFont;
                        cellProperties.HorizontalAlignment = HorizontalAlignment.Left;
                    });

                table.AddSimpleRow(
                    (cellData, cellProperties) =>
                    {
                        cellData.Value = "KoordinataY";
                        cellProperties.PdfFont = PdfRptFont;
                        cellProperties.PdfFontStyle = DocumentFontStyle.Bold;
                        cellProperties.HorizontalAlignment = HorizontalAlignment.Left;
                    },
                    (cellData, cellProperties) =>
                    {
                        cellData.Value = koordinataY;
                        cellProperties.PdfFont = PdfRptFont;
                        cellProperties.HorizontalAlignment = HorizontalAlignment.Left;
                    },
                    (cellData, cellProperties) =>
                    {
                        cellData.Value = "Godina Otvaranja";
                        cellProperties.PdfFont = PdfRptFont;
                        cellProperties.PdfFontStyle = DocumentFontStyle.Bold;
                        cellProperties.HorizontalAlignment = HorizontalAlignment.Left;
                    },
                    (cellData, cellProperties) =>
                    {
                        cellData.Value = godinaOtvaranja;
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
