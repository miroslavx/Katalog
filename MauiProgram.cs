using Microsoft.Extensions.Logging;
using Katalog.Services;
namespace Katalog
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
                    fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
                });

            builder.Services.AddSingleton<DatabaseService>();
            builder.Services.AddTransient<StylistService>();

            builder.Services.AddTransient<MainPage>();
            builder.Services.AddTransient<WardrobePage>();
            builder.Services.AddTransient<AddItemPage>();
            builder.Services.AddTransient<ItemDetailPage>();
            builder.Services.AddTransient<RandomChoicePage>();
            builder.Services.AddTransient<StylePage>();
            builder.Services.AddSingleton<WeatherService>();
            builder.Services.AddTransient<WeatherOutfitPage>(); 



#if DEBUG
            builder.Logging.AddDebug();

#endif

            return builder.Build();
        }
    }

}