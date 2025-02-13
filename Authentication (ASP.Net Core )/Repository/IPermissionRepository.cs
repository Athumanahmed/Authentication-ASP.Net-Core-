using Authentication__ASP.Net_Core__.APIResponse;
using Authentication__ASP.Net_Core__.Entities.Models;
using Authentication__ASP.Net_Core__.Entities.ModelsDTO;

namespace Authentication__ASP.Net_Core__.Repository
{
    public interface IPermissionRepository
    {
        Task<ApiResponse<IEnumerable<ResPermissionDTO>>> GetAllPermissionsAsync();
        
        Task<Permission> AddPermissionAsync(ReqPermissionDTO permissionDto);
        Task<ApiResponse<string>> UpdatePermissionAsync(int permissionId , UpdatePermDTO updatePermDTO);
        Task<ApiResponse<string>> DeletePermissionAsync(int permissionId);

        Task<bool> AssignPermissionToUserAsync(AssignPermissionToUserDTO assignPermissionToUserDTO);
    }
}
