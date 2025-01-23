using App.Core.Apps.Movies.Command;
using App.Core.Apps.Movies.Query;
using App.Core.Dtos;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Serilog;

namespace MoviesApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MovieController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly ILogger<MovieController> _logger;

        public MovieController(IMediator mediator, ILogger<MovieController> logger)
        {
            _mediator = mediator;
            _logger = logger;
        }

        [HttpGet("get-all-movies")]
        public async Task<IActionResult> getAllMovies()
        {
            try
            {
                var movies = await _mediator.Send(new GetAllMoviesQuery { });
                return Ok(movies);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while getting all movies");
                return StatusCode(StatusCodes.Status500InternalServerError, "Internal server error");
            }
        }

        [HttpGet("get-searched-movies")]
        public async Task<IActionResult> getSearchMovies([FromQuery] string movieTitle, [FromQuery] string apiKey)
        {
            try
            {
                if (!string.IsNullOrEmpty(apiKey))
                {
                    var movies = await _mediator.Send(new GetSearchMovieQuery { searchTitle = movieTitle, apiKey = apiKey });
                    return Ok(movies);
                }
                else
                {
                    return Unauthorized("ApiKey missing");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while searching for movies");
                return StatusCode(StatusCodes.Status500InternalServerError, "Internal server error");
            }
        }

        [Authorize(Roles = "Admin")]
        [HttpPost("add-movie")]
        public async Task<IActionResult> addMovie([FromForm] MovieDto movieDto)
        {
            try
            {
                if (movieDto != null)
                {
                    var resp = await _mediator.Send(new AddMovieCommand { MovieDto = movieDto });
                    return Ok(resp);
                }
                return BadRequest("Empty request body");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while adding a movie");
                return StatusCode(StatusCodes.Status500InternalServerError, "Internal server error");
            }
        }

        [Authorize(Roles = "Admin")]
        [HttpDelete("delete-movie")]
        public async Task<IActionResult> deleteMovie(int movieId)
        {
            try
            {
                if (movieId != null)
                {
                    var resp = await _mediator.Send(new DeleteMovieCommand { id = movieId });
                    return Ok(resp);
                }
                return BadRequest("Null Id");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while deleting a movie");
                return StatusCode(StatusCodes.Status500InternalServerError, "Internal server error");
            }
        }

        [Authorize(Roles = "Admin")]
        [HttpPut("update-movie")]
        public async Task<IActionResult> updateMovie(int movieId, [FromForm] MovieDto movieDto)
        {
            try
            {
                if (movieId != null && movieDto != null)
                {
                    var resp = await _mediator.Send(new UpdateMovieCommand { movieId = movieId, movieDto = movieDto });
                    return Ok(resp);
                }
                return BadRequest("Null Values");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while updating a movie");
                return StatusCode(StatusCodes.Status500InternalServerError, "Internal server error");
            }
        }
    }
}
