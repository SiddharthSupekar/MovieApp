using App.Core.Dtos;
using App.Core.Intetrface;
using AutoMapper;
using MediatR;
using Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using App.Core.Apps.Movies.Query;
using Microsoft.Extensions.Logging;
using Serilog;

namespace App.Core.Apps.User.Command
{
    public class AddUserCommand : IRequest<object>
    {
        public UserDto userDto { get; set; }
    }

    public class AddUserCommandHandler : IRequestHandler<AddUserCommand, object>
    {
        private readonly IMovieDbContext _movieDbContext;
        private readonly IMapper _mapper;
        private readonly ILogger<AddUserCommandHandler> _logger;

        public AddUserCommandHandler(IMovieDbContext movieDbContext, IMapper mapper, ILogger<AddUserCommandHandler> logger)
        {
            _movieDbContext = movieDbContext;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<object> Handle(AddUserCommand command, CancellationToken cancellationToken)
        {
            try
            {
                var user = command.userDto;
                var existingUser = await _movieDbContext.Set<Domain.User>().FirstOrDefaultAsync(u => u.email == user.email);
                if (existingUser != null)
                {
                    _logger.LogWarning("User with email {Email} already exists", user.email);
                    return new
                    {
                        status = 409,
                        message = "User with this email already exists"
                    };
                }

                var userData = _mapper.Map<Domain.User>(user);
                Guid apiKey = Guid.NewGuid();
                userData.apiKey = apiKey.ToString();
                userData.password = BCrypt.Net.BCrypt.HashPassword(user.password);
                await _movieDbContext.Set<Domain.User>().AddAsync(userData);
                await _movieDbContext.SaveChangesAsync();

                _logger.LogInformation("User with email {Email} added successfully", user.email);
                return new
                {
                    status = 200,
                    message = "User Added Successfully"
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while adding user with email {Email}", command.userDto.email);
                return new
                {
                    status = 500,
                    message = "Internal server error"
                };
            }
        }
    }
}
