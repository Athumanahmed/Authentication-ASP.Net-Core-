using Authentication__ASP.Net_Core__.APIResponse;
using Authentication__ASP.Net_Core__.Data;
using Authentication__ASP.Net_Core__.Entities.Models;
using Authentication__ASP.Net_Core__.Entities.ModelsDTO;
using Microsoft.EntityFrameworkCore;

namespace Authentication__ASP.Net_Core__.Repository
{
    public class PermissionRepository : IPermissionRepository
    {
        private readonly ApplicationDBContext _context;

        public PermissionRepository(ApplicationDBContext context)
        {
            _context = context;
        }


        public async Task<Permission> AddPermissionAsync(ReqPermissionDTO permissionDto)
        {
            var permission = new Permission
            {
                PermissionName = permissionDto.PermissionName,
                CanRead = permissionDto.CanRead,
                CanWrite = permissionDto.CanWrite,
                RoleId = permissionDto.RoleId,
            };

            _context.Permissions.Add(permission);
            await _context.SaveChangesAsync();
            return permission;
        }

        public async Task<bool> AssignPermissionToUserAsync(AssignPermissionToUserDTO assignPermissionToUserDTO)
        {
            var user = await _context.Users.FindAsync(assignPermissionToUserDTO.UserId);
            if(user == null)
            {
                throw new Exception("User not Found");
            }



            var permission = await _context.Permissions.FindAsync(assignPermissionToUserDTO.PermissionId);
            if (permission == null)
            {
                throw new Exception("Permission not FOund");
            }

            user.Permissions.Add(permission);
            _context.Users.Update(user);
            await _context.SaveChangesAsync();

            return true;
            
        }

        public async Task<ApiResponse<string>> DeletePermissionAsync(int permissionId)
        {
            var permission = await _context.Permissions.FindAsync(permissionId);
            if (permission == null)
            {
                return new ApiResponse<string>(false, "Permission not found", null, 404);
            }

            _context.Permissions.Remove(permission);
            await _context.SaveChangesAsync();

            return new ApiResponse<string>(true, "permission deleted", "permission deleted", 200);
        }

        public async Task<ApiResponse<IEnumerable<ResPermissionDTO>>> GetAllPermissionsAsync()
        {
            var permissions = await _context.Permissions
                .Select(p => new ResPermissionDTO
                {
                    PermissionId = p.PermissionId,
                    PermissionName = p.PermissionName,
                    CanRead = p.CanRead,
                    CanWrite = p.CanWrite,
                })
                .ToListAsync();

            return new ApiResponse<IEnumerable<ResPermissionDTO>>(true, "permissions recieved", permissions, 202);
        }

        public async Task<ApiResponse<string>> UpdatePermissionAsync(int permissionId, UpdatePermDTO updatePermDTO)
        {
            var permission = await _context.Permissions.FindAsync(permissionId);
            if (permission == null)
            {
                return new ApiResponse<string>(false, "Permission not found", null, 404);
            }

            permission.PermissionName = updatePermDTO.PermissionName;
            permission.CanRead = updatePermDTO.CanRead;
            permission.CanWrite = updatePermDTO.CanWrite;

            await _context.SaveChangesAsync();
            return new ApiResponse<string>(true, "permission updated", "permissionupdated", 200);
        }
    }
}
