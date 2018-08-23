using System;
using System.Collections.Generic;

namespace Movies.Dto
{
    public class ActorDto
    {
        public int Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public DateTime BirthDate { get; set; }
        public ICollection<int> Movies { get; set; }
    }
}
