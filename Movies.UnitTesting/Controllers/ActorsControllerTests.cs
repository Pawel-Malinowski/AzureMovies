using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using MockQueryable.Moq;
using Moq;
using Movies.Controllers;
using Movies.Dto;
using Movies.Models;
using Movies.Repositories;
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

            var mock = actorList.AsQueryable().BuildMock();
            mockActorRepository.Setup(x => x.GetAll()).Returns(mock.Object);

            var controller = new ActorsController(mockActorRepository.Object, mockMovieRepository.Object, mockMovieRoleRepository.Object);
            
            //ACT
            ActionResult<ICollection<Actor>> result = await controller.GetActors();

            //ASSERT
            OkObjectResult okObjectResult = Assert.IsType<OkObjectResult>(result.Result);
            ICollection<ActorDto> actorDtos = Assert.IsAssignableFrom<ICollection<ActorDto>>(okObjectResult.Value);
            Assert.Equal(actorList.Count, actorDtos.Count);
        }

        [Fact]
        public async Task GetActorById__WhenActorPresents_ReturnsData()
        {
            //Arrange
            var mockMovieRepository = new Mock<IRepository<Movie>>();
            var mockActorRepository = new Mock<IRepository<Actor>>();
            var mockMovieRoleRepository = new Mock<IRepository<MovieRole>>();
            var actor = new Actor {Id = 5, FirstName = "Al", LastName = "Pacino"};


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
        public async Task GetActorById__WhenActorIsNotPresent_ReturnsNotFound()
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
        public async Task AddActor_ReturnsCreatedResponse(string firstName, string lastName, int year, int month, int day)
        {
            //Arrange
            var mockMovieRepository = new Mock<IRepository<Movie>>();
            var mockActorRepository = new Mock<IRepository<Actor>>();
            var mockMovieRoleRepository = new Mock<IRepository<MovieRole>>();

            //Arrange
            var request = new CreateActorDto() { FirstName = firstName, LastName = lastName, BirthDate = new DateTime(year, month, day)};

            //mockRespository.Setup(x =>
            //    x.AddActor(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<DateTime>())).Returns(Task.FromResult(resultActor));

            //Act
            var controller = new ActorsController(mockActorRepository.Object, mockMovieRepository.Object, mockMovieRoleRepository.Object);

            var result = await controller.CreateActor(request);

            CreatedAtActionResult createdAtActionResult = Assert.IsType<CreatedAtActionResult>(result.Result);
            ActorDto actorDto = Assert.IsType<ActorDto>(createdAtActionResult.Value);
            Assert.Equal(request.FirstName, actorDto.FirstName);
            Assert.Equal(request.LastName, actorDto.LastName);

            Assert.Equal(request.BirthDate, actorDto.BirthDate);
        }

        [Fact]
        public async Task DeleteNonExistingActor_ReturnsNotFound()
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
            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public async Task DeleteExistingActor_ReturnsNoContent()
        {
            //Arrange
            var mockMovieRepository = new Mock<IRepository<Movie>>();
            var mockActorRepository = new Mock<IRepository<Actor>>();
            var mockMovieRoleRepository = new Mock<IRepository<MovieRole>>();

            //Arrange

            mockActorRepository.Setup(x => x.GetAsync(It.IsAny<int>())).Returns(Task.FromResult<Actor>(new Actor()));

            //Act
            var controller = new ActorsController(mockActorRepository.Object, mockMovieRepository.Object, mockMovieRoleRepository.Object);

            IActionResult result = await controller.DeleteActor(1);
            Assert.IsType<NoContentResult>(result);
        }
    }
}
