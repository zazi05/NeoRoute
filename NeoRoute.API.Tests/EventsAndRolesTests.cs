using System.Net;
using System.Net.Http.Json;
using Microsoft.AspNetCore.Mvc.Testing;
using NeoRoute.API.DTOs;
using Xunit;

namespace NeoRoute.API.Tests
{
    public class EventsAndRolesTests : IClassFixture<CustomWebApplicationFactory>
    {
        private readonly CustomWebApplicationFactory _factory;

        public EventsAndRolesTests(CustomWebApplicationFactory factory)
        {
            _factory = factory;
        }

        private HttpClient CreateClient()
        {
            return _factory.CreateClient(new WebApplicationFactoryClientOptions
            {
                AllowAutoRedirect = false,
                HandleCookies = true
            });
        }

        private async Task<(HttpClient client, string email, string password)> CreateLoggedInOrganiser()
        {
            var client = CreateClient();
            var email = $"org_{Guid.NewGuid():N}@test.com";
            var password = "Password123!";

            var reg = await client.PostAsJsonAsync("/api/Auth/register", new RegisterDto
            {
                Email = email,
                Password = password,
                FirstName = "Alice",
                LastName = "Organiser",
                Role = "Organiser"
            });
            Assert.Equal(HttpStatusCode.Created, reg.StatusCode);

            var login = await client.PostAsJsonAsync("/api/Auth/login", new LoginDto
            {
                Email = email,
                Password = password
            });
            var body = await login.Content.ReadAsStringAsync();
            Assert.True(login.StatusCode == HttpStatusCode.OK,
                $"Login failed: {(int)login.StatusCode} - {body}");

            return (client, email, password);
        }

        private async Task<(HttpClient client, string email, string password)> CreateLoggedInParticipant()
        {
            var client = CreateClient();
            var email = $"part_{Guid.NewGuid():N}@test.com";
            var password = "Password123!";

            var reg = await client.PostAsJsonAsync("/api/Auth/register", new RegisterDto
            {
                Email = email,
                Password = password,
                FirstName = "Bob",
                LastName = "Runner",
                Role = "Participant",
                Age = 25,
                Gender = "Male"
            });
            Assert.Equal(HttpStatusCode.Created, reg.StatusCode);

            var login = await client.PostAsJsonAsync("/api/Auth/login", new LoginDto
            {
                Email = email,
                Password = password
            });
            var body = await login.Content.ReadAsStringAsync();
            Assert.True(login.StatusCode == HttpStatusCode.OK,
                $"Login failed: {(int)login.StatusCode} - {body}");

            return (client, email, password);
        }

        [Fact]
        public async Task CreateEvent_AsOrganiser_Returns201()
        {
            var (client, _, _) = await CreateLoggedInOrganiser();

            var dto = new CreateEventDto
            {
                EventName = "Test Race",
                Description = "Unit test event",
                EventDate = DateTime.UtcNow.AddDays(30),
                Location = "Cape Town",
                Distance = "10km",
                EventType = "run"
            };

            var response = await client.PostAsJsonAsync("/api/Events", dto);
            var body = await response.Content.ReadAsStringAsync();
            Assert.True(response.StatusCode == HttpStatusCode.Created,
                $"Expected 201 but got {(int)response.StatusCode}. Body: {body}");
        }

        [Fact]
        public async Task CreateEvent_AsParticipant_Returns403()
        {
            var (client, _, _) = await CreateLoggedInParticipant();

            var dto = new CreateEventDto
            {
                EventName = "Should Fail",
                EventDate = DateTime.UtcNow.AddDays(30),
                Location = "Nowhere",
                EventType = "run"
            };

            var response = await client.PostAsJsonAsync("/api/Events", dto);
            Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
        }

        [Fact]
        public async Task CreateEvent_WhenNotLoggedIn_Returns401()
        {
            var client = CreateClient();
            var dto = new CreateEventDto
            {
                EventName = "Anonymous Fail",
                EventDate = DateTime.UtcNow.AddDays(30),
                Location = "Nowhere"
            };

            var response = await client.PostAsJsonAsync("/api/Events", dto);
            Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
        }

        [Fact]
        public async Task GetEvents_Public_Returns200()
        {
            var client = CreateClient();
            var response = await client.GetAsync("/api/Events");
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }
    }
}
