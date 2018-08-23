using System.Linq;
using Movies.Dto;
using Movies.Models;

namespace Movies.Mappers
{
    public static class MovieMapper
    {
        public static MovieDto ToDto(this Movie movie)
        {
            return new MovieDto()
            {
                Title = movie.Title,
                Year = movie.Year,
                Genre = movie.Genre,
                Actors = movie.MovieRoles?.Select(x => x.ActorId).ToArray()
            };
        }
    }
}
