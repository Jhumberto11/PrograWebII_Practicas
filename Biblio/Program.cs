using Biblio.Data;
using Biblio.Repositories;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);


builder.Services.AddIdentity<IdentityUser, IdentityRole>(
    options =>
    {
        options.Lockout.MaxFailedAccessAttempts = 5;
        options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(15);
        options.Lockout.AllowedForNewUsers = true;
    })
    .AddRoles<IdentityRole>()
    .AddEntityFrameworkStores<BiblioContext>()
    .AddDefaultTokenProviders();

builder.Services.ConfigureApplicationCookie(options =>
{
    options.LoginPath = "/Account/Login";
    options.LogoutPath = "/Account/Logout";
    options.AccessDeniedPath = "/Account/AccessDenied";
});

// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddSingleton<IReposotoryLibro, RepositorioEnMemoria>();
builder.Services.AddScoped<IAutoresService, AutorRepositoryAlternativo>();

builder.Services.AddDbContext<BiblioContext>(options =>
{
    options.UseSqlServer(builder.Configuration.GetConnectionString("BiblioDB"));
});

var app = builder.Build();


using (var scope = app.Services.CreateScope())
{
    var roleManager = scope.ServiceProvider
        .GetRequiredService<RoleManager<IdentityRole>>();

    var userManager = scope.ServiceProvider
        .GetRequiredService<UserManager<IdentityUser>>();

    string[] roles = { "Administrador", "Usuario", "Bibliotecario" };

    foreach (var rol in roles)
    {
        if (!await roleManager.RoleExistsAsync(rol))
        {
            var resultado = await roleManager.CreateAsync(
                new IdentityRole(rol));

            if (!resultado.Succeeded)
            {
                throw new Exception(
                    "No se pudo crear el rol: " + rol);
            }
        }
    }

    var administradorId = app.Configuration["AdministradorId"];

    if (!string.IsNullOrWhiteSpace(administradorId))
    {
        var administrador = await userManager
            .FindByIdAsync(administradorId);

        if (administrador == null)
        {
            throw new Exception(
                "No se encontró el usuario administrador.");
        }

        if (!await userManager.IsInRoleAsync(
            administrador, "Administrador"))
        {
            var resultado = await userManager.AddToRoleAsync(
                administrador, "Administrador");

            if (!resultado.Succeeded)
            {
                throw new Exception(
                    "No se pudo asignar el administrador.");
            }
        }
    }

    var usuarios = await userManager.Users.ToListAsync();

    foreach (var usuario in usuarios)
    {
        var rolesUsuario = await userManager
            .GetRolesAsync(usuario);

        if (usuario.Email.Contains("@biblioteca.sv")){
            var resultado = await userManager.AddToRoleAsync(usuario, "Bibliotecario");
        }
        if (rolesUsuario.Count == 0)
        {
            var resultado = await userManager.AddToRoleAsync(
                usuario, "Usuario");

            if (!resultado.Succeeded)
            {
                throw new Exception(
                    "No se pudo asignar el rol Lector.");
            }
        }
    }

    


}





// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();


app.UseStaticFiles();

app.MapStaticAssets();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}")
    .WithStaticAssets();


app.Run();
