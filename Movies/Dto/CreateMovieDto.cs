﻿using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Movies.Attributes;

namespace Movies.Dto
{
    public class CreateMovieDto
    {
        [Required]
        public string Title { get; set; }
        [FutureYearValidation]
        public int Year { get; set; }
        public string Genre { get; set; }
        [NotEmptyCollection]
        public ICollection<int> ActorIds { get; set; }
    }
}
