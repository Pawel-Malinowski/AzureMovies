using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Movies.Dto;
using Movies.Mappers;
using Movies.Models;
using Movies.Repositories;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

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
            IRepository<Actor> actorRepository,
            IRepository<Movie> movieRepository,
            IRepository<MovieRole> movieRoleRepository)
        {
            _movieRepository = movieRepository;
            _actorRepository = actorRepository;
            _movieRoleRepository = movieRoleRepository;
        }

        /// <summary>
        /// Get movie by Id
        /// </summary>
        /// <param name="movieId"></param>
        /// <response code="200">Movie returned</response>
        /// <response code="404">Movie with provided id doesn't exist</response>
        [HttpGet("{movieId}")]
        [ProducesResponseType(200)]
        [ProducesResponseType(404)]
        public async Task<ActionResult<ActorDto>> GetMovie(int movieId)
        {
            Movie movie = await _movieRepository.GetAll()
                                                .Include(x => x.MovieRoles)
                                                .SingleOrDefaultAsync(x => x.Id == movieId);

            if (movie == null)
                return NotFound();

            return Ok(movie.ToDto());
        }

        /// <summary>
        /// Get all movies
        /// </summary>
        /// <response code="200">Movie list retrieved</response>
        [HttpGet]
        [ProducesResponseType(200)]
        public async Task<ActionResult<ICollection<MovieDto>>> GetMovies()
        {
            IReadOnlyList<Movie> movies = await _movieRepository.GetAll().Include(x => x.MovieRoles).ToListAsync();

            var movieDtos = movies.Select(x => x.ToDto()).ToList();
            return Ok(movieDtos);
        }

        /// <summary>
        /// Get movies in given year
        /// </summary>
        /// <param name="year">year of movie</param>
        /// <response code="200">Movie list retrieved</response>
        [HttpGet("search")]
        [ProducesResponseType(200)]
        public async Task<ActionResult<ICollection<MovieDto>>> GetMoviesByYear(int year)
        {
            IReadOnlyList<Movie> movies = await _movieRepository.SearchFor(x => x.Year == year)
                                                                .Include(x => x.MovieRoles)
                                                                .ToListAsync();
            var movieDtos = movies.Select(x => x.ToDto()).ToList();
            return Ok(movieDtos);
        }

        /// <summary>
        /// List of actors starring in a movie
        /// </summary>
        /// <param name="movieId">Movie id</param>
        /// <response code="200">Actor list starring in a movie returned</response>
        /// <response code="404">Movie not found</response>
        [HttpGet("{movieId}/actors")]
        [ProducesResponseType(200)]
        [ProducesResponseType(404)]
        public async Task<ActionResult<ICollection<ActorDto>>> GetActorsFromMovie(int movieId)
        {
            Movie movie = await _movieRepository.GetAsync(movieId);

            if (movie == null)
                return NotFound();

            IReadOnlyList<Actor> actors = await _actorRepository.GetAll()
                                                              .Include(x => x.MovieRoles)
                                                              .Where(x => x.MovieRoles.Any(r => r.MovieId == movieId))
                                                              .ToListAsync();
            IEnumerable<ActorDto> actorDtos = actors.Select(x => x.ToDto()).ToList();

            return Ok(actorDtos);
        }

        /// <summary>
        /// Add new movie with existing actors
        /// </summary>
        /// <param name="request">Movie json object</param>
        /// <response code="201">Movie with actors successfullym created</response>
        /// <response code="400">Provided movie object is invalid</response>
        /// <response code="404">One of provided actors doesn't exist</response>
        [HttpPost]
        [ProducesResponseType(201)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        public async Task<ActionResult<MovieDto>> CreateMovieWithActors(CreateMovieDto request)
        {
            foreach (var actorId in request.ActorIds)
            {
                Actor actor = await _actorRepository.GetAsync(actorId);

                if (actor == null)
                    return NotFound("Actor: " + actorId);
            }
            var newMovie = new Movie() { Title = request.Title, Year = request.Year, Genre = request.Genre };

            await _movieRepository.AddAsync(newMovie);

            foreach (var actorId in request.ActorIds)
            {
                await _movieRoleRepository.AddAsync(new MovieRole() { Movie = newMovie, ActorId = actorId });
            }
            await _movieRepository.SaveAsync();

            return CreatedAtAction(nameof(GetMovie), new { movieId = newMovie.Id }, newMovie.ToDto());
        }

        /// <summary>
        /// Update information about existing movie
        /// </summary>
        /// <param name="movieId">Movie id</param>
        /// <param name="request">Updated Movie object</param>
        /// <response code="204">Movie successfully updated</response>
        /// <response code="400">Provided movie object is invalid</response>
        /// <response code="404">Movie doesn't exists</response>
        [HttpPut("{movieId}")]
        [ProducesResponseType(204)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> UpdateMovie(int movieId, UpdateMovieDto request)
        {
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

        /// <summary>
        /// Delete movie by id
        /// </summary>
        /// <param name="movieId"></param>
        /// <response code="204">Movie successfully deleted</response>
        /// <response code="404">Movie doesn't exists</response>
        [HttpDelete("{movieId}")]
        [ProducesResponseType(204)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> DeleteMovie(int movieId)
        {
            Movie movie = await _movieRepository.GetAsync(movieId);

            if (movie == null)
                return NotFound();

            _movieRepository.Delete(movie);
            await _movieRepository.SaveAsync();

            return NoContent();
        }
    }
}
