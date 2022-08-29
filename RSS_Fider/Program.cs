var builder = WebApplication.CreateBuilder(args);


// Add services to the container.
builder.Services.AddControllersWithViews();

var app = builder.Build();

app.UseStatusCodePages();

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


//app.MapControllerRoute(
//    name: "default",
//    pattern: "{controller=Rss}/{action=RssRefresh}");

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Rss}/{action=FeedsConfig}");


app.Run();
