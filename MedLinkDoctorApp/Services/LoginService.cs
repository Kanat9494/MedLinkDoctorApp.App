namespace MedLinkDoctorApp.Services;

internal class LoginService
{
    internal LoginService()
    {

    }

    private static LoginService _instance;
    public static LoginService Instance()
    {
        if (_instance == null)
            _instance = new LoginService();

        return _instance;
    }

    public async Task<DoctorAuthResponse> AuthenticateDoctor(string userName, string password)
    {
        var requestDoctor = new DoctorAuthRequest()
        {
            UserName = userName,
            Password = password
        };

        using (HttpClient httpClient = new HttpClient())
        {
            httpClient.BaseAddress = new Uri(MedLinkConstants.SERVER_ROOT_URL);

            var content = new StringContent(JsonConvert.SerializeObject(requestDoctor), Encoding.UTF8, "application/json");

            try
            {
                var response = await httpClient.PostAsync("api/Authentication/AuthenticateDoctor", content);

                var jsonResult = await response.Content.ReadAsStringAsync();

                var authenticatedDoctor = JsonConvert.DeserializeObject<DoctorAuthResponse>(jsonResult);

                return authenticatedDoctor;
            }
            catch (Exception ex)
            {
                return new DoctorAuthResponse
                {
                    StatusCode = 500,
                    ResponseMessage = ex.Message
                };
            }
        }
    }
}
