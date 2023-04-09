namespace MedLinkDoctorApp.Helpers;

internal class FileHelper
{
    internal static byte[] StreamTyByte(Stream input)
    {
        using (MemoryStream ms = new MemoryStream())
        {
            input.CopyTo(ms);
            return ms.ToArray();
        }
    }

    internal static byte[] ImageToByteArray(string imagefilePath)
    {
        byte[] imageArray = File.ReadAllBytes(imagefilePath);
        return imageArray;
    }

    internal static async Task<byte[]> DownloadImageBytesAsync(string imageUrl)
    {
        using (var httpClient = new HttpClient())
        {
            try
            {
                var response = await httpClient.GetAsync(imageUrl);
                response.EnsureSuccessStatusCode();
                var contentStream = await response.Content.ReadAsStreamAsync();

                using (var memoryStream = new MemoryStream())
                {
                    await contentStream.CopyToAsync(memoryStream);

                    return memoryStream.ToArray();
                }
            }
            catch (Exception ex)
            {
                return null;
            }
        }
    }

    internal static async Task<string> SaveFileAsync(byte[] fileData)
    {
        //stream.Position = 0;
        //var memoryStream = new MemoryStream();
        //memoryStream.Position = 0;
        var fileName = "uploaded_image_" + DateTime.UtcNow.ToString("ddMMM_hhmmss") + ".jpeg";
        string filePath = "";

        var fileFullPath = Path.Combine(System.Environment.GetFolderPath(System.Environment.SpecialFolder.Personal), fileName);

        await File.WriteAllBytesAsync(fileFullPath, fileData);

        string mainDir = FileSystem.Current.AppDataDirectory;
        return fileFullPath;
        //#if ANDROID
        //        var context = Platform.CurrentActivity;
        //        Android.Content.ContentResolver contentResolver = context.ContentResolver;
        //        Android.Content.ContentValues contentValues = new();
        //        contentValues.Put(Android.Provider.MediaStore.IMediaColumns.DisplayName, fileName);
        //        contentValues.Put(Android.Provider.MediaStore.IMediaColumns.MimeType, "image/jpeg");
        //        contentValues.Put(Android.Provider.MediaStore.IMediaColumns.RelativePath, "DCIM/" + fileName);
        //        filePath = "DCIM/" + fileName;
        //        Android.Net.Uri imageUri = contentResolver.Insert(Android.Provider.MediaStore.Images.Media.ExternalContentUri, contentValues);
        //        var os = contentResolver.OpenOutputStream(imageUri);
        //        Android.Graphics.BitmapFactory.Options options = new();
        //        options.InJustDecodeBounds = true;
        //        var bitmap = Android.Graphics.BitmapFactory.DecodeStream(stream);
        //        bitmap.Compress(Android.Graphics.Bitmap.CompressFormat.Jpeg, 100, os);
        //        os.Flush();
        //        os.Close();
        //        return filePath;
        //#else
        //        return filePath;
        //#endif
    }
}
