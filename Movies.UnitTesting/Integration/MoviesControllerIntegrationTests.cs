using Movies.Dto;
using Newtonsoft.Json;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Movies.Attributes;
using Xunit;

namespace Movies.UnitTesting.Integration
{
    public class MoviesControllerIntegrationTests : IClassFixture<IntegrationTestFixture>
    {
        private readonly IntegrationTestFixture _fixture;

        public MoviesControllerIntegrationTests(IntegrationTestFixture fixture)
        {
            _fixture = fixture;
        }

        [Theory]
        [InlineData("", 2000, "")]
        [InlineData(null, 2000, "")]
        [InlineData("Titanic", 3000, "")]
        [InlineData("", 3000, "")]
        [InlineData(null, 3000, null)]
        public async Task CreateMovieWithinvalidFields_BadRequestReturned(string title, int year, string genre)
        {
            string jsonString = JsonConvert.SerializeObject(new CreateMovieDto()
            {
                Title = title, Year = year, Genre = genre
            });
            var stringContent = new StringContent(jsonString, Encoding.UTF8, "application/json");
            HttpResponseMessage response = await _fixture.Client.PostAsync("movies", stringContent);

            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        }

        [Theory]
        [InlineData("", 2000, "")]
        [InlineData(null, 2000, "")]
        [InlineData("Titanic", 3000, "")]
        [InlineData("", 3000, "")]
        [InlineData(null, 3000, null)]
        public async Task UpdateMovieWithinvalidFields_BadRequestReturned(string title, int year, string genre)
        {
            string jsonString = JsonConvert.SerializeObject(new UpdateMovieDto()
            {
                Title = title,
                Year = year,
                Genre = genre
            });
            var stringContent = new StringContent(jsonString, Encoding.UTF8, "application/json");
            HttpResponseMessage response = await _fixture.Client.PutAsync("movies", stringContent);

            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        }

    }
}
