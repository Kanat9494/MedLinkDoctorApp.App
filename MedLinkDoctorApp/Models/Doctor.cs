namespace MedLinkDoctorApp.Models;

internal class Doctor
{
    public int DoctorId { get; set; }
    public string AccountName { get; set; }
    public string Password { get; set; }
    public string FullName { get; set; }
    public int WorkExperience { get; set; }
    public double DoctorBalance { get; set; }
    public string AboutDoctor { get; set; }
    public string ProfileImg { get; set; }
    public byte IsBusy { get; set; }
    public byte IsOnline { get; set; }
    public string Specialization { get; set; }
}
