using NLog.Web;
using NLog;
using RPPP_WebApp;
using Microsoft.EntityFrameworkCore;
using RPPP_WebApp.Models;
using RPPP_WebApp.ModelsValidation;
using FluentValidation;
using FluentValidation.AspNetCore;

//NOTE: Add dependencies/services in StartupExtensions.cs and keep this file as-is

var logger = LogManager.Setup().GetCurrentClassLogger();
var builder = WebApplication.CreateBuilder(args);
logger.Debug("init main");

try
{
    builder.Host.UseNLog(new NLogAspNetCoreOptions() { RemoveLoggerFactoryFilter = false });

    //var app = builder.ConfigureServices().ConfigurePipeline();
    /*
    var appSection = builder.Configuration.GetSection("AppSettings");
    builder.Services.Configure<AppSettings>(appSection);

    //IConfiguration configuration = builder.Configuration.AddJsonFile("appsettings.json").Build();
    builder.Services.AddDbContext<RPPP04Context>(options => options.UseSqlServer(builder.Configuration.GetConnectionString("RPPP04")));
    
    builder.Services.AddControllersWithViews();

    builder.Services
          .AddFluentValidationAutoValidation()
          .AddFluentValidationClientsideAdapters()
          .AddValidatorsFromAssemblyContaining<CestovniObjektValidator>();

    var app = builder.Build();

    //middleware pipeline
    if (app.Environment.IsDevelopment())
    {
        app.UseDeveloperExceptionPage();
    }

    app.UseStaticFiles();

    app.UseRouting();

    app.UseEndpoints(endpoints =>
    {
        endpoints.MapDefaultControllerRoute();
    });

    app.Run();
    */
    
    var app = builder.ConfigureServices().ConfigurePipeline();
    app.Run();
    
}
catch (Exception exception)
{
    // NLog: catch setup errors
    logger.Error(exception, "Stopped program because of exception");
    throw;
}
finally
{
  // Ensure to flush and stop internal timers/threads before application-exit (Avoid segmentation fault on Linux)
  NLog.LogManager.Shutdown();
}

public partial class Program { }