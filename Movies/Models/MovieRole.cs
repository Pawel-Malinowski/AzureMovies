using System;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json;

namespace Movies.Models
{
    //NOTE: EF Core 2.1 doesn't support Many-To-Many relationship without in-between table
    //Reference: https://github.com/aspnet/EntityFrameworkCore/issues/1368
    public class MovieRole
    {
        public int MovieId { get; set; }

        [JsonIgnore]
        public Movie Movie { get; set; }

        public int ActorId { get; set; }

        [JsonIgnore]
        public Actor Actor { get; set; }
    }
}
