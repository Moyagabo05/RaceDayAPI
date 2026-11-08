using Microsoft.AspNetCore.Mvc;
using RaceDayWeb.Services;
using RaceDayWeb.ViewModels;
using System.Text.Json;

namespace RaceDayWeb.Controllers
{
    public class AuthController : Controller
    {
        private readonly ApiService _apiService;

        public AuthController(ApiService apiService)
        {
            _apiService = apiService;
        }

        [HttpGet]
        public IActionResult Login() => View();

        [HttpPost]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (!ModelState.IsValid) return View(model);

            var response = await _apiService.PostAsync("/api/Auth/login", model);

            if (response.IsSuccessStatusCode)
            {
                var json = await response.Content.ReadAsStringAsync();
                using var doc = JsonDocument.Parse(json);

                var userId = doc.RootElement.GetProperty("userId").GetInt32();
                var role = doc.RootElement.GetProperty("role").GetString();

                // Save to MVC Session (The ApiService will read these and send them as headers!)
                HttpContext.Session.SetInt32("UserId", userId);
                HttpContext.Session.SetString("UserRole", role);

                return RedirectToAction("Index", "Home");
            }

            ModelState.AddModelError("", "Invalid email or password.");
            return View(model);
        }

        [HttpGet]
        public IActionResult Register() => View();

        [HttpPost] 
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            if (!ModelState.IsValid) return View(model);

            var registerData = new
            {
                email = model.Email,
                password = model.Password,
                role = model.Role,
                firstName = model.FirstName,
                lastName = model.LastName,
                dateOfBirth = model.DateOfBirth,
                gender = model.Gender,
                emergencyContact = model.EmergencyContact,
                clubName = model.ClubName
            };

            var response = await _apiService.PostAsync("/api/Auth/register", registerData);

            if (response.IsSuccessStatusCode)
            {
                return RedirectToAction("Login");
            }

            ModelState.AddModelError("", "Registration failed. Please check your details and try again.");
            return View(model);
        }

        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Index", "Home");
        }
    }
}