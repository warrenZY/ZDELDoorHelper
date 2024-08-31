using Camera.MAUI;
using Material.Components.Maui.Extensions;
using Microsoft.Extensions.Logging;
using ZDELDoorHelper.Services;

namespace ZDELDoorHelper
{
    public static class MauiProgram
    {
        public static MauiApp CreateMauiApp()
        {
            var builder = MauiApp.CreateBuilder();
            builder
                .UseMauiApp<App>()
                .UseMauiCameraView()
                .ConfigureFonts(fonts =>
                {
                    fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                    fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
                })
                .UseMaterialComponents();

#if DEBUG
    		builder.Logging.AddDebug();
#endif

            builder.Services.AddScoped<IZDELDoorHelperTools, ZDELDoorHelperTools>();
            //Store config in xml
            builder.Services.AddScoped<ConfigurationStorageService, ConfigurationStorageService>();
            return builder.Build();
        }
    }
}
