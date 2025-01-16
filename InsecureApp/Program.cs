using InsecureApp.Db;
using InsecureApp.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

// Server Header bilgisi kaldýrýlmamýþ
// Secure-Headers tanýmlarý yok.
builder.WebHost.ConfigureKestrel(options =>
{
  // Server baþlýðýný kaldýrma
  options.AddServerHeader = false;
});

builder.Services.AddControllersWithViews();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();


builder.Services.AddCors(options =>
{
  options.AddPolicy("AllowAll", policy =>
  {
    policy.AllowAnyOrigin()
          .AllowAnyMethod()
          .AllowAnyHeader();
  });
});


// Hizmetleri ekleyin (ConfigureServices)
builder.Services.AddControllersWithViews();
builder.Services.AddDbContext<MyDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("SqlConn")));
builder.Services.AddScoped<IUserService, UserService>();

// JWT Authentication
// JWT validation ayarlarý eksik.
builder.Services.AddAuthentication("Bearer")
    .AddJwtBearer(options =>
    {
      options.Authority = "https://localhost:5000";
      options.Audience = "api";
    });




var app = builder.Build();




// Veritabaný seed iþlemini yap
using (var scope = app.Services.CreateScope())
{
  var context = scope.ServiceProvider.GetRequiredService<MyDbContext>();
  MyDbContextSeed.SeedData(context);
}

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
  app.UseSwagger();
  app.UseSwaggerUI();
}


app.UseHttpsRedirection(); // HSTS baþlýðý eksik.

// CORS açýk býrakýldý
// Tüm domainlere api açýk (Sonarqube yakalamýyor)
app.UseCors("AllowAll");

// Bu ifade, sayfanýn içinde doðrudan yazýlmýþ (inline) JavaScript kodlarýnýn çalýþmasýna izin verir. Örneðin, <script> etiketleri içinde yer alan JavaScript kodlarý veya HTML etiketlerine gömülü onclick, onload gibi olay iþleyicileri. Sonarqube yakalamýyor 
app.Use(async (context, next) =>
{
   context.Response.Headers.ContentSecurityPolicy = "script-src 'self' 'unsafe-inline';"; // Noncompliant
  await next();
});


app.UseAuthorization();
app.UseAuthentication();
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Mvc}/{action=Index}/{id?}");
app.MapControllers();

app.Run();
