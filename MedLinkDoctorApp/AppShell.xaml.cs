namespace MedLinkDoctorApp;

public partial class AppShell : Shell
{
	public AppShell()
	{
		InitializeComponent();

        RegisterRoutingPages();
    }

    private void RegisterRoutingPages()
    {
        Routing.RegisterRoute(nameof(OnlinePage), typeof(OnlinePage));
    }
}
