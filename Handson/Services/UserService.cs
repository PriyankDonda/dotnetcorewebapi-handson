using Handson.IRepository;
using Handson.IServices;
using Handson.Models;
using Handson.RequestResponseModel;
using Microsoft.AspNetCore.Identity;

namespace Handson.Services;

public class UserService : IUserService
{
    private readonly IUserRepository _userRepository;
    private readonly ITokenService _tokenService;
    private readonly IPasswordHasher _passwordHasher;
    private readonly ILogger<UserService> _logger;

    public UserService(
        IUserRepository userRepository,
        ITokenService tokenService,
        IPasswordHasher passwordHasher,
        ILogger<UserService> logger)
    {
        _userRepository = userRepository;
        _tokenService = tokenService;
        _passwordHasher = passwordHasher;
        _logger = logger;
    }
    public async Task<UserBOResponse?> GetUserByIdAsync(int id)
    {
        var user = await _userRepository.GetByIdAsync(id);
        
        if (user == null) 
            return null;

        return UserBOResponse.Create(user);
        
    }

    public async Task<IEnumerable<UserBOResponse>> GetAllUsersAsync()
    {
        var users = await _userRepository.GetAllAsync();
        return users.Select(x => UserBOResponse.Create(x)) ;
    }

    public async Task<bool> RegisterUserAsync(RegisterBORequest request)
    {
        // Validate the request (could use FluentValidation here)
        if (request.Password != request.ConfirmPassword)
        {
            _logger.LogWarning("Password and confirmation do not match");
            return false;
        }

        // Check if user exists
        var existingUser = await _userRepository.GetByUsernameAsync(request.Username);
        if (existingUser != null)
        {
            _logger.LogWarning("Username {Username} already exists", request.Username);
            return false;
        }

        existingUser = await _userRepository.GetByEmailAsync(request.Email);
        if (existingUser != null)
        {
            _logger.LogWarning("Email {Email} already exists", request.Email);
            return false;
        }

        // Create password hash
        _passwordHasher.CreatePasswordHash(request.Password, out byte[] passwordHash, out byte[] passwordSalt);

        // Create new user
        var user = RegisterBORequest.Create(request);
        user.PasswordHash = passwordHash;
        user.PasswordSalt = passwordSalt;
        user.CreatedAt = DateTime.UtcNow;
        user.IsActive = true;

        // Save user
        var userId = await _userRepository.AddAsync(user);
        return userId > 0;
    }

    public async Task<string?> AuthenticateAsync(LoginBORequest request)
    {
        var user = await _userRepository.GetByUsernameAsync(request.Username);
        if (user == null || !user.IsActive)
        {
            _logger.LogWarning("Authentication failed for username: {Username}", request.Username);
            return null;
        }

        if (!_passwordHasher.VerifyPasswordHash(request.Password, user.PasswordHash, user.PasswordSalt))
        {
            _logger.LogWarning("Invalid password for username: {Username}", request.Username);
            return null;
        }

        // Update last login
        user.LastLogin = DateTime.UtcNow;
        await _userRepository.UpdateAsync(user);

        // Generate token
        var token = _tokenService.GenerateToken(user);
        return token;
    }

}