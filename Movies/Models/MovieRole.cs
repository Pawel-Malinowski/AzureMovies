using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;

namespace Movies.Models
{
    //NOTE: EF Core 2.1 doesn't support Many-To-Many relationship without in-between table
    //Reference: https://github.com/aspnet/EntityFrameworkCore/issues/1368
    public class MovieRole
    {
        [Required]
        public int MovieId { get; set; }
        public Movie Movie { get; set; }
        [Required]
        public int ActorId { get; set; }
        public Actor Actor { get; set; }
    }
}
