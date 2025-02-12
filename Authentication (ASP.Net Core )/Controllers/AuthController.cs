using Authentication__ASP.Net_Core__.APIResponse;
using Authentication__ASP.Net_Core__.Entities.Models;
using Authentication__ASP.Net_Core__.Entities.ModelsDTO;
using Authentication__ASP.Net_Core__.Repository;
using Authentication__ASP.Net_Core__.Services;
using Microsoft.AspNetCore.Mvc;

namespace Authentication__ASP.Net_Core__.Controllers
{


    [Route("api/v1/auth")]
    [ApiController]
    public class AuthController :ControllerBase
    {
        private readonly IAuthService _authService;
        private readonly IUserRepository _userRepository;

        public AuthController(IAuthService authService , IUserRepository userRepository)
        {
            _authService = authService;
            _userRepository = userRepository;
        }


        [HttpGet("allusers")]
        public async Task<ActionResult<ApiResponse<IEnumerable<UserDTO>>>> GetAllUsers()
        {
            var response = await _userRepository.GetAllUsersAsync();
            return Ok(response);
        }


        // register user
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
        public async Task<ActionResult<ApiResponse<User>>> DeleteUser(Guid userId)
        {
            var response = await _userRepository.DeleteAsync(userId);

            if (response.Success)
            {
                return Ok(response);
            }

            return NotFound(response);
        }

    }
}
