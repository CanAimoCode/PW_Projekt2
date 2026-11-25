using System.Globalization;
using GameCatalogApp.Data;
using GameCatalogApp.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace GameCatalogApp
{
    public class Program
    {
        public static void Main(string[] args)
        {
            CultureInfo.DefaultThreadCurrentCulture = CultureInfo.InvariantCulture;
            CultureInfo.DefaultThreadCurrentUICulture = CultureInfo.InvariantCulture;

            var builder = WebApplication.CreateBuilder(args);

            builder.Services.AddControllersWithViews();

            string provider = builder.Configuration["DataProvider"]
                ?? throw new InvalidOperationException(
                    "Nie określono DataProvider w appsettings.json"
                );

            provider = provider.ToUpperInvariant();

            var assembly = typeof(IVideoGameRepository).Assembly;

            string repositorySuffix = provider switch
            {
                "SQL" => "Sql",
                "FILE" => "File",
                _ => throw new InvalidOperationException(
                    $"Nieobsługiwany DataProvider: {provider}"
                )
            };

            var videoGameRepositoryType = assembly.GetType(
                $"GameCatalogApp.Data.VideoGameRepository{repositorySuffix}"
            );

            var studioRepositoryType = assembly.GetType(
                $"GameCatalogApp.Data.StudioRepository{repositorySuffix}"
            );

            if (videoGameRepositoryType == null ||
                !typeof(IVideoGameRepository).IsAssignableFrom(
                    videoGameRepositoryType))
            {
                throw new InvalidOperationException(
                    "Nie znaleziono poprawnego repozytorium gier."
                );
            }

            if (studioRepositoryType == null ||
                !typeof(IStudioRepository).IsAssignableFrom(
                    studioRepositoryType))
            {
                throw new InvalidOperationException(
                    "Nie znaleziono poprawnego repozytorium studiów."
                );
            }

            if (provider == "SQL")
            {
                string connectionString =
                    builder.Configuration.GetConnectionString(
                        "DefaultConnection"
                    )!;

                builder.Services.AddDbContext<ApplicationDbContext>(
                    options => options.UseSqlite(connectionString)
                );
            }

            builder.Services.AddScoped(
                typeof(IVideoGameRepository),
                serviceProvider =>
                    (IVideoGameRepository)ActivatorUtilities.CreateInstance(
                        serviceProvider,
                        videoGameRepositoryType
                    )
            );

            builder.Services.AddScoped(
                typeof(IStudioRepository),
                serviceProvider =>
                    (IStudioRepository)ActivatorUtilities.CreateInstance(
                        serviceProvider,
                        studioRepositoryType
                    )
            );

            builder.Services.AddRazorComponents()
                .AddInteractiveServerComponents();

            var app = builder.Build();

            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Home/Error");
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();
            app.UseRouting();
            app.UseAuthorization();
            app.UseAntiforgery();

            app.MapRazorComponents<GameCatalogApp.Components.App>()
                .AddInteractiveServerRenderMode();

            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}");

            app.Run();
        }
    }
}
