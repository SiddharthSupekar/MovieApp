using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace App.Core.Dtos
{
    public class MovieDto
    {
        public string movieName { get; set; }
        public string releaseDate { get; set; }
        public IFormFile? posterImage { get; set; }
        public string genre { get; set; }
    }
}
