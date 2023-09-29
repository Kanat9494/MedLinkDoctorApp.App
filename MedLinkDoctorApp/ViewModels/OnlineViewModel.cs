
using System.Windows.Input;

namespace MedLinkDoctorApp.ViewModels;

internal class OnlineViewModel : BaseViewModel
{
    public OnlineViewModel()
    {
        IsOnline = true;
        IsConfirmMessage = false;
        cancelTokenSource = new CancellationTokenSource();
        cancelToken = cancelTokenSource.Token;

        Task.Run(async () =>
        {
            _senderName = await SecureStorage.Default.GetAsync("AccountName");
            _accessToken = await SecureStorage.Default.GetAsync("DoctorAccessToken");
        }).Wait();

        GetOffer();

        CancelCommand = new Command(OnCancel);
        RejectCommand = new Command(OnReject);
        AcceptCommand = new AsyncRelayCommand(OnAccept);
    }

    string _senderName;
    string _receiverName;
    double _productPrice;
    string _accessToken;
    CancellationTokenSource cancelTokenSource;
    CancellationToken cancelToken;



    public Command CancelCommand { get; }
    public ICommand AcceptCommand { get; }
    public Command RejectCommand { get; }


    private bool _isConfirmMessage;
    public bool IsConfirmMessage
    {
        get => _isConfirmMessage;
        set => SetProperty(ref _isConfirmMessage, value);
    }
    private string _confirmMessage;
    public string ConfirmMessage
    {
        get => _confirmMessage;
        set => SetProperty(ref _confirmMessage, value);
    }
    private bool _isOnline;
    public bool IsOnline
    {
        get => _isOnline;
        set => SetProperty(ref _isOnline, value);
    }
    private int _offerId;
    public int OfferId
    {
        get => _offerId;
        set => SetProperty(ref _offerId, value);
    }


    

    void GetOffer()
    {
        Task findOfferTask = new Task(async () =>
        {
            await Task.Delay(2000);

            while (true)
            {
                if (cancelToken.IsCancellationRequested)
                    break;

                var offer = await ContentService.Instance(_accessToken).GetItemAsync<Offer>($"api/Offers/GetOffer?receiverName={_senderName}");

                if (offer != null)
                {
                    if (offer.StatusCode == 200)
                    {
                        _receiverName = offer.SenderName;
                        await SecureStorage.Default.SetAsync("ReceiverName", offer.SenderName);
                        _productPrice = offer.ProductPrice;
                        ConfirmMessage = $"К вам поступил запрос на консультацию, стоимостью {_productPrice} сом.";
                        OfferId = offer.OfferId;
                        IsConfirmMessage = true;

                        break;
                    }
                }

                await Task.Delay(5000);
            }

        }, cancelToken);

        findOfferTask.Start();
    }

    async void OnCancel()
    {
        cancelTokenSource.Cancel();
        //await Task.Delay(1000);
        cancelTokenSource.Dispose();
        await Shell.Current.GoToAsync($"//{nameof(HomePage)}");
    }

    async void OnReject()
    {
        try
        {
            await Shell.Current.DisplayAlert("Отмена", "Вы отменили запрос", "Ок");
            IsConfirmMessage = false;
            await Task.Delay(1500);
            
            await DeleteOffer(OfferId);
            GetOffer();
        }
        catch (Exception ex)
        {

        }
    }
    async Task OnAccept()
    {
        try
        {
            await Task.Run(async () =>
            {
                while (true)
                {
                    var isSaved = await SaveOffer(1);

                    if (isSaved)
                        break;

                    await Task.Delay(3000);
                }
            });
            
            await DeleteOffer(OfferId);

            await Shell.Current.GoToAsync(nameof(ChatPage));
        }
        catch (Exception ex)
        {

        }
    }

    private async Task<bool> SaveOffer(byte isConfirmed)
    {
        try
        {
            var offer = new Offer
            {
                SenderName = _senderName,
                ReceiverName = _receiverName,
                ProductPrice = _productPrice,
                IsConfirmed = isConfirmed
            };
            var offerId = await ContentService.Instance(_accessToken).PostItemAsync(offer, "api/Offers/SetOffer");

            if (offerId > 0)
            {
                return true;
            }

            return false;
        }
        catch { return false; }
    }

    async Task DeleteOffer(int? offerId)
    {
        if (offerId != null && offerId > 0)
            await ContentService.Instance(_accessToken).DeleteItemAsync($"api/Offers/DeleteOffer?offerId={offerId}");
    }
}
