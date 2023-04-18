using Microsoft.AspNetCore.SignalR.Client;

namespace MedLinkDoctorApp.ViewModels;

internal class OnlineViewModel : BaseViewModel
{
    public OnlineViewModel()
    {
        IsOnline = true;
        IsConfirmMessage = false;
        _isCompleted = false;

        Task.Run(async () =>
        {
            _senderName = await SecureStorage.Default.GetAsync("AccountName");
        }).Wait();

        _hubConnection = new HubConnectionBuilder()
            .WithUrl(MedLinkConstants.SERVER_ROOT_URL + $"/chatHub/{_senderName}")
            .Build();

        Task.Run(async () =>
        {
            await Connect();
        });

        CancelCommand = new Command(OnCancel);
        RejectCommand = new Command(OnReject);
        AcceptCommand = new Command(OnAccept);

        _hubConnection.Closed += async (error) =>
        {
            await Task.Delay(5000);
            await Connect();
        };

        _hubConnection.On<string, string, double>("ReceiveConfirmMessage", (senderName, receiverName, price) =>
        {
            Task.Run(async () =>
            {
                await SecureStorage.Default.SetAsync("ReceiverName", senderName);
                _receiverName = senderName;
            }).Wait();

            _productPrice = price;
            ConfirmMessage = $"К вам поступил запрос на консультацию, стоимостью {price} сом.";
            IsConfirmMessage = true;
        });

        _hubConnection.On<string, string, string>("ReceiveMessage", async (senderName, receiverName, jsonMessage) =>
        {
            try
            {
                var message = JsonConvert.DeserializeObject<Message>(jsonMessage);

                
                Task.Run(async () =>
                {
                    await SecureStorage.Default.SetAsync("ReceiverName", senderName);
                    _receiverName = senderName;

                        
                }).Wait();   

                Task.Run(async () =>
                {
                    await CompleteConfirmMessage();
                }).Wait();

                App.Current.Dispatcher.Dispatch(async () =>
                {
                    await Shell.Current.GoToAsync(nameof(ChatPage));
                });
            }
            catch
            {

            }
        });
    }

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
    string _senderName;
    string _receiverName;
    HubConnection _hubConnection;
    bool _isCompleted;
    double _productPrice;

    public Command CancelCommand { get; }
    public Command AcceptCommand { get; }
    public Command RejectCommand { get; }

    async Task Connect()
    {
        try
        {
            await _hubConnection.StartAsync();
        }
        catch
        {

        }
    }

    async Task Disconnect()
    {
        await _hubConnection.StopAsync();
    }

    async Task ResponseConfirmMessage()
    {
        try
        {
            await _hubConnection.InvokeAsync("SendMessage", _senderName, _receiverName, JsonConvert.SerializeObject(new Message
            {
                SenderName = _senderName,
                ReceiverName = _receiverName,
                Content = MedLinkConstants.CONFIRM_MESSAGE
            }));
        }
        catch
        {

        }
    }

    private async Task OnAcceptedConsultation()
    {
        await Disconnect();
    }

    private async Task CompleteConfirmMessage()
    {
        await ResponseConfirmMessage();
        await OnAcceptedConsultation();
    }

    async void OnCancel()
    {
        await Disconnect();
        await Shell.Current.GoToAsync($"//{nameof(HomePage)}");
    }

    async void OnReject()
    {
        try
        {
            await _hubConnection.InvokeAsync("SendRejectMessage", _senderName, _receiverName);
            IsConfirmMessage = false;
            await Shell.Current.DisplayAlert("Отмена", "Вы отменили запрос", "Ок");
        }
        catch (Exception ex)
        {

        }
    }
    async void OnAccept()
    {
        try
        {
            await _hubConnection.InvokeAsync("SendConfirmMessage", _senderName, _receiverName, _productPrice);
            App.Current.Dispatcher.Dispatch(async () =>
            {
                await Shell.Current.GoToAsync(nameof(ChatPage));
            });
        }
        catch (Exception ex)
        {

        }
    }
}
