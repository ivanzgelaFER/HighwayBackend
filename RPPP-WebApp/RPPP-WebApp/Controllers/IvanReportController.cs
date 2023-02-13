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
  public class IvanReportController : Controller
  {
    private readonly RPPP04Context ctx;
    private readonly IWebHostEnvironment environment;
    private const string ExcelContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";

    public IvanReportController(RPPP04Context ctx, IWebHostEnvironment environment)
    {
      this.ctx = ctx;
      this.environment = environment;
    }

    public IActionResult Index()
    {
      return View();
    }

    public async Task<IActionResult> CestovniObjekti()
    {
      string naslov = "Popis cestovnih objekata";
      var cestovniObjekti = await ctx.CestovniObjekt
                                .Include(co => co.Dionica)
                                .AsNoTracking()
                                .OrderBy(co => co.Naziv)
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
      report.MainTableDataSource(dataSource => dataSource.StronglyTypedList(cestovniObjekti));

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
          column.PropertyName<CestovniObjekt>(co => co.Naziv);
          column.CellsHorizontalAlignment(HorizontalAlignment.Center);
          column.IsVisible(true);
          column.Order(1);
          column.Width(2);
          column.HeaderCell("Naziv objekta", horizontalAlignment: HorizontalAlignment.Center);
        });

        columns.AddColumn(column =>
        {
            column.PropertyName<CestovniObjekt>(co => co.Dionica.Naziv);
            column.CellsHorizontalAlignment(HorizontalAlignment.Center);
            column.IsVisible(true);
            column.Order(4);
            column.Width(2);
            column.HeaderCell("Dionica", horizontalAlignment: HorizontalAlignment.Center);
        });

          columns.AddColumn(column =>
        {
          column.PropertyName<CestovniObjekt>(co => co.TipObjekta);
          column.CellsHorizontalAlignment(HorizontalAlignment.Center);
          column.IsVisible(true);
          column.Order(2);
          column.Width(1);
          column.HeaderCell("Tip objekta", horizontalAlignment: HorizontalAlignment.Center);
        });

        columns.AddColumn(column =>
        {
          column.PropertyName<CestovniObjekt>(co => co.OgranicenjeBrzine);
          column.CellsHorizontalAlignment(HorizontalAlignment.Center);
          column.IsVisible(true);
          column.Order(3);
          column.Width(1);
          column.HeaderCell("Ogr. brzine", horizontalAlignment: HorizontalAlignment.Center);
        });

        columns.AddColumn(column =>
        {
          column.PropertyName<CestovniObjekt>(co => co.BrojPrometnihTraka);
          column.CellsHorizontalAlignment(HorizontalAlignment.Center);
          column.IsVisible(true);
          column.Order(4);
          column.Width(1);
          column.HeaderCell("Broj traka", horizontalAlignment: HorizontalAlignment.Center);
        });

        columns.AddColumn(column =>
        {
            column.PropertyName<CestovniObjekt>(co => co.DuljinaObjekta);
            column.CellsHorizontalAlignment(HorizontalAlignment.Center);
            column.IsVisible(true);
            column.Order(4);
            column.Width(1);
            column.HeaderCell("Duljina", horizontalAlignment: HorizontalAlignment.Center);
        });

        columns.AddColumn(column =>
        {
            column.PropertyName<CestovniObjekt>(co => co.ZaustavniTrak);
            column.CellsHorizontalAlignment(HorizontalAlignment.Center);
            column.IsVisible(true);
            column.Order(4);
            column.Width(1);
            column.HeaderCell("Zaustavni trak", horizontalAlignment: HorizontalAlignment.Center);
        });

        columns.AddColumn(column =>
        {
            column.PropertyName<CestovniObjekt>(co => co.DozvolaTeretnimVozilima);
            column.CellsHorizontalAlignment(HorizontalAlignment.Center);
            column.IsVisible(true);
            column.Order(4);
            column.Width(1);
            column.HeaderCell("Dozvola teretnim vozilima", horizontalAlignment: HorizontalAlignment.Center);
        });

        columns.AddColumn(column =>
        {
            column.PropertyName<CestovniObjekt>(co => co.GodinaIzgradnje);
            column.CellsHorizontalAlignment(HorizontalAlignment.Center);
            column.IsVisible(true);
            column.Order(4);
            column.Width(1);
            column.HeaderCell("Godina izgradnje", horizontalAlignment: HorizontalAlignment.Center);
        });

        columns.AddColumn(column =>
        {
            column.PropertyName<CestovniObjekt>(co => co.PjesackaStaza);
            column.CellsHorizontalAlignment(HorizontalAlignment.Center);
            column.IsVisible(true);
            column.Order(4);
            column.Width(1);
            column.HeaderCell("Pješačka staza", horizontalAlignment: HorizontalAlignment.Center);
        });

        columns.AddColumn(column =>
        {
            column.PropertyName<CestovniObjekt>(co => co.NaplataPrijelaza);
            column.CellsHorizontalAlignment(HorizontalAlignment.Center);
            column.IsVisible(true);
            column.Order(4);
            column.Width(1);
            column.HeaderCell("Naplata prijelaza", horizontalAlignment: HorizontalAlignment.Center);
        });
      });
    
      #endregion
      byte[] pdf = report.GenerateAsByteArray();

      if (pdf != null)
      {
        Response.Headers.Add("content-disposition", "inline; filename=cestovniObjekti.pdf");
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

    public async Task<IActionResult> OdrzavanjaObjekata()
    {
        string naslov = "Popis održavanja objekata";
        var odrzavanja = await ctx.OdrzavanjeObjekta
                                .AsNoTracking()
                                .Include(d => d.Vrsta)
                                .OrderBy(d => d.Id)
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
        report.MainTableDataSource(dataSource => dataSource.StronglyTypedList(odrzavanja));

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
                column.PropertyName<OdrzavanjeObjekta>(d => d.Vrsta.Naziv);
                column.CellsHorizontalAlignment(HorizontalAlignment.Center);
                column.IsVisible(true);
                column.Order(1);
                column.Width(2);
                column.HeaderCell("Vrsta održavanja", horizontalAlignment: HorizontalAlignment.Center);
            });

            columns.AddColumn(column =>
            {
                column.PropertyName<OdrzavanjeObjekta>(oo => oo.ImeFirme);
                column.CellsHorizontalAlignment(HorizontalAlignment.Center);
                column.IsVisible(true);
                column.Order(1);
                column.Width(2);
                column.HeaderCell("Ime firme", horizontalAlignment: HorizontalAlignment.Center);
            });

            columns.AddColumn(column =>
            {
                column.PropertyName<OdrzavanjeObjekta>(oo => oo.RadnimDanomOd);
                column.CellsHorizontalAlignment(HorizontalAlignment.Center);
                column.IsVisible(true);
                column.Order(2);
                column.Width(1);
                column.HeaderCell("Radnim danom od", horizontalAlignment: HorizontalAlignment.Center);
            });

            columns.AddColumn(column =>
            {
                column.PropertyName<OdrzavanjeObjekta>(oo => oo.RadnimDanomDo);
                column.CellsHorizontalAlignment(HorizontalAlignment.Center);
                column.IsVisible(true);
                column.Order(3);
                column.Width(1);
                column.HeaderCell("Radnim danom do", horizontalAlignment: HorizontalAlignment.Center);
            });

            columns.AddColumn(column =>
            {
                column.PropertyName<OdrzavanjeObjekta>(oo => oo.VikendimaOd);
                column.CellsHorizontalAlignment(HorizontalAlignment.Center);
                column.IsVisible(true);
                column.Order(4);
                column.Width(1);
                column.HeaderCell("Vikendima od", horizontalAlignment: HorizontalAlignment.Center);
            });

            columns.AddColumn(column =>
            {
                column.PropertyName<OdrzavanjeObjekta>(oo => oo.VikendimaDo);
                column.CellsHorizontalAlignment(HorizontalAlignment.Center);
                column.IsVisible(true);
                column.Order(4);
                column.Width(1);
                column.HeaderCell("Vikendima do", horizontalAlignment: HorizontalAlignment.Center);
            });

            columns.AddColumn(column =>
            {
                column.PropertyName<OdrzavanjeObjekta>(oo => oo.BrojLjudi);
                column.CellsHorizontalAlignment(HorizontalAlignment.Center);
                column.IsVisible(true);
                column.Order(4);
                column.Width(1);
                column.HeaderCell("Broj ljudi", horizontalAlignment: HorizontalAlignment.Center);
            });

            columns.AddColumn(column =>
            {
                column.PropertyName<OdrzavanjeObjekta>(oo => oo.Cijena);
                column.CellsHorizontalAlignment(HorizontalAlignment.Center);
                column.IsVisible(true);
                column.Order(4);
                column.Width(1);
                column.HeaderCell("Cijena", horizontalAlignment: HorizontalAlignment.Center);
            });

            columns.AddColumn(column =>
            {
                column.PropertyName<OdrzavanjeObjekta>(oo => oo.PredvidenoDana);
                column.CellsHorizontalAlignment(HorizontalAlignment.Center);
                column.IsVisible(true);
                column.Order(4);
                column.Width(1);
                column.HeaderCell("Predviđeno dana", horizontalAlignment: HorizontalAlignment.Center);
            });
        });

        #endregion
        byte[] pdf = report.GenerateAsByteArray();

        if (pdf != null)
        {
            Response.Headers.Add("content-disposition", "inline; filename=odrzavanjeObjekata.pdf");
            return File(pdf, "application/pdf");
            //return File(pdf, "application/pdf", "drzave.pdf"); //Otvara save as dialog
        }
        else
        {
            return NotFound();
        }
    }

    public async Task<IActionResult> VrsteOdrzavanja()
    {
        string naslov = "Vrste održavanja";
        var vrste = await ctx.VrstaOdrzavanja
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
                column.PropertyName<VrstaOdrzavanja>(vo => vo.Naziv);
                column.CellsHorizontalAlignment(HorizontalAlignment.Center);
                column.IsVisible(true);
                column.Order(1);
                column.Width(2);
                column.HeaderCell("Naziv održavanja", horizontalAlignment: HorizontalAlignment.Center);
            });

            columns.AddColumn(column =>
            {
                column.PropertyName<VrstaOdrzavanja>(vo => vo.Izvanredno);
                column.CellsHorizontalAlignment(HorizontalAlignment.Center);
                column.IsVisible(true);
                column.Order(2);
                column.Width(1);
                column.HeaderCell("Izvanredno", horizontalAlignment: HorizontalAlignment.Center);
            });

            columns.AddColumn(column =>
            {
                column.PropertyName<VrstaOdrzavanja>(d => d.Preventivno);
                column.CellsHorizontalAlignment(HorizontalAlignment.Center);
                column.IsVisible(true);
                column.Order(3);
                column.Width(1);
                column.HeaderCell("Preventivno", horizontalAlignment: HorizontalAlignment.Center);
            });

            columns.AddColumn(column =>
            {
                column.PropertyName<VrstaOdrzavanja>(d => d.Periodicnost);
                column.CellsHorizontalAlignment(HorizontalAlignment.Center);
                column.IsVisible(true);
                column.Order(4);
                column.Width(1);
                column.HeaderCell("Periodičnost", horizontalAlignment: HorizontalAlignment.Center);
            });

            columns.AddColumn(column =>
            {
                column.PropertyName<VrstaOdrzavanja>(d => d.GodisnjeDoba);
                column.CellsHorizontalAlignment(HorizontalAlignment.Center);
                column.IsVisible(true);
                column.Order(4);
                column.Width(1);
                column.HeaderCell("Godišnje doba", horizontalAlignment: HorizontalAlignment.Center);
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
    public async Task<IActionResult> ExcelSimpleCestovniObjekti()
    {
      var cestovniObjekti = await ctx.CestovniObjekt
                            .Include(co => co.Dionica)
                            .AsNoTracking()
                            .OrderBy(co => co.Naziv)
                            .ToListAsync();
      byte[] content;
      using (ExcelPackage excel = new ExcelPackage())
      {
        excel.Workbook.Properties.Title = "Popis cestovnih objekata";
        excel.Workbook.Properties.Author = "Ivan cestovni objekti";
        var worksheet = excel.Workbook.Worksheets.Add("Cestovni objekti");

        //First add the headers
        worksheet.Cells[1, 1].Value = "Naziv";
        worksheet.Cells[1, 2].Value = "Dionica";
        worksheet.Cells[1, 3].Value = "Tip objekta";
        worksheet.Cells[1, 4].Value = "Ogranicenje brzine";
        worksheet.Cells[1, 5].Value = "Broj traka";
        worksheet.Cells[1, 6].Value = "Duljina objekta";
        worksheet.Cells[1, 7].Value = "Zaustavni trak";
        worksheet.Cells[1, 8].Value = "Dozvola teretnim vozilima";
        worksheet.Cells[1, 9].Value = "Godina izgradnje";
        worksheet.Cells[1, 10].Value = "Pješačka staza";
        worksheet.Cells[1, 11].Value = "Naplata prijelaza";

        for (int i = 0; i < cestovniObjekti.Count; i++) {
          worksheet.Cells[i + 2, 1].Value = cestovniObjekti[i].Naziv;
          worksheet.Cells[i + 2, 2].Value = cestovniObjekti[i].Dionica.Naziv;
          worksheet.Cells[i + 2, 3].Value = cestovniObjekti[i].TipObjekta;
          worksheet.Cells[i + 2, 4].Value = cestovniObjekti[i].OgranicenjeBrzine;
          worksheet.Cells[i + 2, 5].Value = cestovniObjekti[i].BrojPrometnihTraka;
          worksheet.Cells[i + 2, 6].Value = cestovniObjekti[i].DuljinaObjekta;
          worksheet.Cells[i + 2, 7].Value = cestovniObjekti[i].ZaustavniTrak;
          worksheet.Cells[i + 2, 8].Value = cestovniObjekti[i].DozvolaTeretnimVozilima;
          worksheet.Cells[i + 2, 9].Value = cestovniObjekti[i].GodinaIzgradnje;
          worksheet.Cells[i + 2, 10].Value = cestovniObjekti[i].PjesackaStaza;
          worksheet.Cells[i + 2, 11].Value = cestovniObjekti[i].NaplataPrijelaza;
        }

        worksheet.Cells[1, 1, cestovniObjekti.Count + 1, 11].AutoFitColumns();

        content = excel.GetAsByteArray();
      }
      return File(content, ExcelContentType, "cestovniObjekti.xlsx");
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
            excel.Workbook.Properties.Author = "Ivan dionice";
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

    public async Task<IActionResult> ExcelSimpleOdrzavanja()
    {
        var odrzavanja = await ctx.OdrzavanjeObjekta
                                .Include(oo => oo.Vrsta)
                                .AsNoTracking()
                                .OrderBy(co => co.ImeFirme)
                                .ToListAsync();
        byte[] content;
        using (ExcelPackage excel = new ExcelPackage())
        {
            excel.Workbook.Properties.Title = "Popis održavanja objekata";
            excel.Workbook.Properties.Author = "Ivan";
            var worksheet = excel.Workbook.Worksheets.Add("Održavanje objekata");

            //First add the headers
            worksheet.Cells[1, 1].Value = "Vrsta";
            worksheet.Cells[1, 2].Value = "Ime firme";
            worksheet.Cells[1, 3].Value = "Radnim danom od";
            worksheet.Cells[1, 4].Value = "Radnim danom do";
            worksheet.Cells[1, 5].Value = "Vikendima od";
            worksheet.Cells[1, 6].Value = "Vikendima do";
            worksheet.Cells[1, 7].Value = "Broj ljudi";
            worksheet.Cells[1, 8].Value = "Cijena";
            worksheet.Cells[1, 9].Value = "Predviđeno dana";

            for (int i = 0; i < odrzavanja.Count; i++)
            {
                worksheet.Cells[i + 2, 1].Value = odrzavanja[i].Vrsta.Naziv;
                worksheet.Cells[i + 2, 2].Value = odrzavanja[i].ImeFirme;
                worksheet.Cells[i + 2, 3].Value = odrzavanja[i].RadnimDanomOd.ToString();
                worksheet.Cells[i + 2, 4].Value = odrzavanja[i].RadnimDanomDo.ToString();
                worksheet.Cells[i + 2, 5].Value = odrzavanja[i].VikendimaOd.ToString();
                worksheet.Cells[i + 2, 6].Value = odrzavanja[i].VikendimaDo.ToString();
                worksheet.Cells[i + 2, 7].Value = odrzavanja[i].BrojLjudi;
                worksheet.Cells[i + 2, 8].Value = odrzavanja[i].Cijena;
                worksheet.Cells[i + 2, 9].Value = odrzavanja[i].PredvidenoDana;
            }

            worksheet.Cells[1, 1, odrzavanja.Count + 1, 9].AutoFitColumns();

            content = excel.GetAsByteArray();
        }
        return File(content, ExcelContentType, "odrzavanjaObjekata.xlsx");
    }

    public async Task<IActionResult> ExcelSimpleVrsteOdrzavanja()
    {
        var vrste = await ctx.VrstaOdrzavanja
                                .AsNoTracking()
                                .OrderBy(vo => vo.Naziv)
                                .ToListAsync();
        byte[] content;
        using (ExcelPackage excel = new ExcelPackage())
        {
            excel.Workbook.Properties.Title = "Popis vrsta održavanja";
            excel.Workbook.Properties.Author = "Ivan";
            var worksheet = excel.Workbook.Worksheets.Add("Vrste održavanja");

            //First add the headers
            worksheet.Cells[1, 1].Value = "Naziv";
            worksheet.Cells[1, 2].Value = "Izvanredno";
            worksheet.Cells[1, 3].Value = "Preventivno";
            worksheet.Cells[1, 4].Value = "Periodičnost";
            worksheet.Cells[1, 5].Value = "Godišnje doba";

            for (int i = 0; i < vrste.Count; i++)
            {
                worksheet.Cells[i + 2, 1].Value = vrste[i].Naziv;
                worksheet.Cells[i + 2, 2].Value = vrste[i].Izvanredno;
                worksheet.Cells[i + 2, 3].Value = vrste[i].Preventivno;
                worksheet.Cells[i + 2, 4].Value = vrste[i].Periodicnost;
                worksheet.Cells[i + 2, 5].Value = vrste[i].GodisnjeDoba;
            }

            worksheet.Cells[1, 1, vrste.Count + 1, 5].AutoFitColumns();

            content = excel.GetAsByteArray();
        }
        return File(content, ExcelContentType, "vrsteOdrzavanja.xlsx");
    }

    public async Task<IActionResult> ExcelComplexCestovniObjekti()
    {
        var cestovniObjekti = await ctx.CestovniObjekt.ToListAsync();

        byte[] content;
        using (ExcelPackage excel = new ExcelPackage())
        {
            excel.Workbook.Properties.Title = "MD cestovni objekti";
            excel.Workbook.Properties.Author = "Ivan";
            var worksheet = excel.Workbook.Worksheets.Add("MD cestovni objekti");
            worksheet.Cells[2, 1].Value = "Cestovni objekt";
            worksheet.Cells[4, 1].Value = "Održavanja objekta";
            worksheet.Cells[2, 1].AutoFitColumns();
            worksheet.Cells[4, 1].AutoFitColumns();


            //First add the headers
            for (int i = 0; i < cestovniObjekti.Count; i++)
            {
                worksheet.Cells[1, i * 11 + 2].Value = "Naziv";
                worksheet.Cells[1, i * 11 + 3].Value = "Tip objekta";
                worksheet.Cells[1, i * 11 + 4].Value = "Ogranicenje brzine";
                worksheet.Cells[1, i * 11 + 5].Value = "Broj traka";
                worksheet.Cells[1, i * 11 + 6].Value = "Duljina objekta";
                worksheet.Cells[1, i * 11 + 7].Value = "Zaustavni trak";
                worksheet.Cells[1, i * 11 + 8].Value = "Dozvola teretnim vozilima";
                worksheet.Cells[1, i * 11 + 9].Value = "Godina izgradnje";
                worksheet.Cells[1, i * 11 + 10].Value = "Pješačka staza";
                worksheet.Cells[1, i * 11 + 11].Value = "Naplata prijelaza";
                worksheet.Cells[1, i * 11 + 12].Value = "|";
                worksheet.Cells[2, i * 11 + 2].Value = cestovniObjekti[i].Naziv;
                worksheet.Cells[2, i * 11 + 3].Value = cestovniObjekti[i].TipObjekta;
                worksheet.Cells[2, i * 11 + 4].Value = cestovniObjekti[i].OgranicenjeBrzine;
                worksheet.Cells[2, i * 11 + 5].Value = cestovniObjekti[i].BrojPrometnihTraka;
                worksheet.Cells[2, i * 11 + 6].Value = cestovniObjekti[i].DuljinaObjekta;
                worksheet.Cells[2, i * 11 + 7].Value = cestovniObjekti[i].ZaustavniTrak;
                worksheet.Cells[2, i * 11 + 8].Value = cestovniObjekti[i].DozvolaTeretnimVozilima;
                worksheet.Cells[2, i * 11 + 9].Value = cestovniObjekti[i].GodinaIzgradnje;
                worksheet.Cells[2, i * 11 + 10].Value = cestovniObjekti[i].PjesackaStaza;
                worksheet.Cells[2, i * 11 + 11].Value = cestovniObjekti[i].NaplataPrijelaza;
                worksheet.Cells[2, i * 11 + 12].Value = "|";

                var odrzavanja = await ctx.OdrzavanjeObjekta.Where(oo => oo.CestovniObjektId == cestovniObjekti[i].Id).Include(oo => oo.Vrsta).ToListAsync();
                worksheet.Cells[ 4, i * 11 + 2].Value = "Ime firme";
                worksheet.Cells[ 4, i * 11 + 3].Value = "Radnim danom od";
                worksheet.Cells[ 4, i * 11 + 4].Value = "Radnim danom do";
                worksheet.Cells[ 4, i * 11 + 5].Value = "Vikendima od";
                worksheet.Cells[ 4, i * 11 + 6].Value = "Vikendima do";
                worksheet.Cells[ 4, i * 11 + 7].Value = "Broj ljudi";
                worksheet.Cells[ 4, i * 11 + 8].Value = "Cijena";
                worksheet.Cells[ 4, i * 11 + 9].Value = "Predviđeno dana";
                worksheet.Cells[ 4, i * 11 + 10].Value = "Vrsta";
                worksheet.Cells[4, i * 11 + 2].AutoFitColumns();
                worksheet.Cells[4, i * 11 + 3].AutoFitColumns();
                worksheet.Cells[4, i * 11 + 4].AutoFitColumns();
                worksheet.Cells[4, i * 11 + 5].AutoFitColumns();
                worksheet.Cells[4, i * 11 + 6].AutoFitColumns();
                worksheet.Cells[4, i * 11 + 7].AutoFitColumns();
                worksheet.Cells[4, i * 11 + 8].AutoFitColumns();
                worksheet.Cells[4, i * 11 + 9].AutoFitColumns();
                worksheet.Cells[4, i * 11 + 10].AutoFitColumns();
                for (int j = 0; j < odrzavanja.Count; j++) 
                {
                    worksheet.Cells[j + 5, i * 11 + 2].Value = odrzavanja[j].ImeFirme;
                    worksheet.Cells[j + 5, i * 11 + 3].Value = odrzavanja[j].RadnimDanomOd.ToString();
                    worksheet.Cells[j + 5, i * 11 + 4].Value = odrzavanja[j].RadnimDanomDo.ToString();
                    worksheet.Cells[j + 5, i * 11 + 5].Value = odrzavanja[j].VikendimaOd.ToString();
                    worksheet.Cells[j + 5, i * 11 + 6].Value = odrzavanja[j].VikendimaDo.ToString();
                    worksheet.Cells[j + 5, i * 11 + 7].Value = odrzavanja[j].BrojLjudi;
                    worksheet.Cells[j + 5, i * 11 + 8].Value = odrzavanja[j].Cijena;
                    worksheet.Cells[j + 5, i * 11 + 9].Value = odrzavanja[j].PredvidenoDana;
                    worksheet.Cells[j + 5, i * 11 + 10].Value = odrzavanja[j].Vrsta.Naziv;
                    worksheet.Cells[j + 5, i * 11 + 2].AutoFitColumns();
                    worksheet.Cells[j + 5, i * 11 + 3].AutoFitColumns();
                    worksheet.Cells[j + 5, i * 11 + 4].AutoFitColumns();
                    worksheet.Cells[j + 5, i * 11 + 5].AutoFitColumns();
                    worksheet.Cells[j + 5, i * 11 + 6].AutoFitColumns();
                    worksheet.Cells[j + 5, i * 11 + 7].AutoFitColumns();
                    worksheet.Cells[j + 5, i * 11 + 8].AutoFitColumns();
                    worksheet.Cells[j + 5, i * 11 + 9].AutoFitColumns();
                    worksheet.Cells[j + 5, i * 11 + 10].AutoFitColumns(); 
                    }
            }

            content = excel.GetAsByteArray();
        }
        return File(content, ExcelContentType, "CestovniObjektiMD.xlsx");
    }

    #endregion
     
    public async Task<IActionResult> MDCestovniObjekt()
    {
        string naslov = "Ivan MD cestovni objekti";
        var cestovniObjekti = await ctx.CestovniObjekt.ToListAsync();
        List<ObjektOdrzavanje> objektOdrzavanje = new List<ObjektOdrzavanje>();
        foreach(var co in cestovniObjekti) {
            List<OdrzavanjeObjekta> odrzavanja = await ctx.OdrzavanjeObjekta.Where(oo => oo.CestovniObjektId == co.Id).ToListAsync();
            foreach(var odrzavanje in odrzavanja)
            {
                objektOdrzavanje.Add(new ObjektOdrzavanje {
                Naziv = co.Naziv, TipObjekta = co.TipObjekta, OgranicenjeBrzine = co.OgranicenjeBrzine, BrojPrometnihTraka = co.BrojPrometnihTraka, 
                DuljinaObjekta = co.DuljinaObjekta, ZaustavniTrak = co.ZaustavniTrak, DozvolaTeretnimVozilima = co.DozvolaTeretnimVozilima, GodinaIzgradnje = co.GodinaIzgradnje, 
                PjesackaStaza = co.PjesackaStaza, NaplataPrijelaza = co.NaplataPrijelaza, ImeFirme = odrzavanje.ImeFirme, RadnimDanomOd = odrzavanje.RadnimDanomOd, 
                RadnimDanomDo = odrzavanje.RadnimDanomDo, VikendimaOd = odrzavanje.VikendimaOd, VikendimaDo = odrzavanje.VikendimaDo, BrojLjudi = odrzavanje.BrojLjudi, 
                Cijena = odrzavanje.Cijena, PredvidenoDana = odrzavanje.PredvidenoDana, Id = co.Id});
            }
        }
        objektOdrzavanje.OrderBy(oo => oo.Id).OrderBy(oo => oo.ImeFirme);
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
        report.MainTableDataSource(dataSource => dataSource.StronglyTypedList(objektOdrzavanje));
        
        report.MainTableColumns(columns =>
        {
        #region Stupci po kojima se grupira
        columns.AddColumn(column =>
        {
            column.PropertyName<ObjektOdrzavanje>(oo => oo.Id);
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
            column.PropertyName<ObjektOdrzavanje>(oo => oo.ImeFirme);
            column.CellsHorizontalAlignment(HorizontalAlignment.Center);
            column.IsVisible(true);
            column.Width(2);
            column.HeaderCell("Ime firme", horizontalAlignment: HorizontalAlignment.Center);
        });

        columns.AddColumn(column =>
        {
            column.PropertyName<ObjektOdrzavanje>(oo => oo.RadnimDanomOd);
            column.CellsHorizontalAlignment(HorizontalAlignment.Right);
            column.IsVisible(true);
            column.Width(1);
            column.HeaderCell("Radnim danom od", horizontalAlignment: HorizontalAlignment.Center);
        });
            
        columns.AddColumn(column =>
        {
            column.PropertyName<ObjektOdrzavanje>(oo => oo.RadnimDanomDo);
            column.CellsHorizontalAlignment(HorizontalAlignment.Right);
            column.IsVisible(true);
            column.Width(1);
            column.HeaderCell("Radnim danom do", horizontalAlignment: HorizontalAlignment.Center);
        });

        columns.AddColumn(column =>
        {
            column.PropertyName<ObjektOdrzavanje>(oo => oo.VikendimaOd);
            column.CellsHorizontalAlignment(HorizontalAlignment.Right);
            column.IsVisible(true);
            column.Width(1);
            column.HeaderCell("Vikendima od", horizontalAlignment: HorizontalAlignment.Center);
        });

        columns.AddColumn(column =>
        {
            column.PropertyName<ObjektOdrzavanje>(oo => oo.VikendimaDo);
            column.CellsHorizontalAlignment(HorizontalAlignment.Right);
            column.IsVisible(true);
            column.Width(1);
            column.HeaderCell("Vikendima do", horizontalAlignment: HorizontalAlignment.Center);
        });

        columns.AddColumn(column =>
        {
            column.PropertyName<ObjektOdrzavanje>(oo => oo.BrojLjudi);
            column.CellsHorizontalAlignment(HorizontalAlignment.Right);
            column.IsVisible(true);
            column.Width(1);
            column.HeaderCell("Broj ljudi", horizontalAlignment: HorizontalAlignment.Center);
        });
        columns.AddColumn(column =>
        {
            column.PropertyName<ObjektOdrzavanje>(oo => oo.Cijena);
            column.CellsHorizontalAlignment(HorizontalAlignment.Right);
            column.IsVisible(true);
            column.Width(1);
            column.HeaderCell("Cijena", horizontalAlignment: HorizontalAlignment.Center);
        });
        columns.AddColumn(column =>
        {
            column.PropertyName<ObjektOdrzavanje>(oo => oo.PredvidenoDana);
            column.CellsHorizontalAlignment(HorizontalAlignment.Right);
            column.IsVisible(true);
            column.Width(1);
            column.HeaderCell("Predviđeno dana", horizontalAlignment: HorizontalAlignment.Center);
        });

        });

        #endregion
        byte[] pdf = report.GenerateAsByteArray();

        if (pdf != null)
        {
        Response.Headers.Add("content-disposition", "inline; filename=MDCestovniObjekti.pdf");
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
        var nazivCO = newGroupInfo.GetSafeStringValueOf(nameof(ObjektOdrzavanje.Naziv));
        var tipObjekta = newGroupInfo.GetSafeStringValueOf(nameof(ObjektOdrzavanje.TipObjekta));
        var ogranicenjeBrzine = newGroupInfo.GetValueOf(nameof(ObjektOdrzavanje.OgranicenjeBrzine));
        var brojTraka = newGroupInfo.GetValueOf(nameof(ObjektOdrzavanje.BrojPrometnihTraka));
        var duljinaObj = newGroupInfo.GetValueOf(nameof(ObjektOdrzavanje.DuljinaObjekta));
        var zaustavniTrak = newGroupInfo.GetValueOf(nameof(ObjektOdrzavanje.ZaustavniTrak));
        var dozvolaTeretnim = newGroupInfo.GetValueOf(nameof(ObjektOdrzavanje.DozvolaTeretnimVozilima));
        var godinaIzgradnje = newGroupInfo.GetValueOf(nameof(ObjektOdrzavanje.GodinaIzgradnje));
        var pjesackaStaza = newGroupInfo.GetValueOf(nameof(ObjektOdrzavanje.PjesackaStaza));
        var naplataPrijelaza = newGroupInfo.GetValueOf(nameof(ObjektOdrzavanje.NaplataPrijelaza));

        var table = new PdfGrid(relativeWidths: new[] { 2f, 4f, 2f, 4f }) { WidthPercentage = 100 };

        table.AddSimpleRow(
            (cellData, cellProperties) => {
                cellData.Value = "Naziv:";
                cellProperties.PdfFont = PdfRptFont;
                cellProperties.PdfFontStyle = DocumentFontStyle.Bold;
                cellProperties.HorizontalAlignment = HorizontalAlignment.Left;
            },
            (cellData, cellProperties) => {
                cellData.TableRowData = newGroupInfo; //postavi podatke retka za ćeliju
                        var cellTemplate = new HyperlinkField(BaseColor.Black, false)
                {
                TextPropertyName = nameof(ObjektOdrzavanje.Naziv),
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
                cellData.Value = "Tip objekta:";
                cellProperties.PdfFont = PdfRptFont;
                cellProperties.PdfFontStyle = DocumentFontStyle.Bold;
                cellProperties.HorizontalAlignment = HorizontalAlignment.Left;
            },
            (cellData, cellProperties) => {
                cellData.Value = tipObjekta;
                cellProperties.PdfFont = PdfRptFont;
                cellProperties.HorizontalAlignment = HorizontalAlignment.Left;
            });
            
        table.AddSimpleRow(
            (cellData, cellProperties) =>
            {
                cellData.Value = "Ograničenje brzine";
                cellProperties.PdfFont = PdfRptFont;
                cellProperties.PdfFontStyle = DocumentFontStyle.Bold;
                cellProperties.HorizontalAlignment = HorizontalAlignment.Left;
            },
            (cellData, cellProperties) =>
            {
                cellData.Value = ogranicenjeBrzine;
                cellProperties.PdfFont = PdfRptFont;
                cellProperties.HorizontalAlignment = HorizontalAlignment.Left;
            },
            (cellData, cellProperties) =>
            {
                cellData.Value = "Broj prometnih traka";
                cellProperties.PdfFont = PdfRptFont;
                cellProperties.PdfFontStyle = DocumentFontStyle.Bold;
                cellProperties.HorizontalAlignment = HorizontalAlignment.Left;
            },
            (cellData, cellProperties) =>
            {
                cellData.Value = brojTraka;
                cellProperties.PdfFont = PdfRptFont;
                cellProperties.HorizontalAlignment = HorizontalAlignment.Left;
            });
        table.AddSimpleRow(
            (cellData, cellProperties) =>
            {
                cellData.Value = "Duljine objekta";
                cellProperties.PdfFont = PdfRptFont;
                cellProperties.PdfFontStyle = DocumentFontStyle.Bold;
                cellProperties.HorizontalAlignment = HorizontalAlignment.Left;
            },
            (cellData, cellProperties) =>
            {
                cellData.Value = duljinaObj;
                cellProperties.PdfFont = PdfRptFont;
                cellProperties.HorizontalAlignment = HorizontalAlignment.Left;
            },
            (cellData, cellProperties) =>
            {
                cellData.Value = "Zaustavni trak";
                cellProperties.PdfFont = PdfRptFont;
                cellProperties.PdfFontStyle = DocumentFontStyle.Bold;
                cellProperties.HorizontalAlignment = HorizontalAlignment.Left;
            },
            (cellData, cellProperties) =>
            {
                cellData.Value = zaustavniTrak;
                cellProperties.PdfFont = PdfRptFont;
                cellProperties.HorizontalAlignment = HorizontalAlignment.Left;
            });
        table.AddSimpleRow(
            (cellData, cellProperties) =>
            {
                cellData.Value = "Dozvola teretnim vozilima";
                cellProperties.PdfFont = PdfRptFont;
                cellProperties.PdfFontStyle = DocumentFontStyle.Bold;
                cellProperties.HorizontalAlignment = HorizontalAlignment.Left;
            },
            (cellData, cellProperties) =>
            {
                cellData.Value = dozvolaTeretnim;
                cellProperties.PdfFont = PdfRptFont;
                cellProperties.HorizontalAlignment = HorizontalAlignment.Left;
            },
            (cellData, cellProperties) =>
            {
                cellData.Value = "Godina izgradnje";
                cellProperties.PdfFont = PdfRptFont;
                cellProperties.PdfFontStyle = DocumentFontStyle.Bold;
                cellProperties.HorizontalAlignment = HorizontalAlignment.Left;
            },
            (cellData, cellProperties) =>
            {
                cellData.Value = godinaIzgradnje;
                cellProperties.PdfFont = PdfRptFont;
                cellProperties.HorizontalAlignment = HorizontalAlignment.Left;
            });
        table.AddSimpleRow(
            (cellData, cellProperties) =>
            {
                cellData.Value = "Pješačka staza";
                cellProperties.PdfFont = PdfRptFont;
                cellProperties.PdfFontStyle = DocumentFontStyle.Bold;
                cellProperties.HorizontalAlignment = HorizontalAlignment.Left;
            },
            (cellData, cellProperties) =>
            {
                cellData.Value = pjesackaStaza;
                cellProperties.PdfFont = PdfRptFont;
                cellProperties.HorizontalAlignment = HorizontalAlignment.Left;
            },
            (cellData, cellProperties) =>
            {
                cellData.Value = "Naplata prijelaza";
                cellProperties.PdfFont = PdfRptFont;
                cellProperties.PdfFontStyle = DocumentFontStyle.Bold;
                cellProperties.HorizontalAlignment = HorizontalAlignment.Left;
            },
            (cellData, cellProperties) =>
            {
                cellData.Value = naplataPrijelaza;
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
