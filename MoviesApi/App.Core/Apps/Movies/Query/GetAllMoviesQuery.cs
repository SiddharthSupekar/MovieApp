using App.Core.Apps.Movies.Command;
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

namespace App.Core.Apps.Movies.Query
{
    public class GetAllMoviesQuery : IRequest<List<Movie>>
    {
    }

    public class GetAllMoviesQueryHandler : IRequestHandler<GetAllMoviesQuery, List<Movie>>
    {
        private readonly IMovieDbContext _movieDbContext;
        private readonly IMapper _mapper;
        private readonly ILogger<GetAllMoviesQueryHandler> _logger;

        public GetAllMoviesQueryHandler(IMovieDbContext movieDbContext, IMapper mapper, ILogger<GetAllMoviesQueryHandler> logger)
        {
            _mapper = mapper;
            _movieDbContext = movieDbContext;
            _logger = logger;
        }

        public async Task<List<Movie>> Handle(GetAllMoviesQuery request, CancellationToken cancellationToken)
        {
            try
            {
                var movies = await _movieDbContext.Set<Movie>().Where(m => m.isDeleted == false).AsNoTracking().ToListAsync();
                _logger.LogInformation("Retrieved {MovieCount} movies", movies.Count);
                return movies;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while retrieving movies");
                throw;
            }
        }
    }
}
