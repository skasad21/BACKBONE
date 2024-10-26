using BACKBONE.Application.Interfaces;
using BACKBONE.Application.Interfaces.SecurityInterface;
using BACKBONE.Core.Dtos;
using BACKBONE.Core.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace BACKBONE.EpyDispatch.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IUserRepository _userRepository;
        private readonly IJwtTokenService _jwtTokenService;
        private readonly IRefreshTokenService _refreshTokenService;
       

        //private readonly IUnitOfWork _unitOfWork;

        public AuthController(IUserRepository userRepository, IJwtTokenService jwtTokenService, IRefreshTokenService refreshTokenService)
        {
            _userRepository = userRepository;
            _jwtTokenService = jwtTokenService;
            _refreshTokenService = refreshTokenService;
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromForm] LoginDto loginDto)
        {
            if (loginDto == null || string.IsNullOrEmpty(loginDto.EmpCode) || string.IsNullOrEmpty(loginDto.Password))
            {
                return BadRequest("Invalid login data");
            }
            var userResponse = await _userRepository.GetUserByEmailAsync(loginDto.EmpCode);

            if (userResponse?.Data?.SingleValue == null || !BCrypt.Net.BCrypt.Verify(loginDto.Password, userResponse.Data.SingleValue.PasswordHash))
            {
                return Unauthorized("Invalid credentials");
            }

            var jwtToken = _jwtTokenService.GenerateToken(userResponse.Data.SingleValue);
            if (jwtToken == null)
            {
                return StatusCode(500, "Error generating JWT token");
            }

            var refreshToken = await _refreshTokenService.GenerateRefreshTokenAsync();
            if (refreshToken == null || string.IsNullOrEmpty(refreshToken.ToString()))
            {
                return StatusCode(500, "Error generating refresh token");
            }

            await _refreshTokenService.SaveRefreshTokenAsync(new RefreshToken
            {
                UserId = userResponse.Data.SingleValue.UserId,
                Token = refreshToken.ToString(),
                ExpirationDate = DateTime.Now.AddDays(7)
            });

            return Ok(new AuthResponseDto
            {
                Token = jwtToken,
                RefreshToken = refreshToken.ToString()
            });
        }



        [AllowAnonymous]
        [HttpPost("refresh-token")]
        public async Task<IActionResult> RefreshToken([FromBody] string refreshToken)
        {
            var isValid = await _refreshTokenService.ValidateRefreshTokenAsync(refreshToken);
            if (!isValid)
            {
                return Unauthorized("Invalid refresh token");
            }

            var storedToken = _refreshTokenService.GetRefreshTokenAsync(refreshToken);
            var user = await _userRepository.GetUserByUserIdAsync(storedToken.Result.UserId);

            var newJwtToken = _jwtTokenService.GenerateToken(user.Data.SingleValue);
            var newRefreshToken = await _refreshTokenService.GenerateRefreshTokenAsync();

            await _refreshTokenService.SaveRefreshTokenAsync(new RefreshToken
            {
                UserId = user.Data.SingleValue.UserId,
                Token = newRefreshToken,
                ExpirationDate = DateTime.Now.AddDays(7)
            });

            await _refreshTokenService.RevokeRefreshTokenAsync(refreshToken);

            return Ok(new AuthResponseDto { Token = newJwtToken, RefreshToken = newRefreshToken });
        }
    }
}
