using OnlineShop.Models.DAL.DependencyInjection;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();
// My services
builder.Services.AddDataAccessLayer(builder.Configuration);
//builder.WebHost.ConfigureKestrel(options =>
//{
//    options.ListenAnyIP(44368, listenOptions =>
//    {
//        listenOptions.UseHttps(); // ���������, ��� ������������ HTTPS
//    });
//});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

// ��������� ������� �� ���� IP
app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.Use(async (context, next) =>
{
    if (context.Request.Path == "/")
    {
        context.Response.Redirect("/products");
        return;
    }

    await next();
});

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Product}/{action=Products}/{id?}");



app.Run();
