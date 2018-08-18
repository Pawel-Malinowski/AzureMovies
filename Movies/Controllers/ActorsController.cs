using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Movies.Dto;
using Movies.Models;
using Movies.Repository;

namespace Movies.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class ActorsController : ControllerBase
    {
        private readonly IRepository<Actor> _actorRepository;
        private readonly IRepository<Movie> _movieRepository;
        private readonly IRepository<MovieRole> _movieRoleRepository;

        public ActorsController(
            IRepository<Actor> actorRepository, 
            IRepository<Movie> movieRepository, 
            IRepository<MovieRole> movieRoleRepository)
        {
            _actorRepository = actorRepository;
            _movieRepository = movieRepository;
            _movieRoleRepository = movieRoleRepository;
        }

        [HttpGet]
        [ProducesResponseType(200)]
        public async Task<ActionResult<IEnumerable<Actor>>> GetActors()
        {
            IList<Actor> actors = await _actorRepository.GetAll()
                                                        .Include(x => x.MovieRoles)
                                                        .ToListAsync();

            return Ok(actors);
        }

        [HttpGet("{id}")]
        [ProducesResponseType(200)]
        [ProducesResponseType(404)]
        public async Task<ActionResult<Actor>> GetActor(int id)
        {
            Actor actor = await _actorRepository.GetAsync(id);

            if (actor == null)
                return NotFound();

            return Ok(actor);
        }

        [HttpGet("{actorId}/movies")]
        [ProducesResponseType(200)]
        public async Task<ActionResult<IEnumerable<Movie>>> GetActorFilmography(int actorId)
        {
            IEnumerable<Movie> movies = await _movieRepository.GetAll()
                                                              .Include(x => x.MovieRoles)
                                                              .Where(x => x.MovieRoles.Any(r => r.ActorId == actorId))
                                                              .ToListAsync();

            return Ok(movies);
        }

        [HttpPost]
        [ProducesResponseType(201)]
        [ProducesResponseType(400)]
        public async Task<IActionResult> CreateActor(CreateActorDto request)
        {
            if (!ModelState.IsValid)
                return BadRequest();

            var newActor = new Actor()
            {
                FirstName = request.FirstName,
                LastName = request.LastName,
                BirthDate = request.BirthDate
            };
            await _actorRepository.AddAsync(newActor);
            await _actorRepository.SaveAsync();

            return CreatedAtAction(nameof(GetActor), new { id = newActor.Id }, newActor);
        }

        [HttpPost("{actorId}/movies/{movieId}")]
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> LinkActorWithMovie(int actorId, int movieId)
        {
            if (actorId < 1 || movieId < 1)
                return BadRequest();

            bool actorExists = await _actorRepository.GetAll().AnyAsync(x => x.Id == actorId);

            if (!actorExists)
                return NotFound();

            bool movieExists = await _movieRepository.GetAll().AnyAsync(x => x.Id == movieId);

            if (!movieExists)
                return NotFound();

            bool alreadyLinked = await _movieRoleRepository.GetAll().AnyAsync(x => x.MovieId == movieId && x.ActorId == actorId);

            if (!alreadyLinked)
            {
                await _movieRoleRepository.AddAsync(new MovieRole() { ActorId = actorId, MovieId = movieId });
                await _movieRoleRepository.SaveAsync();
            }

            return Ok();

        }

        [HttpDelete("{id}")]
        [ProducesResponseType(204)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> DeleteActor(int id)
        {
            Actor actor = await _actorRepository.GetAsync(id);

            if (actor == null)
                return NotFound();

            _actorRepository.Delete(actor);
            await _actorRepository.SaveAsync();

            return NoContent();
        }
    }
}
