using System.Collections.Generic;

namespace Movies.Dto
{
    public class MovieDto
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public int Year { get; set; }
        public string Genre { get; set; }
        public ICollection<int> Actors { get; set; }
    }
}
