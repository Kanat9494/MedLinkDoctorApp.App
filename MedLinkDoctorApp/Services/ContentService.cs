namespace MedLinkDoctorApp.Services;

internal class ContentService
{
    public ContentService(string token)
    {
        accessToken = token;
    }

    string accessToken;
    private static ContentService _instance;
    public static ContentService Instance(string token)
    {
        if (_instance == null)
            _instance = new ContentService(token);

        return _instance;
    }

    public async Task<TResponse> GetItemAsync<TResponse>(string requestUrl) where TResponse : BaseResponse
    {
        using (HttpClient httpClient = new HttpClient())
        {
            httpClient.BaseAddress = new Uri(MedLinkConstants.SERVER_ROOT_URL);
            httpClient.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));
            httpClient.DefaultRequestHeaders.Add("Authorization", "Bearer " + accessToken);

            try
            {
                var response = await httpClient.GetStringAsync(httpClient.BaseAddress + requestUrl);
                TResponse result = JsonConvert.DeserializeObject<TResponse>(response);
                result.StatusCode = 200;

                return result;
            }
            catch (HttpRequestException httpEx)
            {
                if (httpEx.StatusCode.HasValue)
                {
                    var result = Activator.CreateInstance<TResponse>();
                    result.StatusCode = (int)httpEx.StatusCode;
                    result.ResponseMessage = httpEx.Message;

                    return result;
                }
                else
                {
                    var result = Activator.CreateInstance<TResponse>();
                    result.StatusCode = 500;
                    result.ResponseMessage = httpEx.Message;

                    return result;
                }
            }
            catch (Exception ex)
            {
                var result = Activator.CreateInstance<TResponse>();
                result.StatusCode = 500;
                result.ResponseMessage = ex.Message;

                return result;
            }
        }
    }

    public async Task UpdateItemAsync(string requestUrl)
    {
        using (HttpClient httpClient = new HttpClient())
        {
            httpClient.BaseAddress = new Uri(MedLinkConstants.SERVER_ROOT_URL);
            httpClient.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));
            httpClient.DefaultRequestHeaders.Add("Authorization", "Bearer " + accessToken);

            try
            {
                await httpClient.GetStringAsync(httpClient.BaseAddress + requestUrl);
            }
            catch (Exception ex)
            {
            }
        }
    }

    public async Task<bool> ServiceQuery(string requestUrl)
    {
        using (HttpClient httpClient = new HttpClient())
        {
            httpClient.BaseAddress = new Uri(MedLinkConstants.SERVER_ROOT_URL);
            httpClient.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));
            httpClient.DefaultRequestHeaders.Add("Authorization", "Bearer " + accessToken);

            try
            {
                var response = await httpClient.GetStringAsync(httpClient.BaseAddress + requestUrl);
                bool isSuccess;
                var result = bool.TryParse(response, out isSuccess);

                return isSuccess;
            }
            catch (Exception ex)
            {
                return false;
            }
        }
    }

    public async Task<IEnumerable<TResponse>> GetItemsAsync<TResponse>(string requestUrl)
    {
        using (HttpClient httpClient = new HttpClient())
        {
            httpClient.BaseAddress = new Uri(MedLinkConstants.SERVER_ROOT_URL);
            httpClient.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));
            httpClient.DefaultRequestHeaders.Add("Authorization", "Bearer " + accessToken);

            try
            {
                var response = await httpClient.GetStringAsync(httpClient.BaseAddress + requestUrl);
                IEnumerable<TResponse> result = JsonConvert.DeserializeObject<IEnumerable<TResponse>>(response);

                return result;
            }
            catch (Exception ex)
            {
                return null;
            }
        }
    }

    internal async Task<int> PutItemAsync<TRequest>(TRequest request, string url)
    {
        using (var httpClient = new HttpClient())
        {
            httpClient.BaseAddress = new Uri(MedLinkConstants.SERVER_ROOT_URL);
            httpClient.DefaultRequestHeaders.Add("Authorization", "Bearer " + accessToken);
            httpClient.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));
            var content = new StringContent(JsonConvert.SerializeObject(request), Encoding.UTF8, "application/json");

            try
            {
                var response = await httpClient.PutAsync(url, content);
                var jsonResult = await response.Content.ReadAsStringAsync();

                var result = JsonConvert.DeserializeObject<int>(jsonResult);
                return result;
            }
            catch
            {
                return 0;
            }
        }
    }

    internal async Task<int> PostItemAsync<TReqeust>(TReqeust request, string url)
    {
        using (var httpClient = new HttpClient())
        {
            httpClient.BaseAddress = new Uri(MedLinkConstants.SERVER_ROOT_URL);
            httpClient.DefaultRequestHeaders.Add("Authorization", "Bearer " + accessToken);
            httpClient.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));
            var content = new StringContent(JsonConvert.SerializeObject(request), Encoding.UTF8, "application/json");
            //httpClient.DefaultRequestHeaders.Add("Authorization", "Bearer " + _accessToken);

            try
            {
                var response = await httpClient.PostAsync(url, content);
                var jsonResult = await response.Content.ReadAsStringAsync();

                var result = JsonConvert.DeserializeObject<int>(jsonResult);
                return result;
            }
            catch { return 0; }
        }
    }

    internal async Task<int> DeleteItemAsync(string url)
    {
        using (var httpClient = new HttpClient())
        {
            httpClient.BaseAddress = new Uri(MedLinkConstants.SERVER_ROOT_URL);
            httpClient.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));
            httpClient.DefaultRequestHeaders.Add("Authorization", "Bearer " + accessToken);

            try
            {
                var response = await httpClient.DeleteAsync(url);
                var jsonResult = await response.Content.ReadAsStringAsync();

                var result = JsonConvert.DeserializeObject<int>(jsonResult);
                return result;
            }
            catch { return 0; }
        }
    }
}
