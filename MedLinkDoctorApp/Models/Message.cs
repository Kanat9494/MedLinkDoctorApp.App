namespace MedLinkDoctorApp.Models;

internal class Message
{
    public string SenderName { get; set; }
    public string ReceiverName { get; set; }
    public string Content { get; set; }
    public string AudioUrl { get; set; }
    public string ImageUrl { get; set; }
}
