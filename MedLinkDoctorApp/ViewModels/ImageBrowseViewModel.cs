namespace MedLinkDoctorApp.ViewModels;

[QueryProperty(nameof(ImageUrl), "ImageUrl")]
internal class ImageBrowseViewModel : BaseViewModel
{
    public ImageBrowseViewModel()
    {

    }

    private string _imageUrl;
    public string ImageUrl
    {
        get => _imageUrl;
        set => SetProperty(ref _imageUrl, value);
    }
}
