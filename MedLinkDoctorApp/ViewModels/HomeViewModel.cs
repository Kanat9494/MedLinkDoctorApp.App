namespace MedLinkDoctorApp.ViewModels;

internal class HomeViewModel : BaseViewModel
{
    public HomeViewModel()
    {
        OnlineCommand = new Command(OnOnline);
    }

    public Command OnlineCommand { get; }

    private async void OnOnline()
    {
        await Shell.Current.GoToAsync(nameof(OnlinePage));
    }
}
