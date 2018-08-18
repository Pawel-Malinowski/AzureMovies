using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Movies.Dto;
using Movies.Models;

namespace Movies.Converters
{
    public static class ActorConverter
    {
        public static ActorDto ToDto(this Actor actor)
        {
            return new ActorDto()
            {
                FirstName = actor.FirstName,
                LastName = actor.LastName,
                BirthDate = actor.BirthDate,
                Movies = actor.MovieRoles?.Select(x => x.MovieId).ToArray()
            };
        }
    }
}
