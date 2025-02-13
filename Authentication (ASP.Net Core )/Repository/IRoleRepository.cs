using Authentication__ASP.Net_Core__.APIResponse;
using Authentication__ASP.Net_Core__.Entities.Models;
using Authentication__ASP.Net_Core__.Entities.ModelsDTO;

namespace Authentication__ASP.Net_Core__.Repository
{
    public interface IRoleRepository
    {
        Task<ApiResponse<IEnumerable<AllRoleDTO>>> GetAllRolesAsync();
        Task<Role> CreateRoleAsync(RoleDTO roleDto);

        Task<ApiResponse<string>> DeleteRoleAsync(int roleId);
        Task <Role> UpdateRoleAsync(int roleId , RoleDTO roleDTO);
        Task <bool> AssignRoleToUserAsync (AssignUserRoleDTO assignUserRoleDTO);
    }
}
