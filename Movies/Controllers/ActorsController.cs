using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Movies.Requests;

namespace Movies.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class ActorsController : ControllerBase
    {
        private readonly DataContext _context;

        public ActorsController(DataContext context)
        {
            _context = context;
        }

        [HttpGet]
        public ActionResult<IEnumerable<string>> GetActors()
        {
            return new string[] { "value1", "value2" };
        }

        [HttpGet("{id}")]
        public ActionResult<string> GetActor(int id)
        {
            return "value";
        }

        [HttpGet("{actorId}/movies")]
        public ActionResult<string> GetActorFilmography(int actorId, int movieId)
        {
            return "value";
        }

        [HttpPost]
        public void CreateActor(CreateActorRequest request)
        {
        }

        [HttpPost("{actorId}/movies/{movieId}")]
        public void LinkActorWithMovie(int actorId, int movieId)
        {
        }

        [HttpDelete("{id}")]
        public void DeleteActor(int id)
        {
        }
    }
}
