using System.Linq;
using Movies.Dto;
using Movies.Models;

namespace Movies.Mappers
{
    public static class ActorMapper
    {
        public static ActorDto ToDto(this Actor actor)
        {
            return new ActorDto()
            {
                Id = actor.Id,
                FirstName = actor.FirstName,
                LastName = actor.LastName,
                BirthDate = actor.BirthDate,
                Movies = actor.MovieRoles?.Select(x => x.MovieId).ToArray()
            };
        }
    }
}
