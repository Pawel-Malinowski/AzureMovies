using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Movies.Models
{
    public class Actor
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public string FirstName { get; set; }
        [Required]
        public string LastName { get; set; }
        [DataType(DataType.Date)]
        public DateTime BirthDate { get; set; }
        public ICollection<MovieRole> MovieRoles { get; set; }
    }
}
