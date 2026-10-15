using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RaceDayAPI.Controllers;
using RaceDayAPI.Data;
using RaceDayAPI.DTOs;
using RaceDayAPI.Models;
using Xunit;

namespace RaceDayAPI.Tests
{
    
    /// Comprehensive unit tests for the EventsController.
    /// Uses an in-memory database to ensure test isolation.
    
    public class EventsControllerTests
    {
        // --- HELPER METHOD ---
        // Creates a fresh, fake in-memory database for every single test so they don't interfere with each other
        private RaceDayDbContext CreateInMemoryContext()
        {
            var options = new DbContextOptionsBuilder<RaceDayDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            var context = new RaceDayDbContext(options);
            context.Database.EnsureCreated();
            return context;
        }

        // --- TEST 1: GET ALL ---
        [Fact]
        public async Task GetAll_ShouldReturnOkResult_WithListOfEvents()
        {
            // ARRANGE
            using var context = CreateInMemoryContext();
            // Seed an Organiser because Events have a required Organiser foreign key
            context.Organisers.Add(new Organiser { OrganiserID = 1, UserID = 1, FirstName = "Test", LastName = "Org", PhoneNumber = "123" });

            // Added OrganiserID and future EventDate to match likely controller filtering logic
            context.Events.Add(new Event
            {
                OrganiserID = 1,
                Name = "Event 1",
                EventType = "Run",
                Distance = 5,
                Location = "Loc",
                Description = "Desc",
                EventDate = DateTime.Now.AddDays(10)
            });
            context.Events.Add(new Event
            {
                OrganiserID = 1,
                Name = "Event 2",
                EventType = "Walk",
                Distance = 10,
                Location = "Loc",
                Description = "Desc",
                EventDate = DateTime.Now.AddDays(10)
            });
            await context.SaveChangesAsync();

            var controller = new EventsController(context);

            // ACT
            var result = await controller.GetAll();

            // ASSERT
            var okResult = Assert.IsType<OkObjectResult>(result);
            var events = Assert.IsAssignableFrom<List<Event>>(okResult.Value);
            Assert.Equal(2, events.Count);
        }

        // --- TEST 2: GET BY ID (Success) ---
        [Fact]
        public async Task GetById_ShouldReturnOkResult_WhenEventExists()
        {
            // ARRANGE
            using var context = CreateInMemoryContext();
            context.Events.Add(new Event
            {
                EventID = 1, // Explicitly set the ID
                OrganiserID = 10,
                Name = "My Event", // FIXED: Changed from "Event 1" to match the assertion below
                EventType = "Run",
                Distance = 5,
                Location = "Loc",
                Description = "Desc",
                EventDate = DateTime.Now.AddDays(10)
            });
            await context.SaveChangesAsync();

            var controller = new EventsController(context);

            // ACT
            var result = await controller.GetById(1);

            // ASSERT
            var okResult = Assert.IsType<OkObjectResult>(result);
            var returnedEvent = Assert.IsType<Event>(okResult.Value);
            Assert.Equal("My Event", returnedEvent.Name);
        }

        // --- TEST 3: GET BY ID (Not Found) ---
        [Fact]
        public async Task GetById_ShouldReturnNotFound_WhenEventDoesNotExist()
        {
            // ARRANGE
            using var context = CreateInMemoryContext();
            var controller = new EventsController(context);

            // ACT
            var result = await controller.GetById(999); // ID 999 doesn't exist

            // ASSERT
            Assert.IsType<NotFoundResult>(result);
        }

        // --- TEST 4: CREATE EVENT ---
        [Fact]
        public async Task Create_ShouldReturnCreatedAtActionResult_AndAddEventToDatabase()
        {
            // ARRANGE
            using var context = CreateInMemoryContext();

            // CRITICAL: The controller hardcodes orgId = 1 and looks for an Organiser with UserID = 1.
            // We MUST seed this fake data, otherwise the controller returns NotFound("Organiser not found.")
            context.Organisers.Add(new Organiser { OrganiserID = 10, UserID = 1, FirstName = "Test", LastName = "Org", PhoneNumber = "123" });
            await context.SaveChangesAsync();

            var controller = new EventsController(context);
            var dto = new EventCreateDto
            {
                Name = "New Marathon",
                Description = "A new race",
                EventDate = DateTime.Now.AddDays(10),
                Location = "Cape Town",
                Distance = 42,
                EventType = "Run"
            };

            // ACT
            var result = await controller.Create(dto);

            // ASSERT
            var createdResult = Assert.IsType<CreatedAtActionResult>(result);
            Assert.Equal(nameof(EventsController.GetById), createdResult.ActionName);

            // Verify it actually saved to the fake database
            var eventInDb = await context.Events.FirstOrDefaultAsync(e => e.Name == "New Marathon");
            Assert.NotNull(eventInDb);
            Assert.Equal(10, eventInDb.OrganiserID); // Should match the seeded OrganiserID
        }

        // --- TEST 5: UPDATE EVENT ---
        [Fact]
        public async Task Update_ShouldReturnOkResult_AndModifyEvent()
        {
            // ARRANGE
            using var context = CreateInMemoryContext();
            context.Events.Add(new Event { EventID = 1, Name = "Old Name", EventType = "Run", Distance = 5, Location = "Loc", Description = "Desc", EventDate = DateTime.Now });
            await context.SaveChangesAsync();

            var controller = new EventsController(context);
            var dto = new EventCreateDto
            {
                Name = "Updated Name", // We are changing the name
                Description = "Desc",
                EventDate = DateTime.Now,
                Location = "Loc",
                Distance = 5,
                EventType = "Run"
            };

            // ACT
            var result = await controller.Update(1, dto);

            // ASSERT
            Assert.IsType<OkObjectResult>(result);

            // Check the database to see if the name actually changed
            var updatedEvent = await context.Events.FindAsync(1);
            Assert.NotNull(updatedEvent);
            Assert.Equal("Updated Name", updatedEvent.Name);
        }

        // --- TEST 6: DELETE EVENT ---
        [Fact]
        public async Task Delete_ShouldReturnOkResult_AndRemoveEvent()
        {
            // ARRANGE
            using var context = CreateInMemoryContext();
            context.Events.Add(new Event { EventID = 1, Name = "ToDelete", EventType = "Run", Distance = 5, Location = "Loc", Description = "Desc", EventDate = DateTime.Now });
            await context.SaveChangesAsync();

            var controller = new EventsController(context);

            // ACT
            var result = await controller.Delete(1);

            // ASSERT
            Assert.IsType<OkObjectResult>(result); // code returns Ok("Event deleted.")

            // Check the database to see if it's actually gone
            var deletedEvent = await context.Events.FindAsync(1);
            Assert.Null(deletedEvent);
        }

        // --- TEST 7: UPDATE EVENT (Not Found) ---
        [Fact]
        public async Task Update_ShouldReturnNotFound_WhenEventDoesNotExist()
        {
            // ARRANGE
            using var context = CreateInMemoryContext();
            var controller = new EventsController(context);

            // Create a dummy DTO to send
            var dto = new EventCreateDto
            {
                Name = "Ghost Event",
                Description = "Desc",
                EventDate = DateTime.Now,
                Location = "Loc",
                Distance = 5,
                EventType = "Run"
            };

            // ACT
            // Try to update an event with ID 999, which we know doesn't exist
            var result = await controller.Update(999, dto);

            // ASSERT
            Assert.IsType<NotFoundResult>(result);
        }
    }
}