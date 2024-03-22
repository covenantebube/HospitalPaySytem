using HospitalPaymentSystem.Core.Abstractions;
using HospitalPaymentSystem.Core.Dtos;
using HospitalPaymentSystem.Core.Utilities;
using HospitalPaymentSystem.Web.Models;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace HospitalPaymentSystem.Web.Controllers
{
    public class PatientController : Controller
    {
        private readonly IPatientServices _patientServices;
        private readonly IPaymentService _paymentService;
        private readonly ISearchService _searchService;

        public PatientController(IPatientServices patientServices, IPaymentService paymentService, ISearchService searchService)
        {
            _patientServices = patientServices;
            _paymentService = paymentService;
            _searchService = searchService;
        }

        public IActionResult RegisterPatient()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> RegisterPatient(PatientDTO patientDTO)
        {
            ModelState.Remove(nameof(patientDTO.Payments));
            if (ModelState.IsValid)
            {
                try
                {
                    await _patientServices.RegisterPatientAsync(patientDTO);
                    TempData["success"] = "Patient registered successfully!";
                    return RedirectToAction("Index");
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError(string.Empty, ex.Message);
                }
            }
            return View(patientDTO);
        }

        public IActionResult PostPayment()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> PostPayment(PaymentDTO paymentDTO)
        {
            if (ModelState.IsValid)
            {
                if (await _paymentService.AddPaymentAsync(paymentDTO))
                {
                    TempData["success"] = "Payment added successfully!";
                    return RedirectToAction("Index");
                }
                else
                {
                    ModelState.AddModelError("PatientID", "The provided Patient ID does not exist. Please make sure to select a valid patient.");
                }
            }
            return View(paymentDTO);
        }

        public async Task<IActionResult> Index(int pageNumber = 1, int pageSize = 10)
        {
            var paginationFilter = new PaginationFilter(pageNumber, pageSize);
            var patients = await _patientServices.GetAllPatientsAsync(paginationFilter);
            return View(patients);
        }

        [HttpPost]
        public async Task<IActionResult> SearchPatients(string searchTerm, int pageNumber = 1, int pageSize = 10)
        {
            if (string.IsNullOrWhiteSpace(searchTerm))
            {
                TempData["error"] = "Search term cannot be empty.";
                return RedirectToAction("Index");
            }

            var paginationFilter = new PaginationFilter(pageNumber, pageSize);
            var patients = await _searchService.SearchPatientAsync(searchTerm, paginationFilter);
            return View(patients);
        }

        public async Task<IActionResult> PatientDetails(int id)
        {
            var patient = await _patientServices.GetPatientByIdAsync(id);
            if (patient == null)
            {
                TempData["error"] = "Patient not found.";
                return RedirectToAction("Index");
            }
            return View(patient);
        }
    }
}
