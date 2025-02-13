using Authentication__ASP.Net_Core__.APIResponse;
using Authentication__ASP.Net_Core__.Data;
using Authentication__ASP.Net_Core__.Entities.Models;
using Authentication__ASP.Net_Core__.Entities.ModelsDTO;
using Microsoft.EntityFrameworkCore;

namespace Authentication__ASP.Net_Core__.Repository
{
    public class RoleRepository : IRoleRepository
    {
        private readonly ApplicationDBContext _context;

        public RoleRepository(ApplicationDBContext context)
        {
            _context = context;
        }

        public async Task<bool> AssignRoleToUserAsync(AssignUserRoleDTO assignUserRoleDTO)
        {
            var user = await _context.Users.FindAsync(assignUserRoleDTO.UserId);
            if (user == null)
            {
                throw new Exception("User not Found");
            }
            
            var role = await _context.Roles.FindAsync(assignUserRoleDTO.RoleId);
            if (role == null)
            {
                throw new Exception("Role not found");
            }

            user.RoleId = role.RoleId;
            _context.Users.Update(user);
            await _context.SaveChangesAsync();

            return true;
           
        }

        public async Task<Role> CreateRoleAsync(RoleDTO roleDto)
        {
            var role = new Role
            {
                RoleName = roleDto.RoleName
            };

            _context.Roles.Add(role);
            await _context.SaveChangesAsync();
            return role;
        }

        public async Task<ApiResponse<string>> DeleteRoleAsync(int roleId)
        {
            var role = await _context.Roles.FindAsync(roleId);

            if (role == null)
            {
                return new ApiResponse<string>(false, "Role not found", null, 404);
            }

            _context.Roles.Remove(role);
            await _context.SaveChangesAsync();

            return new ApiResponse<string>(true, "Role deleted successfully", "Role deleted", 200);
        }


        public async Task<ApiResponse<IEnumerable<AllRoleDTO>>> GetAllRolesAsync()
        {
            var roles = await _context.Roles
                                      .Select(r => new AllRoleDTO
                                      {
                                          RoleId = r.RoleId,
                                          RoleName = r.RoleName
                                      })
                                      .ToListAsync();

            return new ApiResponse<IEnumerable<AllRoleDTO>>(true, "Roles retrieved", roles, 201);
        }


        public async Task<Role> UpdateRoleAsync(int roleId, RoleDTO roleDTO)
        {
           var role =  await _context.Roles.FindAsync(roleId);

            if(role == null)
            {
                throw new Exception("Role Not Found");
            }

            role.RoleName = roleDTO.RoleName;
            _context.Roles.Update(role);
            await _context.SaveChangesAsync();

            return role;
        }
    }
}
