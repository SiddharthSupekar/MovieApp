using App.Core.Dtos;
using App.Core.Interface;
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

namespace App.Core.Apps.User.Command
{
    public class UserLoginCommand : IRequest<object>
    {
        public string email { get; set; }
        public string password { get; set; }
    }

    public class UserLoginCommandHandler : IRequestHandler<UserLoginCommand, object>
    {
        private readonly IMovieDbContext _movieDbContext;
        private readonly IJwtService _jwtService;
        private readonly ILogger<UserLoginCommandHandler> _logger;

        public UserLoginCommandHandler(IMovieDbContext movieDbContext, IJwtService jwtService, ILogger<UserLoginCommandHandler> logger)
        {
            _jwtService = jwtService;
            _movieDbContext = movieDbContext;
            _logger = logger;
        }

        public async Task<object> Handle(UserLoginCommand command, CancellationToken cancellationToken)
        {
            try
            {
                string email = command.email;
                string password = command.password;
                var validEmail = await _movieDbContext.Set<Domain.User>().FirstOrDefaultAsync(u => u.email == email);
                if (validEmail == null)
                {
                    _logger.LogWarning("Email {Email} does not exist", email);
                    return new
                    {
                        status = 404,
                        message = "Email does not exist"
                    };
                }

                if (BCrypt.Net.BCrypt.Verify(password, validEmail.password))
                {
                    int roleId = validEmail.roleId;
                    var role = await _movieDbContext.Set<Roles>().FirstOrDefaultAsync(r => r.roleId == roleId);
                    var apiKey = validEmail.apiKey;
                    var tokenLogin = new TokenDto
                    {
                        email = email,
                        id = validEmail.userId,
                        roleName = role.roleName
                    };
                    var token = _jwtService.GenerateToken(tokenLogin, apiKey);

                    _logger.LogInformation("User with email {Email} logged in successfully", email);
                    return new
                    {
                        status = 200,
                        message = "Login Successful",
                        token = token
                    };
                }

                _logger.LogWarning("Invalid password for email {Email}", email);
                return new
                {
                    status = 400,
                    message = "Invalid Password"
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while logging in user with email {Email}", command.email);
                return new
                {
                    status = 500,
                    message = "Internal server error"
                };
            }
        }
    }
}
