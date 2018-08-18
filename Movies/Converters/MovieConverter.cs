using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Movies.Dto;
using Movies.Models;

namespace Movies.Converters
{
    public static class MovieConverter
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
