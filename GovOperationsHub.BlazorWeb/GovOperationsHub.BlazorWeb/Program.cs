using GovOperationsHub.BlazorWeb.Components;
using GovOperationsHub.BlazorWeb.Providers;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Components.Authorization;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents()
    .AddInteractiveWebAssemblyComponents();

// 1. إعدادات المصادقة الصافية والآمنة بالاعتماد على الكوكيز الرسمية
builder.Services.AddCascadingAuthenticationState();

builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/login";
        options.AccessDeniedPath = "/access-denied";
    });

builder.Services.AddAuthorization();

// 2. تسجيل المندوب في نفس النطاق الزمني للمستخدم (Scoped)
builder.Services.AddScoped<GovOperationsHub.BlazorWeb.Providers.JwtAuthorizationMessageHandler>();

// 3. بناء وتسجيل HttpClient ليعمل داخل نطاق شاشة المستخدم فقط
builder.Services.AddScoped(sp =>
{
    // جلب المندوب الذي يمتلك صلاحية الوصول لمعلومات المستخدم
    var authHandler = sp.GetRequiredService<GovOperationsHub.BlazorWeb.Providers.JwtAuthorizationMessageHandler>();

    // وضع إعدادات تخطي البروكسي لكي لا ينقطع الإنترنت لديك
    authHandler.InnerHandler = new HttpClientHandler { UseProxy = false };

    // إعادة HttpClient جاهز للعمل ومحقون بالتوكن ومضبوط على سيرفرك
    return new HttpClient(authHandler)
    {
        BaseAddress = new Uri("https://localhost:7241/") // تأكد من البورت
    };
});



builder.Services.AddHttpContextAccessor();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseWebAssemblyDebugging();
}
else
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    app.UseHsts();
}

app.UseStatusCodePagesWithReExecute("/not-found", createScopeForStatusCodePages: true);
app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();
app.UseAntiforgery();

app.MapStaticAssets();

app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode()
    .AddInteractiveWebAssemblyRenderMode()
    .AddAdditionalAssemblies(typeof(GovOperationsHub.BlazorWeb.Client._Imports).Assembly);


// =========================================================================
// السحر الجديد: مسارات الدخول والخروج الآمنة (Backend For Frontend - BFF)
// =========================================================================

// مسار تسجيل الدخول الآمن (ينشئ Cookie رسمي مشفر)
app.MapGet("/auth/signin", async (string token, HttpContext ctx) =>
{
    var handler = new JwtSecurityTokenHandler();
    var jwt = handler.ReadJwtToken(token);

    var claims = new List<Claim>();
    foreach (var claim in jwt.Claims)
    {
        string claimType = claim.Type;
        if (claimType == "role" || claimType == "roles") claimType = ClaimTypes.Role;
        else if (claimType == "name" || claimType == "unique_name") claimType = ClaimTypes.Name;
        claims.Add(new Claim(claimType, claim.Value));
    }

    // حفظ التوكن الخام داخل الهوية لنرسله لاحقاً للـ API
    claims.Add(new Claim("RawToken", token));

    var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
    var principal = new ClaimsPrincipal(identity);

    await ctx.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal);
    return Results.Redirect("/"); // بعد نجاح الحفظ، انطلق للرئيسية!
});

// مسار تسجيل الخروج الآمن
// مسار تسجيل الخروج الآمن
app.MapGet("/auth/signout", async (HttpContext ctx) =>
{
    // 1. قراءة التوكن المحفوظ في الجلسة الحالية
    var token = ctx.User.FindFirst("RawToken")?.Value;

    if (!string.IsNullOrEmpty(token))
    {
        try
        {
            // 2. إخبار الـ API ليقوم بتدمير الجلسة في قاعدة البيانات (بدون الاعتماد على MediatR هنا)
            using var client = new HttpClient();
            // ⚠️ تأكد أن البورت 7241 يطابق بورت الـ WebApi الخاص بك
            client.BaseAddress = new Uri("https://localhost:7241/");
            client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

            await client.PostAsync("api/Auth/logout", null);
        }
        catch
        {
            // نتجاهل أي خطأ في الاتصال بالخادم لكي لا نمنع المستخدم من تسجيل الخروج محلياً
        }
    }

    // 3. مسح الـ Cookie وتوجيه المستخدم لصفحة الدخول
    await Microsoft.AspNetCore.Authentication.AuthenticationHttpContextExtensions.SignOutAsync(
        ctx,
        Microsoft.AspNetCore.Authentication.Cookies.CookieAuthenticationDefaults.AuthenticationScheme);

    return Results.Redirect("/login");
});

// =========================================================================

app.Run();