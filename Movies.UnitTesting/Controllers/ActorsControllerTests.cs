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
using System.Threading.Tasks;
using Xunit;

namespace Movies.UnitTesting.Controllers
{
    public class ActorsControllerTests
    {
        [Fact]
        public async Task GetActors_ReturnsExpectedNumberOfResults()
        {
            //Arrange
            var mockMovieRepository = new Mock<IRepository<Movie>>();
            var mockActorRepository = new Mock<IRepository<Actor>>();
            var mockMovieRoleRepository = new Mock<IRepository<MovieRole>>();

            var actorList = new List<Actor>
            {
                new Actor {Id = 1, FirstName = "Al", LastName = "Pacino"},
                new Actor {Id = 2, FirstName = "Robert", LastName = "Douglas"},
                new Actor {Id = 3, FirstName = "Brad", LastName = "Pitt"},
            };

            Mock<IQueryable<Actor>> mock = actorList.AsQueryable().BuildMock();
            mockActorRepository.Setup(x => x.GetAll()).Returns(mock.Object);

            var controller = new ActorsController(mockActorRepository.Object, mockMovieRepository.Object, mockMovieRoleRepository.Object);

            //ACT
            ActionResult<ICollection<ActorDto>> result = await controller.GetActors();

            //ASSERT
            OkObjectResult okObjectResult = Assert.IsType<OkObjectResult>(result.Result);
            ICollection<ActorDto> actorDtos = Assert.IsAssignableFrom<ICollection<ActorDto>>(okObjectResult.Value);
            Assert.Equal(actorList.Count, actorDtos.Count);
        }

        [Fact]
        public async Task GetNonExistingActorFilmography_ReturnsNotFound()
        {
            //Arrange
            var mockMovieRepository = new Mock<IRepository<Movie>>();
            var mockActorRepository = new Mock<IRepository<Actor>>();
            var mockMovieRoleRepository = new Mock<IRepository<MovieRole>>();

            mockActorRepository.Setup(x => x.GetAsync(It.IsAny<int>())).Returns(Task.FromResult<Actor>(null));

            var controller = new ActorsController(mockActorRepository.Object, mockMovieRepository.Object, mockMovieRoleRepository.Object);

            //ACT
            ActionResult<ICollection<MovieDto>> result = await controller.GetActorFilmography(1);

            //ASSERT
            Assert.IsType<NotFoundResult>(result.Result);
        }

        [Fact]
        public async Task GetExistingActorFilmography_ReturnsExpectedMovie()
        {
            //Arrange
            var mockMovieRepository = new Mock<IRepository<Movie>>();
            var mockActorRepository = new Mock<IRepository<Actor>>();
            var mockMovieRoleRepository = new Mock<IRepository<MovieRole>>();

            int actorId = 1;
            mockActorRepository.Setup(x => x.GetAsync(It.IsAny<int>())).Returns(Task.FromResult(new Actor()));
            var movieList = new List<Movie>
            {
                new Movie {Id = 1, Title = "Pulp fiction", Year = 1990, MovieRoles = new List<MovieRole>()
                {
                    new MovieRole(){ActorId = actorId}
                }},
                new Movie {Id = 2, Title = "God father", Year = 1971, MovieRoles = new List<MovieRole>()
                {
                    new MovieRole(){ActorId = actorId}
                }},
                new Movie {Id = 3, Title = "Mission impossible", Year = 1991, MovieRoles = new List<MovieRole>()
                {
                    new MovieRole(){ActorId = actorId}
                }}
            };

            Mock<IQueryable<Movie>> mock = movieList.AsQueryable().BuildMock();
            mockMovieRepository.Setup(x => x.GetAll()).Returns(mock.Object);
            var controller = new ActorsController(mockActorRepository.Object, mockMovieRepository.Object, mockMovieRoleRepository.Object);

            //ACT
            ActionResult<ICollection<MovieDto>> result = await controller.GetActorFilmography(actorId);

            //ASSERT
            OkObjectResult okObjectResult = Assert.IsType<OkObjectResult>(result.Result);
            ICollection<MovieDto> movieDtos = Assert.IsAssignableFrom<ICollection<MovieDto>>(okObjectResult.Value);
            Assert.Equal(movieList.Count, movieDtos.Count);
        }

        [Fact]
        public async Task GetActorById__WhenActorExists_ReturnsData()
        {
            //Arrange
            var mockMovieRepository = new Mock<IRepository<Movie>>();
            var mockActorRepository = new Mock<IRepository<Actor>>();
            var mockMovieRoleRepository = new Mock<IRepository<MovieRole>>();
            var actor = new Actor { Id = 5, FirstName = "Al", LastName = "Pacino" };


            var mock = new List<Actor>
            {
                actor
            }.AsQueryable().BuildMock();
            mockActorRepository.Setup(x => x.GetAll()).Returns(mock.Object);

            var controller = new ActorsController(mockActorRepository.Object, mockMovieRepository.Object, mockMovieRoleRepository.Object);

            //ACT
            var result = await controller.GetActor(actor.Id);

            //ASSERT
            OkObjectResult okObjectResult = Assert.IsType<OkObjectResult>(result.Result);
            ActorDto actorDto = Assert.IsAssignableFrom<ActorDto>(okObjectResult.Value);
            Assert.Equal(actor.FirstName, actorDto.FirstName);
            Assert.Equal(actor.LastName, actorDto.LastName);
            Assert.Equal(actor.FirstName, actorDto.FirstName);
        }

        [Fact]
        public async Task GetActorById__WhenActorIsDoesntExist_ReturnsNotFound()
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

        [Theory]
        [InlineData("Al", "Pacino", 1960, 5, 7)]
        public async Task AddActor_AddAndSaveWasCalled_ReturnsCreatedResponse(string firstName, string lastName, int year, int month, int day)
        {
            //Arrange
            var mockMovieRepository = new Mock<IRepository<Movie>>();
            var mockActorRepository = new Mock<IRepository<Actor>>();
            var mockMovieRoleRepository = new Mock<IRepository<MovieRole>>();

            //Arrange
            var request = new CreateActorDto() { FirstName = firstName, LastName = lastName, BirthDate = new DateTime(year, month, day) };

            //Act
            var controller = new ActorsController(mockActorRepository.Object, mockMovieRepository.Object, mockMovieRoleRepository.Object);

            var result = await controller.CreateActor(request);

            mockActorRepository.Verify(x => x.AddAsync(It.Is<Actor>(a => a.FirstName == request.FirstName &&
                                                                         a.LastName == request.LastName && 
                                                                         a.BirthDate == request.BirthDate)), Times.Once);
            mockActorRepository.Verify(x => x.SaveAsync(), Times.Once);

            CreatedAtActionResult createdAtActionResult = Assert.IsType<CreatedAtActionResult>(result.Result);
            ActorDto actorDto = Assert.IsType<ActorDto>(createdAtActionResult.Value);
            Assert.Equal(request.FirstName, actorDto.FirstName);
            Assert.Equal(request.LastName, actorDto.LastName);
            Assert.Equal(request.BirthDate, actorDto.BirthDate);
        }

        [Fact]
        public async Task DeleteNonExistingActor_ReturnsNotFound_NoDeleteWasCalled()
        {
            //Arrange
            var mockMovieRepository = new Mock<IRepository<Movie>>();
            var mockActorRepository = new Mock<IRepository<Actor>>();
            var mockMovieRoleRepository = new Mock<IRepository<MovieRole>>();

            //Arrange

            mockActorRepository.Setup(x => x.GetAsync(It.IsAny<int>())).Returns(Task.FromResult<Actor>(null));

            //Act
            var controller = new ActorsController(mockActorRepository.Object, mockMovieRepository.Object, mockMovieRoleRepository.Object);

            IActionResult result = await controller.DeleteActor(1);
            mockActorRepository.Verify(x => x.Delete(It.IsAny<Actor>()), Times.Never());
            mockActorRepository.Verify(x => x.SaveAsync(), Times.Never());

            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public async Task DeleteExistingActor_ReturnsNoContent_DeletionOfGivenActorWasCalledAndSaved()
        {
            //Arrange
            var mockMovieRepository = new Mock<IRepository<Movie>>();
            var mockActorRepository = new Mock<IRepository<Actor>>();
            var mockMovieRoleRepository = new Mock<IRepository<MovieRole>>();

            //Arrange
            int actorId = 1;
            mockActorRepository.Setup(x => x.GetAsync(It.IsAny<int>())).Returns(Task.FromResult<Actor>(new Actor(){Id = actorId}));

            //Act
            var controller = new ActorsController(mockActorRepository.Object, mockMovieRepository.Object, mockMovieRoleRepository.Object);

            IActionResult result = await controller.DeleteActor(actorId);
            mockActorRepository.Verify(x => x.Delete(It.Is<Actor>(a => a.Id == actorId)), Times.Once);
            mockActorRepository.Verify(x => x.SaveAsync(), Times.Once);
            Assert.IsType<NoContentResult>(result);
        }

        [Theory]
        [InlineData(1, 1)]
        public async Task LinkActorWithMovie_NonExistingActor_ReturnsNotFound(int actorId, int movieId)
        {
            //Arrange
            var mockMovieRepository = new Mock<IRepository<Movie>>();
            var mockActorRepository = new Mock<IRepository<Actor>>();
            var mockMovieRoleRepository = new Mock<IRepository<MovieRole>>();

            var controller = new ActorsController(mockActorRepository.Object, mockMovieRepository.Object, mockMovieRoleRepository.Object);

            var actorList = new List<Actor>();
            Mock<IQueryable<Actor>> mockActors = actorList.AsQueryable().BuildMock();
            mockActorRepository.Setup(x => x.GetAll()).Returns(mockActors.Object);

            var movieList = new List<Movie>() { new Movie() { Id = movieId } };
            Mock<IQueryable<Movie>> mockMovies = movieList.AsQueryable().BuildMock();
            mockMovieRepository.Setup(x => x.GetAll()).Returns(mockMovies.Object);
            //ACT
            IActionResult result = await controller.LinkActorWithMovie(actorId, movieId);

            //ASSERT
            NotFoundObjectResult notFoundObjectResult = Assert.IsType<NotFoundObjectResult>(result);
            Assert.Equal(nameof(actorId), notFoundObjectResult.Value);
        }

        [Theory]
        [InlineData(1, 1)]
        public async Task LinkActorWithMovie_NonExistingMovie_ReturnsNotFound(int actorId, int movieId)
        {
            //Arrange
            var mockMovieRepository = new Mock<IRepository<Movie>>();
            var mockActorRepository = new Mock<IRepository<Actor>>();
            var mockMovieRoleRepository = new Mock<IRepository<MovieRole>>();

            var controller = new ActorsController(mockActorRepository.Object, mockMovieRepository.Object, mockMovieRoleRepository.Object);

            var actorList = new List<Actor>() { new Actor() { Id = actorId } };
            Mock<IQueryable<Actor>> mockActors = actorList.AsQueryable().BuildMock();
            mockActorRepository.Setup(x => x.GetAll()).Returns(mockActors.Object);

            var movieList = new List<Movie>();
            Mock<IQueryable<Movie>> mockMovies = movieList.AsQueryable().BuildMock();
            mockMovieRepository.Setup(x => x.GetAll()).Returns(mockMovies.Object);
            //ACT
            IActionResult result = await controller.LinkActorWithMovie(actorId, movieId);

            //ASSERT
            NotFoundObjectResult notFoundObjectResult = Assert.IsType<NotFoundObjectResult>(result);
            Assert.Equal(nameof(movieId), notFoundObjectResult.Value);
        }

        [Theory]
        [InlineData(1, 1)]
        public async Task LinkActorWithMovie_MovieRoleAlreadyExists_NoNewMovieRoleIsCreatedAndReturnsOk(int actorId, int movieId)
        {
            //Arrange
            var mockMovieRepository = new Mock<IRepository<Movie>>();
            var mockActorRepository = new Mock<IRepository<Actor>>();
            var mockMovieRoleRepository = new Mock<IRepository<MovieRole>>();

            var controller = new ActorsController(mockActorRepository.Object, mockMovieRepository.Object, mockMovieRoleRepository.Object);

            var actorList = new List<Actor>() { new Actor() { Id = actorId } };
            Mock<IQueryable<Actor>> mockActors = actorList.AsQueryable().BuildMock();
            mockActorRepository.Setup(x => x.GetAll()).Returns(mockActors.Object);

            var movieList = new List<Movie>() { new Movie() { Id = movieId } };
            Mock<IQueryable<Movie>> mockMovies = movieList.AsQueryable().BuildMock();
            mockMovieRepository.Setup(x => x.GetAll()).Returns(mockMovies.Object);

            var movieRoleList = new List<MovieRole>() { new MovieRole() { MovieId = movieId, ActorId = actorId} };
            Mock<IQueryable<MovieRole>> mockMovieRoles = movieRoleList.AsQueryable().BuildMock();
            mockMovieRoleRepository.Setup(x => x.GetAll()).Returns(mockMovieRoles.Object);
            //ACT
            IActionResult result = await controller.LinkActorWithMovie(actorId, movieId);

            mockMovieRoleRepository.Verify(x => x.AddAsync(It.IsAny<MovieRole>()), Times.Never);
            mockMovieRoleRepository.Verify(x => x.SaveAsync(), Times.Never);

            //ASSERT
            Assert.IsType<OkResult>(result);
        }

        [Theory]
        [InlineData(1, 1)]
        public async Task LinkActorWithMovie_MovieRoleDoesntExist_NewMovieRoleIsCreatedAndReturnsOk(int actorId, int movieId)
        {
            //Arrange
            var mockMovieRepository = new Mock<IRepository<Movie>>();
            var mockActorRepository = new Mock<IRepository<Actor>>();
            var mockMovieRoleRepository = new Mock<IRepository<MovieRole>>();

            var controller = new ActorsController(mockActorRepository.Object, mockMovieRepository.Object, mockMovieRoleRepository.Object);

            var actorList = new List<Actor>() { new Actor() { Id = actorId } };
            Mock<IQueryable<Actor>> mockActors = actorList.AsQueryable().BuildMock();
            mockActorRepository.Setup(x => x.GetAll()).Returns(mockActors.Object);

            var movieList = new List<Movie>() { new Movie() { Id = movieId } };
            Mock<IQueryable<Movie>> mockMovies = movieList.AsQueryable().BuildMock();
            mockMovieRepository.Setup(x => x.GetAll()).Returns(mockMovies.Object);

            var movieRoleList = new List<MovieRole>();
            Mock<IQueryable<MovieRole>> mockMovieRoles = movieRoleList.AsQueryable().BuildMock();
            mockMovieRoleRepository.Setup(x => x.GetAll()).Returns(mockMovieRoles.Object);
            //ACT
            IActionResult result = await controller.LinkActorWithMovie(actorId, movieId);

            mockMovieRoleRepository.Verify(x => x.AddAsync(It.IsAny<MovieRole>()), Times.Once);
            mockMovieRoleRepository.Verify(x => x.SaveAsync(), Times.Once);

            //ASSERT
            Assert.IsType<OkResult>(result);
        }
    }
}
