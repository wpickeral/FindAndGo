using FindAndGo.Services;

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

// // Middleware to get a new authorization token from the Kroger API if it has expired
// app.Use(async (context, next) =>
// {
//     var token = context.Request.Cookies["find-and-go.token"];
//
//     if (token == null)
//     {
//         try
//         {
//             // Request a new token
//             var newTokenRequest = await new KrogerService().GetAccessToken();
//             if (newTokenRequest != null)
//             {
//                 // Parse the access_token
//                 var newToken = newTokenRequest["access_token"].ToString();
//                 // Parse the MaxAge
//                 var expiresIn = int.Parse(newTokenRequest["expires_in"].ToString());
//                 // Build the cookie options object
//                 var cookieOptions = new CookieOptions();
//                 cookieOptions.Expires = DateTimeOffset.Now.AddSeconds(expiresIn);
//                 // Add the token to the cookies object with the options
//                 context.Response.Cookies.Append("find-and-go.token", newToken, cookieOptions);
//             }
//         }
//         catch (HttpRequestException e)
//         {
//             Console.WriteLine(e);
//         }
//     }
//
//     await next.Invoke();
// });

app.Run();