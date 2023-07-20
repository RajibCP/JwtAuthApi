using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using JwtAuthApi.DTOs;
using JwtAuthApi.Models;
using JwtAuthApi.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;

namespace JwtAuthApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly ITokenService _tokenService;
        private readonly IUserService _userService;

        public AuthController(ITokenService tokenService, IUserService userService)
        {
            _tokenService = tokenService;
            _userService = userService;
        }

        [HttpPost]
        [Route("login")]
        public ActionResult Login([FromBody] UserLoginDto userLoginDto)
        {
            if(ModelState.IsValid)
            {
                User? existUser = _userService.GetUserByEmail(userLoginDto.Email);

                if (existUser == null)
                {
                    return BadRequest(new AuthResult
                    {
                        Result = false,
                        Errors = new List<string> { "Invalid email" }
                    });
                }

                var userClaims = new[]
                {
                    new Claim(ClaimTypes.Name, existUser.Name),
                    new Claim(ClaimTypes.Email, existUser.Email)
                };

                string accessToken = _tokenService.GenerateAccessToken(userClaims);
                string refreshToken = _tokenService.GenerateRefreshToken();

                existUser.RefreshToken = refreshToken;

                _userService.UpdateUser(existUser);

                return Ok(new AuthResult
                {
                    Result = true,
                    AccessToken = accessToken,
                    RefreshToken = refreshToken
                });
            }
            return Unauthorized();
        }

        [HttpPost]
        [Route("registration")]
        public IActionResult Registration([FromBody] UserRegistrationDTO userRegistrationDto)
        {
            if (ModelState.IsValid)
            {
                User? existUser = _userService.GetUserByEmail(userRegistrationDto.Email);

                if(existUser != null)
                {
                    return BadRequest(new AuthResult
                    {
                        Result = false,
                        Errors = new List<string> { "Email is used" }
                    });
                }

                User user = new User
                {
                    Email = userRegistrationDto.Email,
                    Name = userRegistrationDto.Name,
                    Password = userRegistrationDto.Password
                };

                var userClaims = new[]
                {
                    new Claim(ClaimTypes.Name, user.Name),
                    new Claim(ClaimTypes.Email, user.Email)
                };

                string accessToken = _tokenService.GenerateAccessToken(userClaims);
                string refreshToken = _tokenService.GenerateRefreshToken();

                user.RefreshToken = refreshToken;

                _userService.AddUser(user);

                return Ok(new AuthResult
                {
                    Result = true,
                    AccessToken = accessToken,
                    RefreshToken = refreshToken
                });
            }
            return BadRequest(new AuthResult
            {
                Result = false,
                Errors = new List<string>{"Model Invalid"}
            });
        }

        [HttpPost]
        [Route("refresh")]
        public IActionResult GetAccessToken([FromBody]TokenDto tokenDto)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes("your-secret-key");

            try
            {
                var principal = _tokenService.GetPrincipleFromExpiredToken(tokenDto.AccessToken);

                if (principal.Identity is not ClaimsIdentity identity || !identity.IsAuthenticated)
                {
                    return Unauthorized();
                }

                var email = identity.FindFirst(ClaimTypes.Email)?.Value;

                var user = _userService.GetUserByEmail(email);

                if(user is null || user.RefreshToken != tokenDto.RefreshToken)
                {
                    return BadRequest();
                }

                string accessToken = _tokenService.GenerateAccessToken(principal.Claims);
                string refreshToken = _tokenService.GenerateRefreshToken();

                User newUser = new User {
                    Email = user.Email,
                    Name = user.Name,
                    Password = user.Password,
                    RefreshToken = refreshToken
                };

                _userService.UpdateUser(newUser);

                return Ok(new AuthResult {
                    AccessToken = accessToken,
                    RefreshToken = newUser.RefreshToken,
                    Result = true
                });
            }
            catch
            {
                return Unauthorized();
            }
        }
    }
}

