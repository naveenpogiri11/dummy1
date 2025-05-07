using CareSync.Models;

namespace CareSync.Services
{
    public class DoctorService
    {
        private readonly List<Doctor> DoctorList = new List<Doctor>();
        private bool isDataFetched = false;

        public bool IsDataFetched() => isDataFetched;

        public void MarkDataAsFetched() => isDataFetched = true;

        public void AddDoctors(List<Doctor> doctors)
        {
            DoctorList.AddRange(doctors);
        }

        public void AddDoctor(Doctor doctor)
        {
            DoctorList.Add(doctor);
        }

        public List<Doctor> GetAllDoctors()
        {
            return DoctorList;
        }

        public Doctor GetDoctorById(string id)
        {
            return DoctorList.FirstOrDefault(d => d.DoctorID == id);
        }

        public List<string> GetUniqueSpecialties()
        {
            return DoctorList.Select(d => d.PrimarySpecialty)
                              .Where(spec => !string.IsNullOrEmpty(spec))
                              .Distinct()
                              .ToList();
        }

        public List<Doctor> GetDoctorsBySpecialty(string specialty)
        {
            return DoctorList.Where(d => d.PrimarySpecialty == specialty).ToList();
        }

        public List<Doctor> GetDoctorsByCity(string city)
        {
            return DoctorList.Where(d => d.City == city).ToList();
        }
    }
}
