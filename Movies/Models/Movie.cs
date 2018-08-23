using Movies.Attributes;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Movies.Models
{
    public class Movie
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public string Title { get; set; }
        [FutureYearValidation]
        public int Year { get; set; }
        public string Genre { get; set; }
        public virtual ICollection<MovieRole> MovieRoles { get; set; }
    }
}
