using Microsoft.Extensions.Logging;

namespace MedLinkDoctorApp;

public static class MauiProgram
{
	public static MauiApp CreateMauiApp()
	{
		var builder = MauiApp.CreateBuilder();
		builder
			.UseMauiApp<App>()
			.UseMauiCommunityToolkit()
			.ConfigureFonts(fonts =>
			{
				fonts.AddFont("FiraSans-Regular", "RegularFont");
				fonts.AddFont("FiraSans-Medium.ttf", "MediumFont");
			});

#if DEBUG
		builder.Logging.AddDebug();
#endif

		return builder.Build();
	}
}
