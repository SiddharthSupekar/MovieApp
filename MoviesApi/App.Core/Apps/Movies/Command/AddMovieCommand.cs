using App.Core.Dtos;
using App.Core.Intetrface;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore.Metadata;
using Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Http;
using AutoMapper;
using Microsoft.Extensions.Logging;
using Serilog;

namespace App.Core.Apps.Movies.Command
{
    public class AddMovieCommand : IRequest<object>
    {
        public MovieDto MovieDto { get; set; }
    }

    public class AddMovieCommandHandler : IRequestHandler<AddMovieCommand, object>
    {
        private readonly IMovieDbContext _movieDbContext;
        private readonly IMapper _mapper;
        private readonly ILogger<AddMovieCommandHandler> _logger;

        public AddMovieCommandHandler(IMovieDbContext movieDbContext, IMapper mapper, ILogger<AddMovieCommandHandler> logger)
        {
            _movieDbContext = movieDbContext;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<object> Handle(AddMovieCommand command, CancellationToken cancellationToken)
        {
            try
            {
                string imgPath = null;
                var mov = command.MovieDto;
                var existingMovie = await _movieDbContext.Set<Movie>().FirstOrDefaultAsync(m => m.movieName == mov.movieName && m.releaseDate == mov.releaseDate);
                if (existingMovie != null)
                {
                    return new
                    {
                        status = 409,
                        message = "Movie with the entered Name and Date already exists"
                    };
                }

                if (mov.posterImage != null)
                {
                    imgPath = await getImagePath(mov.posterImage);
                }
                else
                {
                    imgPath = Path.Combine("uploads", "moviePosters", "defaultPoster.jpeg");
                }
                var movieData = _mapper.Map<Movie>(mov);
                movieData.posterImage = imgPath;
                await _movieDbContext.Set<Movie>().AddAsync(movieData);
                await _movieDbContext.SaveChangesAsync();
                return new
                {
                    status = 200,
                    message = "Successfully added"
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while adding a movie");
                return new
                {
                    status = 500,
                    message = "Internal server error"
                };
            }
        }

        public async Task<string> getImagePath(IFormFile image)
        {
            try
            {
                if (image.ContentType == "image/jpeg" || image.ContentType == "image/png")
                {
                    var uploadsFolder = Path.Combine("wwwroot", "uploads", "moviePosters");
                    Directory.CreateDirectory(uploadsFolder);

                    var uniqueFileName = Guid.NewGuid().ToString() + "_" + image.FileName;
                    var filePath = Path.Combine(uploadsFolder, uniqueFileName);

                    using (var fileStream = new FileStream(filePath, FileMode.Create))
                    {
                        await image.CopyToAsync(fileStream);
                    }

                    return Path.Combine("uploads", "moviePosters", uniqueFileName);
                }
                else
                {
                    throw new Exception("Invalid file format. Only JPG and PNG are supported.");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while saving the image");
                throw;
            }
        }
    }
}
