using Microsoft.AspNetCore.Mvc;
using CareSync.Models;
using CareSync.Models.ViewModels;
using CareSync.Services;
using Newtonsoft.Json;
using CareSync.Models.DataAccess;
using Microsoft.EntityFrameworkCore;

namespace CareSync.Controllers
{
    public class HomeController : Controller
    {
        private readonly CareSyncDbContext _context;
        private readonly ILogger<HomeController> _logger;
        private readonly DoctorService _doctorService;
        private readonly HttpClient _httpClient;
        private readonly string BASE_URL = "https://data.cms.gov/provider-data/api/1/datastore/query/mj5m-pzi6/0?limit=500&offset=0&count=true&results=true&schema=true&keys=true&format=json&rowIds=false";

        public HomeController(ILogger<HomeController> logger, DoctorService doctorService, CareSyncDbContext context)
        {
            _logger = logger;
            _doctorService = doctorService;
            _context = context;
            _httpClient = new HttpClient();
        }

        public async Task<IActionResult> Index()
        {
            if (!_doctorService.IsDataFetched())
            {
                HttpResponseMessage response = await _httpClient.GetAsync(BASE_URL);
                if (response.IsSuccessStatusCode)
                {
                    string jsonData = await response.Content.ReadAsStringAsync();
                    var rawData = JsonConvert.DeserializeObject<DoctorResultWrapper>(jsonData);

                    if (rawData?.results != null)
                    {
                        List<Doctor> doctors = rawData.results.Select(raw => new Doctor
                        {
                            DoctorID = raw.ind_enrl_id,
                            FirstName = raw.provider_first_name,
                            Gender = raw.gndr,
                            MedicalDegree = raw.med_sch,
                            PrimarySpecialty = raw.pri_spec,
                            FacilityName = raw.facility_name,
                            City = raw.citytown,
                            State = raw.state,
                            Address = raw.adr_ln_1
                        }).ToList();

                        _doctorService.AddDoctors(doctors);
                        _doctorService.MarkDataAsFetched();
                    }
                }
            }

            return View();
        }

        public IActionResult Chart()
        {

            var doctorAppointments = _context.Appointments
         .Join(_context.Doctors,
               appt => appt.DoctorID,
               doc => doc.DoctorID,
               (appt, doc) => new { doc.FirstName, appt.AppointmentID })
         .GroupBy(x => x.FirstName)
         .Select(g => new { DoctorName = g.Key, Count = g.Count() })
         .ToList();

            ViewBag.BarLabels = string.Join(",", doctorAppointments.Select(x => $"'{x.DoctorName}'"));
            ViewBag.BarData = string.Join(",", doctorAppointments.Select(x => x.Count));

            // Line Chart: Appointments per Month
            var monthlyAppointments = _context.Appointments
                .GroupBy(a => a.AppointmentDate.Month)
                .Select(g => new { Month = g.Key, Count = g.Count() })
                .OrderBy(g => g.Month)
                .ToList();

            ViewBag.LineLabels = string.Join(",", monthlyAppointments.Select(x => $"'{System.Globalization.CultureInfo.CurrentCulture.DateTimeFormat.GetMonthName(x.Month)}'"));
            ViewBag.LineData = string.Join(",", monthlyAppointments.Select(x => x.Count));

            // Pie Chart: Appointments by Specialty
            var specialtyAppointments = _context.Appointments
                .Join(_context.Doctors,
                      a => a.DoctorID,
                      d => d.DoctorID,
                      (a, d) => new { Specialty = d.PrimarySpecialty })
                .GroupBy(x => x.Specialty)
                .Select(g => new { Specialty = g.Key, Count = g.Count() })
                .ToList();

            ViewBag.PieLabels = string.Join(",", specialtyAppointments.Select(x => $"'{x.Specialty}'"));
            ViewBag.PieData = string.Join(",", specialtyAppointments.Select(x => x.Count));

            ViewBag.PageTitle = "Appointment Insights Dashboard";

            return View();
        }

        public IActionResult About()
        {
            return View();
        }

        public IActionResult Contact()
        {
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }
    }

    public class DoctorResultWrapper
    {
        public List<DoctorRaw> results { get; set; }
    }

    public class DoctorRaw
    {
        public string ind_enrl_id { get; set; }
        public string provider_first_name { get; set; }
        public string gndr { get; set; }
        public string med_sch { get; set; }
        public string pri_spec { get; set; }
        public string facility_name { get; set; }
        public string citytown { get; set; }
        public string state { get; set; }
        public string adr_ln_1 { get; set; }
    }
}
