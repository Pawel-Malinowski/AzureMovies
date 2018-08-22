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
        [Fact]
        public async Task UpdateExistingMovie_ReturnsNoContent_UpdateWasCalledAndSaved()
        {
            //Arrange
            var mockMovieRepository = new Mock<IRepository<Movie>>();
            var mockActorRepository = new Mock<IRepository<Actor>>();
            var mockMovieRoleRepository = new Mock<IRepository<MovieRole>>();

            //Arrange
            int movieId = 1;
            mockMovieRepository.Setup(x => x.GetAsync(It.Is<int>(id => id == movieId))).Returns(Task.FromResult<Movie>(new Movie() { Id = movieId }));

            //Act
            var controller = new MoviesController(mockActorRepository.Object, mockMovieRepository.Object, mockMovieRoleRepository.Object);
            var updateMovieDto = new UpdateMovieDto() { Title = "God father 2", Year = 1980, Genre = "Gangster" };

            IActionResult result = await controller.UpdateMovie(movieId, updateMovieDto);

            mockMovieRepository.Verify(x => x.Update(It.Is<Movie>(m => m.Id == movieId &&
                                                                       m.Genre == updateMovieDto.Genre &&
                                                                       m.Title == updateMovieDto.Title &&
                                                                       m.Year == updateMovieDto.Year)), Times.Once());
            mockMovieRepository.Verify(x => x.SaveAsync(), Times.Once());

            Assert.IsType<NoContentResult>(result);
        }

        [Fact]
        public async Task GetMovieById__WhenMovieExists_ReturnsData()
        {
            //Arrange
            var mockMovieRepository = new Mock<IRepository<Movie>>();
            var mockActorRepository = new Mock<IRepository<Actor>>();
            var mockMovieRoleRepository = new Mock<IRepository<MovieRole>>();
            var movie = new Movie { Id = 5, Title = "Pulp Fiction", Year = 1990, Genre = "Action" };


            var mock = new List<Movie>
            {
                movie
            }.AsQueryable().BuildMock();
            mockMovieRepository.Setup(x => x.GetAll()).Returns(mock.Object);

            var controller = new MoviesController(mockActorRepository.Object, mockMovieRepository.Object, mockMovieRoleRepository.Object);

            //ACT
            var result = await controller.GetMovie(movie.Id);

            //ASSERT
            OkObjectResult okObjectResult = Assert.IsType<OkObjectResult>(result.Result);
            MovieDto movieDto = Assert.IsAssignableFrom<MovieDto>(okObjectResult.Value);

            Assert.Equal(movie.Title, movieDto.Title);
            Assert.Equal(movie.Year, movieDto.Year);
            Assert.Equal(movie.Genre, movieDto.Genre);
        }

        [Fact]
        public async Task GetMovieById__WhenMovieIsDoesntExist_ReturnsNotFound()
        {
            //Arrange
            var mockMovieRepository = new Mock<IRepository<Movie>>();
            var mockActorRepository = new Mock<IRepository<Actor>>();
            var mockMovieRoleRepository = new Mock<IRepository<MovieRole>>();

            var mock = new List<Actor>().AsQueryable().BuildMock();
            mockActorRepository.Setup(x => x.GetAll()).Returns(mock.Object);

            var controller = new ActorsController(mockActorRepository.Object, mockMovieRepository.Object, mockMovieRoleRepository.Object);

            //ACT
            var result = await controller.GetActor(1);

            //ASSERT
            Assert.IsType<NotFoundResult>(result.Result);
        }

        [Fact]
        public async Task GetMovies_ReturnsExpectedNumberOfResults()
        {
            //Arrange
            var mockMovieRepository = new Mock<IRepository<Movie>>();
            var mockActorRepository = new Mock<IRepository<Actor>>();
            var mockMovieRoleRepository = new Mock<IRepository<MovieRole>>();

            var movieList = new List<Movie>
            {
                new Movie {Id = 1, Title="Pulp Fiction"},
                new Movie {Id = 2, Title="Memento"},
            };

            Mock<IQueryable<Movie>> mock = movieList.AsQueryable().BuildMock();
            mockMovieRepository.Setup(x => x.GetAll()).Returns(mock.Object);

            var controller = new MoviesController(mockActorRepository.Object, mockMovieRepository.Object, mockMovieRoleRepository.Object);

            //ACT
            ActionResult<ICollection<MovieDto>> result = await controller.GetMovies();

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
            var mockMovieRepository = new Mock<IRepository<Movie>>();
            var mockActorRepository = new Mock<IRepository<Actor>>();
            var mockMovieRoleRepository = new Mock<IRepository<MovieRole>>();

            var mockMovieList = new List<Movie>
            {
                new Movie {Id = 1, Title="Pulp Fiction", Year = 1999},
                new Movie {Id = 2, Title="Memento", Year = 2000}
            };
            var filteredMockMovieList = mockMovieList.Where(x => x.Year == year).ToList();
            Mock<IQueryable<Movie>> mock = filteredMockMovieList.AsQueryable().BuildMock();
            mockMovieRepository.Setup(x => x.SearchFor(It.IsAny<Expression<Func<Movie, bool>>>())).Returns(mock.Object);

            var controller = new MoviesController(mockActorRepository.Object, mockMovieRepository.Object, mockMovieRoleRepository.Object);

            //ACT
            ActionResult<ICollection<MovieDto>> result = await controller.GetMoviesByYear(year);

            //ASSERT
            OkObjectResult okObjectResult = Assert.IsType<OkObjectResult>(result.Result);
            ICollection<MovieDto> movieDtos = Assert.IsAssignableFrom<ICollection<MovieDto>>(okObjectResult.Value);
            Assert.Equal(filteredMockMovieList.Count, movieDtos.Count);
        }

        [Fact]
        public async Task GetActorsFromExistingMovie_ReturnsExpectedNumberOfResults()
        {
            //Arrange
            var mockMovieRepository = new Mock<IRepository<Movie>>();
            var mockActorRepository = new Mock<IRepository<Actor>>();
            var mockMovieRoleRepository = new Mock<IRepository<MovieRole>>();

            int movieId = 1;
            var actorList = new List<Actor>
            {
                new Actor {FirstName = "Al", LastName = "Pacino", MovieRoles = new List<MovieRole>(){new MovieRole(){MovieId = movieId}}},
                new Actor {FirstName = "Jodie", LastName = "Foster", MovieRoles = new List<MovieRole>(){new MovieRole(){MovieId = movieId}}}
            };

            Mock<IQueryable<Actor>> mock = actorList.AsQueryable().BuildMock();
            mockActorRepository.Setup(x => x.GetAll()).Returns(mock.Object);

            mockMovieRepository.Setup(x => x.GetAsync(movieId)).Returns(Task.FromResult(new Movie() { Id = movieId }));
            var controller = new MoviesController(mockActorRepository.Object, mockMovieRepository.Object, mockMovieRoleRepository.Object);

            //ACT
            ActionResult<ICollection<ActorDto>> result = await controller.GetActorsFromMovie(movieId);

            //ASSERT
            OkObjectResult okObjectResult = Assert.IsType<OkObjectResult>(result.Result);
            ICollection<ActorDto> actorDtos = Assert.IsAssignableFrom<ICollection<ActorDto>>(okObjectResult.Value);
            Assert.Equal(actorList.Count, actorDtos.Count);
        }

        [Fact]
        public async Task CreateMovieWithNonExistinggActor_NoMovieCreated_ReturnsNotFound()
        {
            //Arrange
            var mockMovieRepository = new Mock<IRepository<Movie>>();
            var mockActorRepository = new Mock<IRepository<Actor>>();
            var mockMovieRoleRepository = new Mock<IRepository<MovieRole>>();

            int actorId = 1;

            mockActorRepository.Setup(x => x.GetAsync(actorId)).Returns(Task.FromResult<Actor>(null));
            var controller = new MoviesController(mockActorRepository.Object, mockMovieRepository.Object, mockMovieRoleRepository.Object);

            //ACT
            ActionResult<MovieDto> result = await controller.CreateMovieWithActors(new CreateMovieDto()
            {
                Title = "Titanic",
                Year = 1999,
                ActorIds = new[] { actorId }
            });
            mockMovieRepository.Verify(x => x.AddAsync(It.IsAny<Movie>()), Times.Never);
            mockMovieRepository.Verify(x => x.SaveAsync(), Times.Never);

            //ASSERT
            Assert.IsType<NotFoundObjectResult>(result.Result);
        }

        [Fact]
        public async Task CreateMovieWithExistinggActor_NoMovieCreated_ReturnsNotFound()
        {
            //Arrange
            var mockMovieRepository = new Mock<IRepository<Movie>>();
            var mockActorRepository = new Mock<IRepository<Actor>>();
            var mockMovieRoleRepository = new Mock<IRepository<MovieRole>>();

            int actorId = 1;

            mockActorRepository.Setup(x => x.GetAsync(actorId)).Returns(Task.FromResult<Actor>(new Actor(){Id = actorId}));
            var controller = new MoviesController(mockActorRepository.Object, mockMovieRepository.Object, mockMovieRoleRepository.Object);

            var request = new CreateMovieDto()
            {
                Title = "Titanic",
                Year = 1999,
                Genre = "Melodrama",
                ActorIds = new[] {actorId}
            };

            //ACT
            ActionResult<MovieDto> result = await controller.CreateMovieWithActors(request);

            //ASSERT
            mockMovieRepository.Verify(x => x.AddAsync(It.Is<Movie>(a => a.Title == request.Title &&
                                                                         a.Year == request.Year &&
                                                                         a.Genre == request.Genre)), Times.Once);
            mockMovieRoleRepository.Verify(x => x.AddAsync(It.Is<MovieRole>(a => a.ActorId == actorId)), Times.Once);

            mockMovieRepository.Verify(x => x.SaveAsync(), Times.Once);

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
            var mockMovieRepository = new Mock<IRepository<Movie>>();
            var mockActorRepository = new Mock<IRepository<Actor>>();
            var mockMovieRoleRepository = new Mock<IRepository<MovieRole>>();

            int movieId = 1;

            mockMovieRepository.Setup(x => x.GetAsync(movieId)).Returns(Task.FromResult<Movie>(null));
            var controller = new MoviesController(mockActorRepository.Object, mockMovieRepository.Object, mockMovieRoleRepository.Object);

            //ACT
            ActionResult<ICollection<ActorDto>> result = await controller.GetActorsFromMovie(movieId);

            //ASSERT
            Assert.IsType<NotFoundResult>(result.Result);
        }

        [Fact]
        public async Task UpdateNonExistingMovie_ReturnsNotFound_NoUpdateWasCalled()
        {
            //Arrange
            var mockMovieRepository = new Mock<IRepository<Movie>>();
            var mockActorRepository = new Mock<IRepository<Actor>>();
            var mockMovieRoleRepository = new Mock<IRepository<MovieRole>>();

            //Arrange

            mockMovieRepository.Setup(x => x.GetAsync(It.IsAny<int>())).Returns(Task.FromResult<Movie>(null));

            //Act
            var controller = new MoviesController(mockActorRepository.Object, mockMovieRepository.Object, mockMovieRoleRepository.Object);

            IActionResult result = await controller.UpdateMovie(1, new UpdateMovieDto());
            mockMovieRepository.Verify(x => x.Update(It.IsAny<Movie>()), Times.Never());
            mockMovieRepository.Verify(x => x.SaveAsync(), Times.Never());

            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public async Task DeleteNonExistingMovie_ReturnsNotFound_NoDeleteWasCalled()
        {
            //Arrange
            var mockMovieRepository = new Mock<IRepository<Movie>>();
            var mockActorRepository = new Mock<IRepository<Actor>>();
            var mockMovieRoleRepository = new Mock<IRepository<MovieRole>>();

            //Arrange

            mockMovieRepository.Setup(x => x.GetAsync(It.IsAny<int>())).Returns(Task.FromResult<Movie>(null));

            //Act
            var controller = new MoviesController(mockActorRepository.Object, mockMovieRepository.Object, mockMovieRoleRepository.Object);

            IActionResult result = await controller.DeleteMovie(1);
            mockActorRepository.Verify(x => x.Delete(It.IsAny<Actor>()), Times.Never());
            mockActorRepository.Verify(x => x.SaveAsync(), Times.Never());

            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public async Task DeleteExistingMovie_ReturnsNoContent_DeletionOfGivenMovieWasCalledAndSaved()
        {
            //Arrange
            var mockMovieRepository = new Mock<IRepository<Movie>>();
            var mockActorRepository = new Mock<IRepository<Actor>>();
            var mockMovieRoleRepository = new Mock<IRepository<MovieRole>>();

            //Arrange
            int movieId = 1;
            mockMovieRepository.Setup(x => x.GetAsync(It.IsAny<int>())).Returns(Task.FromResult<Movie>(new Movie() { Id = movieId }));

            //Act
            var controller = new MoviesController(mockActorRepository.Object, mockMovieRepository.Object, mockMovieRoleRepository.Object);

            IActionResult result = await controller.DeleteMovie(movieId);
            mockMovieRepository.Verify(x => x.Delete(It.Is<Movie>(a => a.Id == movieId)), Times.Once);
            mockMovieRepository.Verify(x => x.SaveAsync(), Times.Once);
            Assert.IsType<NoContentResult>(result);
        }

    }
}
