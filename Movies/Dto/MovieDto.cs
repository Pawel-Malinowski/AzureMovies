using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Movies.Dto
{
    public class MovieDto
    {
        public string Title { get; set; }
        public int Year { get; set; }
        public string Genre { get; set; }
        public ICollection<int> Actors { get; set; }
    }
}
