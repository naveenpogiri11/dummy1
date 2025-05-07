namespace CareSync.Models.ViewModels
{
    public class BookAppointmentViewModel
    {
        public List<string> Problems { get; set; }       // Problem/Specialty list (for dropdown)
        public List<Doctor> SuggestedDoctors { get; set; } // Doctors filtered by problem/city
        public Appointment Appointment { get; set; }     // Appointment details being booked
        public Patient Patient { get; set; }              // New patient booking the appointment
    }
}