﻿namespace MedLinkDoctorApp.Services;

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
}
