using App.Core.Intetrface;
using AutoMapper;
using Domain;
using MediatR;
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
    public class DeleteMovieCommand : IRequest<object>
    {
        public int id { get; set; }
    }

    public class DeleteMovieCommandHandler : IRequestHandler<DeleteMovieCommand, object>
    {
        private readonly IMovieDbContext _movieDbContext;
        private readonly IMapper _mapper;
        private readonly ILogger<DeleteMovieCommandHandler> _logger;

        public DeleteMovieCommandHandler(IMovieDbContext movieDbContext, IMapper mapper, ILogger<DeleteMovieCommandHandler> logger)
        {
            _movieDbContext = movieDbContext;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<object> Handle(DeleteMovieCommand command, CancellationToken cancellationToken)
        {
            try
            {
                var movieId = command.id;
                var movieData = await _movieDbContext.Set<Movie>().FirstOrDefaultAsync(x => x.movieId == movieId && x.isDeleted == false);
                if (movieData == null)
                {
                    _logger.LogWarning("No movie with id {MovieId} found", movieId);
                    return new
                    {
                        status = 404,
                        message = $"No movie with id {movieId} found"
                    };
                }

                movieData.isDeleted = true;
                _movieDbContext.Set<Movie>().Update(movieData);
                await _movieDbContext.SaveChangesAsync(cancellationToken);

                _logger.LogInformation("Movie with id {MovieId} deleted successfully", movieId);
                return new
                {
                    status = 200,
                    message = "Record deleted successfully"
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while deleting movie with id {MovieId}", command.id);
                return new
                {
                    status = 500,
                    message = "Internal server error"
                };
            }
        }
    }
}
