using Authentication__ASP.Net_Core__.APIResponse;
using Authentication__ASP.Net_Core__.Entities.Models;
using Authentication__ASP.Net_Core__.Entities.ModelsDTO;
using Authentication__ASP.Net_Core__.Repository;
using Authentication__ASP.Net_Core__.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text.Json;

namespace Authentication__ASP.Net_Core__.Controllers
{


    [Route("api/v1/auth")]
    [ApiController]
    [AllowAnonymous]
    public class AuthController :ControllerBase
    {
        private readonly IAuthService _authService;
        private readonly IUserRepository _userRepository;

        public AuthController(IAuthService authService , IUserRepository userRepository)
        {
            _authService = authService;
            _userRepository = userRepository;
        }

        
        [HttpPost("register")]
        public async Task<ActionResult<ApiResponse<string>>> RegisterUser([FromBody] RegisterDTO registerDTO)
        {
            var response = await _authService.RegisterAsync(registerDTO);

            if (response.Success)
            {
                return CreatedAtAction(nameof(RegisterUser), new { email = registerDTO.Email }, response);
            }

            return BadRequest(response);
        }

        
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginDto loginDto)
        {
            if (loginDto == null || string.IsNullOrEmpty(loginDto.Email) || string.IsNullOrEmpty(loginDto.Password))
            {
                return BadRequest(new ApiResponse<string>(false, "Invalid credentials", null, 400));
            }

            // Call the service to handle login
            var response = await _authService.LoginAsync(loginDto);

            if (!response.Success)
            {
                return Unauthorized(new ApiResponse<string>(false, response.Message, null, 401));
            }

            return Ok(response);
        }

        [HttpPost("logout")]
        public async Task<IActionResult> Logout([FromHeader(Name = "Authorization")] string authHeader)
        {
            if (string.IsNullOrEmpty(authHeader) || !authHeader.StartsWith("Bearer "))
            {
                return Unauthorized(new ApiResponse<string>(false, "Token is missing or invalid", null, 401));
            }

            var token = authHeader.Substring("Bearer ".Length).Trim();

            // Decode and validate JWT (optional, you may also skip this part)
            var handler = new JwtSecurityTokenHandler();
            var jwtToken = handler.ReadJwtToken(token);
            var userIdClaim = jwtToken?.Claims.FirstOrDefault(c => c.Type == JwtRegisteredClaimNames.Sub)?.Value;

            if (string.IsNullOrEmpty(userIdClaim) || !Guid.TryParse(userIdClaim, out var userId))
            {
                return Unauthorized(new ApiResponse<string>(false, "Invalid token", null, 401));
            }

            // Call the service to clear the refresh token from the database
            var response = await _authService.LogOutAsync(userId);

            if (!response.Success)
            {
                return StatusCode(response.StatusCode, response);
            }

            return Ok(new ApiResponse<string>(true, "User logged out successfully", null, 200));
        }



        [HttpGet("allusers")]
        [Authorize] // Restricting access to authenticated users
        public async Task<ActionResult<ApiResponse<IEnumerable<UserDTO>>>> GetAllUsers()
        {
            var response = await _userRepository.GetAllUsersAsync();
            return Ok(response);
        }

        // email verification service
        [HttpGet("verify-email")]
        public async Task<IActionResult> VerifyEmail(string token)
        {
            var user = await _userRepository.GetUserByTokenAsync(token);
            if (user == null)
            {
                return BadRequest("Invalid token");
            }

            user.IsEmailConfirmed = true;
            user.EmailVerificationToken = null; // Remove token after verification
            user.EmailVerificationTokenExpiresAt=null;
            await _userRepository.UpdateAsync(user);

            return Ok("Email verified successfully. You can now log in.");
        }


        // verify OTP
        [HttpPost("verify-otp")]
        public async Task<IActionResult> VerifyOTP([FromBody] OTPVerificationRequest request)
        {
            if (request == null || string.IsNullOrEmpty(request.OTPToken))
            {
                return BadRequest(new ApiResponse<User>(false, "Invalid request data", null, 400));
            }

            var response = await _userRepository.GetUserByOTPTokenAsync(request.OTPToken);

            if (!response.Success)
            {
                return StatusCode(response.StatusCode, response);
            }

            // Mark OTP as verified
            response.Data!.IsOTPVerified = true;
            response.Data.OTPToken = null;

            await _userRepository.UpdateAsync(response.Data);

            return Ok(new ApiResponse<User>(true, "OTP Verified Successfully", response.Data, 200));
        }



        // get user by email
        [HttpGet("GetUserByEmail/{email}")]
        public async Task<ActionResult<ApiResponse<User>>> GetUserByEmail(string email)
        {
            var response = await _userRepository.GetUserByEmailAsync(email);
            if (response.Success)
            {
                return Ok(response); 
            }

            return BadRequest(response);
        }


        //update user
        [HttpPut("UpdateUser")]
        [Authorize] // Restricting access to authenticated users
        public async Task<ActionResult<ApiResponse<User>>> UpdateUser([FromBody] User updatedUser)
        {
            var response = await _userRepository.AddUpdateUser(updatedUser);
            if (response.Success)
            {
                return Ok(response);
            }
            return BadRequest(response);
        }

        // delete user
        [HttpDelete("delete/userId")]
        [Authorize] // Restricting access to authenticated users
        public async Task<ActionResult<ApiResponse<User>>> DeleteUser(Guid userId)
        {
            var response = await _userRepository.DeleteAsync(userId);

            if (response.Success)
            {
                return Ok(response);
            }

            return NotFound(response);
        }



        [HttpPost("beem-callback")]
        public async Task<IActionResult> BeemCallback([FromBody] BeemCallbackDto callbackData)
        {
            // Log the callback data for debugging
            Console.WriteLine($"Beem Callback Received: {JsonSerializer.Serialize(callbackData)}");

            // Example: Handle SMS delivery status
            if (callbackData.Status == "DELIVERED")
            {
                // Update your database to mark OTP as delivered
                Console.WriteLine($"OTP successfully delivered to {callbackData.Recipient}");
            }
            else if (callbackData.Status == "FAILED")
            {
                // You might want to retry sending OTP or notify the user
                Console.WriteLine($"OTP failed to deliver to {callbackData.Recipient}");
            }

            return Ok();
        }


    }
}
