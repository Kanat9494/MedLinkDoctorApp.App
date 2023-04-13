namespace MedLinkDoctorApp.ViewModels;

internal class AccountViewModel : BaseViewModel
{
    public AccountViewModel()
    {
        IsLoading = true;

        Task.Run(async () =>
        {
            _accessToken = await SecureStorage.Default.GetAsync("DoctorAccessToken");
            _doctorId = await SecureStorage.Default.GetAsync("DoctorId");

            await InitializeDoctor();
        });
    }

    private DoctorAuthResponse _currentDoctor;
    public DoctorAuthResponse CurrentDoctor
    {
        get => _currentDoctor;
        set => SetProperty(ref _currentDoctor, value);  
    }
    private bool _isLoading;
    public bool IsLoading
    {
        get => _isLoading;
        set => SetProperty(ref _isLoading, value);  
    }

    string _accessToken;
    string _doctorId;

    public async Task InitializeDoctor()
    {
        try
        {
            var response = await ContentService.Instance(_accessToken).GetItemAsync<DoctorAuthResponse>($"api/Doctors/GetDoctorAccount?doctorId={_doctorId}");

            if (response.StatusCode == 200)
            {
                CurrentDoctor = response;
            }

            IsLoading = false;
        }
        catch
        {
            IsLoading = false;
        }
    }
}
