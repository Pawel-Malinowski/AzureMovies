using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Movies.Attributes;

namespace Movies.Requests
{
    public class CreateMovieRequest
    {
        [Required]
      //  [DisplayFormat(ConvertEmptyStringToNull = false)]
        public string Title { get; set; }

        [FutureYearValidation]
        public int Year { get; set; }

        public string Genre { get; set; }

        [NotNullOrEmptyCollection]
        public int[] ActorIds { get; set; }
    }
}
