using Microsoft.Extensions.Logging;
using MundialApp.Configuration;
using MundialApp.Repositories;
using MundialApp.Repositories.Infrastructure;
using MundialApp.Services;

namespace MundialApp
{
    public static class MauiProgram
    {
        public static MauiApp CreateMauiApp()
        {
            var builder = MauiApp.CreateBuilder();
            builder
                .UseMauiApp<App>()
                .ConfigureFonts(fonts =>
                {
                    fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                });

            builder.Services.Configure<OracleOptions>(options =>
            {
                options.ConnectionString = "User Id=mundial;Password=1234;Data Source=localhost:1521/XEPDB1;";
            });
            builder.Services.AddMauiBlazorWebView();
            builder.Services.AddSingleton<SessionState>();
            builder.Services.AddSingleton<PdfViewerService>();
            builder.Services.AddSingleton<IOracleConnectionFactory, OracleConnectionFactory>();
            builder.Services.AddScoped<AuthRepository>();
            builder.Services.AddScoped<LookupRepository>();
            builder.Services.AddScoped<DashboardRepository>();
            builder.Services.AddScoped<TeamRepository>();
            builder.Services.AddScoped<PlayerRepository>();
            builder.Services.AddScoped<MatchRepository>();
            builder.Services.AddScoped<UserRepository>();
            builder.Services.AddScoped<ConfederationRepository>();
            builder.Services.AddScoped<CountryRepository>();
            builder.Services.AddScoped<CityRepository>();
            builder.Services.AddScoped<StadiumRepository>();
            builder.Services.AddScoped<GroupRepository>();
            builder.Services.AddScoped<CoachRepository>();
            builder.Services.AddScoped<QueryRepository>();
            builder.Services.AddScoped<ReportRepository>();
            builder.Services.AddScoped<AuthService>();
            builder.Services.AddScoped<LookupService>();
            builder.Services.AddScoped<DashboardService>();
            builder.Services.AddScoped<TeamService>();
            builder.Services.AddScoped<PlayerService>();
            builder.Services.AddScoped<MatchService>();
            builder.Services.AddScoped<UserService>();
            builder.Services.AddScoped<ConfederationService>();
            builder.Services.AddScoped<CountryService>();
            builder.Services.AddScoped<CityService>();
            builder.Services.AddScoped<StadiumService>();
            builder.Services.AddScoped<GroupService>();
            builder.Services.AddScoped<CoachService>();
            builder.Services.AddScoped<QueryService>();
            builder.Services.AddScoped<ReportService>();

#if DEBUG
    		builder.Services.AddBlazorWebViewDeveloperTools();
    		builder.Logging.AddDebug();
#endif

            return builder.Build();
        }
    }
}
