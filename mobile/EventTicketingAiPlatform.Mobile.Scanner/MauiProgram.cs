using EventTicketingAiPlatform.Mobile.Scanner.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System.Reflection;
using ZXing.Net.Maui.Controls;

namespace EventTicketingAiPlatform.Mobile.Scanner;

public static class MauiProgram
{
    public static MauiApp CreateMauiApp()
    {
        var builder = MauiApp.CreateBuilder();

        builder
            .UseMauiApp<App>()
            .UseBarcodeReader()
            .ConfigureFonts(fonts =>
            {
                fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
            });

        builder.Services.AddMauiBlazorWebView();

#if DEBUG
        builder.Services.AddBlazorWebViewDeveloperTools();
        builder.Logging.AddDebug();
#endif

        var assembly = Assembly.GetExecutingAssembly();

        using var stream = assembly.GetManifestResourceStream(
            "EventTicketingAiPlatform.Mobile.Scanner.wwwroot.appsettings.json");

        if (stream is null)
            throw new InvalidOperationException("Mobile appsettings.json was not found as embedded resource.");

        var config = new ConfigurationBuilder()
            .AddJsonStream(stream)
            .Build();

        builder.Configuration.AddConfiguration(config);

        builder.Services.Configure<AppSettings>(config);
        builder.Services.AddHttpClient<ScannerApiClient>();

        return builder.Build();
    }
}