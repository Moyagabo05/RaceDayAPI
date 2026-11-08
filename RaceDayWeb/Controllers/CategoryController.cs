using Microsoft.AspNetCore.Mvc;
using RaceDayWeb.Services;
using RaceDayWeb.ViewModels;

namespace RaceDayWeb.Controllers;

[Route("Category")]
public class CategoryController : Controller
{
    private readonly ApiService _apiService;
    public CategoryController(ApiService apiService) => _apiService = apiService;

    [HttpGet("{eventId}")]
    public async Task<IActionResult> Index(int eventId)
    {
        var categories = await _apiService.GetAsync<List<CategoryViewModel>>($"/api/events/{eventId}/categories") ?? [];
        ViewBag.EventId = eventId;
        return View(categories);
    }

    [HttpGet("Create/{eventId}")]
    public IActionResult Create(int eventId) => View(new CategoryEditViewModel { EventId = eventId });

    [HttpPost("Create/{eventId}")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(int eventId, CategoryEditViewModel model)
    {
        if (!ModelState.IsValid) return View(model);
        var response = await _apiService.PostAsync($"/api/events/{eventId}/categories", model);
        if (response.IsSuccessStatusCode) { TempData["SuccessMessage"] = "Category added."; return RedirectToAction(nameof(Index), new { eventId }); }
        TempData["ErrorMessage"] = "Unable to add category.";
        return View(model);
    }

    [HttpPost("Delete/{eventId}/{categoryId}")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(int eventId, int categoryId)
    {
        var response = await _apiService.DeleteAsync($"/api/events/{eventId}/categories/{categoryId}");
        TempData[response.IsSuccessStatusCode ? "SuccessMessage" : "ErrorMessage"] = response.IsSuccessStatusCode ? "Category deleted." : "Unable to delete category.";
        return RedirectToAction(nameof(Index), new { eventId });
    }
}
