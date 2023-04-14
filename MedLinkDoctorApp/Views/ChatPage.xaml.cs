namespace MedLinkDoctorApp.Views;

[XamlCompilation(XamlCompilationOptions.Compile)]
public partial class ChatPage : ContentPage
{
	public ChatPage()
	{
		InitializeComponent();

		BindingContext = new ChatViewModel();
	}

	protected override bool OnBackButtonPressed()
	{
		return true;
	}
}