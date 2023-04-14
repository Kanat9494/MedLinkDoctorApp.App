namespace MedLinkDoctorApp.ViewModels;

internal class HomeViewModel : BaseViewModel
{
    public HomeViewModel()
    {
        Task.Run(async () =>
        {
            accessToken = await SecureStorage.Default.GetAsync("DoctorAccessToken");
            doctorId = await SecureStorage.Default.GetAsync("DoctorId");
        });
        OnlineCommand = new Command(OnOnline);
        OffLineCommand = new Command(OnOffline);
    }

    public Command OnlineCommand { get; }
    public Command OffLineCommand { get; }
    string accessToken;
    string doctorId;

    private async void OnOnline()
    {
        await ContentService.Instance(accessToken).UpdateItemAsync($"api/Doctors/ChangeOnlineStatus?doctorId={doctorId}&status=1");
        await Shell.Current.GoToAsync(nameof(OnlinePage));
    }

    private async void OnOffline()
    {
        await ContentService.Instance(accessToken).UpdateItemAsync($"api/Doctors/ChangeOnlineStatus?doctorId={doctorId}&status=0");
    }
}
