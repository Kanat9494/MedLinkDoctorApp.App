namespace MedLinkDoctorApp.Views;

[XamlCompilation(XamlCompilationOptions.Compile)]
public partial class OnlinePage : ContentPage
{
	public OnlinePage()
	{
		InitializeComponent();

		this.BindingContext = new OnlineViewModel();
	}

    //protected override bool OnBackButtonPressed()
    //{
    //    return true;
    //}
}