using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Movies.Requests;

namespace Movies.Controllers
{

//GET /movies/{movieId}/actors

//POST /movies(movie in body)
//PUT /movies/{movieId}
//PATCH /movies/{movieId}
//DELETE /movies/{movieId}
    [Route("[controller]")]
    [ApiController]
    public class MoviesController : ControllerBase
    {
        private readonly DataContext _context;

        public MoviesController(DataContext context)
        {
            _context = context;
        }
        //[ResponseType(typeof(BookDto))]
        //public async Task<IHttpActionResult> GetBook(int id)
        [HttpGet]
        public ActionResult<IEnumerable<string>> GetMovies()
        {
            return new string[] { "value1", "value2" };
        }


        [HttpGet("year/{year}")]
        public ActionResult<IEnumerable<string>> GetMoviesByYear(int year)
        {
            return new string[] { "value1", "value2" };
        }

        [HttpGet("{id}")]
        public ActionResult<string> GetMovie(int id)
        {
            return "value";
        }
        
        [HttpGet("{movieId}/actors")]
        public ActionResult<string> GetActorsFromMovie(int movieId, int actorId)
        {
            return "value";
        }

        [HttpPost]
        public void CreateMovieWithActors(CreateMovieRequest request)
        {
        }

        [HttpPost("{movieId}/actors/{actorId}")]
        public void LinkMovieWithActor(int movieId, int actorId)
        {
        }

        [HttpDelete("{id}")]
        public void DeleteMovie(int id)
        {
        }
    }
}
