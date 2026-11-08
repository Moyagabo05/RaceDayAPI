using Microsoft.AspNetCore.Mvc;
using RaceDayWeb.Services;
using RaceDayWeb.ViewModels;

namespace RaceDayWeb.Controllers;

[Route("Results")]
public class ResultsController : Controller
{
    private readonly ApiService _apiService;

    public ResultsController(ApiService apiService) => _apiService = apiService;

    [HttpGet("MyResults")]
    public async Task<IActionResult> MyResults()
    {
        var results = await _apiService.GetAsync<List<MyResultViewModel>>("/api/Results/my-results");
        return View(results ?? new List<MyResultViewModel>());
    }

    [HttpGet("Capture/{eventId}")]
    public async Task<IActionResult> Capture(int eventId)
    {
        var enrolments = await _apiService.GetAsync<List<CaptureResultViewModel>>($"/api/Results/events/{eventId}/enrolments");
        ViewBag.EventId = eventId;
        return View(enrolments ?? new List<CaptureResultViewModel>());
    }

    [HttpPost("Capture")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Capture(int eventId, List<CaptureResultViewModel> results)
    {
        var response = await _apiService.PostAsync($"/api/Results/events/{eventId}/capture", results);
        if (response.IsSuccessStatusCode)
            TempData["SuccessMessage"] = "Results captured successfully!";
        else
        {
            var apiError = await response.Content.ReadAsStringAsync();
            TempData["ErrorMessage"] = $"Failed to capture results ({(int)response.StatusCode}). {apiError}";
        }
        return RedirectToAction(nameof(Capture), new { eventId });
    }
}
