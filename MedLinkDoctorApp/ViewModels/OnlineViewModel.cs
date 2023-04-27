namespace MedLinkDoctorApp.ViewModels;

internal class OnlineViewModel : BaseViewModel
{
    public OnlineViewModel()
    {
        IsOnline = true;
        IsConfirmMessage = false;
        _isCompleted = false;
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

        //_hubConnection.On<string, string, double>("ReceiveConfirmMessage", (senderName, receiverName, price) =>
        //{
        //    Task.Run(async () =>
        //    {
        //        await SecureStorage.Default.SetAsync("ReceiverName", senderName);
        //        _receiverName = senderName;
        //    }).Wait();

        //    _productPrice = price;
        //    ConfirmMessage = $"К вам поступил запрос на консультацию, стоимостью {price} сом.";
        //    IsConfirmMessage = true;
        //});

        //_hubConnection.On<string, string, string>("ReceiveMessage", async (senderName, receiverName, jsonMessage) =>
        //{
        //    try
        //    {
        //        var message = JsonConvert.DeserializeObject<Message>(jsonMessage);

                
        //        Task.Run(async () =>
        //        {
        //            await SecureStorage.Default.SetAsync("ReceiverName", senderName);
        //            _receiverName = senderName;

                        
        //        }).Wait();   

        //        Task.Run(async () =>
        //        {
        //            await CompleteConfirmMessage();
        //        }).Wait();

        //        App.Current.Dispatcher.Dispatch(async () =>
        //        {
        //            await Shell.Current.GoToAsync(nameof(ChatPage));
        //        });
        //    }
        //    catch
        //    {

        //    }
        //});
    }

    string _senderName;
    string _receiverName;
    bool _isCompleted;
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

                var offer = await ContentService.Instance(_accessToken).GetItemAsync<Offer>($"api/Offer/GetOffer?receiverName={_senderName}");

                if (offer != null)
                {
                    if (offer.StatusCode == 200)
                    {
                        _receiverName = offer.ReceiverName;
                        ConfirmMessage = $"К вам поступил запрос на консультацию, стоимостью {offer.ProductPrice} сом.";
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
        await Task.Delay(1000);
        cancelTokenSource.Dispose();
        await Shell.Current.GoToAsync($"//{nameof(HomePage)}");
    }

    async void OnReject()
    {
        try
        {
            await Shell.Current.DisplayAlert("Отмена", "Вы отменили запрос", "Ок");
            IsConfirmMessage = false;
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
            await ContentService.Instance(_accessToken).UpdateItemAsync($"api/Offer/SetOffer?senderName={_senderName}&receiverName={_receiverName}");
            await ContentService.Instance(_accessToken).UpdateItemAsync($"api/Offer/DeleteOffer?offerId={_offerId}");
            

            await Shell.Current.GoToAsync(nameof(ChatPage));
        }
        catch (Exception ex)
        {

        }
    }
}
