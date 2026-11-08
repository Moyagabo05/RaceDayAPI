using RaceDayWeb.Services;

var builder = WebApplication.CreateBuilder(args);

// 1. Add Session Management (Required to hold the API session cookie)
builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

// 2. Add HttpContextAccessor (Needed to read the cookie)
builder.Services.AddHttpContextAccessor();

// 3. Add HttpClientFactory
builder.Services.AddHttpClient();

// 4. Register our custom ApiService
builder.Services.AddScoped<ApiService>();

// 5. Add Controllers and Views
builder.Services.AddControllersWithViews();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();

// Enable Session
app.UseSession();

app.UseAuthorization();

// Map controllers that use explicit attribute routes such as /Events/Details/{id}.
app.MapControllers();
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();