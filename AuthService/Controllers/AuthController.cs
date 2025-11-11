using AuthService.Models;
using AuthService.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;
using System.Net;
using System.Security.Claims;

namespace AuthService.Controllers
{
    [Route("api/")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAuthenticationService _authService;

        public AuthController(IAuthenticationService authService)
        {
            _authService = authService;
        }

        [HttpPost("register")]
        public async Task<ActionResult<AuthResponse>> Register([FromBody] RegisterRequest model)
        {
            Console.WriteLine("I entered the controller for register method");
            // Check ModelState for validation errors before proceeding
            if (!ModelState.IsValid)
            {
                // Create a dictionary of property names and their validation errors
                var errors = ModelState
                    .Where(e => e.Value.Errors.Count > 0)
                    .ToDictionary(
                        kvp => kvp.Key, // Property name
                        kvp => kvp.Value.Errors.Select(e => e.ErrorMessage).ToList() // List of error messages
                    );

                return BadRequest(new { HandCrafted = true, Errors = errors });
            }

            try
            {
                var result = await _authService.Register(model);
                return Ok(result);
            }
            catch (Models.ValidationException ex)
            {
                // For validation exceptions with structured errors
                return BadRequest(new { ex.Errors });
            }
            catch (Exception ex)
            {
                // For general application exceptions
                return StatusCode((int)HttpStatusCode.InternalServerError, "An unexpected error occurred.");
            }
        }

        [HttpPost("login")]
        public async Task<ActionResult<AuthResponse>> Login([FromBody] LoginRequest model)
        {
            // Check ModelState for validation errors before proceeding
            if (!ModelState.IsValid)
            {
                // Create a dictionary of property names and their validation errors
                var errors = ModelState
                    .Where(e => e.Value.Errors.Count > 0)
                    .ToDictionary(
                        kvp => kvp.Key, // Property name
                        kvp => kvp.Value.Errors.Select(e => e.ErrorMessage).ToList() // List of error messages
                    );

                return BadRequest(new { Errors = errors });
            }

            try
            {
                var result = await _authService.Login(model);
                return Ok(result);
            }
            catch (Models.ValidationException ex)
            {
                // For validation exceptions with structured errors
                return BadRequest(new { ex.Errors });
            }
            catch (LoginException ex)
            {
                // For authentication exceptions
                return Unauthorized(ex.Message);   
            }
            catch (Exception ex)
            {
                return StatusCode((int)HttpStatusCode.InternalServerError, "An unexpected error occurred.");
            }
        }

        [Authorize]
        [HttpGet("userinfo")]
        public async Task<ActionResult<UserInfoResponse>> GetUserInfo()
        {
            try
            {
                var username = User.FindFirstValue(ClaimTypes.Name);
                if (string.IsNullOrEmpty(username))
                {
                    return Unauthorized(new { Message = "User not authenticated" });
                }

                var result = await _authService.GetUserInfo(username);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode((int)HttpStatusCode.InternalServerError, "An unexpected error occurred.");
            }
        }

        [HttpGet("hello")]
        public string Hello()
        {
            return "Hello";
        }
    }
}

