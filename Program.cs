using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using System.Diagnostics;
using ValorantManager.Data;
using ValorantManager.Services;

const string url = "http://localhost:4000";

ProcessStartInfo info = new ProcessStartInfo();
info.FileName = url;
info.UseShellExecute = true;

new Process() { StartInfo = info }.Start();

Process thisProc = Process.GetCurrentProcess();
Process.GetProcessesByName(thisProc.ProcessName).Where(x => x.Id != thisProc.Id).All(x => { x.Kill(); return true; });


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
    
}
app.UseHsts();

app.UseAuthorization();
//app.UseHttpsRedirection();

app.UseStaticFiles();

app.UseRouting();


app.MapBlazorHub();
app.MapFallbackToPage("/_Host");

app.Run(url);
