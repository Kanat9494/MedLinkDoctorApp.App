namespace MedLinkDoctorApp.Models;

internal class Message : BaseResponse
{
    public int MessageId { get; set; }
    public string SenderName { get; set; }
    public string ReceiverName { get; set; }
    public string? Content { get; set; }
    public string AudioUrl { get; set; }
    public string ImageUrl { get; set; }
    public byte IsRead { get; set; }
    public byte[] ImageData { get; set; }
}
