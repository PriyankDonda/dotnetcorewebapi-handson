using System.Security.Claims;
using Handson.IServices;
using Handson.RequestResponseModel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Handson.Controllers;

[ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class UsersController : ControllerBase
    {
        private readonly IUserService _userService;
        private readonly ICacheService _cacheService;
        private readonly ILogger<UsersController> _logger;

        public UsersController(
            IUserService userService,
            ICacheService cacheService,
            ILogger<UsersController> logger)
        {
            _userService = userService;
            _cacheService = cacheService;
            _logger = logger;
        }

        [HttpGet]
        [Authorize(Roles = "Admin")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerable<UserBOResponse>>> GetAllUsers()
        {
            try
            {
                // Check cache first
                var cacheKey = "AllUsers";
                var cachedData = await _cacheService.GetAsync<IEnumerable<UserBOResponse>>(cacheKey);
                
                if (cachedData != null)
                {
                    return Ok(cachedData);
                }

                // Get from service
                var users = await _userService.GetAllUsersAsync();
                
                // Cache the data
                await _cacheService.SetAsync(cacheKey, users, TimeSpan.FromMinutes(5));
                
                return Ok(users);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving all users");
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }

        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<UserBOResponse>> GetUserById(int id)
        {
            try
            {
                // Check cache first
                var cacheKey = $"User_{id}";
                var cachedData = await _cacheService.GetAsync<UserBOResponse>(cacheKey);
                
                if (cachedData != null)
                {
                    return Ok(cachedData);
                }

                var user = await _userService.GetUserByIdAsync(id);
                
                if (user == null)
                {
                    return NotFound();
                }

                // Cache the data
                await _cacheService.SetAsync(cacheKey, user, TimeSpan.FromMinutes(5));
                
                return Ok(user);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving user with ID {UserId}", id);
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }

        [HttpGet("profile")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<UserBOResponse>> GetCurrentUserProfile()
        {
            try
            {
                // Get current user ID from claims
                var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                
                if (string.IsNullOrEmpty(userIdClaim) || !int.TryParse(userIdClaim, out var userId))
                {
                    return Unauthorized();
                }

                var user = await _userService.GetUserByIdAsync(userId);
                
                if (user == null)
                {
                    return NotFound();
                }
                
                return Ok(user);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving current user profile");
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }
    }
    