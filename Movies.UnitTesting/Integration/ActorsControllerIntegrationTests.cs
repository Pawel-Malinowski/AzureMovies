using Movies.Dto;
using Newtonsoft.Json;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace Movies.UnitTesting.Integration
{
    public class ActorsControllerIntegrationTests : IClassFixture<IntegrationTestFixture>
    {
        private readonly IntegrationTestFixture _fixture;

        public ActorsControllerIntegrationTests(IntegrationTestFixture fixture)
        {
            _fixture = fixture;
        }

        [Theory]
        [InlineData("Al", "Pacino")]
        public async Task CreateActorWithRequiredFields_Success(string firstName, string lastName)
        {
            string jsonString = JsonConvert.SerializeObject(new CreateActorDto() { FirstName = firstName, LastName = lastName });
            var stringContent = new StringContent(jsonString, Encoding.UTF8, "application/json");
            HttpResponseMessage response = await _fixture.Client.PostAsync("actors", stringContent);

            Assert.Equal(HttpStatusCode.Created, response.StatusCode);
            var responseString = await response.Content.ReadAsStringAsync();
            var actorDto = JsonConvert.DeserializeObject<ActorDto>(responseString);

            Assert.Equal(firstName, actorDto.FirstName);
            Assert.Equal(lastName, actorDto.LastName);
        }

        [Theory]
        [InlineData("Al", "")]
        [InlineData("Al", null)]
        [InlineData("", "Pacino")]
        [InlineData(null, "Pacino")]
        public async Task CreateActorWithoutRequiredFields_BadRequestReturned(string firstName, string lastName)
        {
            string jsonString = JsonConvert.SerializeObject(new CreateActorDto() { FirstName = firstName, LastName = lastName });
            var stringContent = new StringContent(jsonString, Encoding.UTF8, "application/json");
            HttpResponseMessage response = await _fixture.Client.PostAsync("actors", stringContent);

            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        }

    }
}
