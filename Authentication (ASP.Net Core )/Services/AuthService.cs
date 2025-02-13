using Authentication__ASP.Net_Core__.APIResponse;
using Authentication__ASP.Net_Core__.Entities.Models;
using Authentication__ASP.Net_Core__.Entities.ModelsDTO;
using Authentication__ASP.Net_Core__.Repository;
using Microsoft.AspNetCore.Http.HttpResults;
using System.Security.Cryptography;
using System.Text;
using Twilio.Http;
using Twilio.Jwt.AccessToken;
namespace Authentication__ASP.Net_Core__.Services
{
    public class AuthService : IAuthService
    {
        private readonly IUserRepository _userRepository;
        private readonly EmailVerificationService _emailVerificationService;
        private readonly TwilioSmsService _twilioSmsService;
        private readonly JwtService _jwtService;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public AuthService(IUserRepository userRepository , EmailVerificationService emailVerificationService , TwilioSmsService twilioSmsService,JwtService jwtService , IHttpContextAccessor httpContextAccessor)
        {
            _userRepository = userRepository;
            _emailVerificationService = emailVerificationService;
            _twilioSmsService = twilioSmsService;
            _jwtService = jwtService;
            _httpContextAccessor = httpContextAccessor;
        }
        public async Task<ApiResponse<UserDTO>>RegisterAsync(RegisterDTO registerDTO)
        {
            var existingUser = await _userRepository.GetUserByEmailAsync(registerDTO.Email);
            if(existingUser.Success)
            {
                return new ApiResponse<UserDTO>(true, "Email exists", null, 400);
            }

            //validate phone number
            if(!IsValidPhoneNumber(registerDTO.PhoneNumber))
            {
                throw new InvalidOperationException("Invalid Phone number, ypur phone number should start with + ");
            }

            // generating OTP
            var otp = _twilioSmsService.GenerateOtp();

            var hashedPassword = HashPassword(registerDTO.Password);

            // generate email verification token
            var emailToken = Convert.ToBase64String(RandomNumberGenerator.GetBytes(10));

            var newUser = new User
            {
                FirtstName = registerDTO.FirstName,
                MiddleName = registerDTO.MiddleName,
                LastName = registerDTO.LastName,
                Email = registerDTO.Email,
                PhoneNumber = registerDTO.PhoneNumber,
                PasswordHash = hashedPassword,
                //email verfication
                EmailVerificationToken = emailToken,
                EmailVerificationTokenExpiresAt = DateTime.UtcNow.AddMinutes(5),
                IsEmailConfirmed = false,
                // otp verifcication
                OTPToken=otp,
                IsOTPVerified = false,
                OTPVerificationExpiry = DateTime.UtcNow.AddMinutes(5),

                CreatedAt=DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow,
            };

            await _userRepository.AddAsync(newUser);

            // Send Email Verification Link

            // using custom email template here this is for testing....

            //var verificationUrl = $"https://localhost:7005/api/auth/verify-email?token={newUser.EmailVerificationToken}";
            //await _emailVerificationService.SendEmailAsync(newUser.Email, "Verify Your Email",
            //    $"Please verify your email by clicking <a href='{verificationUrl}'>here</a> this email verfication will expire in {newUser.EmailVerificationTokenExpiresAt?.ToString("yyyy-MM-dd HH:mm:ss")}");


            // this sends the email template using the custom template
            await _emailVerificationService.SendEmailAsync(newUser.Email, newUser.FirtstName, newUser.EmailVerificationToken);

            // Sending the OTP Via Sms
            await _twilioSmsService.SendOTPAsync(newUser.PhoneNumber, otp);

            // map the user entity to registerDTO
            var newUserDTO = new UserDTO
            {
                FirstName = newUser.FirtstName,
                MiddleName = newUser.MiddleName,
                LastName = newUser.LastName,
                Email = newUser.Email,
                PhoneNumber=newUser.PhoneNumber,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow,
            };
            return new ApiResponse<UserDTO>(true, "User created , Please check ypur email for Verification of Account", newUserDTO, 201);

        }

        public async Task<ApiResponse<LoginResponseDto>> LoginAsync(LoginDto loginDto)
        {
            // Get user by email using the GetUserByEmailAsync method
            var apiResponse = await _userRepository.GetUserByEmailAsync(loginDto.Email);

            // Check if user exists and return error response if not found
            if (!apiResponse.Success || apiResponse.Data == null)
            {
                return new ApiResponse<LoginResponseDto>(false, "Invalid Credentials", null, 400);
            }

            var user = apiResponse.Data;

            // Validate password
            if (!VerifyPassword(user.PasswordHash, loginDto.Password))
            {
                return new ApiResponse<LoginResponseDto>(false, "Invalid Credentials", null, 400);
            }

            // Generate access and refresh tokens
            var accessToken = _jwtService.GenerateAccessToken(user.Id.ToString(), user.Email);
            var refreshToken = _jwtService.GeneratedRefreshToken();

            // Save refresh token to database
            user.RefreshToken = refreshToken;
            user.RefreshTokenExpiryTime = DateTime.UtcNow.AddDays(1);
            await _userRepository.UpdateAsync(user);

            // Return successful response with tokens and user info
            var loginResponseDto = new LoginResponseDto
            {
                Success = true,
                Message = "Login successful",
                AccessToken = accessToken,
                RefreshToken = refreshToken,
                UserId = user.Id,
                Email = user.Email
            };

            return new ApiResponse<LoginResponseDto>(true, "Login successful", loginResponseDto, 200);
        }

        public async Task<ApiResponse<string>> LogOutAsync(Guid userId)
        {
            // Getting user from database
            var apiResponse = await _userRepository.GetUserByIdAsync(userId);

            if (!apiResponse.Success || apiResponse.Data == null)
            {
                return new ApiResponse<string>(false, "User not found", null, 404);
            }

            var user = apiResponse.Data;

            // Clear refresh token from the database
            user.RefreshToken = null;
            user.RefreshTokenExpiryTime = null;
            await _userRepository.UpdateAsync(user);

            // Return success response
            return new ApiResponse<string>(true, "User logged out successfully", null, 200);
        }

        public async Task<ApiResponse<IEnumerable<UserDTO>>> GetAllUsersAsync()
        {
            var users = await _userRepository.GetAllUsersAsync();

            var userDTOs = users.Select(user => new UserDTO
            {
                FirstName = user.FirtstName,
                MiddleName = user.MiddleName,
                LastName = user.LastName,
                Email = user.Email
            });

            return new ApiResponse<IEnumerable<UserDTO>>(true, "Users retrieved successfully", userDTOs, 200);
        }


        // hashing password
        private string HashPassword(string password)
        {
            return BCrypt.Net.BCrypt.HashPassword(password);
        }


        // validating phone number
        private bool IsValidPhoneNumber(string phoneNumber)
        {
            
            return phoneNumber.StartsWith("+") && phoneNumber.Length > 10;
        }


        private bool VerifyPassword(string hashedPassword, string inputPassword)
        {
            // Implement password hashing comparison (e.g., using BCrypt)
            return BCrypt.Net.BCrypt.Verify(inputPassword, hashedPassword);
        }

    }
}
