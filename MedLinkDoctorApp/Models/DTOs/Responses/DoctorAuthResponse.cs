namespace MedLinkDoctorApp.Models.DTOs.Responses;

internal class DoctorAuthResponse : BaseResponse
{
    public int DoctorId { get; set; }
    public string AccountName { get; set; }
    public string? FullName { get; set; }
    public string AccessToken { get; set; }
    public double DoctorBalance { get; set; }
    public string? ProfileImg { get; set; }
    public string? Specialization { get; set; }
}
