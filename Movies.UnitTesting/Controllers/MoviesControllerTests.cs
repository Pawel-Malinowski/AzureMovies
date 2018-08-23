using Microsoft.AspNetCore.Mvc;
using MockQueryable.Moq;
using Moq;
using Movies.Controllers;
using Movies.Dto;
using Movies.Models;
using Movies.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Xunit;

namespace Movies.UnitTesting.Controllers
{
    public class MoviesControllerTests
    {
        private readonly MoviesController _controller;

        private readonly Mock<IRepository<Movie>> _mockMovieRepository;

        private readonly Mock<IRepository<Actor>> _mockActorRepository;

        private readonly Mock<IRepository<MovieRole>> _mockMovieRoleRepository;


        public MoviesControllerTests()
        {
            _mockMovieRepository = new Mock<IRepository<Movie>>();
            _mockActorRepository = new Mock<IRepository<Actor>>();
            _mockMovieRoleRepository = new Mock<IRepository<MovieRole>>();

            _controller = new MoviesController(_mockActorRepository.Object, _mockMovieRepository.Object, _mockMovieRoleRepository.Object);
        }

        [Fact]
        public async Task UpdateExistingMovie_UpdateWasCalledAndSaved_ReturnsNoContent()
        {
            //Arrange
            int movieId = 1;
            _mockMovieRepository.Setup(x => x.GetAsync(It.Is<int>(id => id == movieId))).Returns(Task.FromResult<Movie>(new Movie() { Id = movieId }));

            var updateMovieDto = new UpdateMovieDto() { Title = "God father 2", Year = 1980, Genre = "Gangster" };

            //Act
            IActionResult result = await _controller.UpdateMovie(movieId, updateMovieDto);
            
            //Assert
            _mockMovieRepository.Verify(x => x.Update(It.Is<Movie>(m => m.Id == movieId &&
                                                                       m.Genre == updateMovieDto.Genre &&
                                                                       m.Title == updateMovieDto.Title &&
                                                                       m.Year == updateMovieDto.Year)), Times.Once());
            _mockMovieRepository.Verify(x => x.SaveAsync(), Times.Once());

            Assert.IsType<NoContentResult>(result);
        }

        [Fact]
        public async Task GetMovieById__WhenMovieExists_ReturnsData()
        {
            //Arrange
            var movie = new Movie { Id = 5, Title = "Pulp Fiction", Year = 1990, Genre = "Action" };

            var mock = new List<Movie>
            {
                movie
            }.AsQueryable().BuildMock();
            _mockMovieRepository.Setup(x => x.GetAll()).Returns(mock.Object);

            //ACT
            var result = await _controller.GetMovie(movie.Id);

            //ASSERT
            OkObjectResult okObjectResult = Assert.IsType<OkObjectResult>(result.Result);
            MovieDto movieDto = Assert.IsAssignableFrom<MovieDto>(okObjectResult.Value);

            Assert.Equal(movie.Id, movieDto.Id);
            Assert.Equal(movie.Title, movieDto.Title);
            Assert.Equal(movie.Year, movieDto.Year);
            Assert.Equal(movie.Genre, movieDto.Genre);
        }

        [Fact]
        public async Task GetMovieById__WhenMovieDoesntExist_ReturnsNotFound()
        {
            //Arrange
            var mock = new List<Movie>().AsQueryable().BuildMock();
            _mockMovieRepository.Setup(x => x.GetAll()).Returns(mock.Object);

            //ACT
            var result = await _controller.GetMovie(1);

            //ASSERT
            Assert.IsType<NotFoundResult>(result.Result);
        }

        [Fact]
        public async Task GetMovies_ReturnsExpectedNumberOfResults()
        {
            //Arrange
            var movieList = new List<Movie>
            {
                new Movie {Id = 1, Title="Pulp Fiction"},
                new Movie {Id = 2, Title="Memento"},
            };

            Mock<IQueryable<Movie>> mock = movieList.AsQueryable().BuildMock();
            _mockMovieRepository.Setup(x => x.GetAll()).Returns(mock.Object);

            //ACT
            ActionResult<ICollection<MovieDto>> result = await _controller.GetMovies();

            //ASSERT
            OkObjectResult okObjectResult = Assert.IsType<OkObjectResult>(result.Result);
            ICollection<MovieDto> actorDtos = Assert.IsAssignableFrom<ICollection<MovieDto>>(okObjectResult.Value);
            Assert.Equal(movieList.Count, actorDtos.Count);
        }

        [Theory]
        [InlineData(1999)]
        [InlineData(2000)]
        [InlineData(2001)]
        public async Task GetMoviesByYear_ReturnsExpectedNumberOfResults(int year)
        {
            //Arrange
            var mockMovieList = new List<Movie>
            {
                new Movie {Id = 1, Title="Pulp Fiction", Year = 1999},
                new Movie {Id = 2, Title="Memento", Year = 2000}
            };
            var filteredMockMovieList = mockMovieList.Where(x => x.Year == year).ToList();
            Mock<IQueryable<Movie>> mock = filteredMockMovieList.AsQueryable().BuildMock();
            _mockMovieRepository.Setup(x => x.SearchFor(It.IsAny<Expression<Func<Movie, bool>>>())).Returns(mock.Object);

            //ACT
            ActionResult<ICollection<MovieDto>> result = await _controller.GetMoviesByYear(year);

            //ASSERT
            OkObjectResult okObjectResult = Assert.IsType<OkObjectResult>(result.Result);
            ICollection<MovieDto> movieDtos = Assert.IsAssignableFrom<ICollection<MovieDto>>(okObjectResult.Value);
            Assert.Equal(filteredMockMovieList.Count, movieDtos.Count);
        }

        [Fact]
        public async Task GetActorsFromExistingMovie_ReturnsExpectedNumberOfResults()
        {
            //Arrange
            int movieId = 1;
            var actorList = new List<Actor>
            {
                new Actor {FirstName = "Al", LastName = "Pacino", MovieRoles = new List<MovieRole>(){new MovieRole(){MovieId = movieId}}},
                new Actor {FirstName = "Jodie", LastName = "Foster", MovieRoles = new List<MovieRole>(){new MovieRole(){MovieId = movieId}}}
            };

            Mock<IQueryable<Actor>> mock = actorList.AsQueryable().BuildMock();
            _mockActorRepository.Setup(x => x.GetAll()).Returns(mock.Object);
            _mockMovieRepository.Setup(x => x.GetAsync(movieId)).Returns(Task.FromResult(new Movie() { Id = movieId }));
            
            //ACT
            ActionResult<ICollection<ActorDto>> result = await _controller.GetActorsFromMovie(movieId);

            //ASSERT
            OkObjectResult okObjectResult = Assert.IsType<OkObjectResult>(result.Result);
            ICollection<ActorDto> actorDtos = Assert.IsAssignableFrom<ICollection<ActorDto>>(okObjectResult.Value);
            Assert.Equal(actorList.Count, actorDtos.Count);
        }

        [Fact]
        public async Task CreateMovieWithNonExistingActor_NoMovieCreated_ReturnsNotFound()
        {
            //Arrange
            int actorId = 1;
            _mockActorRepository.Setup(x => x.GetAsync(actorId)).Returns(Task.FromResult<Actor>(null));

            //ACT
            ActionResult<MovieDto> result = await _controller.CreateMovieWithActors(new CreateMovieDto()
            {
                Title = "Titanic",
                Year = 1999,
                ActorIds = new[] { actorId }
            });

            //ASSERT
            _mockMovieRepository.Verify(x => x.AddAsync(It.IsAny<Movie>()), Times.Never);
            _mockMovieRepository.Verify(x => x.SaveAsync(), Times.Never);

            Assert.IsType<NotFoundObjectResult>(result.Result);
        }

        [Fact]
        public async Task CreateMovieWithExistinggActor_NoMovieCreated_ReturnsNotFound()
        {
            //Arrange
            int actorId = 1;

            _mockActorRepository.Setup(x => x.GetAsync(actorId)).Returns(Task.FromResult<Actor>(new Actor(){Id = actorId}));
            var request = new CreateMovieDto()
            {
                Title = "Titanic",
                Year = 1999,
                Genre = "Melodrama",
                ActorIds = new[] {actorId}
            };

            //ACT
            ActionResult<MovieDto> result = await _controller.CreateMovieWithActors(request);

            //ASSERT
            _mockMovieRepository.Verify(x => x.AddAsync(It.Is<Movie>(a => a.Title == request.Title &&
                                                                         a.Year == request.Year &&
                                                                         a.Genre == request.Genre)), Times.Once);
            _mockMovieRoleRepository.Verify(x => x.AddAsync(It.Is<MovieRole>(a => a.ActorId == actorId)), Times.Once);
            _mockMovieRepository.Verify(x => x.SaveAsync(), Times.Once);

            CreatedAtActionResult createdAtActionResult = Assert.IsType<CreatedAtActionResult>(result.Result);
            MovieDto movieDto = Assert.IsType<MovieDto>(createdAtActionResult.Value);
            Assert.Equal(request.Title, movieDto.Title);
            Assert.Equal(request.Year, movieDto.Year);
            Assert.Equal(request.Genre, movieDto.Genre);
        }

        [Fact]
        public async Task GetActorsFromNonExistingMovie_ReturnsNotFound()
        {
            //Arrange
            int movieId = 1;
            _mockMovieRepository.Setup(x => x.GetAsync(movieId)).Returns(Task.FromResult<Movie>(null));

            //ACT
            ActionResult<ICollection<ActorDto>> result = await _controller.GetActorsFromMovie(movieId);

            //ASSERT
            Assert.IsType<NotFoundResult>(result.Result);
        }

        [Fact]
        public async Task UpdateNonExistingMovie_NoUpdateWasCalled_ReturnsNotFound()
        {
            //Arrange
            _mockMovieRepository.Setup(x => x.GetAsync(It.IsAny<int>())).Returns(Task.FromResult<Movie>(null));

            //Act
            IActionResult result = await _controller.UpdateMovie(1, new UpdateMovieDto());

            //Assert
            _mockMovieRepository.Verify(x => x.Update(It.IsAny<Movie>()), Times.Never());
            _mockMovieRepository.Verify(x => x.SaveAsync(), Times.Never());

            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public async Task DeleteNonExistingMovie_NoDeleteWasCalled_ReturnsNotFound()
        {
            //Arrange
            _mockMovieRepository.Setup(x => x.GetAsync(It.IsAny<int>())).Returns(Task.FromResult<Movie>(null));

            //Act
            IActionResult result = await _controller.DeleteMovie(1);

            //Assert
            _mockMovieRepository.Verify(x => x.Delete(It.IsAny<Movie>()), Times.Never());
            _mockMovieRepository.Verify(x => x.SaveAsync(), Times.Never());

            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public async Task DeleteExistingMovie_DeleteOfGivenMovieWasCalledAndSaved_ReturnsNoContent()
        {
            //Arrange
            int movieId = 1;
            _mockMovieRepository.Setup(x => x.GetAsync(It.IsAny<int>())).Returns(Task.FromResult<Movie>(new Movie() { Id = movieId }));

            //Act
            IActionResult result = await _controller.DeleteMovie(movieId);

            //Assert
            _mockMovieRepository.Verify(x => x.Delete(It.Is<Movie>(a => a.Id == movieId)), Times.Once);
            _mockMovieRepository.Verify(x => x.SaveAsync(), Times.Once);

            Assert.IsType<NoContentResult>(result);
        }
    }
}
