using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain
{
    public class Movie
    {
        public int movieId { get; set; }
        public string movieName { get; set; }
        public string releaseDate { get; set; }
        public string? posterImage {  get; set; }
        public string genre { get; set; }
        public bool isDeleted {  get; set; } = false;
    }
}
