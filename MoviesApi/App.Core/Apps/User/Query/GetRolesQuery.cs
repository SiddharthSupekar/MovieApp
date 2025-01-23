using App.Core.Apps.User.Command;
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

namespace App.Core.Apps.User.Query
{
    public class GetRolesQuery : IRequest<List<Roles>>
    {
    }

    public class GetRolesQueryHandler : IRequestHandler<GetRolesQuery, List<Roles>>
    {
        private readonly IMovieDbContext _movieDbContext;
        private readonly ILogger<GetRolesQueryHandler> _logger;

        public GetRolesQueryHandler(IMovieDbContext movieDbContext, ILogger<GetRolesQueryHandler> logger)
        {
            _movieDbContext = movieDbContext;
            _logger = logger;
        }

        public async Task<List<Roles>> Handle(GetRolesQuery request, CancellationToken cancellationToken)
        {
            try
            {
                var roles = await _movieDbContext.Set<Roles>().ToListAsync(cancellationToken);
                _logger.LogInformation("Retrieved {RoleCount} roles", roles.Count);
                return roles;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while retrieving roles");
                throw;
            }
        }
    }
}
