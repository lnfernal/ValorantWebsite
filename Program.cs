using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using System.Diagnostics;
using ValorantManager.Data;
using ValorantManager.Services;


#if !DEBUG
if (!File.Exists("aspnetcorev2_inprocess.dll"))
{
    Console.Clear();
    Console.WriteLine("Please extract Valorant Website.zip to a folder instead of doubling clicking it inside a zipping program.");

    Console.ReadKey();
    Process.GetCurrentProcess().Kill();
}


const string url = "http://localhost:4000";
if (args.Length == 0)
{
    Process.Start(Process.GetCurrentProcess().ProcessName, "--urls " + url);
    Process.GetCurrentProcess().Kill();
    return;
}

ProcessStartInfo info = new ProcessStartInfo();
info.FileName = url;
info.UseShellExecute = true;

new Process() { StartInfo = info }.Start();

Process thisProc = Process.GetCurrentProcess();
Process.GetProcessesByName(thisProc.ProcessName).Where(x => x.Id != thisProc.Id).All(x => { x.Kill(); return true; });


#endif
var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorPages();
builder.Services.AddServerSideBlazor();
builder.Services.AddScoped<ValorantService>();
builder.Services.AddScoped<ICookie, Cookie>();
builder.Services.AddHttpClient();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseStaticFiles();

app.UseRouting();


app.MapBlazorHub();
app.MapFallbackToPage("/_Host");

app.Run();
