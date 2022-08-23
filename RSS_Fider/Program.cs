using System.Net;

var builder = WebApplication.CreateBuilder(args);



// Add services to the container.
builder.Services.AddControllersWithViews();

var app = builder.Build();

builder.Configuration.AddXmlFile("config.xml",optional:false,reloadOnChange:true);

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}






app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.MapControllerRoute(
    name:"RssFider",
    pattern:"{controller=RSS}/{action=RssIndex}");

app.MapControllerRoute(
    name: "RssFider2",
    pattern: "{controller=RSS}/{action=RssRefresh}");

app.MapControllerRoute(
    name: "RssFider3",
    pattern: "{controller=RSS}/{action=RssStart}");




app.Run();
