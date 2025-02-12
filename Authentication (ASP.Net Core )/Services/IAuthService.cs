using Authentication__ASP.Net_Core__.APIResponse;
using Authentication__ASP.Net_Core__.Entities.Models;
using Authentication__ASP.Net_Core__.Entities.ModelsDTO;

namespace Authentication__ASP.Net_Core__.Services
{
    public interface IAuthService
    {
        Task<ApiResponse<UserDTO>> RegisterAsync(RegisterDTO registerDTO);
    }
}
