using Authentication__ASP.Net_Core__.APIResponse;
using Authentication__ASP.Net_Core__.Entities.Models;
using Authentication__ASP.Net_Core__.Entities.ModelsDTO;
using Authentication__ASP.Net_Core__.Repository;
using Microsoft.AspNetCore.Http.HttpResults;
using System.Security.Cryptography;
using System.Text;
namespace Authentication__ASP.Net_Core__.Services
{
    public class AuthService : IAuthService
    {
        private readonly IUserRepository _userRepository;
        private readonly EmailVerificationService _emailVerificationService;

        public AuthService(IUserRepository userRepository , EmailVerificationService emailVerificationService)
        {
            _userRepository = userRepository;
            _emailVerificationService = emailVerificationService;
        }
        public async Task<ApiResponse<UserDTO>>RegisterAsync(RegisterDTO registerDTO)
        {
            var existingUser = await _userRepository.GetUserByEmailAsync(registerDTO.Email);
            if(existingUser.Success)
            {
                return new ApiResponse<UserDTO>(true, "Email exists", null, 400);
            }

            var hashedPassword = HashPassword(registerDTO.Password);

            // generate email verification token
            var emailToken = Convert.ToBase64String(RandomNumberGenerator.GetBytes(10));

            var newUser = new User
            {
                FirtstName = registerDTO.FirstName,
                MiddleName = registerDTO.MiddleName,
                LastName = registerDTO.LastName,
                Email = registerDTO.Email,
                PasswordHash = hashedPassword,
                EmailVerificationToken = emailToken,
                EmailVerificationTokenExpiresAt = DateTime.UtcNow.AddMinutes(5),
                IsEmailConfirmed = false,
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


            // map the user entity to registerDTO
            var newUserDTO = new UserDTO
            {
                FirstName = newUser.FirtstName,
                MiddleName = newUser.MiddleName,
                LastName = newUser.LastName,
                Email = newUser.Email,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow,
            };
            return new ApiResponse<UserDTO>(true, "User created , Please check ypur email for Verification of Account", newUserDTO, 201);

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
            using var sha256 = SHA256.Create();
            var hashedBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
            return BitConverter.ToString(hashedBytes).Replace("-", "").ToLower();
        }
    }
}
