using App.Core.Apps.Movies.Command;
using App.Core.Intetrface;
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
    public class GetSearchMovieQuery : IRequest<object>
    {
        public string searchTitle { get; set; }
        public string apiKey { get; set; }
    }

    public class GetSearchMovieQueryHandler : IRequestHandler<GetSearchMovieQuery, object>
    {
        private readonly IMovieDbContext _movieDbContext;
        private readonly ILogger<GetSearchMovieQueryHandler> _logger;

        public GetSearchMovieQueryHandler(IMovieDbContext movieDbContext, ILogger<GetSearchMovieQueryHandler> logger)
        {
            _movieDbContext = movieDbContext;
            _logger = logger;
        }

        public async Task<object> Handle(GetSearchMovieQuery request, CancellationToken cancellationToken)
        {
            try
            {
                var apiKey = request.apiKey;
                if (await _movieDbContext.Set<Domain.User>().FirstOrDefaultAsync(a=> a.apiKey ==  apiKey) != null)
                {
                    var title = request.searchTitle;
                    var movies = await _movieDbContext.Set<Movie>()
                        .Where(m => (m.movieName.StartsWith(title) || EF.Functions.Like(m.movieName, $"%{title}%")) && m.isDeleted == false)
                        .ToListAsync();

                    _logger.LogInformation("Retrieved {MovieCount} movies matching the search title {SearchTitle}", movies.Count, title);
                    return new
                    {
                        status = 200,
                        message = "Success",
                        movies = movies
                    };
                }
                else
                {
                    return new
                    {
                        status = 401,
                        message = "Unauthorized"
                    };
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while searching for movies with title {SearchTitle}", request.searchTitle);
                throw;
            }
        }
    }
}
