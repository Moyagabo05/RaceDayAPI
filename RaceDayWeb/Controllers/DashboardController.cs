using Microsoft.AspNetCore.Mvc;
using RaceDayWeb.Services;
using RaceDayWeb.ViewModels;

namespace RaceDayWeb.Controllers;

[Route("Dashboard")]
public class DashboardController : Controller
{
    private readonly ApiService _apiService;
    public DashboardController(ApiService apiService) => _apiService = apiService;

    [HttpGet("")]
    public async Task<IActionResult> Index()
    {
        if (HttpContext.Session.GetString("UserRole") != "Organiser") return Forbid();
        var dashboard = await _apiService.GetAsync<DashboardViewModel>("/api/Events/dashboard");
        return dashboard == null ? Problem("The dashboard could not be loaded from the API.") : View(dashboard);
    }
}
