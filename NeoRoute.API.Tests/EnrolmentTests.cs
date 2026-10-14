using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using Microsoft.AspNetCore.Mvc.Testing;
using NeoRoute.API.DTOs;
using Xunit;

namespace NeoRoute.API.Tests
{
    public class EnrolmentTests : IClassFixture<CustomWebApplicationFactory>
    {
        private readonly CustomWebApplicationFactory _factory;

        public EnrolmentTests(CustomWebApplicationFactory factory)
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

        [Fact]
        public async Task Enrol_AsParticipant_Returns201()
        {
            var orgClient = CreateClient();
            var orgEmail = $"org_{Guid.NewGuid():N}@test.com";
            var orgPass = "Password123!";

            var regOrg = await orgClient.PostAsJsonAsync("/api/Auth/register", new RegisterDto
            {
                Email = orgEmail,
                Password = orgPass,
                FirstName = "Org",
                LastName = "User",
                Role = "Organiser"
            });
            Assert.Equal(HttpStatusCode.Created, regOrg.StatusCode);

            var loginOrg = await orgClient.PostAsJsonAsync("/api/Auth/login", new LoginDto
            {
                Email = orgEmail,
                Password = orgPass
            });
            Assert.Equal(HttpStatusCode.OK, loginOrg.StatusCode);

            var eventResponse = await orgClient.PostAsJsonAsync("/api/Events", new CreateEventDto
            {
                EventName = "Enrolment Test Race",
                EventDate = DateTime.UtcNow.AddDays(20),
                Location = "Test City",
                EventType = "run"
            });
            var eventBody = await eventResponse.Content.ReadAsStringAsync();
            Assert.True(eventResponse.StatusCode == HttpStatusCode.Created,
                $"Create event failed: {(int)eventResponse.StatusCode} - {eventBody}");

            using var eventDoc = JsonDocument.Parse(eventBody);
            var eventId = eventDoc.RootElement.GetProperty("eventId").GetInt32();

            var catResponse = await orgClient.PostAsJsonAsync("/api/Categories", new CreateCategoryDto
            {
                CategoryName = "Open",
                Distance = "10km"
            });
            var catBody = await catResponse.Content.ReadAsStringAsync();
            Assert.True(catResponse.StatusCode == HttpStatusCode.Created,
                $"Create category failed: {(int)catResponse.StatusCode} - {catBody}");

            using var catDoc = JsonDocument.Parse(catBody);
            var categoryId = catDoc.RootElement.GetProperty("categoryId").GetInt32();

            var partClient = CreateClient();
            var partEmail = $"part_{Guid.NewGuid():N}@test.com";
            var partPass = "Password123!";

            var regPart = await partClient.PostAsJsonAsync("/api/Auth/register", new RegisterDto
            {
                Email = partEmail,
                Password = partPass,
                FirstName = "Part",
                LastName = "User",
                Role = "Participant",
                Age = 30
            });
            Assert.Equal(HttpStatusCode.Created, regPart.StatusCode);

            var loginPart = await partClient.PostAsJsonAsync("/api/Auth/login", new LoginDto
            {
                Email = partEmail,
                Password = partPass
            });
            Assert.Equal(HttpStatusCode.OK, loginPart.StatusCode);

            var enrolResponse = await partClient.PostAsJsonAsync("/api/Enrolments", new CreateEnrollmentDto
            {
                EventId = eventId,
                CategoryId = categoryId
            });
            var enrolBody = await enrolResponse.Content.ReadAsStringAsync();
            Assert.True(enrolResponse.StatusCode == HttpStatusCode.Created,
                $"Enrol failed: {(int)enrolResponse.StatusCode} - {enrolBody}");
        }

        [Fact]
        public async Task Enrol_AsOrganiser_Returns403()
        {
            var client = CreateClient();
            var orgEmail = $"org2_{Guid.NewGuid():N}@test.com";
            var orgPass = "Password123!";

            await client.PostAsJsonAsync("/api/Auth/register", new RegisterDto
            {
                Email = orgEmail,
                Password = orgPass,
                FirstName = "Org",
                LastName = "User",
                Role = "Organiser"
            });

            var login = await client.PostAsJsonAsync("/api/Auth/login", new LoginDto
            {
                Email = orgEmail,
                Password = orgPass
            });
            Assert.Equal(HttpStatusCode.OK, login.StatusCode);

            var response = await client.PostAsJsonAsync("/api/Enrolments", new CreateEnrollmentDto
            {
                EventId = 1,
                CategoryId = 1
            });

            Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
        }
    }
}
