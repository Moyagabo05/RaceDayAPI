using Microsoft.AspNetCore.Mvc;
using RaceDayWeb.Services;
using RaceDayWeb.ViewModels;

namespace RaceDayWeb.Controllers;

[Route("Profile")]
public class ProfileController : Controller
{
    private readonly ApiService _apiService;
    public ProfileController(ApiService apiService) => _apiService = apiService;

    [HttpGet("")]
    public async Task<IActionResult> Index()
    {
        if (HttpContext.Session.GetInt32("UserId") == null)
            return RedirectToAction("Login", "Auth");

        var profile = await _apiService.GetAsync<ProfileViewModel>("/api/Profile");
        return profile == null ? NotFound() : View(profile);
    }

    [HttpGet("Edit")]
    public async Task<IActionResult> Edit()
    {
        if (HttpContext.Session.GetInt32("UserId") == null) return RedirectToAction("Login", "Auth");
        var profile = await _apiService.GetAsync<ProfileViewModel>("/api/Profile");
        return profile == null ? NotFound() : View(profile);
    }

    [HttpPost("Edit")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(ProfileViewModel model)
    {
        if (!ModelState.IsValid) return View(model);

        if (model.ProfilePicture is { Length: > 0 })
        {
            var upload = await _apiService.UploadFileAsync("/api/Upload/banner", model.ProfilePicture);
            if (!upload.IsSuccessStatusCode)
            {
                var uploadError = await upload.Content.ReadAsStringAsync();
                ModelState.AddModelError(nameof(model.ProfilePicture),
                    $"The profile picture could not be uploaded ({(int)upload.StatusCode}). {uploadError}");
                return View(model);
            }

            var uploadJson = await upload.Content.ReadAsStringAsync();
            using var uploadDocument = System.Text.Json.JsonDocument.Parse(uploadJson);
            model.ProfilePictureUrl = uploadDocument.RootElement.GetProperty("url").GetString();
        }

        var profileData = new
        {
            firstName = model.FirstName,
            lastName = model.LastName,
            emergencyContact = model.EmergencyContact,
            clubName = model.ClubName,
            organisationName = model.OrganisationName,
            phoneNumber = model.PhoneNumber
            ,profilePictureUrl = model.ProfilePictureUrl
        };
        var response = await _apiService.PutAsync("/api/Profile", profileData);
        if (!response.IsSuccessStatusCode)
        {
            ModelState.AddModelError("", "Unable to update profile.");
            return View(model);
        }

        TempData["SuccessMessage"] = "Profile updated successfully.";
        return RedirectToAction(nameof(Index));
    }
}
