using GovOperationsHub.Application;
using GovOperationsHub.Application.Common.Interfaces;
using GovOperationsHub.Infrastructure;
using GovOperationsHub.WebApi.Middlewares;
using GovOperationsHub.WebApi.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;

using System.Text;


var builder = WebApplication.CreateBuilder(args);

// 1. تسجيل خدمات الطبقات الأخرى (Application & Infrastructure)
builder.Services.AddApplicationServices();
builder.Services.AddInfrastructureServices(builder.Configuration);

// 2. تسجيل خدمات الـ WebApi
builder.Services.AddHttpContextAccessor();
builder.Services.AddScoped<ICurrentUserService, CurrentUserService>();

builder.Services.AddControllers();

// 3. إعداد سياسة الـ CORS للواجهات الأمامية (Blazor و Angular مستقبلاً)
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontendApp", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});

// 4. إعداد التوثيق عبر Swagger وتوصيف النظام بالعربية
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// 1. تسجيل Identity وربطه بـ ApplicationDbContext
builder.Services.AddIdentity<IdentityUser, IdentityRole>()
    .AddEntityFrameworkStores<GovOperationsHub.Infrastructure.Data.ApplicationDbContext>()
    .AddDefaultTokenProviders();

// 2. إعدادات المصادقة (Authentication) و JWT
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.SaveToken = true;
    options.RequireHttpsMetadata = false; // في بيئة الإنتاج يجب أن تكون true
    options.TokenValidationParameters = new TokenValidationParameters()
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidAudience = builder.Configuration["JwtSettings:ValidAudience"],
        ValidIssuer = builder.Configuration["JwtSettings:ValidIssuer"],
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["JwtSettings:Secret"]!))
    };
});

// 3. تفعيل التفويض (Authorization)
builder.Services.AddAuthorization();

var app = builder.Build();

// 5. تفعيل ميدلوير معالجة الأخطاء في مقدمة خط المعالجة (Pipeline)
app.UseMiddleware<ExceptionHandlingMiddleware>();

// 6. تشغيل واجهة Swagger
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "GovOperationsHub API v1");
        c.RoutePrefix = string.Empty; // فتح Swagger مباشرة كصفحة رئيسية عند تشغيل المشروع
    });
}

app.UseHttpsRedirection();

app.UseCors("AllowFrontendApp");

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    try
    {
        await GovOperationsHub.Infrastructure.Seeders.IdentitySeeder.SeedRolesAndAdminAsync(services);
    }
    catch (Exception ex)
    {
        // تجاهل أخطاء البذر في بيئة التطوير لتجنب توقف الإقلاع
        Console.WriteLine($"Error seeding data: {ex.Message}");
    }
}

app.Run();