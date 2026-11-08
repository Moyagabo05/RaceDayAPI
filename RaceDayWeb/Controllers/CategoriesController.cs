using Microsoft.AspNetCore.Mvc;
using RaceDayWeb.Services;
using RaceDayWeb.ViewModels;

namespace RaceDayWeb.Controllers;

[Route("Categories")]
public class CategoriesController : Controller
{
    private readonly ApiService _apiService;

    public CategoriesController(ApiService apiService)
    {
        _apiService = apiService;
    }

    [HttpGet("")]
    public async Task<IActionResult> Index(int eventId)
    {
        if (HttpContext.Session.GetString("UserRole") != "Organiser")
            return Forbid();

        var categories = await _apiService.GetAsync<List<CategoryViewModel>>(
            $"/api/events/{eventId}/categories");

        ViewBag.EventId = eventId;
        return View(categories ?? new List<CategoryViewModel>());
    }

    [HttpGet("Create/{eventId}")]
    public IActionResult Create(int eventId)
    {
        if (HttpContext.Session.GetString("UserRole") != "Organiser")
            return Forbid();

        return View(new CategoryEditViewModel { EventId = eventId });
    }

    [HttpPost("Create/{eventId}")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(int eventId, CategoryEditViewModel model)
    {
        if (HttpContext.Session.GetString("UserRole") != "Organiser")
            return Forbid();

        model.EventId = eventId;
        if (!ModelState.IsValid)
            return View(model);

        var categoryData = new
        {
            categoryName = model.CategoryName,
            minAge = model.MinAge,
            maxAge = model.MaxAge
        };

        var response = await _apiService.PostAsync(
            $"/api/events/{eventId}/categories", categoryData);

        if (response.IsSuccessStatusCode)
        {
            TempData["SuccessMessage"] = "Category added successfully.";
            return RedirectToAction(nameof(Index), new { eventId });
        }

        var apiError = await response.Content.ReadAsStringAsync();
        ModelState.AddModelError(string.Empty,
            $"Unable to add category ({(int)response.StatusCode}). {apiError}");
        return View(model);
    }
}
