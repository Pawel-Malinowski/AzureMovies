using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Movies.Requests
{
    public class CreateMovieRequest
    {
        public string Title { get; set; }

        public int Year { get; set; }

        public string Genre { get; set; }

        public int[] ActorIds { get; set; }
    }
}
