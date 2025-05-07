namespace CareSync.Models
{
    public class Doctor
    {
        public string DoctorID { get; set; }           // From "ind_enrl_id"
        public string FirstName { get; set; }          // From "provider_first_name"
        public string Gender { get; set; }             // From "gndr"
        public string MedicalDegree { get; set; }      // From "med_sch"
        public string PrimarySpecialty { get; set; }   // From "pri_spec"
        public string FacilityName { get; set; }       // From "facility_name"
        public string City { get; set; }               // From "citytown"
        public string State { get; set; }              // From "state"
        public string Address { get; set; }            // From "adr_ln_1"
    }
}