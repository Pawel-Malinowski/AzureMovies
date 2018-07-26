using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Movies.Requests
{
    public class UpdateMovieRequest
    {
        public string Title { get; set; }

        public int Year { get; set; }

        public string Genre { get; set; }
    }
}
