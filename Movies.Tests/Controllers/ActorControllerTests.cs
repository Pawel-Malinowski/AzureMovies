using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Movies.Controllers;
using Movies.Models;
using Movies.Repository;
using Movies.Requests;
using System;
using System.Threading.Tasks;

namespace Movies.Tests
{
    [TestClass]
    public class ActorsControllerTests
    {
        [TestInitialize]
        public void Init()
        {

        }

        [TestMethod]
        public async Task AddActorWithModelError_ReturnsBadRequest(string firstName, string lastName)
        {
            var mockRespository = new Mock<IDataRepository>();

            //mockRespository.Setup(x =>
            //    x.AddActor(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<DateTime>())).Returns(Task.FromResult(resultActor));

            var controller = new ActorsController(mockRespository.Object);

            controller.ModelState.AddModelError("error", "error");

            var result = await controller.CreateActor(null);

            Assert.IsInstanceOfType(result, typeof(BadRequestResult));

        }

        [TestMethod]
        public async Task AddActor_ReturnsCreatedResponse(string firstName, string lastName)
        {
            var mockRespository = new Mock<IDataRepository>();
            var createActorRequest = new CreateActorRequest() { FirstName = firstName, LastName = lastName };

            //mockRespository.Setup(x =>
            //    x.AddActor(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<DateTime>())).Returns(Task.FromResult(resultActor));

            var controller = new ActorsController(mockRespository.Object);

            var result = await controller.CreateActor(createActorRequest);

            Assert.IsInstanceOfType(result, typeof(CreatedResult));

        }
    }
}
