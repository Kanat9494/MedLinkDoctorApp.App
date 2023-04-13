namespace MedLinkDoctorApp.Views;

[XamlCompilation(XamlCompilationOptions.Compile)]
public partial class AccountPage : ContentPage
{
	public AccountPage()
	{
		InitializeComponent();

		BindingContext = new AccountViewModel();
	}
}