using Microsoft.AspNetCore.Authorization;
using OnlineShop.Models.DAL.DependencyInjection;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddMemoryCache();

// My services
builder.Services.AddDataAccessLayer(builder.Configuration);


builder.WebHost.ConfigureKestrel(serverOptions =>
{
    serverOptions.ListenAnyIP(44368, listenOptions =>
    {
        listenOptions.UseHttps(); // ��������� HTTPS ��� ������ IP-������
    });
});



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


app.Use(async (context, next) =>
{
    if (context.Request.Path == "/")
    {
        context.Response.Redirect("/products");
        return;
    }

    // �������� ����������� ��� ���������� ���������
    var endpoint = context.GetEndpoint();
    var authorizeAttribute = endpoint?.Metadata.GetMetadata<AuthorizeAttribute>();

    // ���� ������� [Authorize] ������������ � ������������ �� �����������
    if (authorizeAttribute != null && !context.User.Identity.IsAuthenticated)
    {
        // ��������������� �� �������� ������� � ���������� ��� �������� ���������� ����
        context.Response.Redirect("/products?authModal=true");
        return;
    }

    await next();
});


app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Product}/{action=Products}/{id?}");



app.Run();
