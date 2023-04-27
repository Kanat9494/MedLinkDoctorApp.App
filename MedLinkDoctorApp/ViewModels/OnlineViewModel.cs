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
        AcceptCommand = new Command(OnAccept);
    }

    string _senderName;
    string _receiverName;
    double _productPrice;
    string _accessToken;
    int _offerId;
    CancellationTokenSource cancelTokenSource;
    CancellationToken cancelToken;

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


    public Command CancelCommand { get; }
    public Command AcceptCommand { get; }
    public Command RejectCommand { get; }

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
                        _offerId = offer.OfferId;
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
            while (true)
            {
                var isSaved = await ContentService.Instance(_accessToken).ServiceQuery($"api/Offers/SetOffer?senderName={_senderName}&receiverName={_receiverName}&status=0");
                var isDeleted = await ContentService.Instance(_accessToken).ServiceQuery($"api/Offers/DeleteOffer?offerId={_offerId}");
                if (isSaved && isDeleted)
                    break;
            }
            GetOffer();
        }
        catch (Exception ex)
        {

        }
    }
    async void OnAccept()
    {
        try
        {
            await Task.Run(async () =>
            {
                while (true)
                {
                    var isSaved = await ContentService.Instance(_accessToken).ServiceQuery($"api/Offers/SetOffer?senderName={_senderName}&receiverName={_receiverName}&status=1");
                    var isDeleted = await ContentService.Instance(_accessToken).ServiceQuery($"api/Offers/DeleteOffer?offerId={_offerId}");

                    if (isSaved && isDeleted)
                        break;

                    await Task.Delay(3000);
                }
            });
            

            await Shell.Current.GoToAsync(nameof(ChatPage));
        }
        catch (Exception ex)
        {

        }
    }
}
