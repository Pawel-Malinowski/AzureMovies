using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Text;

namespace Movies.Models
{
    public class Movie
    {
        public int Id { get; set; }

        public string Title { get; set; }

        public int Year { get; set; }

        public string Genre { get; set; }

        public virtual ICollection<MovieRole> MovieRoles { get; set; }
    }
}
