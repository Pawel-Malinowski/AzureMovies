using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Movies.Conventions;
using Movies.Models;
using Movies.Repository;
using Movies.Requests;

namespace Movies.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class MoviesController : ControllerBase
    {
        private readonly IDataRepository _repository;

        public MoviesController(IDataRepository repository)
        {
            _repository = repository;
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Actor>> GetMovie(int id)
        {
            Movie movie = await _repository.GetMovie(id);

            if (movie == null) 
                return NotFound();

            return Ok(movie);
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Movie>>> GetMoviesByYear([QueryParameterModelConvention]int? year)
        {
            IEnumerable<Movie> movies;

            if (year.HasValue)
            {
                movies = await _repository.GetMovies(x => x.Year == year);
            }
            else
            {
                movies = await _repository.GetMovies();
            }
            return Ok(movies);
        }

        [HttpGet("{movieId}/actors")]
        public async Task<ActionResult<IEnumerable<Actor>>> GetActorsFromMovie(int movieId)
        {
            Movie movie = await _repository.GetMovie(movieId);

            if (movie == null)
                return NotFound();


            IEnumerable<Actor> actors = await _repository.GetActorsFromMovie(movieId);

            return Ok(actors);
        }

        [HttpPost]
        public async Task<IActionResult> CreateMovieWithActors(CreateMovieRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest();

            Movie movie = await _repository.AddMovie(request.Title, request.Genre, request.Year, request.ActorIds);

            return CreatedAtAction(nameof(GetMovie), new { id = movie.Id }, movie);
        }

        [HttpPost("{movieId}/actors/{actorId}")]
        public async Task<IActionResult> LinkMovieWithActor(int movieId, int actorId)
        {
            if (actorId < 1 || movieId < 1)
                return BadRequest();

            bool result = await _repository.Link(actorId, movieId);

            if (!result)
                return NotFound();

            return Ok();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteMovie(int id)
        {
            bool result = await _repository.DeleteMovie(id);

            if (!result)
                return NotFound();

            return Ok();
        }
    }
}
