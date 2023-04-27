namespace MedLinkDoctorApp.Models;

internal class Offer : BaseResponse
{
    public int OfferId { get; set; }    
    public string SenderName { get; set; }
    public string ReceiverName { get; set; }
    public double ProductPrice { get; set; }
}
