using EVCS.SOAP.WebMVC.TriNM.SOAPServices;

var builder = WebApplication.CreateBuilder(args);

// Register SOAP Client for calling SOAP API Services
builder.Services.AddSingleton<StationSoapClient>();
builder.Services.AddScoped<IStationSoapClient>(sp => sp.GetRequiredService<StationSoapClient>());

// Add MVC support
builder.Services.AddControllersWithViews();

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

// MVC Routing
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Station}/{action=Index}/{id?}");

app.Run();

