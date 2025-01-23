using App.Core.Apps.User.Command;
using App.Core.Apps.User.Query;
using App.Core.Dtos;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Serilog;

namespace MoviesApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly ILogger<UserController> _logger;

        public UserController(IMediator mediator, ILogger<UserController> logger)
        {
            _mediator = mediator;
            _logger = logger;
        }

        [HttpGet("get-roles")]
        public async Task<IActionResult> getRoles()
        {
            try
            {
                var roles = await _mediator.Send(new GetRolesQuery { });
                return Ok(roles);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while getting roles");
                return StatusCode(StatusCodes.Status500InternalServerError, "Internal server error");
            }
        }

        [HttpPost("add-user")]
        public async Task<IActionResult> addUser(UserDto userDto)
        {
            try
            {
                if (userDto != null)
                {
                    var resp = await _mediator.Send(new AddUserCommand { userDto = userDto });
                    return Ok(resp);
                }
                return BadRequest("Null object");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while adding a user");
                return StatusCode(StatusCodes.Status500InternalServerError, "Internal server error");
            }
        }

        [HttpPost("user-login")]
        public async Task<IActionResult> userLogin(LoginDto loginDto)
        {
            try
            {
                if (loginDto.email != null && loginDto.password != null)
                {
                    var resp = await _mediator.Send(new UserLoginCommand { email = loginDto.email, password = loginDto.password });
                    return Ok(resp);
                }
                return BadRequest("Email or Password is null");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while logging in user");
                return StatusCode(StatusCodes.Status500InternalServerError, "Internal server error");
            }
        }
    }
}
