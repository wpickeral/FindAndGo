using FindAndGo.Services;
using FindAndGo.Middleware;


var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddHttpClient<IKrogerService, KrogerService>();

var app = builder.Build();

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

/* Custom middleware to get a new auth token and set it as a cookie.

   There are two possible scenarios where this custom middleware is useful:
   1. If the user has never accessed this website, as soon as the user visits a webpage the token will be requested
   2. If the token expires after the max life of the token (30 minutes), a new token will be fetched from the Kroger 
     api and set as a new cookie. 
  
  Additionally, this middleware allows us to place the token refresh logic on every request in one central location,
  Otherwise, we would have to insert the logic into every Controller/Action that hits the Kroger API*/

app.UseWhen((context) => !context.Request.Cookies.ContainsKey("find-and-go.token"),
    (app) => { app.UseRequestKrogerAccessToken(); });

app.Run();