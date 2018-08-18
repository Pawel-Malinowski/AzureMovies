using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Movies.Conventions;
using Movies.Dto;
using Movies.Models;
using Movies.Repository;

namespace Movies.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class MoviesController : ControllerBase
    {
        private readonly IRepository<Movie> _movieRepository;
        private readonly IRepository<Actor> _actorRepository;
        private readonly IRepository<MovieRole> _movieRoleRepository;

        public MoviesController(
            IRepository<Movie> movieRepository, 
            IRepository<Actor> actorRepository,
            IRepository<MovieRole> movieRoleRepository)
        {
            _movieRepository = movieRepository;
            _actorRepository = actorRepository;
            _movieRoleRepository = movieRoleRepository;
        }

        [HttpGet("{id}")]
        [ProducesResponseType(200)]
        [ProducesResponseType(404)]
        public async Task<ActionResult<Actor>> GetMovie(int id)
        {
            Movie movie = await _movieRepository.GetAsync(id);

            if (movie == null) 
                return NotFound();

            return Ok(movie);
        }

        [HttpGet]
        [ProducesResponseType(200)]
        public async Task<ActionResult<IEnumerable<Movie>>> GetMovies()
        {
            IReadOnlyList<Movie> movies = await _movieRepository.GetAll().ToListAsync();

            return Ok(movies);
        }

        [HttpGet("search")]
        [ProducesResponseType(200)]
        public async Task<ActionResult<IEnumerable<Movie>>> GetMoviesByYear([QueryParameterModelConvention]int year)
        {
            IReadOnlyList<Movie> movies = await _movieRepository.SearchFor(x => x.Year == year).ToListAsync();

            return Ok(movies);
        }

        [HttpGet("{movieId}/actors")]
        [ProducesResponseType(200)]
        [ProducesResponseType(404)]
        public async Task<ActionResult<IEnumerable<Actor>>> GetActorsFromMovie(int movieId)
        {
            Movie movie = await _movieRepository.GetAsync(movieId);

            if (movie == null)
                return NotFound();


            IEnumerable<Actor> actors = await _actorRepository.GetAll()
                                                              .Include(x => x.MovieRoles)
                                                              .Where(x => x.MovieRoles.Any(r => r.MovieId == movieId))
                                                              .ToListAsync();

            return Ok(actors);
        }

        [HttpPost]
        [ProducesResponseType(201)]
        [ProducesResponseType(400)]
        public async Task<IActionResult> CreateMovieWithActors(CreateMovieDto request)
        {
            if (!ModelState.IsValid)
                return BadRequest();

            var newMovie = new Movie() { Title = request.Title, Year = request.Year, Genre = request.Genre };

            await _movieRepository.AddAsync(newMovie);

            foreach (var actorId in request.ActorIds)
            {
                await _movieRoleRepository.AddAsync(new MovieRole() { Movie = newMovie, ActorId = actorId });
            }
            await _movieRepository.SaveAsync();

            return CreatedAtAction(nameof(GetMovie), new { id = newMovie.Id }, newMovie);
        }

        [HttpPut]
        [ProducesResponseType(204)]
        [ProducesResponseType(400)]
        public async Task<IActionResult> UpdateMovie(int movieId, UpdateMovieDto request)
        {
            if (!ModelState.IsValid)
                return BadRequest();

            Movie movie = await _movieRepository.GetAsync(movieId);

            if (movie == null)
                return NotFound();

            movie.Title = request.Title;
            movie.Year = request.Year;
            movie.Genre = request.Genre;

            _movieRepository.Update(movie);
            await _movieRepository.SaveAsync();

            return NoContent();
        }

        [HttpDelete("{id}")]
        [ProducesResponseType(204)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> DeleteMovie(int id)
        {
            Movie movie = await _movieRepository.GetAsync(id);

            if (movie == null)
                return NotFound();

            _movieRepository.Delete(movie);
            await _movieRepository.SaveAsync();

            return NoContent();
        }
    }
}
