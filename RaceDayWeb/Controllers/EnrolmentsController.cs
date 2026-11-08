using Microsoft.AspNetCore.Mvc;
using RaceDayWeb.Services;
using RaceDayWeb.ViewModels;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace RaceDayWeb.Controllers
{
    [Route("Enrolments")]
    public class EnrolmentsController : Controller
    {
        private readonly ApiService _apiService;

        public EnrolmentsController(ApiService apiService)
        {
            _apiService = apiService;
        }

        // GET: /Enrolments/MyEnrolments
        [HttpGet("MyEnrolments")]
        public async Task<IActionResult> MyEnrolments()
        {
            var enrolments = await _apiService.GetAsync<List<MyEnrolmentViewModel>>("/api/Enrolments/my-enrolments");
            return View(enrolments ?? new List<MyEnrolmentViewModel>());
        }

        [HttpGet("Details/{eventId}")]
        public async Task<IActionResult> Details(int eventId)
        {
            var enrolments = await _apiService.GetAsync<List<EnrolmentDisplayViewModel>>($"/api/Enrolments/events/{eventId}/enrolments");
            if (enrolments == null) return Forbid();
            ViewBag.EventId = eventId;
            return View(enrolments);
        }
    }
}
