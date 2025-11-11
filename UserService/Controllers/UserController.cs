using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.Blazor;
using UserService.Data;

namespace UserService.Controllers
{
    public class UserController : Controller
    {
        private readonly AppDbContext _context;
        private readonly ILogger<UserController> _logger;
        private readonly IMediator _mediator;

        public UserController(AppDbContext context, ILogger<UserController> logger, IMediator mediator)
        {
            _context = context;
            _logger = logger;
            _mediator = mediator;
        }
        #region INTERNAL_API

        [HttpPost("/internal_api/users/create")]
        public async Task<IActionResult> CreateUser([FromBody] CreateUser request)
        {
            try
            {
                var response = await _mediator.Send(request);
                return Ok(response);
            }
            catch (BusinessException ex)
            {
                Console.WriteLine("Error in create user: " + ex.Message);
                return StatusCode(500, new { Success = false, ex.Message });
            }
            catch
            {
                return StatusCode(500, new { Success = false, Message = "Internal server error." });
            }
        }

        [HttpPost("/internal_api/users/delete")]
        public async Task<IActionResult> DeleteUser([FromBody] DeleteUser deleteUserRequest)
        {
            try
            {
                var response = await _mediator.Send(deleteUserRequest);
                return Ok(response);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error in delete user: " + ex.Message);
                return StatusCode(500, new { Error = ex.Message });
            }
        }

        #endregion

        #region PUBLIC_API

        // Debug Endpoints
        [HttpGet("/ping")]
        public Task<string> Ping()
        {
            var response = _mediator.Send(new Ping());
            return response;
        }

        [Authorize]
        [HttpGet("/hello")]
        public Task<string> Hello()
        {
            return Task.FromResult("Hello " + User.Identity.Name);
        }

        // Profile
        [HttpGet("/api/profile")]
        public async Task<IActionResult> GetProfile()
        {
            // var response = await _mediator.Send(new FindUserByUsername() { Username = "testUSER" });
            if (User.Identity == null || User.Identity.Name == null)
            {
                return NotFound(new { Error = "You are not logged in" });
            }

            var response = await _mediator.Send(new FindUserByUsername() { Username = User.Identity.Name!, ShouldNotifyVisit = false });
            if (response == null) return NotFound(new { Error = "User not found" });
            return Ok(response);
        }

        [HttpPut("/api/profile")]
        // public Task<User> UpdateUser([FromForm][Bind("Avatar", "Name", "Biography")]  UpdateUser updateUserRequest)
        public async Task<IActionResult> UpdateUser([FromForm] UpdateUser updateUserRequest)
        {
            if (updateUserRequest == null) return BadRequest();

            if (User.Identity == null || User.Identity.Name == null)
            {
                return NotFound(new { Error = "You are not logged in" });
            }

            updateUserRequest.Username = User.Identity.Name;

            try
            {
                var response = await _mediator.Send(updateUserRequest);
                return Ok(response);
            }
            catch (BusinessException ex)
            {
                Console.WriteLine("Error in update user: " + ex.Message);
                return StatusCode(500, new { Error = ex.Message });
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error in update user: " + ex.StackTrace);
                return StatusCode(500, new { Error = "Internal server error." });
            }
        }

        [HttpGet("/api/users/{username}")]
        public async Task<IActionResult> GetUser(string username)
        {
            try
            {
                var response = await _mediator.Send(new FindUserByUsername() { Username = username, ShouldNotifyVisit = true });
                return Ok(response);
            }
            catch (UserNotFound ex)
            {
                return NotFound(new { Error = "User not found" });
            }
            catch (BusinessException ex)
            {
                return StatusCode(500, new { Error = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return StatusCode(500, new { Error = "Internal server error." });
            }
        }

        [HttpGet("/api/search-users")]
        public async Task<IActionResult> SearchUsers(string searchTerm)
        {
            if (searchTerm == null) return NotFound(new { Error = "Search term is empty" });

            try
            {
                var response = await _mediator.Send(new SearchUsers() { SearchTerm = searchTerm });
                if (response == null) return NotFound(new { Error = "User not found" });
                return Ok(response);
            } 
            catch (BusinessException ex)
            {
                return StatusCode(500, new { Error = ex.Message });
            }
            catch
            {
                return StatusCode(500, new { Error = "Internal server error." });
            }
        }


        [HttpGet("/api/top-users")]
        public async Task<IActionResult> GetTopUsers(int limit)
        {
            try
            {
                var response = await _mediator.Send(new FindTopUsers { Limit = limit });
                return Ok(response);
            }
            catch (UserNotFound ex)
            {
                return NotFound(new { Error = ex.Message });
            }
            catch (BusinessException ex)
            {
                return StatusCode(500, new { Error = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in get top users");
                return StatusCode(500, new { Error = "Internal server error." });
            }
        }

        #endregion
    }
}
