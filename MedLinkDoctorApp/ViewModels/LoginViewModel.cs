using MedLinkDoctorApp.Services;

namespace MedLinkDoctorApp.ViewModels;

internal class LoginViewModel : BaseViewModel
{
    internal LoginViewModel()
    {
        IsLoading = false;
        LoginCommand = new Command(async () => await OnLogin());

        UserName = "996701555268";
        Password = "5252";
    }

    private bool _isLoading;
    public bool IsLoading
    {
        get => _isLoading;
        set => SetProperty(ref _isLoading, value);
    }
    private string _userName;
    public string UserName
    {
        get => _userName;
        set => SetProperty(ref _userName, value);
    }
    private string _password;
    public string Password
    {
        get => _password;
        set => SetProperty(ref _password, value);
    }
    private DoctorAuthResponse _currentDoctor;
    public DoctorAuthResponse CurrentDoctor
    {
        get => _currentDoctor;
        set => SetProperty(ref _currentDoctor, value);
    }

    public Command LoginCommand { get; }

    private async Task OnLogin()
    {
        IsLoading = true;
        if (string.IsNullOrEmpty(UserName) || string.IsNullOrEmpty(Password))
        {
            await Shell.Current.DisplayAlert("Пустые значения",
                "Пожалуйста введите логин и пароль для входа!", "Ок");
            IsLoading = false;
        }
        else
        {
            CurrentDoctor = await LoginService.Instance().AuthenticateDoctor(userName: UserName, password: Password);

            if (CurrentDoctor.StatusCode == 200)
            {
                await SecureStorage.Default.SetAsync("DoctorAccessToken", CurrentDoctor.AccessToken);
                await SecureStorage.Default.SetAsync("DoctorId", CurrentDoctor.DoctorId.ToString());
                await SecureStorage.Default.SetAsync("AccountName", CurrentDoctor.AccountName);

                await Shell.Current.GoToAsync($"//{nameof(HomePage)}");
            }
            else
            {
                await Shell.Current.DisplayAlert("Не удалось войти в систему",
                    $"{CurrentDoctor.ResponseMessage}", "Ок");
                IsLoading = false;
            }
        }
    }
}
