using System;
using System.ComponentModel.DataAnnotations;

namespace Movies.Dto
{
    public class CreateActorDto
    {
        [Required]
        public string FirstName { get; set; }
        [Required]
        public string LastName { get; set; }
        public DateTime BirthDate { get; set; }
    }
}
