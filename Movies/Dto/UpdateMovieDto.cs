using System.ComponentModel.DataAnnotations;
using Movies.Attributes;

namespace Movies.Dto
{
    public class UpdateMovieDto
    {
        [Required]
        public string Title { get; set; }
        [FutureYearValidation]
        public int Year { get; set; }
        public string Genre { get; set; }
    }
}
