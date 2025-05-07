using Microsoft.AspNetCore.Mvc;
using CareSync.Models;
using CareSync.Models.ViewModels;
using CareSync.Models.DataAccess;   // Important: for DbContext
using System.Linq;
using CareSync.Services;

namespace CareSync.Controllers
{
    public class AppointmentController : Controller
    {
        private readonly CareSyncDbContext _context;
        private readonly DoctorService _doctorService;

        


        public AppointmentController(CareSyncDbContext context, DoctorService doctorService)
        {
            _context = context;
            _doctorService = doctorService;


        }

        private string GenerateRandomId(int length)
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            var random = new Random();
            return new string(Enumerable.Repeat(chars, length).Select(s => s[random.Next(s.Length)]).ToArray());
        }

        // READ: Show all appointments
        public IActionResult Index()
        {
            var appointments = _context.Appointments.Join(_context.Patients, a => a.PatientID, p => p.PatientID, (a, p) => new { a, p })
        .Join(_context.Doctors, ap => ap.a.DoctorID, d => d.DoctorID, (ap, d) => new AppointmentDisplayViewModel{
            AppointmentID = ap.a.AppointmentID,
            PatientName = ap.p.Name,
            DoctorName = d.FirstName,
            Date = ap.a.AppointmentDate,
            Time = ap.a.AppointmentTime
        }).ToList();
            return View(appointments);
        }

        // CREATE: Show Create Form
        public IActionResult Create()
        {
            var doctors = _doctorService.GetAllDoctors(); // fetch from in-memory API-loaded doctors

            var viewModel = new BookAppointmentViewModel
            {
                Problems = doctors.Select(d => d.PrimarySpecialty)
                                  .Where(s => !string.IsNullOrEmpty(s))
                                  .Distinct()
                                  .ToList(),

                SuggestedDoctors = doctors,

                Appointment = new Appointment(),
                Patient = new Patient()
            };

            return View(viewModel);
        } 

        // CREATE: Save New Appointment
        [HttpPost]
        public IActionResult Create(BookAppointmentViewModel model)
        {

            // Check if doctor already exists
            // Step 1: Get doctor info from DoctorService in memory
            var matchingDoctor = _doctorService.GetAllDoctors()
                                  .FirstOrDefault(d => d.DoctorID == model.Appointment.DoctorID);

            if (matchingDoctor == null)
            {
                ModelState.AddModelError("", "No doctor available for the selected specialty.");
                return View(model);  // stay on same page
            }

            // Step 2: Check if doctor already exists in DB
            var existingDoctor = _context.Doctors.FirstOrDefault(d => d.DoctorID == matchingDoctor.DoctorID);

            if (existingDoctor == null)
            {
                // Step 3: If not exists, insert into database
                var doctorEntity = new Doctor
                {
                    DoctorID = matchingDoctor.DoctorID,
                    FirstName = matchingDoctor.FirstName,
                    Gender = matchingDoctor.Gender,
                    MedicalDegree = matchingDoctor.MedicalDegree,
                    PrimarySpecialty = matchingDoctor.PrimarySpecialty,
                    FacilityName = matchingDoctor.FacilityName,
                    City = matchingDoctor.City,
                    State = matchingDoctor.State,
                    Address = matchingDoctor.Address
                };

                _context.Doctors.Add(doctorEntity);
                _context.SaveChanges();
            }

            // Check if patient already exists

            var newPatient = new Patient
            {
                PatientID = GenerateRandomId(6),
                Name = model.Patient.Name,
                ContactNumber = model.Patient.ContactNumber
            };

            // Insert Patient FIRST
            _context.Patients.Add(newPatient);
            _context.SaveChanges(); // Save Patient first

            var newAppointment = new Appointment
            {
                AppointmentID = GenerateRandomId(6),
                PatientID = newPatient.PatientID,
                DoctorID = model.Appointment.DoctorID,
                AppointmentDate = model.Appointment.AppointmentDate,
                AppointmentTime = model.Appointment.AppointmentTime
            };

            
            _context.Appointments.Add(newAppointment);
            _context.SaveChanges();

            ViewBag.Message = "Appointment Booked Successfully!";

            return RedirectToAction("Index");
        }

        // UPDATE: Show Edit Form
        public IActionResult Edit(string id)
        {
            var appt = _context.Appointments.FirstOrDefault(a => a.AppointmentID == id);
            return View(appt);
        }

        // UPDATE: Save Edited Appointment
        [HttpPost]
        public IActionResult Edit(Appointment updated)
        {
            var appt = _context.Appointments.FirstOrDefault(a => a.AppointmentID == updated.AppointmentID);
            if (appt != null)
            {
                appt.DoctorID = updated.DoctorID;
                appt.AppointmentDate = updated.AppointmentDate;
                appt.AppointmentTime = updated.AppointmentTime;
                _context.SaveChanges();
            }
            return RedirectToAction("Index");
        }

        // DELETE: Show Delete Confirmation
        public IActionResult Delete(string id)
        {
            var appt = _context.Appointments.FirstOrDefault(a => a.AppointmentID == id);
            return View(appt);
        }

        // DELETE: Confirm Delete
        [HttpPost, ActionName("Delete")]
        public IActionResult DeleteConfirmed(string id)
        {
            var appt = _context.Appointments.FirstOrDefault(a => a.AppointmentID == id);
            if (appt != null)
            {
                _context.Appointments.Remove(appt);
                _context.SaveChanges();
            }
            return RedirectToAction("Index");
        }

        // DETAILS: View Single Appointment Details
        public IActionResult Details(string id)
        {
            var appt = _context.Appointments.FirstOrDefault(a => a.AppointmentID == id);
            return View(appt);
        }
    }
}
