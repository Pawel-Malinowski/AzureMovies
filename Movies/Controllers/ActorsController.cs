using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Movies.Models;
using Movies.Repository;
using Movies.Requests;

namespace Movies.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class ActorsController : ControllerBase
    {
        private readonly IDataRepository _repository;

        public ActorsController(IDataRepository repository)
        {
            _repository = repository;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Actor>>> GetActors()
        {
            var result = await _repository.GetActors();

            return Ok(result);
        }

        [HttpGet("{id}")]
        [ProducesResponseType(200)]
        [ProducesResponseType(404)]
        public async Task<ActionResult<Actor>> GetActor(int id)
        {
            Actor actor = await _repository.GetActor(id);

            if (actor == null)
                return NotFound();

            return Ok(actor);
        }

        [HttpGet("{actorId}/movies")]
        public async Task<ActionResult<IEnumerable<Movie>>> GetActorFilmography(int actorId)
        {
            IEnumerable<Movie> movies = await _repository.GetActorFilmography(actorId);

            return Ok(movies);
        }

        [HttpPost]
        [ProducesResponseType(201)]
        [ProducesResponseType(400)]
        public async Task<IActionResult> CreateActor(CreateActorRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest();

            Actor actor = await _repository.AddActor(request.FirstName, request.LastName, request.BirthDate);

            return CreatedAtAction(nameof(GetActor), new { id = actor.Id }, actor);
        }

        [HttpPost("{actorId}/movies/{movieId}")]
        public async Task<IActionResult> LinkActorWithMovie(int actorId, int movieId)
        {
            if (actorId < 1 || movieId < 1)
                return BadRequest();
            bool result = await _repository.Link(actorId, movieId);

            if (!result)
                return NotFound();

            return Ok();

        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteActor(int id)
        {
            bool result = await _repository.DeleteActor(id);

            if (!result)
                return NotFound();

            return Ok();
        }
    }
}
