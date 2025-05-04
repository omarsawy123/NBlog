using Application.Dtos;
using Application.Shared;
using Domain.Entites;
using Domain.Enums;
using FluentValidation;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Application.Services.Auth
{
    public class AuthService
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ILogger<AuthService> _logger;
        private readonly IValidator<RegisterDto> _registerValidator;
        private readonly IValidator<LoginDto> _loginValidator;
        private readonly JwtSettings _jwtSettings;

        public AuthService(UserManager<ApplicationUser> userManager,
            ILogger<AuthService> logger,
            IValidator<RegisterDto> validator,
            IValidator<LoginDto> loginValidator,
            IOptions<JwtSettings> jwtSettings)
        {
            _userManager = userManager;
            _logger = logger;
            _registerValidator = validator;
            _loginValidator = loginValidator;
            _jwtSettings = jwtSettings.Value;
        }


        public async Task<IEnumerable<UsersDto>> GetAllUsers()
        {
            return await _userManager.Users.Select(u => new UsersDto
            {
                Id = u.Id,
                UserName = u.UserName!,
                Email = u.Email!
            }).ToListAsync();
        }

        /// <summary>
        /// Attempts to register a new user with the provided registration details by validating input, checking for an existing email, creating the user, and assigning a default role.
        /// </summary>
        /// <param name="registerDto">An object containing the user's registration details, including username, email, and password.</param>
        /// <returns>
        /// A Task that represents the asynchronous operation. The Result object indicates:
        /// <list type="bullet">
        ///   <item>
        ///     <description>Success (HTTP 201) if the registration completes successfully.</description>
        ///   </item>
        ///   <item>
        ///     <description>Failure (HTTP 400) if input validation fails, the email is already registered, or user creation fails.</description>
        ///   </item>
        ///   <item>
        ///     <description>Failure (HTTP 500) if role assignment fails or an unexpected error occurs.</description>
        ///   </item>
        /// </list>
        /// </returns>
        public async Task<Result> RegisterUser(RegisterDto registerDto)
        {
            try
            {

                var validationResult = await _registerValidator.ValidateAsync(registerDto);

                if (!validationResult.IsValid)
                {
                    return Result.Failure(StatusCodes.Status400BadRequest, validationResult.Errors.Select(e => e.ErrorMessage));
                }

                var userExists = await _userManager.FindByEmailAsync(registerDto.Email);

                if (userExists != null)
                {
                    return Result.Failure(StatusCodes.Status400BadRequest, "Email already exists");
                }

                var user = new ApplicationUser
                {
                    UserName = registerDto.UserName,
                    Email = registerDto.Email
                };

                var userResult = await _userManager.CreateAsync(user, registerDto.Password);

                if (userResult is null || !userResult.Succeeded)
                {
                    string userErrors = string.Join(",", userResult?.Errors.Select(r => r.Description) ?? []);
                    return Result.Failure(StatusCodes.Status400BadRequest, userErrors);
                }

                var roleResult = await _userManager.AddToRoleAsync(user, Enum.GetName(RoleType.User)!);

                if (roleResult is null || !roleResult.Succeeded)
                {
                    string roleErrors = string.Join(",", roleResult?.Errors.Select(r => r.Description) ?? []);
                    return Result.Failure(StatusCodes.Status500InternalServerError, roleErrors);
                }


                return Result.Success(StatusCodes.Status201Created);

            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while registering user");
                return Result.Failure(StatusCodes.Status500InternalServerError, "Error occurred while registering user");
            }

        }

        public async Task<Result> LoginUser(LoginDto loginDto)
        {
            try
            {
                var validationResult = await _loginValidator.ValidateAsync(loginDto);
                if (!validationResult.IsValid)
                {
                    return Result.Failure(StatusCodes.Status400BadRequest, validationResult.Errors.Select(e => e.ErrorMessage));
                }

                var user = await _userManager.FindByEmailAsync(loginDto.Email);

                if (user == null)
                {
                    return Result.Failure(StatusCodes.Status404NotFound, "User not found");
                }

                var result = await _userManager.CheckPasswordAsync(user, loginDto.Password);

                if (!result)
                {
                    return Result.Failure(StatusCodes.Status400BadRequest, "Invalid user credintials");
                }

                var userRoles = await _userManager.GetRolesAsync(user);

                var token = GenerateJwtToken(user, string.Join(",", userRoles));

                return Result<string>.Success(token, StatusCodes.Status200OK);

            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while user login");
                return Result.Failure(StatusCodes.Status500InternalServerError, "Error occurred user login");
            }
        }

        private string GenerateJwtToken(ApplicationUser user, string userRoles)
        {
            var claims = new Claim[]
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Email, user.Email!),
                new Claim(ClaimTypes.Role,string.Join(",",userRoles))

            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtSettings.Key));
            var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: _jwtSettings.Issuer,
                audience: _jwtSettings.Audience,
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(_jwtSettings.ExpiryInMinutes),
                signingCredentials: credentials
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        public async Task<Result> DeleteUser(string userId)
        {
            try
            {
                var user = await _userManager.FindByIdAsync(userId);

                if (user == null)
                {
                    return Result.Failure(StatusCodes.Status404NotFound, "User not found");
                }

                var result = await _userManager.DeleteAsync(user);

                if (result.Succeeded)
                {
                    return Result.Success(StatusCodes.Status204NoContent);
                }

                string errors = string.Join(",", result.Errors.Select(r => r.Description));
                return Result.Failure(StatusCodes.Status400BadRequest, errors);

            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while deleting user");
                return Result.Failure(StatusCodes.Status500InternalServerError, "Error occurred while deleting user");
            }
        }
    }
}
