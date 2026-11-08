using Microsoft.AspNetCore.Mvc;
using RaceDayWeb.Services;
using RaceDayWeb.ViewModels;
using System.Text.Json;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace RaceDayWeb.Controllers
{
    [Route("Events")]
    public class EventsController : Controller
    {
        private readonly ApiService _apiService;

        public EventsController(ApiService apiService)
        {
            _apiService = apiService;
        }

        // GET: /Events
        [HttpGet("")]
        public async Task<IActionResult> Index()
        {
            var events = await _apiService.GetAsync<List<EventDisplayViewModel>>("/api/Events");
            return View(events ?? new List<EventDisplayViewModel>());
        }

        // GET: /Events/Details/5
        [HttpGet("Details/{id:int}")]
        public async Task<IActionResult> Details(int id)
        {
            var evt = await _apiService.GetAsync<EventDetailsViewModel>($"/api/Events/{id}");
            if (evt == null) return NotFound();

            var categories = await _apiService.GetAsync<List<CategoryViewModel>>($"/api/events/{id}/categories");
            if (categories != null) evt.Categories = categories;

            return View(evt);
        }

        [HttpPost("Enrol/{eventId}")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Enrol(int eventId, int categoryId)
        {
            if (HttpContext.Session.GetString("UserRole") != "Participant") return Forbid();

            var response = await _apiService.PostAsync($"/api/Enrolments/events/{eventId}/enrol", new { categoryId });
            if (response.IsSuccessStatusCode)
            {
                TempData["SuccessMessage"] = "You have been enrolled successfully.";
                return RedirectToAction(nameof(Details), new { id = eventId });
            }

            var apiError = await response.Content.ReadAsStringAsync();
            TempData["ErrorMessage"] = response.StatusCode == System.Net.HttpStatusCode.Conflict
                ? "You are already enrolled in this event."
                : $"Unable to enrol in this event ({(int)response.StatusCode}). {apiError}";
            return RedirectToAction(nameof(Details), new { id = eventId });
        }

        // GET: /Events/Create
        [HttpGet("Create")]
        public IActionResult Create()
        {
            return View();
        }

        // POST: /Events/Create
        [HttpPost("Create")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(EventCreateViewModel model)
        {
            if (!ModelState.IsValid) return View(model);

            string? bannerUrl = null;

            if (model.BannerImage != null && model.BannerImage.Length > 0)
            {
                var uploadResponse = await _apiService.UploadFileAsync("/api/Upload/banner", model.BannerImage);

                if (uploadResponse.IsSuccessStatusCode)
                {
                    var json = await uploadResponse.Content.ReadAsStringAsync();
                    bannerUrl = JsonDocument.Parse(json).RootElement.GetProperty("url").GetString();
                }
                else
                {
                    ModelState.AddModelError("BannerImage", "Failed to upload image to Azure.");
                    return View(model);
                }
            }

            var eventData = new
            {
                name = model.Name,
                description = model.Description,
                eventDate = model.EventDate,
                location = model.Location,
                distance = model.Distance,
                eventType = model.EventType,
                bannerImageUrl = bannerUrl
            };

            var response = await _apiService.PostAsync("/api/Events", eventData);

            if (response.IsSuccessStatusCode)
            {
                return RedirectToAction(nameof(Index));
            }

            ModelState.AddModelError("", "Failed to create event.");
            return View(model);
        }

        // GET: /Events/Edit/5
        [HttpGet("Edit/{id:int}")]
        public async Task<IActionResult> Edit(int id)
        {
            ViewBag.EventId = id;
            var evt = await _apiService.GetAsync<EventDisplayViewModel>($"/api/Events/{id}");
            if (evt == null) return NotFound();

            var model = new EventCreateViewModel
            {
                Name = evt.Name,
                Description = evt.Description,
                EventDate = evt.EventDate,
                Location = evt.Location,
                Distance = evt.Distance,
                EventType = evt.EventType,
                BannerImageUrl = evt.BannerImageUrl
            };
            return View(model);
        }

        // POST: /Events/Edit/5
        [HttpPost("Edit/{id}")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, EventCreateViewModel model)
        {
            ViewBag.EventId = id;
            if (!ModelState.IsValid) return View(model);

            string? bannerUrl = null;

            if (model.BannerImage != null && model.BannerImage.Length > 0)
            {
                var uploadResponse = await _apiService.UploadFileAsync("/api/Upload/banner", model.BannerImage);

                if (uploadResponse.IsSuccessStatusCode)
                {
                    var json = await uploadResponse.Content.ReadAsStringAsync();
                    bannerUrl = JsonDocument.Parse(json).RootElement.GetProperty("url").GetString();
                }
                else
                {
                    var uploadError = await uploadResponse.Content.ReadAsStringAsync();
                    ModelState.AddModelError(nameof(model.BannerImage),
                        $"The image upload failed ({(int)uploadResponse.StatusCode}). {uploadError}");
                    return View(model);
                }
            }

            var eventData = new
            {
                name = model.Name,
                description = model.Description,
                eventDate = model.EventDate,
                location = model.Location,
                distance = model.Distance,
                eventType = model.EventType,
                bannerImageUrl = bannerUrl ?? model.BannerImageUrl
            };

            var response = await _apiService.PutAsync($"/api/Events/{id}", eventData);

            if (response.IsSuccessStatusCode)
            {
                return RedirectToAction(nameof(Index));
            }

            ModelState.AddModelError("", "Failed to update event.");
            return View(model);
        }

        // POST: /Events/Delete/5
        [HttpPost("Delete/{id:int}")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            var response = await _apiService.DeleteAsync($"/api/Events/{id}");

            if (response.IsSuccessStatusCode)
            {
                return RedirectToAction(nameof(Index));
            }

            var apiError = await response.Content.ReadAsStringAsync();
            TempData["ErrorMessage"] =
                $"Failed to delete event ({(int)response.StatusCode}). {apiError}";
            return RedirectToAction(nameof(Index));
        }
    }
}