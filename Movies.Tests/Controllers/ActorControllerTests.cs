using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MockQueryable.Moq;
using Moq;
using Movies.Controllers;
using Movies.Dto;
using Movies.Models;
using Movies.Repository;

namespace Movies.Tests.Controllers
{
    [TestClass]
    public class ActorsControllerTests
    {
        //   private Mock<IRepository<Movie>> _mockMovieRepository;
        //   private Mock<IRepository<Actor>> _mockActorRepository;
        //   private Mock<IRepository<MovieRole>> _mockMovieRoleRepository;

        [TestInitialize]
        public void Init()
        {
        }

        [TestMethod]
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

            //3 - setup the mock as Queryable for Moq
            mockActorRepository.Setup(x => x.GetAll()).Returns(mock.Object);

            var controller = new ActorsController(mockActorRepository.Object, mockMovieRepository.Object, mockMovieRoleRepository.Object);
            //ACT
            var result = await controller.GetActors();

            //ASSERT
            Assert.IsInstanceOfType(result.Result, typeof(OkObjectResult));

            var okObjectResult = (OkObjectResult) result.Result;
            Assert.IsInstanceOfType(okObjectResult.Value, typeof(ICollection<ActorDto>));

            var resultActors = (ICollection<ActorDto>) okObjectResult.Value;
            Assert.AreEqual(actorList.Count, resultActors.Count);
        }


        [DataTestMethod]
        [DataRow("Al", "Pacino")]
        [DataRow("", "Pacino")]
        public async Task AddActor_ReturnsCreatedResponse(string firstName, string lastName)
        {
            //Arrange
            var mockMovieRepository = new Mock<IRepository<Movie>>();
            var mockActorRepository = new Mock<IRepository<Actor>>();
            var mockMovieRoleRepository = new Mock<IRepository<MovieRole>>();

            //Arrange
            var createActorRequest = new CreateActorDto() { FirstName = firstName, LastName = lastName };

            //mockRespository.Setup(x =>
            //    x.AddActor(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<DateTime>())).Returns(Task.FromResult(resultActor));

            //Act
            var controller = new ActorsController(mockActorRepository.Object, mockMovieRepository.Object, mockMovieRoleRepository.Object);

            var result = await controller.CreateActor(createActorRequest);

            Assert.IsInstanceOfType(result.Result, typeof(CreatedAtActionResult));
        }

        //[DataTestMethod]
        //[DataRow("Antonio", "", 1960, 5, 4)]
        //public async Task AddActorWithoutLastName_ReturnsActorDtoWithSameData(string firstName, string lastName, int year, int month, int day)
        //{
        //    var createActorRequest = new CreateActorDto() { FirstName = firstName, LastName = lastName, BirthDate = new DateTime(year, month, day) };

        //    //mockRespository.Setup(x =>
        //    //    x.AddActor(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<DateTime>())).Returns(Task.FromResult(resultActor));

        //    var controller = new ActorsController(_mockActorRepository.Object, _mockMovieRepository.Object, _mockMovieRoleRepository.Object);

        //    var result = await controller.CreateActor(createActorRequest);

        //    Assert.IsNotNull(result.Value);
        //    Assert.AreEqual(result.Value.FirstName, createActorRequest.FirstName);
        //    Assert.AreEqual(result.Value.LastName, createActorRequest.LastName);
        //    Assert.AreEqual(result.Value.BirthDate, createActorRequest.BirthDate);
        //}
    }
}
