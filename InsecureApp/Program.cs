using InsecureApp.Db;
using InsecureApp.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

// Server Header bilgisi kald�r�lmam��
// Secure-Headers tan�mlar� yok.
builder.WebHost.ConfigureKestrel(options =>
{
  // Server ba�l���n� kald�rma
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
// JWT validation ayarlar� eksik.
builder.Services.AddAuthentication("Bearer")
    .AddJwtBearer(options =>
    {
      options.Authority = "https://localhost:5000";
      options.Audience = "api";
    });




var app = builder.Build();




// Veritaban� seed i�lemini yap
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


app.UseHttpsRedirection(); // HSTS ba�l��� eksik.

// CORS a��k b�rak�ld�
// T�m domainlere api a��k (Sonarqube yakalam�yor)
app.UseCors("AllowAll");

// Bu ifade, sayfan�n i�inde do�rudan yaz�lm�� (inline) JavaScript kodlar�n�n �al��mas�na izin verir. �rne�in, <script> etiketleri i�inde yer alan JavaScript kodlar� veya HTML etiketlerine g�m�l� onclick, onload gibi olay i�leyicileri. Sonarqube yakalam�yor 
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
