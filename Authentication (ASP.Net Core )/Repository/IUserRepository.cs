using Authentication__ASP.Net_Core__.APIResponse;
using Authentication__ASP.Net_Core__.Entities.Models;

namespace Authentication__ASP.Net_Core__.Repository
{
    public interface IUserRepository
    {
        Task<IEnumerable<User>> GetAllUsersAsync();
        Task<ApiResponse<User>> GetUserByEmailAsync(string email);
        Task<ApiResponse<User>> GetUserByIdAsync(Guid userId);
        Task<ApiResponse<User>> AddAsync(User user);
        Task<ApiResponse<User>> UpdateAsync(User user);
        Task<ApiResponse<string>> DeleteAsync(Guid userId);
        Task<ApiResponse<User>> AddUpdateUser(User user);
        Task<User?> GetUserByTokenAsync(string token);
        Task<ApiResponse<User>> GetUserByOTPTokenAsync(string otpToken);

    }
}
