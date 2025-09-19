using BoardJob.Domain.DTOs.Requests.User;
using BoardJob.Domain.Services.Abstractions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace BoardJob.API.Controllers
{
    [ApiController]
    [Route("api/[controller]/")]
    [Authorize]
    public class UserController : ControllerBase
    {
        private readonly IUserService _userService;

        public UserController(IUserService userService)
        {
            _userService = userService;
        }

        /// <summary>
        /// Exposes some details regarding the current user. 
        /// The action method gets user details from the incoming token. 
        /// The token information is represented by accessing the HttpContext.User property and getting the value of ClaimType.Email
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public async Task<IActionResult> GetUserDataAsync()
        {
            var claim = HttpContext.User.Claims.FirstOrDefault(x => x.Type == ClaimTypes.Email);

            if (claim == null)
            {
                return Unauthorized();
            }

            var userData = await _userService.GetUserAsync(new GetUserRequest { Email = claim.Value });
            return Ok(userData);
        }

        /// <summary>
        /// Binds the request.Email and request.Password fields and sends the request object using IUserService interface. 
        /// Returns the TokenResponse with the generated token
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [AllowAnonymous]
        [HttpPost("sign-in")]
        public async Task<IActionResult> SignInAsync(SignInRequest request)
        {
            var token = await _userService.SignInAsync(request);

            if (token == null)
            {
                return BadRequest();
            }
            return Ok(token);
        }

        /// <summary>
        /// Registers a new user and returns the 201 Created HTTP code if the operation has success
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [AllowAnonymous] // Allowed to call the action method without being authenticated
        [HttpPost("sign-up")]
        public async Task<IActionResult> SignUpAsync(SignUpRequest request)
        {
            var user = await _userService.SignUpAsync(request);

            if (user == null) return BadRequest();
            return CreatedAtAction(nameof(GetUserDataAsync), new { }, null);
        }

        [HttpDelete]
        public async Task<IActionResult> DeleteUserAsync(DeleteUserRequest request)
        {
            var isUserDeleted = await _userService.DeleteUserAsync(request);

            if (!isUserDeleted)
                return BadRequest();

            return NoContent();
        }
    }
}
