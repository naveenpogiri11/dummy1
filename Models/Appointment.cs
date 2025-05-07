namespace CareSync.Models
{
    public class Appointment
    {
        public string AppointmentID { get; set; }
        public string PatientID { get; set; }
        public string DoctorID { get; set; }
        public DateTime AppointmentDate { get; set; }
        public string AppointmentTime { get; set; }
    }
}
