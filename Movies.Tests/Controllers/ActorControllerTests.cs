using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualStudio.TestTools.UnitTesting;
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
        private Mock<IRepository<Movie>> _mockMovieRepository;
        private Mock<IRepository<Actor>> _mockActorRepository;
        private Mock<IRepository<MovieRole>> _mockMovieRoleRepository;

        [TestInitialize]
        public void Init()
        {
             _mockMovieRepository = new Mock<IRepository<Movie>>();
            _mockActorRepository = new Mock<IRepository<Actor>>();
            _mockMovieRoleRepository = new Mock<IRepository<MovieRole>>();
        }

        [TestMethod]
        public async Task AddActorWithModelError_ReturnsBadRequest(string firstName, string lastName)
        {
            

            //mockRespository.Setup(x =>
            //    x.AddActor(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<DateTime>())).Returns(Task.FromResult(resultActor));

            var controller = new ActorsController(_mockActorRepository.Object, _mockMovieRepository.Object, _mockMovieRoleRepository.Object);

            controller.ModelState.AddModelError("error", "error");

            var result = await controller.CreateActor(null);
            
            Assert.IsInstanceOfType(result, typeof(BadRequestResult));

        }

        [TestMethod]
        public async Task AddActor_ReturnsCreatedResponse(string firstName, string lastName)
        {
            var createActorRequest = new CreateActorDto() { FirstName = firstName, LastName = lastName };

            //mockRespository.Setup(x =>
            //    x.AddActor(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<DateTime>())).Returns(Task.FromResult(resultActor));

            var controller = new ActorsController(_mockActorRepository.Object, _mockMovieRepository.Object, _mockMovieRoleRepository.Object);

            var result = await controller.CreateActor(createActorRequest);

            Assert.IsInstanceOfType(result, typeof(CreatedResult));
        }
    }
}
