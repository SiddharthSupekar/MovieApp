using App.Core.Dtos;
using App.Core.Intetrface;
using AutoMapper;
using Domain;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Serilog;

namespace App.Core.Apps.Movies.Command
{
    public class UpdateMovieCommand : IRequest<object>
    {
        public MovieDto movieDto { get; set; }
        public int movieId { get; set; }
    }

    public class UpdateMovieCommandHandler : IRequestHandler<UpdateMovieCommand, object>
    {
        private readonly IMovieDbContext _movieDbContext;
        private readonly IMapper _mapper;
        private readonly ILogger<UpdateMovieCommandHandler> _logger;

        public UpdateMovieCommandHandler(IMovieDbContext movieDbContext, IMapper mapper, ILogger<UpdateMovieCommandHandler> logger)
        {
            _mapper = mapper;
            _movieDbContext = movieDbContext;
            _logger = logger;
        }

        public async Task<object> Handle(UpdateMovieCommand command, CancellationToken cancellationToken)
        {
            try
            {
                string imgPath = null;
                int id = command.movieId;
                var dto = command.movieDto;
                var movie = await _movieDbContext.Set<Movie>().FirstOrDefaultAsync(m => m.movieId == id);
                if (movie == null)
                {
                    _logger.LogWarning("Movie with id {MovieId} not found", id);
                    return new
                    {
                        status = 404,
                        message = "Movie not found"
                    };
                }

                var existingImg = movie.posterImage;
                var movieData = _mapper.Map(dto, movie);

                if (dto.posterImage != null)
                {
                    imgPath = await getImagePath(dto.posterImage);
                    movieData.posterImage = imgPath;
                }
                else
                {
                    movieData.posterImage = existingImg;
                }

                _movieDbContext.Set<Movie>().Update(movieData);
                await _movieDbContext.SaveChangesAsync(cancellationToken);

                _logger.LogInformation("Movie with id {MovieId} updated successfully", id);
                return new
                {
                    status = 200,
                    message = "Updated Successfully"
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while updating movie with id {MovieId}", command.movieId);
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
