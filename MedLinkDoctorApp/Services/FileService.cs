namespace MedLinkDoctorApp.Services;

internal class FileService
{
    internal static async Task<string> UploadFile(byte[] fileData, string token)
    {
        using (HttpClient httpClient = new HttpClient())
        {
            httpClient.BaseAddress = new Uri(MedLinkConstants.SERVER_ROOT_URL);
            var mediaFile = new MediaFile()
            {
                FileId = 1,
                FileName = "uploaded_image_" + DateTime.UtcNow.ToString("ddMMM_hhmmss") + ".jpg",
                FileBytes = fileData,
                FilePath = "images"
            };

            var content = new StringContent(JsonConvert.SerializeObject(mediaFile), Encoding.UTF8, "application/json");
            httpClient.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));
            httpClient.DefaultRequestHeaders.Add("Authorization", "Bearer " + token);

            try
            {
                var response = await httpClient.PostAsync("api/Media/UploadFile", content);
                return mediaFile.FilePath + "/" + mediaFile.FileName;
            }
            catch (Exception ex)
            {
                return null;
            }
        }
    }
}
