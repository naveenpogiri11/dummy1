namespace CareSync.Models.ViewModels
{
    public class AppointmentDisplayViewModel
    {
        public string AppointmentID { get; set; }
        public string PatientName { get; set; }
        public string DoctorName { get; set; }
        public DateTime Date { get; set; }
        public string Time { get; set; }
    }
}
