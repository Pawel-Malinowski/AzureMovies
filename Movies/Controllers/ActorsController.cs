using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Movies.Converters;
using Movies.Dto;
using Movies.Models;
using Movies.Repositories;

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

        /// <summary>
        /// Get all actors
        /// </summary>
        /// <response code="200">Actor list retrieved</response>
        [HttpGet]
        [ProducesResponseType(200)]
        public async Task<ActionResult<ICollection<Actor>>> GetActors()
        {
            ICollection<Actor> actors = await _actorRepository.GetAll()
                                                        .Include(x => x.MovieRoles)
                                                        .ToListAsync();
            var actorDtos = actors.Select(x => x.ToDto()).ToList();
            return Ok(actorDtos);
        }

        /// <summary>
        /// Get actor by Id
        /// </summary>
        /// <param name="actorId"></param>
        /// <response code="200">Actor found</response>
        /// <response code="404">Actor with provided id doesn't exist</response>
        [HttpGet("{actorId}")]
        [ProducesResponseType(200)]
        [ProducesResponseType(404)]
        public async Task<ActionResult<ActorDto>> GetActor(int actorId)
        {
            Actor actor = await _actorRepository.GetAll()
                                                .Include(x => x.MovieRoles)
                                                .SingleOrDefaultAsync(x => x.Id == actorId);

            if (actor == null)
                return NotFound();

            return Ok(actor.ToDto());
        }

        /// <summary>
        /// Get movies with given actor
        /// </summary>
        /// <param name="actorId"></param>
        /// <response code="200">Actor filmography returned</response>
        /// <response code="404">Actor with provided id doesn't exist</response>
        [HttpGet("{actorId}/movies")]
        [ProducesResponseType(200)]
        [ProducesResponseType(404)]
        public async Task<ActionResult<ICollection<MovieDto>>> GetActorFilmography(int actorId)
        {
            Actor actor = await _actorRepository.GetAsync(actorId);

            if (actor == null)
                return NotFound();

            ICollection<Movie> movies = await _movieRepository.GetAll()
                                                              .Include(x => x.MovieRoles)
                                                              .Where(x => x.MovieRoles.Any(r => r.ActorId == actorId))
                                                              .ToListAsync();
            var movieDtos = movies.Select(x => x.ToDto());
            return Ok(movieDtos);
        }

        /// <summary>
        /// Add new actor
        /// </summary>
        /// <param name="request">Actor json object</param>
        /// <response code="201">Actor successfully created</response>
        /// <response code="400">Provided actor model is invalid</response>
        [HttpPost]
        [ProducesResponseType(201)]
        [ProducesResponseType(400)]
        public async Task<ActionResult<ActorDto>> CreateActor(CreateActorDto request)
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

            return CreatedAtAction(nameof(GetActor), new { id = newActor.Id }, newActor.ToDto());
        }

        /// <summary>
        /// Link existing actor with existing movie
        /// </summary>
        /// <param name="actorId"></param>
        /// <param name="movieId"></param>
        /// <response code="200">Actor successfully linked with movie</response>
        /// <response code="404">Actor or movie not found</response>
        /// <response code="400">Either of parameters are invalid</response>
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
                return NotFound(nameof(actorId));

            bool movieExists = await _movieRepository.GetAll().AnyAsync(x => x.Id == movieId);

            if (!movieExists)
                return NotFound(nameof(movieId));

            bool roleExists = await _movieRoleRepository.GetAll().AnyAsync(x => x.MovieId == movieId && x.ActorId == actorId);

            if (!roleExists)
            {
                await _movieRoleRepository.AddAsync(new MovieRole() { ActorId = actorId, MovieId = movieId });
                await _movieRoleRepository.SaveAsync();
            }

            return Ok();
        }

        /// <summary>
        /// Delete existing actor
        /// </summary>
        /// <param name="actorId">Actor Id</param>
        /// <response code="204">Actor deleted successfully</response>
        /// <response code="404">Actor doesn't exist</response>
        [HttpDelete("{actorId}")]
        [ProducesResponseType(204)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> DeleteActor(int actorId)
        {
            Actor actor = await _actorRepository.GetAsync(actorId);

            if (actor == null)
                return NotFound();

            _actorRepository.Delete(actor);
            await _actorRepository.SaveAsync();

            return NoContent();
        }
    }
}
