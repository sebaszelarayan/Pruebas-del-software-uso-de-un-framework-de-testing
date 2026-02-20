using tl2_recuperacionparcial2_Gonz0x.Interfaces;
using tl2_recuperacionparcial2_Gonz0x.Repositorios;
using tl2_recuperacionparcial2_Gonz0x.Services;
using Microsoft.AspNetCore.Http;

var builder = WebApplication.CreateBuilder(args);

// -------------------------
//     SESIÓN
// -------------------------
builder.Services.AddHttpContextAccessor();

builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

// -------------------------
//     INYECCIÓN DE DEPENDENCIAS (DI)
// -------------------------

// Repositorios CON cadena de conexión
//  Configuración de Cadena de Conexión
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection")!.ToString();
builder.Services.AddSingleton<string>(connectionString);

// Registrar los repositorios pasando la cadena inyectada
builder.Services.AddScoped<ITareaRepository, TareaRepository>();
builder.Services.AddScoped<IUserRepository, UsuarioRepository>();

// Servicios
builder.Services.AddScoped<IAuthenticationService, AuthenticationService>();

// MVC
builder.Services.AddControllersWithViews();

// -------------------------
//     PIPELINE
// -------------------------
var app = builder.Build();

app.UseSession();
app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}"
);

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.Run();
