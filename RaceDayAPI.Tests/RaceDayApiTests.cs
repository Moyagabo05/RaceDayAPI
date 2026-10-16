using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using RaceDayAPI.Controllers;
using RaceDayAPI.Data;
using RaceDayAPI.DTOs;
using RaceDayAPI.Helpers;
using RaceDayAPI.Models;
using Microsoft.AspNetCore.Routing;
using Xunit;

namespace RaceDayAPI.Tests
{
    // --- MOCK SESSION HELPER ---
    public class MockSession : ISession
    {
        private readonly Dictionary<string, byte[]> _store = new Dictionary<string, byte[]>();
        public string Id => "mock-session-id";
        public bool IsAvailable => true;
        public IEnumerable<string> Keys => _store.Keys;
        public void Clear() => _store.Clear();
        public Task CommitAsync(CancellationToken cancellationToken = default) => Task.CompletedTask;
        public Task LoadAsync(CancellationToken cancellationToken = default) => Task.CompletedTask;
        public void Remove(string key) => _store.Remove(key);
        public void Set(string key, byte[] value) => _store[key] = value;
        public bool TryGetValue(string key, out byte[] value) => _store.TryGetValue(key, out value);
    }

    public class RaceDayApiTests
    {
        // --- SHARED HELPERS ---
        private RaceDayDbContext CreateInMemoryContext()
        {
            var options = new DbContextOptionsBuilder<RaceDayDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                // FIX 1: Suppress the transaction warning for the In-Memory database
                .ConfigureWarnings(warnings => warnings.Ignore(InMemoryEventId.TransactionIgnoredWarning))
                .Options;
            var context = new RaceDayDbContext(options);
            context.Database.EnsureCreated();
            return context;
        }

        private AuthController CreateAuthController(RaceDayDbContext context)
        {
            var controller = new AuthController(context);
            var httpContext = new DefaultHttpContext { Session = new MockSession() };
            controller.ControllerContext = new ControllerContext { HttpContext = httpContext };
            return controller;
        }

        private EventsController CreateEventsController(RaceDayDbContext context, int? userId = 1, string role = "Organiser")
        {
            var controller = new EventsController(context);
            var httpContext = new DefaultHttpContext { Session = new MockSession() };
            if (userId.HasValue) httpContext.Session.SetInt32("UserId", userId.Value);
            if (role != null) httpContext.Session.SetString("UserRole", role);
            controller.ControllerContext = new ControllerContext { HttpContext = httpContext };
            return controller;
        }

        private EnrolmentsController CreateEnrolmentsController(RaceDayDbContext context, int? userId = 1, string role = "Participant")
        {
            var controller = new EnrolmentsController(context);
            var httpContext = new DefaultHttpContext { Session = new MockSession() };
            if (userId.HasValue) httpContext.Session.SetInt32("UserId", userId.Value);
            if (role != null) httpContext.Session.SetString("UserRole", role);
            controller.ControllerContext = new ControllerContext { HttpContext = httpContext };
            return controller;
        }

        // ==========================================
        // 1. AUTHENTICATION TESTS
        // ==========================================
        [Fact]
        public async Task Auth_Register_ShouldReturnOk_WhenValidParticipant()
        {
            using var context = CreateInMemoryContext();
            var controller = CreateAuthController(context);
            var dto = new RegisterDto
            {
                Email = "test@test.com",
                Password = "Password123!",
                Role = "Participant",
                FirstName = "John",
                LastName = "Doe",
                DateOfBirth = new DateTime(1990, 1, 1)
            };

            var result = await controller.Register(dto);
            Assert.IsType<OkObjectResult>(result);
        }

        [Fact]
        public async Task Auth_Login_ShouldReturnOk_WhenCredentialsAreCorrect()
        {
            using var context = CreateInMemoryContext();
            context.Users.Add(new User { UserID = 1, Email = "test@test.com", PasswordHash = BCrypt.Net.BCrypt.HashPassword("Password123!"), Role = "Organiser", CreatedAt = DateTime.Now });
            await context.SaveChangesAsync();

            var controller = CreateAuthController(context);
            var dto = new LoginDto { Email = "test@test.com", Password = "Password123!" };

            var result = await controller.Login(dto);
            Assert.IsType<OkObjectResult>(result);
        }

        // ==========================================
        // 2. EVENT MANAGEMENT TESTS
        // ==========================================
        [Fact]
        public async Task Events_GetAll_ShouldReturnOkResult_WithListOfEvents()
        {
            using var context = CreateInMemoryContext();
            context.Organisers.Add(new Organiser { OrganiserID = 1, UserID = 1, FirstName = "Test", LastName = "Org", PhoneNumber = "123" });
            context.Events.Add(new Event { OrganiserID = 1, Name = "Event 1", EventType = "Run", Distance = 5, Location = "Loc", Description = "Desc", EventDate = DateTime.Now.AddDays(10) });
            await context.SaveChangesAsync();

            var controller = CreateEventsController(context);
            var result = await controller.GetAll();

            var okResult = Assert.IsType<OkObjectResult>(result);
            var events = Assert.IsAssignableFrom<List<Event>>(okResult.Value);
            Assert.Single(events);
        }

        [Fact]
        public async Task Events_Create_ShouldReturnCreatedAtActionResult_WhenOrganiserIsLoggedIn()
        {
            using var context = CreateInMemoryContext();
            context.Organisers.Add(new Organiser { OrganiserID = 10, UserID = 1, FirstName = "Test", LastName = "Org", PhoneNumber = "123" });
            await context.SaveChangesAsync();

            var controller = CreateEventsController(context, userId: 1, role: "Organiser");
            var dto = new EventCreateDto { Name = "New Marathon", Description = "A new race", EventDate = DateTime.Now.AddDays(10), Location = "Cape Town", Distance = 42, EventType = "Run" };

            var result = await controller.Create(dto);

            Assert.IsType<CreatedAtActionResult>(result);
            var eventInDb = await context.Events.FirstOrDefaultAsync(e => e.Name == "New Marathon");
            Assert.NotNull(eventInDb);
        }

        // FIX 2: Test the RequireRoleAttribute directly to prove role rejection 
        // (Direct controller invocation bypasses Action Filters, so we test the filter itself)
        [Fact]
        public void RequireRoleAttribute_ShouldReturnForbid_WhenRoleDoesNotMatch()
        {
            // Arrange: Create the attribute requiring "Organiser"
            var attribute = new RequireRoleAttribute("Organiser");

            // Simulate a logged-in Participant
            var httpContext = new DefaultHttpContext();
            httpContext.Session = new MockSession();
            httpContext.Session.SetInt32("UserId", 1);
            httpContext.Session.SetString("UserRole", "Participant");

            var actionContext = new ActionExecutingContext(
                new ActionContext(httpContext, new RouteData(), new ActionDescriptor()),
                new List<IFilterMetadata>(),
                new Dictionary<string, object>(),
                new EventsController(null)
            );

            // Act: Trigger the filter
            attribute.OnActionExecuting(actionContext);

            // Assert: It should return 403 Forbidden
            Assert.IsType<ForbidResult>(actionContext.Result);
        }

        // ==========================================
        // 3. ENROLMENT TESTS
        // ==========================================
        [Fact]
        public async Task Enrolments_Enrol_ShouldReturnOk_WhenParticipantEnrols()
        {
            using var context = CreateInMemoryContext();
            context.Participants.Add(new Participant { ParticipantID = 1, UserID = 1, FirstName = "Test", LastName = "User", DateOfBirth = DateTime.Now, Gender = "Male", EmergencyContact = "123" });
            context.Events.Add(new Event { EventID = 1, OrganiserID = 1, Name = "Test Event", EventType = "Run", Distance = 5, Location = "Loc", Description = "Desc", EventDate = DateTime.Now.AddDays(10) });
            context.Categories.Add(new Category { CategoryID = 1, EventID = 1, CategoryName = "Open" });
            await context.SaveChangesAsync();

            var controller = CreateEnrolmentsController(context, userId: 1, role: "Participant");
            var dto = new EnrolmentRequestDto { CategoryId = 1 };

            var result = await controller.Enrol(1, dto);
            Assert.IsType<OkObjectResult>(result);
        }

        [Fact]
        public async Task Enrolments_Enrol_ShouldReturnUnauthorized_WhenNotLoggedIn()
        {
            using var context = CreateInMemoryContext();
            var controller = CreateEnrolmentsController(context, userId: null, role: null);
            var dto = new EnrolmentRequestDto { CategoryId = 1 };

            var result = await controller.Enrol(1, dto);
            Assert.IsType<UnauthorizedObjectResult>(result);
        }
    }
}